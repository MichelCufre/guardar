using System.Collections.Generic;
using WIS.Domain.General.API.Dtos.Salida;
using WIS.Domain.Interfaces;

namespace WIS.WMS_API.Models.Mappers.Interfaces
{
    public interface IEjecucionMapper
    {
        EjecucionesPendientesResponse MapToResponse(List<InterfazEjecucion> interfaces);
        EstadoEjecucionResponse MapToResponse(InterfazEstado estado);
    }
}
