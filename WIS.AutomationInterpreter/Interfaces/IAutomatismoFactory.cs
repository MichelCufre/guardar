namespace WIS.AutomationInterpreter.Interfaces
{
    public interface IAutomatismoFactory
    {
        IAutoStoreClientService GetIntegrationService(int cdInterfazExterna);
    }
}
