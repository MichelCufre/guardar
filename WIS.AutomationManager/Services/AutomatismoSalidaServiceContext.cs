using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.DataModel;
using WIS.Domain.General;

namespace WIS.AutomationManager.Services
{
    public class AutomatismoSalidaServiceContext : AutomatismoServiceContext
    {
        protected int _userId = 0;

        protected readonly SalidaStockAutomatismoRequest _request;

        public readonly Dictionary<string, Producto> Productos = new Dictionary<string, Producto>();

        public AutomatismoSalidaServiceContext(IUnitOfWork uow,
            SalidaStockAutomatismoRequest request,
            int userId) : base(uow)
        {
            _request = request;
            _userId = userId;
        }

        public async override Task Load()
        {
            await base.Load();

            var productos = _request.Detalles
                .Select(p => new Producto
                {
                    CodigoEmpresa = _request.Empresa,
                    Codigo = p.Producto,
                });

            foreach (var p in _uow.ProductoRepository.GetProductos(productos, out HashSet<string> noEditables))
            {
                Productos[p.Codigo] = p;
            }
        }

        public bool ExisteProducto(string producto)
        {
            return Productos.ContainsKey(producto);
        }
    }
}
