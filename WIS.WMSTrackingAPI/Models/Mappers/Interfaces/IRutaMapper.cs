using System.Collections.Generic;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Picking;
using WIS.Domain.Tracking.Models;

namespace WIS.WMSTrackingAPI.Models.Mappers.Interfaces
{
    public interface IRutaMapper
    {
        Ruta Map(RutaZonaRequest request);
        RutaResponse MapToResponse(Ruta ruta);
    }
}
