using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace WIS.Domain.Integracion.Authentication
{
    public interface IAuthenticationMethod
    {
        AuthenticationHeaderValue GetAuthorizationHeaderValue(IHttpContextAccessor httpContextAccessor);
    }
}
