using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.Security;

namespace WIS.Domain.Services.Interfaces
{
    public interface IAuthorizationService
    {
        void ChangePassword(ChangePassword model);
        void ValidateCurrentPassword(ValidateCurrentPassword model);
    }
}
