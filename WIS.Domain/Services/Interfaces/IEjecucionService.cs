using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Interfaces;

namespace WIS.Domain.Services.Interfaces
{
    public interface IEjecucionService
    {
        Task<InterfazEjecucion> AddEjecucion(InterfazEjecucion ejecucion);
        Task<InterfazEjecucion> AddEjecucion(int cdIntExterna, int empresa, string ds_deferencia, string data, string archivo, string loginName, string idRequest, string entidadParam = ParamManager.PARAM_EMPR, string entidad = null);
        Task<InterfazData> AddEjecucionData(InterfazData ejecucion);
        Task<InterfazError> AddError(InterfazEjecucion ejecucion, int nroRegistro, string error);
        Task<InterfazEjecucion> AddErrores(InterfazEjecucion ejecucion, List<ValidationsError> errores);
        Task<InterfazEjecucion> UpdateEjecucion(InterfazEjecucion ejecucion);
        Task<InterfazEjecucion> GetEjecucion(long nroEjecucion);
        Task<bool> ExisteEjecucion(long nroEjecucion);
        Task<List<InterfazEjecucion>> GetNotificacionesPendientes();
        Task<List<InterfazEjecucion>> GetSalidasPendientes(int empresa, List<string> gruposConsulta);
        Task<List<InterfazError>> GetErrores(long nroEjecucion);
        Task<InterfazData> GetEjecucionData(long nroEjecucion);
        Task<InterfazEstado> ConsultarEstado(long nroEjecucion, int empresa, List<string> gruposConsulta);
        Task ConfirmarLectura(long nroEjecucion, int empresa, List<string> gruposConsulta, bool ok, List<string> errores);
        Task<InterfazEjecucion> IniciarReprocesamiento(InterfazEjecucion ejecucion);
        bool IsValidUser(UsuarioRequest usuario);
        List<string> GetGruposConsulta(string loginName);
    }
}
