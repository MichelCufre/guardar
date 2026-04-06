using System.Collections.Generic;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.General.API.Dtos.Entrada;
using static WIS.Domain.Automatismo.Dtos.ConfirmacionSalidaStockRequest;

namespace WIS.AutomationManager.Models.Mappers.Interfaces
{
    public interface ISalidaStockAutomatismoMapper
    {
        PickingRequest Map(ConfirmacionSalidaStockRequest request);
    }
}
