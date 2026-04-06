using MigraDoc.DocumentObjectModel.Shapes;
using System.Collections.Generic;
using WIS.AutomationManager.Models.Mappers.Interfaces;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.API.Dtos.Entrada;
using static WIS.Domain.Automatismo.Dtos.ConfirmacionSalidaStockRequest;

namespace WIS.AutomationManager.Models.Mappers
{
    public class SalidaStockAutomatismoMapper : ISalidaStockAutomatismoMapper
    {
        public PickingRequest Map(ConfirmacionSalidaStockRequest request)
        {
            var picking = new PickingRequest()
            {
                Empresa = request.Empresa,
            };

            foreach (var i in request.Detalles)
            {
                picking.Detalles.Add(new DetallePickingRequest()
                {
                    Preparacion = request.Preparacion,
                    Pedido = request.Pedido,
                    CodigoAgente = request.CodigoAgente,
                    TipoAgente = request.TipoAgente,
                    CodigoProducto = i.Producto,
                    //Identificador = i.Identificador,
                    //Cantidad = i.Cantidad,
                    //IdExternoContenedor = i.Contenedor.ToString(),
                    TipoContenedor = BarcodeDb.TIPO_CONTENEDOR_W,
                });
            }
            return picking;
        }
    }
}
