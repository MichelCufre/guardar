using WIS.Extension;
using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Services
{
    public class CodigoBarrasServiceContext : ServiceContext, ICodigoBarrasServiceContext
    {
        protected List<CodigoBarras> _codigosBarras = new List<CodigoBarras>();

        public HashSet<int> TiposCodigoBarras { get; set; } = new HashSet<int>();
        public Dictionary<string, Producto> productos { get; set; } = new Dictionary<string, Producto>();
        public Dictionary<string, CodigoBarras> CodigosBarras { get; set; } = new Dictionary<string, CodigoBarras>();

        public CodigoBarrasServiceContext(IUnitOfWork uow, List<CodigoBarras> codigosBarras, int userId, int empresa) : base(uow, userId, empresa)
        {
            _codigosBarras = codigosBarras;
        }

        public async override Task Load()
        {
            await base.Load();

            var tiposOperaciones = new Dictionary<string, string>();
            var keysProductos = new Dictionary<string, Producto>();
            var keysBarras = new Dictionary<string, CodigoBarras>();

            foreach (var tcb in _uow.ProductoCodigoBarraRepository.GetTiposCodigosBarras())
            {
                TiposCodigoBarras.Add(tcb.Id);
            }

            foreach (var cb in _codigosBarras)
            {
                var keyProducto = $"{cb.Producto}.{Empresa}";
                tiposOperaciones[cb.Codigo] = cb.TipoOperacionId;
                keysProductos[keyProducto] = new Producto() { Codigo = cb.Producto.Truncate(40), CodigoEmpresa = Empresa };

                var keyBarras= $"{cb.Codigo.Truncate(50)}.{Empresa}";
                keysBarras[keyBarras] = new CodigoBarras() { Codigo = cb.Codigo.Truncate(50), Empresa = Empresa };
            }

            foreach (var p in _uow.ProductoRepository.GetProductos(keysProductos.Values))
            {
                p.AceptaDecimales = !string.IsNullOrEmpty(p.AceptaDecimalesId) && p.AceptaDecimalesId == "S";
                p.ManejoIdentificador = (new ProductoMapper()).MapManejoIdentificador(p.ManejoIdentificadorId);

                var keyProducto = $"{p.CodigoEmpresa}.{p.Codigo}";
                productos[keyProducto] = p;
            }

            var codigosBarras = _uow.CodigoBarrasRepository.GetCodigosBarras(keysBarras.Values);

            foreach (var cb in codigosBarras)
            {
                cb.TipoOperacionId = tiposOperaciones[cb.Codigo];
                CodigosBarras[cb.Codigo] = cb;
            }
        }

        public virtual bool ExisteTipoCodigoBarras(int tipoCodigoBarras)
        {
            return TiposCodigoBarras.Contains(tipoCodigoBarras);
        }

        public virtual CodigoBarras GetCodigoBarras(string codigo)
        {
            return CodigosBarras.GetValueOrDefault(codigo, null);
        }

        public virtual Producto GetProducto(int empresa, string codigo)
        {
            var keyProducto = $"{empresa}.{codigo}";
            return productos.GetValueOrDefault(keyProducto, null);
        }
    }
}
