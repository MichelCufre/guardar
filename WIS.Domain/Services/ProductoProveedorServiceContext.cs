using WIS.Extension;
using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Services
{
    public class ProductoProveedorServiceContext : ServiceContext, IProductoProveedorServiceContext
    {
        protected List<ProductoProveedor> _productosProveedor = new List<ProductoProveedor>();

        public HashSet<string> TiposAgente { get;set;} = new HashSet<string>();
        public Dictionary<string, string> Agentes { get;set;} = new Dictionary<string, string>();
        public Dictionary<string, Producto> productos { get;set;} = new Dictionary<string, Producto>();
        public Dictionary<string, string> CodigosExternos { get;set;} = new Dictionary<string, string>();
        public Dictionary<string, ProductoProveedor> ProductosProveedor { get;set;} = new Dictionary<string, ProductoProveedor>();

        public ProductoProveedorServiceContext(IUnitOfWork uow, List<ProductoProveedor> productoProveedor, int userId, int empresa) : base(uow, userId, empresa)
        {
            _productosProveedor = productoProveedor;
        }

        public async override Task Load()
        {
            await base.Load();

            var tiposOperaciones = new Dictionary<string, string>();
            var keysAgentes = new Dictionary<string, Agente>();
            var keysProductos = new Dictionary<string, Producto>();

            foreach (var ta in _uow.AgenteRepository.GetTiposAgentes())
            {
                TiposAgente.Add(ta);
            }

            foreach (var pp in _productosProveedor)
            {
                var keyAgente = $"{pp.TipoAgente}.{pp.CodigoAgente}.{Empresa}";
                keysAgentes[keyAgente] = new Agente() { Tipo = pp.TipoAgente.Truncate(3), Codigo = pp.CodigoAgente.Truncate(40), Empresa = Empresa };

                var keyProducto = $"{pp.CodigoProducto}.{Empresa}";
                keysProductos[keyProducto] = new Producto() { Codigo = pp.CodigoProducto.Truncate(40), CodigoEmpresa = Empresa };
            }

            foreach (var a in _uow.AgenteRepository.GetAgentes(keysAgentes.Values))
            {
                Agentes[($"{a.Tipo}.{a.Codigo}")] = a.CodigoInterno;
            }

            foreach (var p in _uow.ProductoRepository.GetProductos(keysProductos.Values))
            {
                p.AceptaDecimales = !string.IsNullOrEmpty(p.AceptaDecimalesId) && p.AceptaDecimalesId == "S";
                p.ManejoIdentificador = (new ProductoMapper()).MapManejoIdentificador(p.ManejoIdentificadorId);

                var keyProducto = $"{p.CodigoEmpresa}.{p.Codigo}";
                productos[keyProducto] = p;
            }

            foreach (var pp in _productosProveedor)
            {
                var cliente = Agentes.GetValueOrDefault($"{pp.TipoAgente}.{pp.CodigoAgente}", string.Empty);

                if (!string.IsNullOrEmpty(cliente))
                {
                    var key = $"{pp.CodigoProducto}.{cliente}.{pp.Empresa}";
                    pp.Cliente = cliente;
                    tiposOperaciones[key] = pp.TipoOperacionId;
                }
            }

            CodigosExternos = _uow.ProductoRepository.GetCodigosExternos(_productosProveedor);

            var productosProveedor = _uow.ProductoRepository.GetProductosProveedor(_productosProveedor);

            foreach (var pp in productosProveedor)
            {
                var key = $"{pp.CodigoProducto}.{pp.Cliente}.{pp.Empresa}";
                pp.TipoOperacionId = tiposOperaciones[key];
                ProductosProveedor[key] = pp;
            }
        }

        public virtual bool ExisteTipoAgente(string tipoAgente)
        {
            return TiposAgente.Contains(tipoAgente);
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

        public virtual bool ExisteCodExterno(string producto, int empresa, string cliente, string codigoExterno)
        {
            var key = $"{codigoExterno}.{cliente}.{empresa}";
            return CodigosExternos.ContainsKey(key) && CodigosExternos[key] != producto;
        }

        public virtual ProductoProveedor GetProductoProveedor(string producto, string tipoAgente, string codigoAgente, int empresa)
        {
            var cliente = Agentes.GetValueOrDefault($"{tipoAgente}.{codigoAgente}", string.Empty);
            var key = $"{producto}.{cliente}.{empresa}";
            return ProductosProveedor.GetValueOrDefault(key, null);
        }

        public virtual Producto GetProducto(int empresa, string codigo)
        {
            var keyProducto = $"{empresa}.{codigo}";
            return productos.GetValueOrDefault(keyProducto, null);
        }
    }
}
