using System.Collections.Generic;
using System.Threading.Tasks;

namespace WIS.Domain.Services.Interfaces
{
    public interface IStockServiceContext : IServiceContext
    {
        HashSet<int> Familias { get; set; }
        HashSet<short> Ramos { get; set; }
        HashSet<string> Clases { get; set; }
        HashSet<string> Predios { get; set; }
        HashSet<string> Ubicaciones { get; set; }
        HashSet<string> GruposConsulta { get; set; }

        Task Load();

        bool ExisteClase(string clase);
        bool ExisteFamilia(int familia);
        bool ExisteGrupoConsulta(string grupoConsulta);
        bool ExistePredio(string predio);
        bool ExisteRamo(short ramo);
        bool ExisteUbicacion(string ubicacion);
    }
}