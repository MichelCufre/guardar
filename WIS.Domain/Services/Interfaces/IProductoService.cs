using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.General;

namespace WIS.Domain.Services.Interfaces
{
    public interface IProductoService
    {
        Task<Producto> GetProducto(string codigo, int codigoEmpresa);
        Task<ValidationsResult> AgregarProductos(List<Producto> productos, int userId);
        Task<ProductoProveedor> GetProductoProveedor(string producto, int empresa, string tipoAgente, string codigoAgente);
        Task<ValidationsResult> AgregarProductosProveedor(List<ProductoProveedor> productos, int userId);
        void NotificarAutomatismo(IUnitOfWork uow, List<Producto> codigos);
    }
}
