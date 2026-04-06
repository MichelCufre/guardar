using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WIS.Authentication
{
    public interface IAuthService
    {
        bool IsTokenValid(string token);
        string GenerateToken(IAuthContainerModel model);
        List<Claim> GetTokenClaims(string token);
        TokenValidationParameters GetTokenValidationParameters();
    }
}
