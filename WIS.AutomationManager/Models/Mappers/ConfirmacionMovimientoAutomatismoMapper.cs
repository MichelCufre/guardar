using System.Collections.Generic;
using WIS.AutomationManager.Models.Mappers.Interfaces;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.General.API.Dtos.Entrada;
using static WIS.Domain.Automatismo.Dtos.ConfirmacionMovimientoStockRequest;

namespace WIS.AutomationManager.Models.Mappers
{
    public class ConfirmacionMovimientoAutomatismoMapper : IConfirmacionMovimientoAutomatismoMapper
    {
        public TransferenciaStockRequest Map(ConfirmacionMovimientoStockRequest request, string ubicacionOrigen, string ubicacionDestino)
        {
            var transferenciaStock = new TransferenciaStockRequest();

            transferenciaStock.Empresa = request.Empresa;
            transferenciaStock.IdEntrada = request.IdEntrada;
            transferenciaStock.Usuario = request.Usuario;
            transferenciaStock.Transferencias = Map(request.Detalles, request.Empresa, ubicacionOrigen, ubicacionDestino);

            return transferenciaStock;
        }

        public List<TransferenciaRequest> Map(List<ConfirmacionMovimientoStockLineaRequest> request, int empresa, string ubicacionOrigen, string ubicacionDestino)
        {
            var productos = new List<TransferenciaRequest>();
            if (request != null && request.Count > 0)
            {
                foreach (ConfirmacionMovimientoStockLineaRequest item in request)
                {
                    productos.Add(new TransferenciaRequest()
                    {
                        CodigoProducto = item.Producto,
                        Identificador = item.Identificador,
                        Cantidad = item.Cantidad,
                        IdLinea = item.IdLinea,
                        Ubicacion = ubicacionOrigen,
                        UbicacionDestino = ubicacionDestino
                    });

                }
            }
            return productos;
        }

    }
}
