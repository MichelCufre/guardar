using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;

namespace WIS.Domain.Services.Interfaces
{
    public interface IProductoServiceContext : IServiceContext
    {
        Dictionary<string, Producto> Productos { get; set; }
        HashSet<string> Clases { get; set; }
        HashSet<int> Familias { get; set; }
        HashSet<short> Ramos { get; set; }
        HashSet<string> UnidadesMedida { get; set; }
        HashSet<short> Rotatividades { get; set; }
        HashSet<string> NAMs { get; set; }
        HashSet<string> GruposConsulta { get; set; }
        HashSet<string> ProductosNoEditables { get; set; }
        Dictionary<string, string> ProductosEmpresa { get; set; }

        Task Load();

        bool ExisteClase(string clase);
        bool ExisteFamilia(int familia);
        bool ExisteGrupoConsulta(string grupoConsulta);
        bool ExisteNAM(string nam);
        bool ExisteProductoEmpresa(string codigoProductoEmpresa, string codigoProducto);
        bool ExisteRamo(short ramo);
        bool ExisteRotatividad(short rotatividad);
        bool ExisteUnidadMedida(string unidadMedida);
        Producto GetProducto(string codigo);
        bool PermiteEdicion(string codigo);
        bool ExisteVentanaLiberacion(string valor);
    }
}