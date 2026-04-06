using WIS.Automation;
using WIS.AutomationManager.Services;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;

namespace WIS.AutomationManager.Interfaces
{
    public interface IAutomatismoNotificationService
    {
        AutomatismoResponse NotificarProductos(IAutomatismo automatismo, ProductosAutomatismoRequest request);
        AutomatismoResponse NotificarCodigosBarras(IAutomatismo automatismo, CodigosBarrasAutomatismoRequest request);
        AutomatismoResponse NotificarEntrada(IAutomatismo automatismo, EntradaStockAutomatismoRequest request);
        AutomatismoResponse NotificarSalida(IAutomatismo automatismo, SalidaStockAutomatismoRequest request);
        ValidationsResult ProcesarNotificacionAjustes(IUnitOfWork uow, IAutomatismo automatimo, AjustesDeStockRequest request);
        ValidationsResult ProcesarConfirmacionEntrada(IUnitOfWork uow, IAutomatismo automatimo, TransferenciaStockRequest request, EntradaStockAutomatismoRequest entradaNotificada, AutomatismoConfirmacionEntradaServiceContext context);
        ValidationsResult ProcesarConfirmacionSalida(IUnitOfWork uow, IAutomatismo automatimo, PickingRequest request);
        ValidationsResult ProcesarConfirmacionMovimiento(IUnitOfWork uow, IAutomatismo automatimo, TransferenciaStockRequest request, AutomatismoConfirmacionMovimientoServiceContext context);
    }
}
