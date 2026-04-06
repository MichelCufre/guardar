using System.Collections.Generic;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Picking;

namespace WIS.WMS_API.Models.Mappers.Interfaces
{
    public interface IPedidoMapper
    {
        List<Pedido> Map(PedidosRequest request);
        List<Pedido> Map(ModificarPedidosRequest request);
        PedidoDetalleResponse MapDetalleToResponse(DetallePedido det);
        PedidoResponse MapToResponse(Pedido pedido, string tipoAgente, string codigoAgente);
    }
}
