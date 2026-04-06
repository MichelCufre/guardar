using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WIS.Configuration;
using WIS.Http;
using WIS.Http.Extensions;
using WIS.Report.Execution;
using WIS.Report.Execution.Serialization;
using WIS.Serialization;
using WIS.WebApplication.Models;
using WIS.WebApplication.Models.Managers;

namespace WIS.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ISessionManager _sessionManager;
        private readonly IWebApiClient _apiClient;
        private readonly string _internalEndpoint;
        private readonly int? _internalTimeout;

        public ReportController(IWebApiClient apiClient, ISessionManager sessionManager, IOptions<ModuleUrl> moduleUrls, IOptions<ApplicationSettings> appSettings)
        {
            this._apiClient = apiClient;
            this._sessionManager = sessionManager;
            this._internalEndpoint = moduleUrls.Value.Internal;
            this._internalTimeout = appSettings.Value.InternalTimeout;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Fetch(ReportRequest request, CancellationToken cancelToken)
        {
            //TODO: Comprobar si usuario puede ver reporte

            var user = this._sessionManager.GetUserInfo();

            if (user == null)
                return Redirect("/api/Security/Logout");

            var transferData = new ReportWrapper
            {
                Application = request.Application,
                User = user.UserId,
                Predio = user.Predio.ToString(),
                SessionData = null
            };

            transferData.SetData(request);

            ReportWrapper responseData = await this.CallService(transferData, cancelToken);

            var response = new ServerResponse();

            if (responseData.Status == TransferWrapperStatus.Error)
            {
                response.SetError(responseData.Message, responseData.MessageArguments);

                return Content(response.Serialize(), "application/json");
            }

            var data = responseData.GetData<ReportContent>();

            //Esto se maneja así por un tema de performance, mantener múltiples descargas en sesión podría generar un memory leak.
            //De esta manera solo se mantiene una descarga por sesión
            _sessionManager.SetValue(DownloadSessionKey.ReportFile, data);

            return Content(response.Serialize(), "application/json");
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Download()
        {
            var download = _sessionManager.GetValue<ReportContent>(DownloadSessionKey.ReportFile);

            if (download != null)
            {
                _sessionManager.SetValue(DownloadSessionKey.ReportFile, null);

                return File(Convert.FromBase64String(download.Content), "application/pdf", download.FileName);
            }

            return NotFound();
        }

        private async Task<ReportWrapper> CallService(ReportWrapper transferData, CancellationToken cancelToken)
        {
            HttpResponseMessage response = await this._apiClient.PostAsync(this._internalEndpoint, this._internalTimeout, "Report/GetReport", transferData, cancelToken);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Error, status: " + response.StatusCode + " - " + response.ReasonPhrase + "-" + await response.Content.ReadAsStringAsync());

            var result = await response.Content.ReadAsStringAndDeserializeAsync<ReportWrapper>();

            return result;
        }
    }
}
