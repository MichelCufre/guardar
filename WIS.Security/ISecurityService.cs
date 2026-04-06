using System.Collections.Generic;
using WIS.Security.Models;

namespace WIS.Security
{
    public interface ISecurityService
    {
        bool CanUserAccessApplication();
        bool IsUserAllowed(string resource);
        bool IsEmpresaAllowed(int cdEmpresa);
        Dictionary<string, bool> CheckPermissions(List<string> resources);
        void UpdateUserLanguage(int userid, string language);
        Usuario GetUser(SecurityRequest userInfo);
        bool IsValidPassword(string password, string hashedPassword, string salt);
    }
}
