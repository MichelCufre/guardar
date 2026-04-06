using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;

namespace WIS.Domain.Services.Interfaces
{
    public interface IAgenteServiceContext : IServiceContext
    {
        HashSet<short> Rutas { get; set; } 
        HashSet<string> TiposAgentes { get; set; } 
        HashSet<string> GruposConsulta { get; set; } 
        HashSet<string> Paises { get; set; } 
        Dictionary<string, string> SubdivisionesPaises { get; set; } 
        Dictionary<string, PaisSubdivisionLocalidad> LocalidadesSubdivisiones { get; set; } 
        Dictionary<string, Agente> Agentes { get; set; }

        List<ClienteDiasValidezVentana> ClienteDiasValidezVentanas { get; set; }

        Task Load();
        
        bool ExisteGrupoConsulta(string grupoConsulta);        
        bool ExisteLocalidad(string localidad, string subdivision);        
        bool ExistePais(string pais);        
        bool ExisteRuta(short ruta);        
        bool ExisteTipoAgente(string tipoAgente);        
        Agente GetAgente(string tipo, string codigo);                
        PaisSubdivisionLocalidad GetLocalidadId(string localidad, string subdivision);        
        PaisSubdivision GetSubdivision(string subdivision);
    }
}