using System.Collections.Generic;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.Automatismo.Interfaces;

namespace WIS.AutomationManager.Models.Mappers.Interfaces
{
    public interface IPtlAutomatismoMapper
    {
        PtlResponse Map(IAutomatismo ptl);
        List<PtlResponse> Map(List<IAutomatismo> ptls);
        PtlPosicionEnUso Map(IPtl ptl, PtlActionRequest accion, int nroEjecucion);
        PtlPosicionEnUso Map(IPtl ptl, PtlCommandConfirmRequest command);
        List<PtlCommandConfirmRequest> Map(IPtl ptl, List<PtlPosicionEnUso> ptlPosicionEnUsos);
        PtlCommandConfirmRequest Map(IPtl ptl, PtlPosicionEnUso pos);
    }
}
