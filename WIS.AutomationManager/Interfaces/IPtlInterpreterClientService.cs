using System.Collections.Generic;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.General;

namespace WIS.AutomationManager.Interfaces
{
    public interface IPtlInterpreterClientService
    {
        ValidationsResult TurnLigthOnOrOff(IPtl ptl, List<PtlPosicionEnUso> accion, bool isOn);
        ValidationsResult StartOfOperation(IPtl ptl);
        ValidationsResult ResetOfOperation(IPtl ptl);
    }
}
