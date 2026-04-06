namespace WIS.AutomationInterpreter.Interfaces
{
    public interface IPtlFactory
    {
        IPtlClientService GetIntegrationService(int cdInterfazExterna);
    }
}
