using System.Collections.Generic;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Interfaces;
using WIS.Domain.Picking;
using WIS.Domain.Produccion;

namespace WIS.WMS_API.Models.Mappers.Interfaces
{
    public interface IProduccionMapper
    {
        List<IngresoProduccion> Map(ProduccionesRequest request, InterfazEjecucion ejecucion);
        ProduccionResponse MapToResponse(IngresoProduccion ingreso);
    }
}
