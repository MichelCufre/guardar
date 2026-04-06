using WIS.Automation;
using WIS.AutomationManager.Interfaces;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;

namespace WIS.AutomationManager.Services
{
    public abstract class AutomatismoNotificationService : IAutomatismoNotificationService
    {
        protected readonly IAutomatismoInterpreterClientService _interpreterService;
        protected readonly IAutomatismoWmsApiClientService _wmsApiClientService;
        public AutomatismoNotificationService(
            IAutomatismoWmsApiClientService wmsApiClientService,
            IAutomatismoInterpreterClientService interpreterService)
        {
            _interpreterService = interpreterService;
            _wmsApiClientService = wmsApiClientService;
        }

        public virtual AutomatismoResponse CallInterpreterSendProductos(IAutomatismo automatismo, ProductosAutomatismoRequest request)
        {
            return _interpreterService.SendProductos(automatismo, request);
        }

        public virtual AutomatismoResponse CallInterpreterSendCodigosBarras(IAutomatismo automatismo, CodigosBarrasAutomatismoRequest request)
        {
            return _interpreterService.SendCodigosBarras(automatismo, request);
        }

        public virtual AutomatismoResponse CallInterpreterSendEntrada(IAutomatismo automatismo, EntradaStockAutomatismoRequest request)
        {
            return _interpreterService.SendEntrada(automatismo, request);
        }

        public virtual AutomatismoResponse CallInterpreterSendSalida(IAutomatismo automatismo, SalidaStockAutomatismoRequest request)
        {
            return _interpreterService.SendSalida(automatismo, request);
        }

        public virtual ValidationsResult CallWMSSendAjustes(IAutomatismo automatismo, AjustesDeStockRequest ajustes)
        {
            return _wmsApiClientService.NotificarAjuste(ajustes);
        }

        public virtual ValidationsResult CallWMSSendEntradas(IAutomatismo automatismo, TransferenciaStockRequest entradas)
        {
            return _wmsApiClientService.ConfirmarEntrada(entradas);
        }

        public virtual ValidationsResult CallWMSSendSalidas(IAutomatismo automatismo, PickingRequest salidas)
        {
            return _wmsApiClientService.ConfirmarSalida(salidas);
        }

        public virtual ValidationsResult CallWMSSendAnularPendiente(IAutomatismo automatismo, AnularPickingPedidoPendienteRequest anulaciones)
        {
            return _wmsApiClientService.ConfirmarAnulacionesPendientes(anulaciones);
        }

        public virtual ValidationsResult CallWMSSendMovimiento(IAutomatismo automatismo, TransferenciaStockRequest entradas)
        {
            return _wmsApiClientService.ConfirmarMovimiento(entradas);
        }

        public abstract AutomatismoResponse NotificarCodigosBarras(IAutomatismo automatismo, CodigosBarrasAutomatismoRequest request);

        public abstract ValidationsResult ProcesarNotificacionAjustes(IUnitOfWork uow, IAutomatismo automatimo, AjustesDeStockRequest ajustes);

        public abstract ValidationsResult ProcesarConfirmacionEntrada(IUnitOfWork uow, IAutomatismo automatimo, TransferenciaStockRequest entradas, EntradaStockAutomatismoRequest entradaNotificada, AutomatismoConfirmacionEntradaServiceContext context);

        public abstract ValidationsResult ProcesarConfirmacionSalida(IUnitOfWork uow, IAutomatismo automatimo, PickingRequest salidas);

        public abstract ValidationsResult ProcesarConfirmacionMovimiento(IUnitOfWork uow, IAutomatismo automatimo, TransferenciaStockRequest entradas, AutomatismoConfirmacionMovimientoServiceContext context);

        public abstract AutomatismoResponse NotificarEntrada(IAutomatismo automatismo, EntradaStockAutomatismoRequest request);

        public abstract AutomatismoResponse NotificarProductos(IAutomatismo automatismo, ProductosAutomatismoRequest request);

        public abstract AutomatismoResponse NotificarSalida(IAutomatismo automatismo, SalidaStockAutomatismoRequest request);

    }
}
