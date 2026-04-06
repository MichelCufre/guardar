using System.Collections.Generic;
using System.Linq;
using WIS.AutomationManager.Models.Mappers.Interfaces;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.General.API.Dtos.Entrada;

namespace WIS.AutomationManager.Models.Mappers
{
	public class ConfirmacionEntradaAutomatismoMapper : IConfirmacionEntradaAutomatismoMapper
	{
		public TransferenciaStockRequest Map(ConfirmacionEntradaStockRequest request)
		{
			var transferenciaStock = new TransferenciaStockRequest();

			transferenciaStock.Empresa = request.Empresa;
			transferenciaStock.IdEntrada = request.IdEntrada;
			transferenciaStock.Usuario = request.Usuario;
			transferenciaStock.Transferencias = Map(request.Detalles, request.Empresa);

			return transferenciaStock;
		}

        public List<TransferenciaRequest> Map(List<ConfirmacionEntradaStockLineaRequest> request, int empresa)
        {
            var productos = new List<TransferenciaRequest>();
            if (request != null && request.Count > 0)
            {
                foreach (ConfirmacionEntradaStockLineaRequest item in request)
                {
                    productos.Add(Map(item, empresa));
                }
            }

            return productos;
        }

        public TransferenciaRequest Map(ConfirmacionEntradaStockLineaRequest request, int empresa)
        {
            return new TransferenciaRequest()
            {
                CodigoProducto = request.Producto,
                Identificador = request.Identificador,
                Cantidad = request.Cantidad,
                CantidadSolicitada = request.CantidadSolicitada,
                IdLinea = request.IdLinea
            };
        }
    }
}
