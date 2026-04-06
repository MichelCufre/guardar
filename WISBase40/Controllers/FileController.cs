using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WIS.Configuration;
using WIS.File.Execution;
using WIS.File.Execution.Serialization;
using WIS.FormComponent.Execution.Configuration;
using WIS.Http;
using WIS.Http.Extensions;
using WIS.Serialization;
using WIS.WebApplication.Models;
using WIS.WebApplication.Models.Managers;

namespace WIS.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : BaseController
    {
        private readonly ISessionManager _sessionManager;
        private readonly IWebApiClient _apiClient;
        private readonly string _internalEndpoint;
        private readonly int? _internalTimeout;

        public FileController(IWebApiClient apiClient, ISessionManager sessionManager, IOptions<ModuleUrl> moduleUrls, IOptions<ApplicationSettings> appSettings)
        {
            this._apiClient = apiClient;
            this._sessionManager = sessionManager;
            this._internalEndpoint = moduleUrls.Value.Internal;
            this._internalTimeout = appSettings.Value.InternalTimeout;
        }

        [HttpGet("[action]")]
        public IActionResult Download(string fileId, string application)
        {
            var user = this._sessionManager.GetUserInfo();

            if (user == null)
                return Redirect("/api/Security/Logout");

            var transferData = new FileWrapper
            {
                Application = application,
                User = user.UserId,
                Predio = user.Predio.ToString(),
                SessionData = null
            };

            transferData.SetData(new FileDownloadRequest()
            {
                Application = application,
                FileId = fileId,
            });

            var task = this.CallService("File/GetFile", transferData, new CancellationToken());
            task.Wait();

            FileWrapper responseData = task.Result;

            var response = new ServerResponse();

            if (responseData.Status == TransferWrapperStatus.Error)
            {
                response.SetError(responseData.Message, responseData.MessageArguments);

                return Content(response.Serialize(), "application/json");
            }

            var data = responseData.GetData<FileDownloadResponse>();

            return File(data.FileContents, data.ContetType, data.FileName);
        }

        private async Task<FileWrapper> CallService(string action, FileWrapper transferData, CancellationToken cancelToken)
        {
            HttpResponseMessage response = await this._apiClient.PostAsync(this._internalEndpoint, this._internalTimeout, action, transferData, cancelToken);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Error, status: " + response.StatusCode + " - " + response.ReasonPhrase + "-" + await response.Content.ReadAsStringAsync());

            var result = await response.Content.ReadAsStringAndDeserializeAsync<FileWrapper>();

            return result;
        }

        [HttpPost("[action]")]
        public ActionResult Upload([FromBody] FileUploadRequest request)
        {
            var user = this._sessionManager.GetUserInfo();

            if (user == null)
                return Redirect("/api/Security/Logout");

            var transferData = new FileWrapper
            {
                Application = request.Application,
                User = user.UserId,
                Predio = user.Predio.ToString(),
                SessionData = null,
            };

            transferData.SetData(request);

            var task = this.CallService("File/AddFile", transferData, new CancellationToken());
            task.Wait();

            var responseData = task.Result;
            var content = responseData.GetResolvedData<FileUploadResponse>();
            var response = new ServerResponse(content);

            if (responseData.Status == TransferWrapperStatus.Error)
            {
                response.SetError(responseData.Message, responseData.MessageArguments);
            }

            return Content(response.Serialize(), "application/json");
        }

        [HttpPost("[action]")]
        public IActionResult Delete([FromBody] FileDeleteRequest request)
        {
            var user = this._sessionManager.GetUserInfo();

            if (user == null)
                return Redirect("/api/Security/Logout");

            var transferData = new FileWrapper
            {
                Application = request.Application,
                User = user.UserId,
                Predio = user.Predio.ToString(),
                SessionData = null
            };

            transferData.SetData(request);

            var task = this.CallService("File/DeleteFile", transferData, new CancellationToken());
            task.Wait();

            var responseData = task.Result;
            var content = responseData.GetResolvedData<FileDeleteResponse>();
            var response = new ServerResponse(content);

            if (responseData.Status == TransferWrapperStatus.Error)
            {
                response.SetError(responseData.Message, responseData.MessageArguments);
            }

            return Content(response.Serialize(), "application/json");
        }
    }
}
