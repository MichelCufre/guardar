using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Produccion;

namespace WIS.Domain.Services.Interfaces
{
    public interface IConsumirProduccionService
    {
        Task<ValidationsResult> ProcesarConsumo(ConsumirProduccion consumo, int userId);
    }
}
