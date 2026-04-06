using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Threading.Tasks;
using WIS.Application.Webhook;
using WIS.Exceptions;
using WIS.Webhook.Execution;
using WIS.Webhook.Execution.Serialization;

namespace WIS.BackendService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly IWebhookService _webhookService;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(IWebhookService webhookService, ILogger<WebhookController> logger)
        {
            this._webhookService = webhookService;
            this._logger = logger;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Test(WebhookWrapper wrapper)
        {
            WebhookWrapper response = new WebhookWrapper();

            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", wrapper.User))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", wrapper.Application))
            {
                WebhookRequest request = wrapper.GetData<WebhookRequest>();

                try
                {
                    WebhookContent content = await this._webhookService.Test(wrapper.User, request);

                    response.SetData(content);
                }
                catch (InvalidUserException ex)
                {
                    this._logger.LogError(ex, "Webhook - Test - Unauthenticated");
                    return Unauthorized();
                }
                catch (UserNotAllowedException ex)
                {
                    this._logger.LogError(ex, "Webhook - Test - Forbidden");
                    return Unauthorized();
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "Webhook - Test");
                    response.SetError(ex.Message);
                }

                return Ok(response);
            }
        }
    }
}
