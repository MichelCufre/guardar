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
using WIS.Http;
using WIS.Http.Extensions;
using WIS.PageComponent.Execution;
using WIS.PageComponent.Execution.Serialization;
using WIS.Security.Models;
using WIS.WebApplication.Models.Managers;

namespace WIS.WebApplication.Models
{
    public class PageCallService : IPageCallService
    {
        private readonly IWebApiClient _apiClient;
        private readonly ISessionManager _sessionManager;
        private readonly string _internalEndpoint;
        private readonly int? _internalTimeout;
        private readonly ILogger<PageCallService> _logger;

        public PageCallService(IWebApiClient apiClient, ISessionManager sessionManager, IOptions<ModuleUrl> moduleUrls, IOptions<ApplicationSettings> appSettings, ILogger<PageCallService> logger)
        {
            this._apiClient = apiClient;
            this._sessionManager = sessionManager;
            this._internalEndpoint = moduleUrls.Value.Internal;
            this._internalTimeout = appSettings.Value.InternalTimeout;
            this._logger = logger;
        }

        public async Task<IPageWrapper> CallPageServiceAsync(ServerRequest request, PageAction action, CancellationToken cancelToken)
        {
            PageWrapper result = new PageWrapper();

            string controller = request.GetBaseApplication();
            string application = request.Application;
            Usuario user = null;

            try
            {
                user = this._sessionManager.GetUserInfo();
                var session = _sessionManager.GetValue<string>("WIS_SESSION");

                if (session == null)
                    session = JsonConvert.SerializeObject(new Dictionary<string, object>());


                var transferData = new PageWrapper
                {
                    Application = application.Replace("Custom", ""),
                    Action = action,
                    User = user.UserId,
                    Data = request.Data,
                    SessionData = session,
                    PageToken = request.PageToken
                };

                HttpResponseMessage response = await _apiClient.PostAsync(this._internalEndpoint, this._internalTimeout, controller, application + "/Page", transferData, cancelToken);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new InvalidUserException("Invalid user");

                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    throw new UserNotAllowedException("User not allowed");

                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException("Error, status: " + response.StatusCode + " - " + response.ReasonPhrase + "-" + await response.Content.ReadAsStringAsync());

                result = await response.Content.ReadAsStringAndDeserializeAsync<PageWrapper>();

                if (!string.IsNullOrEmpty(result.SessionData))
                    _sessionManager.SetValue("WIS_SESSION", result.SessionData);
            }
            catch (UserNotAllowedException)
            {
                throw;
            }
            catch (InvalidUserException)
            {
                throw;
            }
            catch (Exception ex)
            {
                using (MappedDiagnosticsLogicalContext.SetScoped("UserId", user?.UserId))
                using (MappedDiagnosticsLogicalContext.SetScoped("Application", application))
                {
                    this._logger.LogError(ex, "WebApplication PageController - CallPageServiceAsync");
                }

            }

            return result;
        }
    }
}
