using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WIS.Configuration;
using WIS.Exceptions;
using WIS.Http;
using WIS.TrafficOfficer;
using WIS.TrafficOfficer.Configuration;
using WIS.WebApplication.Models.Managers;

namespace WIS.WebApplication.Models
{
    public class TrafficOfficerFrontendService : ITrafficOfficerFrontendService
    {
        private readonly IWebApiClient _client;
        private readonly IConfiguration _configuration;
        private readonly ISessionManager _sessionManager;
        private readonly string _endpoint;
        private readonly string _systemName;

        public TrafficOfficerFrontendService(IWebApiClient client
            , IConfiguration configuration
            , ISessionManager sessionManager
            , IOptions<TrafficOfficerSettings> settings)
        {
            this._client = client;
            this._configuration = configuration;
            this._sessionManager = sessionManager;
            this._endpoint = settings.Value.Endpoint;
            this._systemName = settings.Value.SystemName;
        }

        public string ClientId
        {
            get
            {
                return this._configuration.GetValue<string>("AuthSettings:ClientId");
            }
        }

        public int? UserId
        {
            get
            {
                return this._sessionManager.GetUserInfo()?.UserId;
            }
        }

        public string UserLogin
        {
            get
            {
                return this._sessionManager.GetUserInfo()?.Username;
            }
        }

        public string UserName
        {
            get
            {
                return this._sessionManager.GetUserInfo()?.Name;
            }
        }

        public string SessionToken
        {
            get
            {
                return GetSessionValue(SessionManager.SessionToken)?.ToString();
            }
            private set
            {
                SetSessionValue(SessionManager.SessionToken, value);
            }
        }

        public bool TooManySessions
        {
            get
            {
                return (bool?)GetSessionValue(SessionManager.TooManySessions) ?? false;
            }
            private set
            {
                SetSessionValue(SessionManager.TooManySessions, value);
            }
        }

        private object GetSessionValue(string key)
        {
            var jsonSession = this._sessionManager.GetValue<string>(SessionManager.WIS_SESSION);

            if (!string.IsNullOrEmpty(jsonSession))
            {
                var session = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonSession);

                if (session.ContainsKey(SessionManager.SessionToken))
                    return session[key];
            }

            return null;
        }

        private void SetSessionValue(string key, object value)
        {
            var jsonSession = this._sessionManager.GetValue<string>(SessionManager.WIS_SESSION);
            var session = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(jsonSession))
                session = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonSession);

            session[key] = value;
            jsonSession = JsonConvert.SerializeObject(session);

            this._sessionManager.SetValue(SessionManager.WIS_SESSION, jsonSession);
        }

        public async Task<bool> ClearToken(string token, string application, CancellationToken cancelToken)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            var payload = new ConcurrencyControlRequest
            {
                Token_thread = token,
                PaginaOrigen = application.Replace("Custom",""),
            };

            HttpResponseMessage response = await this._client.DeleteAsync(new Uri(new Uri(this._endpoint), "TrafficOfficerAPI/Concurrency/RemoveThreadOperation"), payload, cancelToken);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Error al comunicarse con servicio de traffic officer. Status: {response.StatusCode}, Message: {response.ReasonPhrase}");

            var result = JsonConvert.DeserializeObject<TrafficOfficerResponse>(await response.Content.ReadAsStringAsync());

            if (result.Estado == TrafficOfficerResponseStatus.ERROR)
                throw new ConcurrencyControlException(result.Message);

            return true;
        }

        public async Task<bool> DeleteUserLocks(CancellationToken cancelToken)
        {
            var payload = new ConcurrencyControlRequest
            {
                Userid = this.UserId,
                Sistema = this._systemName
            };

            HttpResponseMessage response = await this._client.DeleteAsync(new Uri(new Uri(this._endpoint), "TrafficOfficerAPI/Concurrency/RemoveLockByUserSystem"), payload, cancelToken);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Error al comunicarse con servicio de traffic officer. Status: {response.StatusCode}, Message: {response.ReasonPhrase}");

            var result = JsonConvert.DeserializeObject<TrafficOfficerResponse>(await response.Content.ReadAsStringAsync());

            if (result.Estado == TrafficOfficerResponseStatus.ERROR)
                throw new ConcurrencyControlException(result.Message);

            return true;
        }

        public async Task CreateSession(CancellationToken cancelToken)
        {
            var payload = new UserSessionRequest
            {
                UserId = this.UserId,
                UserLogin = this.UserLogin,
                UserName = this.UserName,
                ClientId = this.ClientId,
                ClientName = "WMS Web Panel"
            };

            HttpResponseMessage response = await this._client.PostAsync(new Uri(new Uri(this._endpoint), "TrafficOfficerAPI/Session/CreateSession"), payload, cancelToken);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Error al comunicarse con servicio de traffic officer. Status: {response.StatusCode}, Message: {response.ReasonPhrase}");

            var result = JsonConvert.DeserializeObject<TrafficOfficerResponse>(await response.Content.ReadAsStringAsync());

            if (result.Estado == TrafficOfficerResponseStatus.ERROR)
            {
                if (result.CodErrorDetallado == TrafficOfficerErrorCode.TooManySessions)
                    throw new TooManySessionsException();
                else if (result.CodErrorDetallado == TrafficOfficerErrorCode.ExpiredLicense)
                    throw new ExpiredLicenseException();
                else if (result.CodErrorDetallado == TrafficOfficerErrorCode.InvalidLicense)
                    throw new InvalidLicenseException();
                else
                    throw new InvalidOperationException($"Error al comunicarse con servicio de traffic officer. Status: {result.CodErrorDetallado}, Message: {result.Message}");
            }

            SessionToken = result.DataResponse.Token_thread;
            TooManySessions = result.DataResponse.TooManySessions;
        }

        public async Task<bool> IsSessionValid(CancellationToken cancelToken)
        {
            var payload = new UserSessionRequest
            {
                UserId = this.UserId,
                Token = this.SessionToken
            };

            HttpResponseMessage response = await this._client.PostAsync(new Uri(new Uri(this._endpoint), "TrafficOfficerAPI/Session/IsSessionValid"), payload, cancelToken);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Error al comunicarse con servicio de traffic officer. Status: {response.StatusCode}, Message: {response.ReasonPhrase}");

            var result = JsonConvert.DeserializeObject<TrafficOfficerResponse>(await response.Content.ReadAsStringAsync());

            return result.Estado != TrafficOfficerResponseStatus.ERROR;
        }

        public async Task UpdateSessionActivity(CancellationToken cancelToken)
        {
            var payload = new UserSessionRequest
            {
                UserId = this.UserId,
                Token = this.SessionToken
            };

            HttpResponseMessage response = await this._client.PostAsync(new Uri(new Uri(this._endpoint), "TrafficOfficerAPI/Session/UpdateSessionActivity"), payload, cancelToken);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Error al comunicarse con servicio de traffic officer. Status: {response.StatusCode}, Message: {response.ReasonPhrase}");

            var result = JsonConvert.DeserializeObject<TrafficOfficerResponse>(await response.Content.ReadAsStringAsync());

            if (result.Estado == TrafficOfficerResponseStatus.ERROR)
            {
                if (result.CodErrorDetallado == TrafficOfficerErrorCode.TooManySessions)
                    throw new TooManySessionsException();
                else if (result.CodErrorDetallado == TrafficOfficerErrorCode.ExpiredLicense)
                    throw new ExpiredLicenseException();
                else if (result.CodErrorDetallado == TrafficOfficerErrorCode.InvalidLicense)
                    throw new InvalidLicenseException();
                else
                    throw new InvalidOperationException($"Error al comunicarse con servicio de traffic officer. Status: {result.CodErrorDetallado}, Message: {result.Message}");
            }

            TooManySessions = result.DataResponse.TooManySessions;
        }

        public async Task UpdateThreadOperationActivity(string pageToken, CancellationToken cancelToken)
        {
            var payload = new ConcurrencyControlRequest
            {
                Token_thread = pageToken
            };

            HttpResponseMessage response = await this._client.PostAsync(new Uri(new Uri(this._endpoint), "TrafficOfficerAPI/Concurrency/UpdateThreadOperationActivity"), payload, cancelToken);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Error al comunicarse con servicio de traffic officer. Status: {response.StatusCode}, Message: {response.ReasonPhrase}");

            var result = JsonConvert.DeserializeObject<TrafficOfficerResponse>(await response.Content.ReadAsStringAsync());

            if (result.Estado == TrafficOfficerResponseStatus.ERROR)
            {
                throw new ExpiredPageTokenException();
            }
        }

        public async Task RemoveSession(CancellationToken cancelToken)
        {
            var payload = new UserSessionRequest
            {
                UserId = this.UserId,
                Token = this.SessionToken
            };

            SessionToken = string.Empty;

            HttpResponseMessage response = await this._client.DeleteAsync(new Uri(new Uri(this._endpoint), "TrafficOfficerAPI/Session/RemoveSession"), payload, cancelToken);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Error al comunicarse con servicio de traffic officer. Status: {response.StatusCode}, Message: {response.ReasonPhrase}");

            var result = JsonConvert.DeserializeObject<TrafficOfficerResponse>(await response.Content.ReadAsStringAsync());

            if (result.Estado == TrafficOfficerResponseStatus.ERROR)
                throw new InvalidOperationException($"Error al comunicarse con servicio de traffic officer. Status: {result.CodErrorDetallado}, Message: {result.Message}");
        }
    }
}
