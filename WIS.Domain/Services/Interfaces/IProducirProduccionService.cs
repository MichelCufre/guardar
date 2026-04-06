using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Produccion;

namespace WIS.Domain.Services.Interfaces
{
    public interface IProducirProduccionService
    {
        Task<ValidationsResult> ProcesarProduccion(ProducirProduccion produccion, int userId);
    }
}
