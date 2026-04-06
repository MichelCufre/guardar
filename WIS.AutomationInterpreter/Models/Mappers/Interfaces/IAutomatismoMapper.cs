using WIS.Domain.Automatismo.Dtos;

namespace WIS.AutomationInterpreter.Models.Mappers.Interfaces
{
    public interface IAutomatismoMapper
    {
        NotificacionAjustesStockRequest Map(NotificacionAjustesStockAutomatismoRequest request);
        ConfirmacionEntradaStockRequest Map(ConfirmacionEntradaStockAutomatismoRequest request);
        ConfirmacionSalidaStockRequest Map(ConfirmacionSalidaStockAutomatismoRequest request);
    }
}
