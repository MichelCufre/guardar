using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using WIS.Exceptions;
using WIS.Security;
using WIS.Security.Serialization;

namespace WIS.BackendService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : Controller
    {
        private readonly ISecurityService _securityService;
        private readonly ILogger<SecurityController> _logger;

        public SecurityController(ISecurityService securityService, ILogger<SecurityController> logger)
        {
            this._securityService = securityService;
            this._logger = logger;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult UpdateUserLanguage(SecurityWrapper wrapper)
        {
            SecurityWrapper response = new SecurityWrapper();

            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", wrapper.User))
            {
                try
                {
                    SecurityRequest request = wrapper.GetData<SecurityRequest>();

                    this._securityService.UpdateUserLanguage(wrapper.User, request.Language);

                    response.SetData(new SecurityContent());
                }
                catch (InvalidUserException ex)
                {
                    this._logger.LogError(ex, "Security - UpdateUserLanguage - Unauthenticated");
                    return Unauthorized();
                }
                catch (UserNotAllowedException ex)
                {
                    this._logger.LogError(ex, "Security - UpdateUserLanguage - Forbidden");
                    return Unauthorized();
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "Security - UpdateUserLanguage");
                    response.SetError(ex.Message);
                }

                return Ok(response);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult GetUserByUsername(SecurityWrapper wrapper)
        {
            SecurityWrapper response = new SecurityWrapper();

            try
            {
                SecurityRequest request = wrapper.GetData<SecurityRequest>();
                SecurityContent content = new SecurityContent(); 
                
                content.Usuario = this._securityService.GetUser(request);

                response.SetData(content);
            }
            catch (InvalidUserException ex)
            {
                this._logger.LogError(ex, "Security - GetUserByUsername - Unauthenticated");
                return Unauthorized();
            }
            catch (UserNotAllowedException ex)
            {
                this._logger.LogError(ex, "Security - GetUserByUsername - Forbidden");
                return Unauthorized();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Security - GetUserByUsername");
                response.SetError(ex.Message);
            }

            return Ok(response);
        }
    }
}
