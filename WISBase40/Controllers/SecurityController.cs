using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WIS.Configuration;
using WIS.Http;
using WIS.Http.Extensions;
using WIS.Security;
using WIS.Security.Models;
using WIS.Security.Serialization;
using WIS.Serialization;
using WIS.Translation;
using WIS.WebApplication.ActionFilters;
using WIS.WebApplication.Models;
using WIS.WebApplication.Models.Managers;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WIS.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : BaseController
    {
        private readonly ISessionManager _sessionManager;
        private readonly IWebApiClient _apiClient;
        private readonly ITrafficOfficerFrontendService _trafficOfficer;
        private readonly string _internalEndpoint;
        private readonly int? _internalTimeout;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public SecurityController(
            ISessionManager sessionManager,
            IOptions<ModuleUrl> moduleUrls,
            IWebApiClient apiClient,
            ITrafficOfficerFrontendService trafficOfficer,
            IOptions<ApplicationSettings> appSettings
        )
        {
            this._sessionManager = sessionManager;
            this._apiClient = apiClient;
            this._trafficOfficer = trafficOfficer;
            this._internalEndpoint = moduleUrls.Value.Internal;
            this._internalTimeout = appSettings.Value.InternalTimeout;
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize, CheckAuthorization]
        public async Task<IActionResult> GetUserData()
        {
            Usuario user = this._sessionManager.GetUserInfo();

            if (user == null)
                return Redirect("/api/Security/Logout");

            UserData data = new UserData
            {
                UserName = user.Name,
                Language = user.Language,
                Predio = user.Predio
            };

            data.Predios.AddRange(user.Predios.Select(p => p.Numero).OrderBy(p => p));

            return Content(JsonConvert.SerializeObject(data), "application/json");
        }

        [HttpGet]
        [Route("[action]")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            try
            {
                HttpContext.Session.Clear();
                // RedirectUri does not replace CallbackPath, they serve different purposes.
                // AuthServer returns to the CallbackPath, finishes authenticating, and then if auth was successful it redirects to the RedirectUri.
                return Challenge(new AuthenticationProperties { RedirectUri = "/Default" }, OpenIdConnectDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, ex.Message);
            }

            return Ok();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await this._trafficOfficer.DeleteUserLocks(default);
                await this._trafficOfficer.RemoveSession(default);
            }
            catch (Exception ex)
            { 
                this._logger.Error(ex, ex.Message);
            }

            HttpContext.Session.Clear();

            return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize, CheckAuthorization]
        public async Task<IActionResult> UpdateUserLanguage([FromBody] TranslationLanguage data, CancellationToken cancelToken)
        {
            var response = new ServerResponse();

            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", User))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", "System"))
            {
                try
                {
                    Usuario user = this._sessionManager.GetUserInfo();

                    if (user == null)
                        return Redirect("/api/Security/Logout");

                    var transferData = new SecurityWrapper
                    {
                        User = user.UserId,
                        PageToken = ""
                    };

                    transferData.SetData(new SecurityRequest()
                    {
                        UserId = user.UserId,
                        Language = data.Language
                    });

                    HttpResponseMessage postResponse = await this._apiClient.PostAsync(this._internalEndpoint, this._internalTimeout, "Security/UpdateUserLanguage", transferData, cancelToken);

                    if (!postResponse.IsSuccessStatusCode)
                        throw new InvalidOperationException("Error, status: " + postResponse.StatusCode + " - " + postResponse.ReasonPhrase + "-" + await postResponse.Content.ReadAsStringAsync());

                    var result = await postResponse.Content.ReadAsStringAndDeserializeAsync<SecurityWrapper>();

                    if (result.Status == TransferWrapperStatus.Error)
                        response.SetError("General_Sec0_Error_Error60");

                    //Actualizar información de usuario en sesión
                    user.Language = data.Language;

                    this._sessionManager.SetValue(SessionManager.UserInfo, user);
                }
                catch (Exception ex)
                {
                    this._logger.Error(ex, "WebApplication SecurityController - UpdateUserLanguage");
                    response.SetError("General_Sec0_Error_Error60");
                }

                return Content(response.Serialize(), "application/json");
            }
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize, CheckAuthorization]
        public async Task<IActionResult> SetUserPredio([FromBody] SetPredioRequest request, CancellationToken cancelToken)
        {
            var response = new ServerResponse();

            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", User))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", "System"))
            {
                try
                {
                    Usuario user = this._sessionManager.GetUserInfo();

                    if (user == null)
                        return Redirect("/api/Security/Logout");

                    if (user.Predios.Count == 1 && user.Predio != request.Predio
                        || user.Predios.Count > 1 && request.Predio != "S/D" && !user.Predios.Select(p => p.Numero).Contains(request.Predio))
                        return Unauthorized();

                    //Actualizar información de usuario en sesión
                    user.Predio = request.Predio;

                    this._sessionManager.SetValue(SessionManager.UserInfo, user);
                }
                catch (Exception ex)
                {
                    this._logger.Error(ex, "WebApplication SecurityController - SelectPredio");
                    response.SetError("General_Sec0_Error_Error60");
                }

                return Content(response.Serialize(), "application/json");
            }
        }
    }
}
