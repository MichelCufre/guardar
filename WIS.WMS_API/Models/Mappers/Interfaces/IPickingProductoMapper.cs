using System.Collections.Generic;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;

namespace WIS.WMS_API.Models.Mappers.Interfaces
{
    public interface IPickingProductoMapper
    {
        List<UbicacionPickingProducto> Map(PickingProductosRequest request);
        PickingProductoResponse MapToResponse(UbicacionPickingProducto codigoBarras);
    }
}
