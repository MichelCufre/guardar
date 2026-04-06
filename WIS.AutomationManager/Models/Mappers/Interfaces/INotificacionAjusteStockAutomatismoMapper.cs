using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.General.API.Dtos.Entrada;

namespace WIS.AutomationManager.Models.Mappers.Interfaces
{
    public interface INotificacionAjusteStockAutomatismoMapper
    {
        AjustesDeStockRequest Map(NotificacionAjustesStockRequest request);
        AjustesDeStockRequest Map(ConfirmacionMovimientoStockRequest request, string ubicacionPicking, string motivoAjuste);
    }
}
