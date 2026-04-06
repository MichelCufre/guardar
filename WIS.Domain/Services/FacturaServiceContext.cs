using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Extension;

namespace WIS.Domain.Services
{
    public class FacturaServiceContext : ServiceContext, IFacturaServiceContext
    {
        protected List<Factura> _facturas = new List<Factura>();

        public HashSet<string> Predios { get; set; } = new HashSet<string>();
        public HashSet<string> Monedas { get; set; } = new HashSet<string>();
        public HashSet<string> TiposFactura { get; set; } = new HashSet<string>();

        public HashSet<Stock> SeriesExistentes { get; set; } = new HashSet<Stock>();
        public Dictionary<string, string> Agentes { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, Factura> Facturas { get; set; } = new Dictionary<string, Factura>();
        public Dictionary<string, Producto> Productos { get; set; } = new Dictionary<string, Producto>();

        public FacturaServiceContext(IUnitOfWork uow, List<Factura> facturas, int userId, int empresa) : base(uow, userId, empresa)
        {
            _facturas = facturas;
        }

        public async override Task Load()
        {
            await base.Load();

            var keysFacturas = new Dictionary<string, Factura>();
            var keysAgentes = new Dictionary<string, Agente>();
            var keysProductos = new Dictionary<string, Producto>();
            var keysProductoSerie = new Dictionary<string, Producto>();

            foreach (var f in _facturas)
            {
                var keyAgente = $"{TipoAgenteDb.Proveedor}.{f.CodigoAgente}.{Empresa}";
                keysAgentes[keyAgente] = new Agente() { Tipo = TipoAgenteDb.Proveedor, Codigo = f.CodigoAgente.Truncate(40), Empresa = Empresa };

                foreach (var d in f.Detalles)
                {
                    var keyProducto = $"{d.Producto}.{Empresa}";
                    keysProductos[keyProducto] = new Producto() { Codigo = d.Producto.Truncate(40), CodigoEmpresa = Empresa };
                }
            }

            foreach (var p in _uow.ProductoRepository.GetProductos(keysProductos.Values))
            {
                p.AceptaDecimales = !string.IsNullOrEmpty(p.AceptaDecimalesId) && p.AceptaDecimalesId == "S";
                p.ManejoIdentificador = (new ProductoMapper()).MapManejoIdentificador(p.ManejoIdentificadorId);
                Productos[p.Codigo] = p;

                if (p.ManejoIdentificador == ManejoIdentificador.Serie)
                    keysProductoSerie[p.Codigo] = p;
            }

            foreach (var p in _uow.PredioRepository.GetPrediosUsuario(UserId))
            {
                Predios.Add(p.Numero);
            }

            foreach (var a in _uow.AgenteRepository.GetAgentes(keysAgentes.Values))
            {
                Agentes[($"{a.Tipo}.{a.Codigo}")] = a.CodigoInterno;
            }

            foreach (var m in _uow.MonedaRepository.GetMonedas())
            {
                Monedas.Add(m.Codigo);
            }

            foreach (var f in _facturas)
            {
                var cliente = Agentes.GetValueOrDefault($"{TipoAgenteDb.Proveedor}.{f.CodigoAgente}", string.Empty);

                if (!string.IsNullOrEmpty(cliente))
                {
                    f.CodigoInternoCliente = cliente;
                }

                var keyFactura = $"{f.NumeroFactura}.{Empresa}.{f.Serie}.{f.CodigoInternoCliente}";
                keysFacturas[keyFactura] = new Factura()
                {
                    NumeroFactura = f.NumeroFactura.Truncate(12),
                    IdEmpresa = Empresa,
                    Serie = f.Serie.Truncate(3),
                    CodigoInternoCliente = f.CodigoInternoCliente.Truncate(10)
                };
            }

            var facturas = _uow.FacturaRepository.GetFacturas(keysFacturas.Values);

            foreach (var f in facturas)
            {
                var key = $"{f.NumeroFactura}.{f.IdEmpresa}.{f.Serie}.{f.CodigoInternoCliente}";
                Facturas[key] = f;
            }

            if (keysProductoSerie.Count > 0)
                SeriesExistentes = _uow.StockRepository.GetLotesExistente(keysProductoSerie.Values).ToHashSet();

            TiposFactura = _uow.DominioRepository.GetDominios(CodigoDominioDb.TiposDeFacturas).Select(p => p.Valor?.ToUpper()).ToHashSet();
        }

        public virtual Agente GetAgente(string codigo, int empresa, string tipo)
        {
            var cliente = Agentes.GetValueOrDefault($"{tipo}.{codigo}", string.Empty);

            if (string.IsNullOrEmpty(cliente))
            {
                return null;
            }

            return new Agente()
            {
                Codigo = codigo,
                CodigoInterno = cliente,
                Empresa = empresa,
                Tipo = tipo
            };
        }

        public virtual bool ExistePredio(string predio)
        {
            return Predios.Contains(predio);
        }

        public virtual Producto GetProducto(int empresa, string codigo)
        {
            return Productos.GetValueOrDefault(codigo, null);
        }

        public virtual Factura GetFactura(string factura, string serie, int empresa, string cliente)
        {
            var key = $"{factura}.{empresa}.{serie}.{cliente}";
            return Facturas.GetValueOrDefault(key, null);
        }

        public virtual bool ExisteFactura(string factura, string serie, int empresa, string cliente)
        {
            var f = GetFactura(factura, serie, empresa, cliente);
            return f != null && f.Estado != EstadoFacturaDb.Cancelada;
        }

        public virtual bool ExisteMoneda(string moneda)
        {
            return Monedas.Contains(moneda);
        }

        public virtual bool ExisteSerie(string codigoProducto, string identificador)
        {
            return SeriesExistentes.Any(s => s.Producto == codigoProducto && s.Identificador == identificador);
        }

        public virtual bool ExisteTipoFactura(string tipo)
        {
            return TiposFactura.Contains(tipo);
        }

    }
}
