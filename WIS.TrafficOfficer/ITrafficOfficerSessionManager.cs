namespace WIS.TrafficOfficer
{
    public interface ITrafficOfficerSessionManager
    {
        bool IsSessionValid(string token);
    }
}
