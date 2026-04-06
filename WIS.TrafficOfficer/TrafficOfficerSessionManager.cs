using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using WIS.Http;
using WIS.Security;
using WIS.TrafficOfficer.Configuration;

namespace WIS.TrafficOfficer
{
    public class TrafficOfficerSessionManager : ITrafficOfficerSessionManager
    {
        private readonly IWebApiClient _client;
        private readonly IOptions<TrafficOfficerSettings> _settings;
        private readonly IIdentityService _identity;

        public TrafficOfficerSessionManager(IWebApiClient client, IOptions<TrafficOfficerSettings> settings, IIdentityService identity)
        {
            this._client = client;
            this._settings = settings;
            this._identity = identity;
        }

        public bool IsSessionValid(string token)
        {
            var payload = new UserSessionRequest
            {
                UserId = this._identity.UserId,
                Token = token
            };

            HttpResponseMessage response = this._client.Post(new Uri(new Uri(this._settings.Value.Endpoint), "TrafficOfficerAPI/Session/IsSessionValid"), payload);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Error al comunicarse con servicio de traffic officer. Status: {response.StatusCode}, Message: {response.ReasonPhrase}");

            var result = JsonConvert.DeserializeObject<TrafficOfficerResponse>(response.Content.ReadAsStringAsync().Result);

            return result.Estado != TrafficOfficerResponseStatus.ERROR;
        }
    }
}
