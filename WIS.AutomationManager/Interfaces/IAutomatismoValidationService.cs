using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.AutomationManager.Services;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Validation;

namespace WIS.AutomationManager.Interfaces
{
    public interface IAutomatismoValidationService
    {
        Task<ValidationsResult> ValidateAutomatismo(IUnitOfWork uow, string zona);
        Task<ValidationsResult> ValidateEnvioInterfazByCodigo(IUnitOfWork uow, string codigo, int cdInterfaz);
        Task<ValidationsResult> ValidateEnvioInterfaz(IUnitOfWork uow, string zona, int cdInterfaz);
        Task<(ValidationsResult, Dictionary<string, IAutomatismo>)> ValidateEnvioInterfaz(IUnitOfWork uow, IEnumerable<string> zonas, int cdInterfaz);
        Task<ValidationsResult> ValidarEnvioInterfazByPuesto(IUnitOfWork uow, string puesto, int cdInterfaz);
        List<Error> ValidateProductoAutomatismo(ProductoAutomatismoRequest request, AutomatismoProductoServiceContext context, out bool errorProcedimiento);
        List<Error> ValidateCodigoBarrasAutomatismo(CodigoBarraAutomatismoRequest request, AutomatismoCodigoBarraServiceContext context, out bool errorProcedimiento);
        List<Error> ValidateEntradaAutomatismo(EntradaStockAutomatismoRequest request, AutomatismoEntradaServiceContext context, out bool errorProcedimiento);
        List<Error> ValidateSalidaAutomatismo(SalidaStockLineaAutomatismoRequest request, AutomatismoSalidaServiceContext context, out bool errorProcedimiento);
        List<Error> ValidateUbicacionPickingAutomatismo(UbicacionPickingAutomatismoRequest request, AutomatismoUbicacionPickingServiceContext context, out bool errorProcedimiento);
        List<Error> ValidateNotificacionAjustesAutomatismo(AjustesDeStockRequest request, AutomatismoNotificacionAjusteStockServiceContext context, out bool errorProcedimiento);
        List<Error> ValidateConfirmacionEntradaAutomatismo(TransferenciaStockRequest request, AutomatismoConfirmacionEntradaServiceContext context, out bool errorProcedimiento);
        List<Error> ValidateConfirmacionSalidaAutomatismo(PickingRequest request, AutomatismoConfirmacionSalidaServiceContext context, out bool errorProcedimiento);
        List<Error> ValidateConfirmacionMovimientoAutomatismo(TransferenciaStockRequest request, AutomatismoConfirmacionMovimientoServiceContext context, out bool errorProcedimiento);
    }
}
