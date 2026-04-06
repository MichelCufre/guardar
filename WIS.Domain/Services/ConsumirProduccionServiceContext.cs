using WIS.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Produccion.Interfaces;
using WIS.Domain.Produccion.Models;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Domain.Validation;
using WIS.Exceptions;

namespace WIS.Domain.Services
{
    public class ConsumirProduccionServiceContext : ServiceContext, IConsumirProduccionServiceContext
    {
        protected ConsumirProduccion _consumo = new ConsumirProduccion();
        protected List<DominioDetalle> _dominiosMotivos;
        protected IngresoProduccion _ingreso;
        protected EspacioProduccion _espacio;
        protected EspacioProduccion _produccionEspacioActiva;
        protected Dictionary<string, Producto> _productos = new Dictionary<string, Producto>();
        protected int _cantidadOrdenesActivas;

        public List<IngresoProduccionDetalleReal> DetallesInsumos { get; set; } = new List<IngresoProduccionDetalleReal>();
        public List<string> KeysAjustes { get; set; } = new List<string>();
        public bool NotificarProduccion { get; set; } = false;
        public ILogicaProduccion LogicaProduccion { get; set; }


        public ConsumirProduccionServiceContext(IUnitOfWork uow, ConsumirProduccion consumo, int userId, ILogicaProduccionFactory logicaProduccionFactory, int empresa) : base(uow, userId, empresa)
        {
            _consumo = consumo;

            if (!string.IsNullOrEmpty(_consumo.IdProduccionExterno))
                LogicaProduccion = logicaProduccionFactory.GetLogicaProduccion(_uow, _consumo.IdProduccionExterno, Empresa);
            else
            {
                var produccionEspacioActiva = _uow.EspacioProduccionRepository.GetEspacioProduccion(_consumo.IdEspacio);

                if (produccionEspacioActiva != null)
                {
                    if (produccionEspacioActiva.NumeroIngreso == null)
                    {
                        var idExterno = _uow.EspacioProduccionRepository.GetFirstProduccionExternoOrdenAsociadaEspacio(produccionEspacioActiva.Id);

                        if (idExterno == null)
                        {
                            var error = new Error("WMSAPI_msg_Error_EspacioSinOrdenAsociada");
                            var message = Translator.Traducir(uow, error, UserId);
                            throw new ValidationFailedException(message.Mensaje);
                        }

                        LogicaProduccion = logicaProduccionFactory.GetLogicaProduccion(_uow, idExterno, Empresa);
                    }
                    else
                    {
                        var idExterno = _uow.ProduccionRepository.GetIdExternoByIdIngreso(produccionEspacioActiva.NumeroIngreso);
                        LogicaProduccion = logicaProduccionFactory.GetLogicaProduccion(_uow, idExterno, Empresa);
                    }
                }
            }
        }

        public async override Task Load()
        {
            await base.Load();

            var keysProductos = new Dictionary<string, Producto>();

            foreach (var prod in _consumo.Insumos)
            {
                var keyProducto = $"{prod.Producto}.{Empresa}";
                keysProductos[keyProducto] = new Producto() { Codigo = prod.Producto.Truncate(40), CodigoEmpresa = Empresa };
            }

            foreach (var p in _uow.ProductoRepository.GetProductos(keysProductos.Values))
            {
                p.AceptaDecimales = !string.IsNullOrEmpty(p.AceptaDecimalesId) && p.AceptaDecimalesId == "S";
                p.ManejoIdentificador = (new ProductoMapper()).MapManejoIdentificador(p.ManejoIdentificadorId);

                var keyProducto = $"{p.CodigoEmpresa}.{p.Codigo}";
                _productos[keyProducto] = p;
            }

            _dominiosMotivos = _uow.DominioRepository.GetDominios(TipoIngresoProduccion.MOTIVO_CONSUMO);

            if (LogicaProduccion != null)
            {
                _ingreso = LogicaProduccion.GetIngresoProduccion();
                _espacio = _uow.EspacioProduccionRepository.GetEspacioProduccionByIngreso(_ingreso.Id);
                if (_consumo.IniciarProduccion)
                    DetallesInsumos = _uow.IngresoProduccionRepository.GetDetallesInsumos(_ingreso.Id, _ingreso.IdEspacioProducion).ToList();
                else
                    DetallesInsumos = _uow.IngresoProduccionRepository.GetDetallesInsumos(_ingreso.Id).ToList();
            }

            if (!string.IsNullOrEmpty(_consumo.IdEspacio) || _ingreso != null)
            {
                _produccionEspacioActiva = _uow.EspacioProduccionRepository.GetEspacioProduccion(_consumo.IdEspacio ?? _ingreso.IdEspacioProducion);
                _cantidadOrdenesActivas = _uow.EspacioProduccionRepository.GetIngresosActivosEspacio(_consumo.IdEspacio ?? _ingreso.IdEspacioProducion);
            }
        }

        public virtual bool ExisteMotivo(string motivo)
        {
            return _dominiosMotivos.Any(d => d.Id == motivo);
        }

        public virtual Producto GetProducto(int empresa, string codigo)
        {
            var keyProducto = $"{empresa}.{codigo}";
            return _productos.GetValueOrDefault(keyProducto, null);
        }

        public virtual IngresoProduccion GetIngreso()
        {
            return _ingreso;
        }

        public virtual EspacioProduccion GetEspacioProduccion()
        {
            return _espacio;
        }

        public virtual EspacioProduccion GetProduccionEspacioActiva()
        {
            return _produccionEspacioActiva;
        }

        public virtual int GetCantidadOrdenesActivas()
        {
            return _cantidadOrdenesActivas;
        }

        public virtual List<string> GetIdsBloqueos(IFormatProvider formatProvider)
        {
            //Solo utilizar post validaciones ya que previamente no se tiene la ubicación ni el lote.

            return DetallesInsumos
                .GroupBy(s => new { s.Empresa, s.Producto, s.Faixa, s.Identificador })
                .Select(s => new Stock()
                {
                    Ubicacion = _espacio.IdUbicacionProduccion,
                    Empresa = s.Key.Empresa.Value,
                    Producto = s.Key.Producto,
                    Faixa = s.Key.Faixa.Value,
                    Identificador = s.Key.Identificador,
                })
                .ToList()
                .Select(s => s.GetLockId(formatProvider))
                .Distinct()
                .ToList();
        }
    }
}
