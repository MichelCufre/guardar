using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WIS.Authentication
{
    public interface IAuthContainerModel
    {
        string SecretKey { get; set; }
        string SecurityAlgorithm { get; set; }
        int ExpireMinutes { get; set; }

        List<Claim> Claims { get; set; }
    }
}
