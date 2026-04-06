using System.Collections.Generic;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Picking;
using WIS.Domain.Tracking.Models;

namespace WIS.WMSTrackingAPI.Models.Mappers.Interfaces
{
    public interface IPuntoEntregaMapper
    {
        PuntoEntregaAgentes Map(PuntoEntregaAgentesRequest request);
    }
}
