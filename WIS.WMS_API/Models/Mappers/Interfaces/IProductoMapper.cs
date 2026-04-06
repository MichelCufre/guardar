using System.Collections.Generic;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;

namespace WIS.WMS_API.Models.Mappers.Interfaces
{
    public interface IProductoMapper
    {
        List<ProductoProveedor> Map(ProductosProveedorRequest request);
        List<Producto> Map(ProductosRequest request);
        ProductoResponse MapToResponse(Producto producto);
        ProductoProveedorResponse MapToResponse(ProductoProveedor productoProveedor, string tipoAgente, string codigoAgente);
    }
}
