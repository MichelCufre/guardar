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
using WIS.Exceptions;
using WIS.GridComponent.Execution;
using WIS.GridComponent.Execution.Serialization;
using WIS.Http;
using WIS.Http.Extensions;
using WIS.Security.Models;
using WIS.WebApplication.Models.Managers;

namespace WIS.WebApplication.Models
{
    public class GridCallService : IGridCallService
    {
        private readonly IWebApiClient _apiClient;
        private readonly ISessionManager _sessionManager;
        private readonly string _internalEndpoint;
        private readonly int? _internalTimeout;
        private readonly ILogger<GridCallService> _logger;

        public GridCallService(
            IWebApiClient apiClient,
            ISessionManager sessionManager,
            IOptions<ModuleUrl> moduleUrls,
            IOptions<ApplicationSettings> appSettings,
            ILogger<GridCallService> logger)
        {
            this._apiClient = apiClient;
            this._sessionManager = sessionManager;
            this._internalEndpoint = moduleUrls.Value.Internal;
            this._internalTimeout = appSettings.Value.InternalTimeout;
            this._logger = logger;
        }

        public async Task<IGridWrapper> CallGridServiceAsync(ServerRequest request, GridAction action, CancellationToken cancelToken)
        {
            string controller = request.GetBaseApplication();
            string application = request.Application;
            Usuario user = null;
            GridWrapper result = new GridWrapper();

            try
            {
                var session = this._sessionManager.GetValue<string>("WIS_SESSION");

                user = this._sessionManager.GetUserInfo();

                if (session == null)
                    session = JsonConvert.SerializeObject(new Dictionary<string, object>());

                var transferData = new GridWrapper
                {
                    Application = application.Replace("Custom", ""),
                    Action = action,
                    GridId = request.ComponentId,
                    User = user.UserId,
                    Data = request.Data,
                    Predio = user.Predio.ToString(),
                    SessionData = session,
                    PageToken = request.PageToken
                };

                HttpResponseMessage response = await _apiClient.PostAsync(this._internalEndpoint, this._internalTimeout, controller, application + "/Grid", transferData, cancelToken);

                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException("Error, status: " + response.StatusCode + " - " + response.ReasonPhrase + "-" + await response.Content.ReadAsStringAsync());

                result = await response.Content.ReadAsStringAndDeserializeAsync<GridWrapper>();

                if (!string.IsNullOrEmpty(result.SessionData))
                    _sessionManager.SetValue("WIS_SESSION", result.SessionData);
            }
            catch (Exception ex)
            {
                using (MappedDiagnosticsLogicalContext.SetScoped("UserId", user?.UserId))
                using (MappedDiagnosticsLogicalContext.SetScoped("Application", application))
                {
                    result.SetError(ex is ExpectedException ? ex.Message : "General_Sec0_Error_Operacion");
                    this._logger.LogError(ex, "WebApplication GridController - CallGridServiceAsync");
                }
            }

            return result;
        }
        public async Task<IGridWrapper> CallGridServiceAsync(ServerRequest request, string url, GridAction action, CancellationToken cancelToken)
        {
            var session = _sessionManager.GetValue<string>("WIS_SESSION");
            var user = this._sessionManager.GetUserInfo();

            var transferData = new GridWrapper
            {
                Application = request.Application,
                Action = action, //TODO: Ver si quitar
                GridId = request.ComponentId,
                User = user.UserId,
                Data = request.Data,
                Predio = user.Predio.ToString(),
                SessionData = session,
                PageToken = request.PageToken
            };

            HttpResponseMessage response = await _apiClient.PostAsync(this._internalEndpoint, this._internalTimeout, url, transferData, cancelToken);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Error, status: " + response.StatusCode + " - " + response.ReasonPhrase + "-" + await response.Content.ReadAsStringAsync());

            var result = await response.Content.ReadAsStringAndDeserializeAsync<GridWrapper>();

            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", user.UserId))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", request.Application))
            {
                try
                {
                    if (session == null)
                        session = JsonConvert.SerializeObject(new Dictionary<string, object>());

                    if (!string.IsNullOrEmpty(result.SessionData))
                        _sessionManager.SetValue("WIS_SESSION", result.SessionData);

                }
                catch (Exception ex)
                {
                    // Logger.Log(LogLevel.BAJO, transferData.User.ToString(), LogType.FRONTEND, "CallGridServiceAsync", ex);
                    this._logger.LogError(ex, "WebApplication GridController - CallGridServiceAsync");
                }

            }
            return result;
        }
    }
}
