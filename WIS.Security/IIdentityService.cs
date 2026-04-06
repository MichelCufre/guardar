using System;

namespace WIS.Security
{
    public interface IIdentityService
    {
        int UserId { get; set; }
        string Application { get; set; }
        string Predio { get; set; }

        IFormatProvider GetFormatProvider();
    }
}
