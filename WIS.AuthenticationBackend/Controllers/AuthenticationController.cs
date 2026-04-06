using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WIS.AuthenticationBackend.Model;

namespace WIS.AuthenticationBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger _logger;

        public AuthenticationController(IAuthenticationService authenticationService, IUnitOfWork uow, ILogger<AuthenticationController> logger)
        {
            this._uow = uow;
            this._authenticationService = authenticationService;
            this._logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate(AuthenticationRequest request)
        {
            try
            {
                _logger.LogDebug($"Authenticating user {request.Username}...");

                AuthenticationResponse user = await this._authenticationService.Authenticate(request);

                _logger.LogDebug($"Authentication success for user {request.Username}");

                return Content(JsonConvert.SerializeObject(user));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogDebug(ex, $"Invalid operation for user {request.Username}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal server error for user {request.Username}");
                return StatusCode(500);
            }
        }
    }
}
