using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Recepcion;

namespace WIS.Domain.Services.Interfaces
{
    public interface IReferenciaRecepcionService
    {
        Task<ReferenciaRecepcion> GetReferenciaById(int idReferencia);
        Task<ValidationsResult> AgregarReferencias(List<ReferenciaRecepcion> referencias, int userId);
        Task<ReferenciaRecepcion> GetReferencia(string nuReferencia, int codigoEmpresa, string tipo, string tipoAgente, string codigoAgente);
        Task<ValidationsResult> ModificarReferencias(List<ReferenciaRecepcion> referencias, int userId);
        Task<ValidationsResult> AnularReferencias(List<ReferenciaRecepcion> referencias, int userId);
    }
}
