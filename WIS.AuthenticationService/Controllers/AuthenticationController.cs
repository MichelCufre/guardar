using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WIS.AuthenticationService.Model;

namespace WIS.AuthenticationService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private IUserService _userService;
        private ILogger _logger;

        public AuthenticationController(IUserService userService, ILogger<AuthenticationController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Ok("Ready");
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody]AuthenticateRequest model, CancellationToken cancelToken)
        {
            try
            {
                _logger.LogDebug($"Authenticating user {model.Username}...");

                var response = await _userService.Authenticate(model, cancelToken);

                _logger.LogDebug($"Authentication success for user {model.Username}");

                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogDebug(ex, $"Invalid operation for user {model.Username}");
                return BadRequest("Username or password is incorrect");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal server error for user {model.Username}");
                return StatusCode(500, "Internal server error");
            }            
        }
    }
}
