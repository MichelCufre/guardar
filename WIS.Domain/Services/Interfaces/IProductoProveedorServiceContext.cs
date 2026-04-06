using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;

namespace WIS.Domain.Services.Interfaces
{
    public interface IProductoProveedorServiceContext : IServiceContext
    {
        HashSet<string> TiposAgente { get; set; }
        Dictionary<string, string> Agentes { get; set; }
        Dictionary<string, Producto> productos { get; set; }
        Dictionary<string, string> CodigosExternos { get; set; }
        Dictionary<string, ProductoProveedor> ProductosProveedor { get; set; }

        Task Load();

        bool ExisteCodExterno(string producto, int empresa, string cliente, string codigoExterno);
        bool ExisteTipoAgente(string tipoAgente);
        Agente GetAgente(string codigo, int empresa, string tipo);
        Producto GetProducto(int empresa, string codigo);
        ProductoProveedor GetProductoProveedor(string producto, string tipoAgente, string codigoAgente, int empresa);
    }
}