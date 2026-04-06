using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;

namespace WIS.Domain.Services.Interfaces
{
    public interface IAgenteService
    {
        Task<Agente> GetAgente(string codigo, int empresa, string tipo);
        Task<ValidationsResult> AgregarAgentes(List<Agente> agentes, int userId);
        Task<Agente> GetAgente(string codigoInterno, int empresa);
        Task<Dictionary<string, Agente>> GetAgentesEgreso(int cdCamion);
    }
}
