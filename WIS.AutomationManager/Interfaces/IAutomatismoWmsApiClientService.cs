using System.Collections.Generic;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;

namespace WIS.AutomationManager.Interfaces
{
    public interface IAutomatismoWmsApiClientService
    {
        ValidationsResult SepararProductoPreparacion(int userId, int preparacion, int contenedor, int contenedorVirtual, string cdEndereco, int cD_EMPRESA, string cD_PRODUCTO, decimal cantidadIngresada, int? tipoAgrupacion, string comparteAgrupacion, string tipoEtiqueta);
        ValidationsResult CambiarContenedor(int contenedorOrigen, int userId, string cdEndereco, int contenedorDestino, int tipoOperacion, int? tipoAgrupacion, string comparteAgrupacion, string tipoEtiqueta);
        ValidationsResult SepararCrossDocking(int userId, int preparacion, int agenda, string codigoAgente, string tipoAgente, int contenedorDestino, string ubicacion, int empresa, string producto, string lote, decimal cantidadIngresada, string tipoEtiqueta);
        ValidationsResult NotificarAjuste(AjustesDeStockRequest ajuste);
        ValidationsResult Picking(List<DetallePickingRequest> picking, int empresa);
        ValidationsResult ConfirmarEntrada(TransferenciaStockRequest entradas);
        ValidationsResult ConfirmarSalida(PickingRequest salidas);
        ValidationsResult ConfirmarAnulacionesPendientes(AnularPickingPedidoPendienteRequest anulaciones);
        ValidationsResult ConfirmarMovimiento(TransferenciaStockRequest entradas);
    }
}
