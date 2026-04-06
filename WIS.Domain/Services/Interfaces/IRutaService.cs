using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Tracking.Validation;

namespace WIS.Domain.Services.Interfaces
{
    public interface IRutaService
    {
        Task<Ruta> GetRutaByZona(string zona);
        Task<TrackingValidationResult> AddRutaByZona(Ruta ruta, string loginName);
        Task<TrackingValidationResult> UpdateRutaByZona(Ruta ruta, string loginName);
    }
}
