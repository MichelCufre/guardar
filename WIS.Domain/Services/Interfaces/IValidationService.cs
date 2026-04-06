using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.Interfaces;
using WIS.Domain.Picking;
using WIS.Domain.Picking.Dtos;
using WIS.Domain.Produccion;
using WIS.Domain.Recepcion;
using WIS.Domain.Recorridos;
using WIS.Domain.StockEntities;
using WIS.Domain.Tracking.Models;
using WIS.Domain.Validation;

namespace WIS.Domain.Services.Interfaces
{
    public interface IValidationService
    {
        Task<List<Error>> ValidateEmpresa(Empresa empresa, IEmpresaServiceContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidateAgente(Agente agente, IAgenteServiceContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidatePedido(Pedido pedido, IPedidoServiceContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidateProducto(Producto Producto, IProductoServiceContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidateCodigoBarras(CodigoBarras codigoBarras, ICodigoBarrasServiceContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidateProductoProveedor(ProductoProveedor producto, IProductoProveedorServiceContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidateModificarDetalleReferencia(ReferenciaRecepcion referencia, IModificarDetalleReferenciaServiceContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidateAnularReferencia(ReferenciaRecepcion referencia, IAnularReferenciaServiceContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidateReferenciaRecepcion(ReferenciaRecepcion referencia, IReferenciaRecepcionServiceContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidateAgenda(Agenda agenda, IAgendaServiceContext context, out bool errorProcedimiento);
        List<Error> ValidateFiltrosStock(IUnitOfWork uow, FiltrosStock filtros, IStockServiceContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidateModificarPedido(Pedido pedido, IModificarPedidoServiceContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidateAjusteStock(AjusteStock ajuste, IAjustesDeStockServiceContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidateEgreso(Camion egreso, IEgresoServiceContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidateLpn(IUnitOfWork uow, Lpn lpn, ILpnServiceContext context, out bool errorProcedimiento);
        Task<ValidationResult<InterfazEjecucion>> ValidateReprocess(long nroInterfaz, int interfazExterna);
		Task<List<Error>> ValidateTransferencia(TransferenciaStock transferencia, ITransferenciaStockServiceContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidateTransferenciaSaldos(List<TransferenciaStock> transferencias, ITransferenciaStockServiceContext context);
        Task<List<Error>> ValidateCrossDocking(CrossDockingUnaFase detalle, ICrossDockingServiceContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidateDetalleCrossDocking(CrossDockingUnaFase detalle, ICrossDockingServiceContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidateAnularPickingPedidoPendiente(AnularPickingPedidoPendiente detalle, IAnularPickingPedidoPendienteContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidateAnularPickingPedidoPendienteAutomatismo(AnularPickingPedidoPendienteAutomatismo detalle, AnularPickingPedidoPendienteAutomatismoContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidatePicking(DetallePreparacion detalle, IPickingServiceContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidatePickingSaldos(List<DetallePreparacion> pickeos, IPickingServiceContext context);
        bool ValidateMaxItems(ValidationsResult result, int nroRegistro, int count, int max, bool total = false);
        string Translate(Error error);
		Task <List <Error>> ValidateControlCalidad(ControlCalidadAPI controles, IControlCalidadServiceContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidarPuntoEntregaAgente(PuntoEntregaAgentes puntoEntrega, IPuntoEntregaServiceContext context);
        Task<List<Error>> ValidarRutaZona(Ruta ruta, bool nuevaRuta);
        Task<List<Error>> ValidateIngreso(IngresoProduccion ingreso, IProduccionServiceContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidateProducirProduccion(ProducirProduccion produccion, IProducirProduccionServiceContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidateConsumirProduccion(ConsumirProduccion consumo, IConsumirProduccionServiceContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidateUbicacionImportada(UbicacionExterna ubicacion, IUbicacionServiceContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidateDetalleRecorrido(DetalleRecorrido detalle, IRecorridoServiceContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidateSaldosConsumo(ConsumirProduccion consumo, IConsumirProduccionServiceContext context);
        Task<List<Error>> ValidateSaldosProduccion(ProducirProduccion produccion, IProducirProduccionServiceContext context);
        Task<List<Error>> ValidateFactura(Factura factura, IFacturaServiceContext context, out bool errorProcedimiento);
        Task<List<Error>> ValidateUbicacionesPicking(UbicacionPickingProducto Producto, PickingProductoServiceContext context, out bool errorProcedimiento);
    }
}
