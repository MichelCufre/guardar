using System.Collections.Generic;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.General.API.Dtos.Entrada;

namespace WIS.AutomationManager.Models.Mappers.Interfaces
{
    public interface IConfirmacionEntradaAutomatismoMapper
    {
        TransferenciaStockRequest Map(ConfirmacionEntradaStockRequest request);
        List<TransferenciaRequest> Map(List<ConfirmacionEntradaStockLineaRequest> request, int empresa);
        TransferenciaRequest Map(ConfirmacionEntradaStockLineaRequest request, int empresa);
    }
}
