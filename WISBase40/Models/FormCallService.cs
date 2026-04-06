using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WIS.Configuration;
using WIS.FormComponent.Execution;
using WIS.FormComponent.Execution.Serialization;
using WIS.Http;
using WIS.Http.Extensions;
using WIS.Security.Models;
using WIS.WebApplication.Models.Managers;

namespace WIS.WebApplication.Models
{
    public class FormCallService : IFormCallService
    {
        private readonly IWebApiClient _apiClient;
        private readonly ISessionManager _sessionManager;
        private readonly string _internalEndpoint;
        private readonly int? _internalTimeout;
        private readonly ILogger<FormCallService> _logger;

        public FormCallService(IWebApiClient apiClient, ISessionManager sessionManager, IOptions<ModuleUrl> moduleUrls, IOptions<ApplicationSettings> appSettings, ILogger<FormCallService> logger)
        {
            this._apiClient = apiClient;
            this._sessionManager = sessionManager;
            this._internalEndpoint = moduleUrls.Value.Internal;
            this._internalTimeout = appSettings.Value.InternalTimeout;
            this._logger = logger;
        }

        public async Task<IFormWrapper> CallFormServiceAsync(ServerRequest request, FormAction action, CancellationToken cancelToken)
        {
            string controller = request.GetBaseApplication();
            string application = request.Application;
            FormWrapper result = new FormWrapper();

            Usuario user = null;

            try
            {
                var session = _sessionManager.GetValue<string>("WIS_SESSION");
                
                user = this._sessionManager.GetUserInfo();

                if (session == null)
                    session = JsonConvert.SerializeObject(new Dictionary<string, object>());

                var transferData = new FormWrapper
                {
                    Application = application.Replace("Custom", ""),                    
                    Action = action,
                    FormId = request.ComponentId,
                    User = user.UserId,
                    Data = request.Data,
                    Predio = user.Predio.ToString(),
                    SessionData = session,
                    PageToken = request.PageToken
                };

                HttpResponseMessage response = await _apiClient.PostAsync(this._internalEndpoint, this._internalTimeout, controller, application + "/Form", transferData, cancelToken);

                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException("Error, status: " + response.StatusCode + " - " + response.ReasonPhrase + "-" + await response.Content.ReadAsStringAsync());

                result = await response.Content.ReadAsStringAndDeserializeAsync<FormWrapper>();

                if (!string.IsNullOrEmpty(result.SessionData))
                    _sessionManager.SetValue("WIS_SESSION", result.SessionData);
            }
            catch (Exception ex)
            {

                using (MappedDiagnosticsLogicalContext.SetScoped("UserId", user?.UserId))
                using (MappedDiagnosticsLogicalContext.SetScoped("Application", application))
                {
                    this._logger.LogError(ex, "WebApplication FormController - CallFormServiceAsync");
                }

            }
            return result;
        }
    }
}
