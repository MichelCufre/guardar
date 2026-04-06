using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.Integracion.Dtos;

namespace WIS.AutomationInterpreter.Interfaces
{
    public interface IPtlClientService
    {
        PtlCommandResponse TurnLigthOnOrOff(AutomatismoInterpreterRequest request);

        PtlCommandResponse ResetOfOperation(AutomatismoInterpreterRequest request);

        PtlCommandResponse StartOfOperation(AutomatismoInterpreterRequest request);
    }
}
