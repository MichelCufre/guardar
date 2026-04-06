using WIS.Domain.Automatismo.Interfaces;

namespace WIS.AutomationManager.Interfaces
{
    public interface IPtlFactory
    {
        IPtl GetPtl(IAutomatismo automatismo);
        IPtlService GetService(IPtl ptl);
        IPtlService GetService(string tipoPtl);
    }
}
