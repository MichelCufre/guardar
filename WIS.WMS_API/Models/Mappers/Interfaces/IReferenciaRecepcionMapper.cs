using System.Collections.Generic;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Recepcion;

namespace WIS.WMS_API.Models.Mappers.Interfaces
{
    public interface IReferenciaRecepcionMapper
    {
        List<ReferenciaRecepcion> Map(ReferenciasRecepcionRequest request);
        ReferenciaRecepcionResponse MapToResponse(ReferenciaRecepcion referencia, string tipoAgente, string codigoAgente);
        ReferenciaRecepcionDetalleResponse MapDetalleToResponse(ReferenciaRecepcionDetalle det);
        List<ReferenciaRecepcion> Map(ModificacionDetalleReferenciaRequest request);
        List<ReferenciaRecepcion> Map(AnularReferenciasRequest request);
    }
}