using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.General.API.Dtos.Entrada;

namespace WIS.AutomationManager.Models.Mappers.Interfaces
{
    public interface IConfirmacionSalidaAutomatismoMapper
    {
        PickingRequest Map(ConfirmacionSalidaStockRequest request);
    }
}
