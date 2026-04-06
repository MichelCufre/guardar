using WIS.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
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
    public class ProducirProduccionServiceContext : ServiceContext, IProducirProduccionServiceContext
    {
        protected ProducirProduccion _produccion = new ProducirProduccion();
        protected List<DominioDetalle> _dominiosMotivos;
        protected Dictionary<string, Producto> _productos = new Dictionary<string, Producto>();
        protected IngresoProduccion _ingreso;
        protected EspacioProduccion _espacio;
        protected EspacioProduccion _produccionEspacioActiva;

        public HashSet<Stock> SeriesExistentes { get; set; } = new HashSet<Stock>();
        public List<string> KeysAjustes { get; set; } = new List<string>();
        public bool NotificarProduccion { get; set; } = false;        
        public ILogicaProduccion LogicaProduccion { get; set; }

        public ProducirProduccionServiceContext(IUnitOfWork uow, ProducirProduccion produccion, int userId, ILogicaProduccionFactory logicaProduccionFactory, int empresa) : base(uow, userId, empresa)
        {
            _produccion = produccion;

            if (!string.IsNullOrEmpty(_produccion.IdProduccionExterno))
                LogicaProduccion = logicaProduccionFactory.GetLogicaProduccion(_uow, _produccion.IdProduccionExterno, Empresa);
            else
            {
                var produccionEspacioActiva = _uow.EspacioProduccionRepository.GetEspacioProduccion(_produccion.IdEspacio);

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
            var keysProductoSerie = new Dictionary<string, Producto>();

            foreach (var prod in _produccion.Productos)
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

                if (p.ManejoIdentificador == ManejoIdentificador.Serie)
                    keysProductoSerie[p.Codigo] = p;
            }

            _dominiosMotivos = _uow.DominioRepository.GetDominios(TipoIngresoProduccion.MOTIVO_PRODUCCION);

            if (LogicaProduccion != null)
            {
                _ingreso = LogicaProduccion.GetIngresoProduccion();
                _espacio = _uow.EspacioProduccionRepository.GetEspacioProduccionByIngreso(_ingreso.Id);
            }

            _produccionEspacioActiva = _uow.EspacioProduccionRepository.GetEspacioProduccion(_produccion.IdEspacio);

            if (keysProductoSerie.Count > 0)
                SeriesExistentes = _uow.StockRepository.GetLotesExistente(keysProductoSerie.Values).ToHashSet();
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

        public virtual List<string> GetIdsBloqueos(IFormatProvider formatProvider)
        {
            //Solo utilizar post validaciones ya que previamente no se tiene la ubicación ni el lote.

            var keysStock = _produccion.Productos
                .GroupBy(s => new { s.Ubicacion, s.Empresa, s.Producto, s.Faixa, s.Identificador })
                .Select(s => new Stock()
                {
                    Ubicacion = s.Key.Ubicacion,
                    Empresa = s.Key.Empresa,
                    Producto = s.Key.Producto,
                    Faixa = s.Key.Faixa,
                    Identificador = s.Key.Identificador,
                }).ToList();

            var keysStockInsumos = _ingreso.Consumidos
                .Where(c => c.QtReal > 0)
                .GroupBy(s => new { s.Empresa, s.Producto, s.Faixa, s.Identificador })
                .Select(s => new Stock()
                {
                    Ubicacion = _espacio.IdUbicacionProduccion,
                    Empresa = s.Key.Empresa.Value,
                    Producto = s.Key.Producto,
                    Faixa = s.Key.Faixa.Value,
                    Identificador = s.Key.Identificador,
                }).ToList();

            keysStock.AddRange(keysStockInsumos);

            return keysStock
                .Select(s => s.GetLockId(formatProvider))
                .Distinct()
                .ToList();
        }

        public virtual bool ExisteSerie(string codigoProducto, string identificador)
        {
            return SeriesExistentes.Any(s => s.Producto == codigoProducto && s.Identificador == identificador);
        }
    }
}
