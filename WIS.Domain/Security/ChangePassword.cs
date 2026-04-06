using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Security
{
    public class ChangePassword
    {
        public string LoginName { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
