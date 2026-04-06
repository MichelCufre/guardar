using WIS.Domain.Automatismo.Interfaces;

namespace WIS.AutomationManager.Interfaces
{
    public interface IAutomatismoNotificationFactory
    {
        IAutomatismoNotificationService Create(IAutomatismo automatismo);
    }
}
