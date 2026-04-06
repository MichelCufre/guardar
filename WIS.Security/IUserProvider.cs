using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Security
{
    public interface IUserProvider
    {
        BasicUserData GetUserData(int userId);
        BasicUserData GetUserData(string loginName);
    }
}
