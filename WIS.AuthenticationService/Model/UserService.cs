using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using WIS.Authentication;
using WIS.Http;

namespace WIS.AuthenticationService.Model
{
    public class UserService : IUserService
    {
        private readonly IWebApiClient _client;
        private readonly IAuthService _authService;
        private readonly IOptions<AppSettings> _settings;

        public UserService(IWebApiClient client, IAuthService authService, IOptions<AppSettings> settings)
        {
            this._client = client;
            this._authService = authService;
            this._settings = settings;
        }

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest request, CancellationToken cancelToken)
        {
            HttpResponseMessage response = await this._client.PostAsync(new Uri(new Uri(this._settings.Value.ServiceUrl), "authentication"), request, cancelToken);

            string content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException(content);

            User user = JsonConvert.DeserializeObject<User>(content);

            return new AuthenticateResponse(user,  this._authService.GenerateToken(new JwtContainerModel
            {
                Claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, Convert.ToString(user.UserId)),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email)
                }
            }));
        }
    }
}
