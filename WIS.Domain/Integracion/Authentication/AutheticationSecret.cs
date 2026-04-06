using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace WIS.Domain.Integracion.Authentication
{
    public class AutheticationSecret : IAuthenticationMethod
    {
        private string _secret { get; set; }

        public AutheticationSecret(string secret)
        {
            _secret = secret;
        }

        public AuthenticationHeaderValue GetAuthorizationHeaderValue(IHttpContextAccessor httpContextAccessor)
        {
            return null;
        }
    }
}
