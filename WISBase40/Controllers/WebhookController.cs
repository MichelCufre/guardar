using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WIS.Configuration;
using WIS.Http;
using WIS.Http.Extensions;
using WIS.Serialization;
using WIS.WebApplication.Models;
using WIS.WebApplication.Models.Managers;
using WIS.Webhook.Execution;
using WIS.Webhook.Execution.Serialization;

namespace WIS.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : Controller
    {
        private readonly ISessionManager _sessionManager;
        private readonly IWebApiClient _apiClient;
        private readonly string _internalEndpoint;
        private readonly int? _internalTimeout;

        public WebhookController(IWebApiClient apiClient, ISessionManager sessionManager, IOptions<ModuleUrl> moduleUrls, IOptions<ApplicationSettings> appSettings)
        {
            this._apiClient = apiClient;
            this._sessionManager = sessionManager;
            this._internalEndpoint = moduleUrls.Value.Internal;
            this._internalTimeout = appSettings.Value.InternalTimeout;
        }

        [HttpPost("[action]")]
        public IActionResult Test(WebhookRequest request)
        {
            var user = this._sessionManager.GetUserInfo();

            if (user == null)
                return Redirect("/api/Security/Logout");

            var transferData = new WebhookWrapper
            {
                Application = request.Application,
                User = user.UserId,
                Predio = user.Predio.ToString(),
                SessionData = null
            };

            transferData.SetData(request);

            var task = this.CallService(transferData, new CancellationToken());
            task.Wait();

            WebhookWrapper responseData = task.Result;

            var response = new ServerResponse();

            if (responseData.Status == TransferWrapperStatus.Error)
            {
                response.SetError(responseData.Message, responseData.MessageArguments);

                return Content(response.Serialize(), "application/json");
            }

            return Ok(responseData.GetData<WebhookContent>());
        }

        private async Task<WebhookWrapper> CallService(WebhookWrapper transferData, CancellationToken cancelToken)
        {
            HttpResponseMessage response = await this._apiClient.PostAsync(this._internalEndpoint, this._internalTimeout, "Webhook/Test", transferData, cancelToken);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Error, status: " + response.StatusCode + " - " + response.ReasonPhrase + "-" + await response.Content.ReadAsStringAsync());

            var result = await response.Content.ReadAsStringAndDeserializeAsync<WebhookWrapper>();

            return result;
        }
    }
}
