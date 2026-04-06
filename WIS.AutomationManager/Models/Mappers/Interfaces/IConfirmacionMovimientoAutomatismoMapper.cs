using System.Collections.Generic;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.General.API.Dtos.Entrada;
using static WIS.Domain.Automatismo.Dtos.ConfirmacionMovimientoStockRequest;

namespace WIS.AutomationManager.Models.Mappers.Interfaces
{
    public interface IConfirmacionMovimientoAutomatismoMapper
    {
        TransferenciaStockRequest Map(ConfirmacionMovimientoStockRequest request, string ubicacionOrigen, string ubicacionDestino);
        List<TransferenciaRequest> Map(List<ConfirmacionMovimientoStockLineaRequest> request, int empresa, string ubicacionOrigen, string ubicacionDestino);
    }
}
