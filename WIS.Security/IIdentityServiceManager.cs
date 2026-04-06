namespace WIS.Security
{
    public interface IIdentityServiceManager
    {
        void SetUser(BasicUserData user, string application, string predio);
    }
}
