using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;

namespace WIS.Domain.Services.Interfaces
{
    public interface ICodigoBarrasServiceContext : IServiceContext
    {
        public HashSet<int> TiposCodigoBarras { get; set; }
        public Dictionary<string, Producto> productos { get; set; }
        public Dictionary<string, CodigoBarras> CodigosBarras { get; set; }

        Task Load();

        bool ExisteTipoCodigoBarras(int tipoCodigoBarras);
        CodigoBarras GetCodigoBarras(string codigo);
        Producto GetProducto(int empresa, string codigo);
    }
}