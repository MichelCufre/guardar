using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;

namespace WIS.Domain.Services.Interfaces
{
    public interface IEmpresaServiceContext : IServiceContext
    {
        public List<int> UsuariosAsignables { get; set; }
        public List<string> TiposRecepcion { get; set; }
        public Dictionary<string, List<string>> ReportesTipoRecepcion { get; set; }
        public HashSet<string> GruposConsulta { get; set; }
        public HashSet<string> Paises { get; set; }
        public Dictionary<int, Empresa> Empresas { get; set; }
        public Dictionary<string, string> SubdivisionesPaises { get; set; }
        public Dictionary<string, PaisSubdivisionLocalidad> LocalidadesSubdivisiones { get; set; }

        Task Load();

        bool ExisteLocalidad(string localidad, string subdivision);
        bool ExistePais(string pais);
        Empresa GetEmpresa(int id);
        PaisSubdivisionLocalidad GetLocalidadId(string localidad, string subdivision);
        List<string> GetReportesTipoRecepcion(string tpRecepcion);
        PaisSubdivision GetSubdivision(string subdivision);
    }
}