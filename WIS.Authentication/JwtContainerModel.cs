using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WIS.Authentication
{
    public class JwtContainerModel : IAuthContainerModel
    {
        public string SecretKey { get; set; }
        public string SecurityAlgorithm { get; set; } = SecurityAlgorithms.HmacSha256Signature;
        public int ExpireMinutes { get; set; } = 10080; //7 dias
        public List<Claim> Claims { get; set; } = new List<Claim>();
    }
}
