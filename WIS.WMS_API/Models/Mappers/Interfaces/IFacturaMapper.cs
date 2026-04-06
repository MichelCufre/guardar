using System.Collections.Generic;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.General.API.Dtos.Salida;
using WIS.Domain.Recepcion;

namespace WIS.WMS_API.Models.Mappers.Interfaces
{
    public interface IFacturaMapper
    {
        List<Factura> Map(FacturasRequest request);
        FacturaResponse MapToResponse(Factura factura);
    }
}
