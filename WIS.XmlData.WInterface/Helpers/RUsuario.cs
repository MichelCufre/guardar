using WIS.Persistence.Database;

namespace WIS.XmlData.WInterface.Helpers
{
    public class RUsuario
    {
        public USERS GetUserByLoginName(WISDB context, string loginName)
        {
            return context.USERS.FirstOrDefault(u => u.LOGINNAME.ToLower().Trim() == loginName.ToLower().Trim());
        }
    }
}
