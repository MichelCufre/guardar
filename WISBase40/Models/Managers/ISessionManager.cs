using WIS.Security.Models;

namespace WIS.WebApplication.Models.Managers
{
    public interface ISessionManager
    {
        void SetValue(string key, object value);
        object GetValue(string key);
        T GetValue<T>(string key);
        Usuario GetUserInfo();
    }
}
