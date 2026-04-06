using System;
using System.Linq;
using WIS.AutomationManager.Models.Mappers.Interfaces;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.General.API.Dtos.Entrada;

namespace WIS.AutomationManager.Models.Mappers
{
    public class NotificacionAjusteStockAutomatismoMapper : INotificacionAjusteStockAutomatismoMapper
    {
        public AjustesDeStockRequest Map(NotificacionAjustesStockRequest request)
        {
            var ajusteStockRequest = new AjustesDeStockRequest();

            ajusteStockRequest.Archivo = request.Archivo;
            ajusteStockRequest.Empresa = request.Empresa;
            ajusteStockRequest.DsReferencia = request.DsReferencia;
            ajusteStockRequest.Usuario = request.Usuario;

            ajusteStockRequest.Ajustes = request.Ajustes
                .Select(a => new AjusteStockRequest
                {
                    Producto = a.Producto,
                    Identificador = a.Identificador,
                    FechaVencimiento = a.FechaVencimiento,
                    MotivoAjuste = a.Causa,
                    TipoAjuste = "AA",
                    FechaMotivo = DateTime.Now,
                    DescripcionMotivo = a.Causa,
                    Cantidad = a.Cantidad,
                })
                .ToList();

            return ajusteStockRequest;
        }
        public AjustesDeStockRequest Map(ConfirmacionMovimientoStockRequest request, string ubicacionPicking, string motivoAjuste)
        {
            var ajusteStockRequest = new AjustesDeStockRequest();

            ajusteStockRequest.Archivo = request.Archivo;
            ajusteStockRequest.Empresa = request.Empresa;
            ajusteStockRequest.DsReferencia = request.DsReferencia;
            ajusteStockRequest.Usuario = request.Usuario;

            ajusteStockRequest.Ajustes = request.Detalles
                .Select(a => new AjusteStockRequest
                {
                    Producto = a.Producto,
                    Identificador = a.Identificador,
                    FechaVencimiento = a.FechaVencimiento,
                    MotivoAjuste = motivoAjuste,
                    TipoAjuste = "AA",
                    FechaMotivo = DateTime.Now,
                    DescripcionMotivo = "Ajuste Automatismo",
                    Cantidad = a.Cantidad,
                    Ubicacion = ubicacionPicking
                }).ToList();

            return ajusteStockRequest;
        }
    }
}
