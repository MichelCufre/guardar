using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using WIS.Exceptions;
using WIS.Http;
using WIS.Security;
using WIS.TrafficOfficer.Configuration;

namespace WIS.TrafficOfficer
{
    public class TrafficOfficerService : ITrafficOfficerService, ITrafficOfficerServiceManager
    {
        private string _token;
        private readonly IWebApiClient _client;
        private readonly IIdentityService _securityService;
        private readonly IOptions<TrafficOfficerSettings> _configuration;

        public TrafficOfficerService(IWebApiClient client,
            IIdentityService securityService,
            IOptions<TrafficOfficerSettings> configuration)
        {
            this._client = client;
            this._configuration = configuration;
            this._securityService = securityService;
        }


        public string CreateToken()
        {
            var payload = new ThreadOperationRequest
            {
                Userid = this._securityService.UserId,
                Pagina = this._securityService.Application,
                Sistema = this._configuration.Value.SystemName
            };

            HttpResponseMessage response = this._client.Post(new Uri(new Uri(this._configuration.Value.Endpoint), "TrafficOfficerAPI/Concurrency/CreateToken"), payload);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Error al comunicarse con servicio de traffic officer. Status: {response.StatusCode}, Message: {response.ReasonPhrase}");

            var result = JsonConvert.DeserializeObject<TrafficOfficerResponse>(response.Content.ReadAsStringAsync().Result);

            if (result.Estado == TrafficOfficerResponseStatus.ERROR)
                throw new ConcurrencyControlException(result.Message);

            this._token = result.DataResponse.Token_thread;

            return result.DataResponse.Token_thread;
        }

        public TrafficOfficerTransaction CreateTransaccion()
        {
            var payload = new ConcurrencyControlRequest
            {
                Userid = this._securityService.UserId,
                Token_thread = this._token,
                //Pagina = this._application,
                //Sistema = "WIS40"
            };

            HttpResponseMessage response = this._client.Post(new Uri(new Uri(this._configuration.Value.Endpoint), "TrafficOfficerAPI/Concurrency/CreateTransaccion"), payload);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Error al comunicarse con servicio de traffic officer. Status: {response.StatusCode}, Message: {response.ReasonPhrase}");

            var result = JsonConvert.DeserializeObject<TrafficOfficerResponse>(response.Content.ReadAsStringAsync().Result);

            if (result.Estado == TrafficOfficerResponseStatus.ERROR)
                throw new ConcurrencyControlException(result.Message);

            return new TrafficOfficerTransaction(result.DataResponse.Transaccion);
        }

        public void SetPageToken(string token)
        {
            this._token = token;
        }

        public string TransferToken(string destination)
        {
            if (string.IsNullOrEmpty(this._token))
                throw new InvalidOperationException("Token de traffic officer no definido");

            var payload = new ConcurrencyControlRequest
            {
                Userid = this._securityService.UserId,
                Token_thread = this._token,
                PaginaOrigen = this._securityService.Application,
                PaginaDestino = destination
            };

            HttpResponseMessage response = this._client.Put(new Uri(new Uri(this._configuration.Value.Endpoint), "TrafficOfficerAPI/Concurrency/TraspasoLock"), payload);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Error al comunicarse con servicio de traffic officer. Status: {response.StatusCode}, Message: {response.ReasonPhrase}");

            var result = JsonConvert.DeserializeObject<TrafficOfficerResponse>(response.Content.ReadAsStringAsync().Result);

            if (result.Estado == TrafficOfficerResponseStatus.ERROR)
                throw new ConcurrencyControlException(result.Message);

            return result.DataResponse.Token_thread;
        }

        public bool IsLocked(string entity, string key, bool isGlobal = false)
        {
            if (string.IsNullOrEmpty(this._token))
                throw new InvalidOperationException("Token de traffic officer no definido");

            var parameters = new Dictionary<string, string>()
            {
                ["userId"] = Convert.ToString(this._securityService.UserId),
                ["entidad"] = entity,
                ["idBloqueo"] = HttpUtility.UrlEncode(key),
                ["token"] = HttpUtility.UrlEncode(this._token),
                ["paginaOrigen"] = this._securityService.Application
            };

            string metodo = "TrafficOfficerAPI/Concurrency/IsLock";

            if (isGlobal)
                metodo = "TrafficOfficerAPI/Concurrency/IsGlobalLock";

            HttpResponseMessage response = this._client.Get(new Uri(new Uri(this._configuration.Value.Endpoint), metodo), parameters);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Error al comunicarse con servicio de traffic officer. Status: {response.StatusCode}, Message: {response.ReasonPhrase}");

            var result = JsonConvert.DeserializeObject<TrafficOfficerResponse>(response.Content.ReadAsStringAsync().Result);

            if (!result.IsErrorRegistroBloqueado() && result.Estado == TrafficOfficerResponseStatus.ERROR)
                throw new EntityLockedException(result.Message);

            return result.IsErrorRegistroBloqueado();
        }

        public string AddLock(string entity, string key, TrafficOfficerTransaction transaction = default, bool isGlobal = false)
        {
            if (this._token == null)
                throw new InvalidOperationException("Token de traffic officer no definido");

            if (transaction != null && transaction.Token == null)
                throw new InvalidOperationException("Token de transacción traffic officer no definido");

            var payload = new ConcurrencyControlRequest
            {
                Userid = this._securityService.UserId,
                Token_thread = this._token,
                Entidad = entity,
                Id_Bloqueo = key,
                PaginaOrigen = this._securityService.Application,
                IsGlobal = isGlobal,
            };

            if (transaction != null)
                payload.Transaccion = transaction.Token;

            HttpResponseMessage response = this._client.Post(new Uri(new Uri(this._configuration.Value.Endpoint), "TrafficOfficerAPI/Concurrency/AddLock"), payload);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Error al comunicarse con servicio de traffic officer. Status: {response.StatusCode}, Message: {response.ReasonPhrase}");

            var result = JsonConvert.DeserializeObject<TrafficOfficerResponse>(response.Content.ReadAsStringAsync().Result);

            if (result.Estado == TrafficOfficerResponseStatus.ERROR)
                throw new ConcurrencyControlException(result.Message);

            return result.DataResponse.Token_thread;
        }

        public string AddLockList(string entity, List<string> keys, TrafficOfficerTransaction transaction = default, bool isGlobal = false)
        {
            if (this._token == null)
                throw new InvalidOperationException("Token de traffic officer no definido");

            if (transaction != null && transaction.Token == null)
                throw new InvalidOperationException("Token de transacción traffic officer no definido");

            var payload = new ConcurrencyControlListRequest
            {
                Userid = this._securityService.UserId,
                Token_thread = this._token,
                Entidad = entity,
                Ids_Bloqueo = keys,
                IsGlobal = isGlobal,
                PaginaOrigen = this._securityService.Application,
            };

            if (transaction != null)
                payload.Transaccion = transaction.Token;

            HttpResponseMessage response = this._client.Post(new Uri(new Uri(this._configuration.Value.Endpoint), "TrafficOfficerAPI/Concurrency/AddLockList"), payload);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Error al comunicarse con servicio de traffic officer. Status: {response.StatusCode}, Message: {response.ReasonPhrase}");

            var result = JsonConvert.DeserializeObject<TrafficOfficerResponse>(response.Content.ReadAsStringAsync().Result);

            if (result.Estado == TrafficOfficerResponseStatus.ERROR)
                throw new ConcurrencyControlException(result.Message);

            return result.DataResponse.Token_thread;
        }

        public bool ClearToken(bool includeGlobal = false)
        {
            if (string.IsNullOrEmpty(this._token))
                return true;

            var payload = new ConcurrencyControlRequest
            {
                Token_thread = this._token,
                Userid = this._securityService.UserId,
                PaginaOrigen = this._securityService.Application,
                IsGlobal = includeGlobal,
            };

            HttpResponseMessage response = this._client.Delete(new Uri(new Uri(this._configuration.Value.Endpoint), "TrafficOfficerAPI/Concurrency/RemoveThreadOperation"), payload);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Error al comunicarse con servicio de traffic officer. Status: {response.StatusCode}, Message: {response.ReasonPhrase}");

            var result = JsonConvert.DeserializeObject<TrafficOfficerResponse>(response.Content.ReadAsStringAsync().Result);

            if (result.Estado == TrafficOfficerResponseStatus.ERROR)
                throw new ConcurrencyControlException(result.Message);

            return true;
        }

        public bool DeleteTransaccion(TrafficOfficerTransaction transaction)
        {

            if (string.IsNullOrEmpty(transaction.Token))
                return true;

            var payload = new ConcurrencyControlRequest
            {
                Token_thread = this._token,
                Transaccion = transaction.Token,
                Userid = this._securityService.UserId
            };

            HttpResponseMessage response = this._client.Delete(new Uri(new Uri(this._configuration.Value.Endpoint), "TrafficOfficerAPI/Concurrency/RemoveLockByTransaccion"), payload);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Error al comunicarse con servicio de traffic officer. Status: {response.StatusCode}, Message: {response.ReasonPhrase}");

            var result = JsonConvert.DeserializeObject<TrafficOfficerResponse>(response.Content.ReadAsStringAsync().Result);

            if (result.Estado == TrafficOfficerResponseStatus.ERROR)
                throw new ConcurrencyControlException(result.Message);

            return true;
        }

        public bool RemoveLockByIdLock(string entity, string idLock, int userId)
        {
            if (string.IsNullOrEmpty(this._token))
                return true;

            var payload = new ConcurrencyControlRequest
            {
                Token_thread = this._token,
                Id_Bloqueo = idLock,
                Userid = userId,
                Entidad = entity
            };

            HttpResponseMessage response = this._client.Delete(new Uri(new Uri(this._configuration.Value.Endpoint), "TrafficOfficerAPI/Concurrency/RemoveLockByIdLock"), payload);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Error al comunicarse con servicio de traffic officer. Status: {response.StatusCode}, Message: {response.ReasonPhrase}");

            var result = JsonConvert.DeserializeObject<TrafficOfficerResponse>(response.Content.ReadAsStringAsync().Result);

            if (result.Estado == TrafficOfficerResponseStatus.ERROR)
                throw new ConcurrencyControlException(result.Message);

            return true;
        }

        public bool RemoveLockListByIdLock(string entity, List<string> keys, int userId)
        {
            if (string.IsNullOrEmpty(this._token))
                return true;

            var payload = new ConcurrencyControlListRequest
            {
                Token_thread = this._token,
                Ids_Bloqueo = keys,
                Userid = userId,
                Entidad = entity,
                PaginaOrigen = this._securityService.Application,
            };

            HttpResponseMessage response = this._client.Delete(new Uri(new Uri(this._configuration.Value.Endpoint), "TrafficOfficerAPI/Concurrency/RemoveLockListByIdLock"), payload);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Error al comunicarse con servicio de traffic officer. Status: {response.StatusCode}, Message: {response.ReasonPhrase}");

            var result = JsonConvert.DeserializeObject<TrafficOfficerResponse>(response.Content.ReadAsStringAsync().Result);

            if (result.Estado == TrafficOfficerResponseStatus.ERROR)
                throw new ConcurrencyControlException(result.Message);

            return true;
        }

        public List<ItemLock> GetLockList(string entity, List<string> keys, TrafficOfficerTransaction transaction = default, bool isGlobal = false)
        {
            if (this._token == null)
                throw new InvalidOperationException("Token de traffic officer no definido");

            if (transaction != null && transaction.Token == null)
                throw new InvalidOperationException("Token de transacción traffic officer no definido");

            var payload = new ConcurrencyControlListRequest
            {
                Userid = this._securityService.UserId,
                Token_thread = this._token,
                Entidad = entity,
                Ids_Bloqueo = keys,
                PaginaOrigen = this._securityService.Application,
                IsGlobal = isGlobal
            };

            if (transaction != null)
                payload.Transaccion = transaction.Token;

            HttpResponseMessage response = this._client.Post(new Uri(new Uri(this._configuration.Value.Endpoint), "TrafficOfficerAPI/Concurrency/GetLockList"), payload);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Error al comunicarse con servicio de traffic officer. Status: {response.StatusCode}, Message: {response.ReasonPhrase}");

            var result = JsonConvert.DeserializeObject<TrafficOfficerResponse>(response.Content.ReadAsStringAsync().Result);

            if (!result.IsErrorRegistroBloqueado() && result.Estado == TrafficOfficerResponseStatus.ERROR)
                throw new EntityLockedException(result.Message);

            return result.DataResponse.colItemLock;
        }

        public List<ItemLock> GetLockListWithKeyPrefixes(string entity, List<string> keys, TrafficOfficerTransaction transaction = default)
        {
            if (this._token == null)
                throw new InvalidOperationException("Token de traffic officer no definido");

            if (transaction != null && transaction.Token == null)
                throw new InvalidOperationException("Token de transacción traffic officer no definido");

            var payload = new ConcurrencyControlListRequest
            {
                Userid = this._securityService.UserId,
                Token_thread = this._token,
                Entidad = entity,
                Ids_Bloqueo = keys,
                PaginaOrigen = this._securityService.Application,
            };

            if (transaction != null)
                payload.Transaccion = transaction.Token;

            HttpResponseMessage response = this._client.Post(new Uri(new Uri(this._configuration.Value.Endpoint), "TrafficOfficerAPI/Concurrency/GetLockListWithKeyPrefixes"), payload);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Error al comunicarse con servicio de traffic officer. Status: {response.StatusCode}, Message: {response.ReasonPhrase}");

            var result = JsonConvert.DeserializeObject<TrafficOfficerResponse>(response.Content.ReadAsStringAsync().Result);

            if (!result.IsErrorRegistroBloqueado() && result.Estado == TrafficOfficerResponseStatus.ERROR)
                throw new EntityLockedException(result.Message);

            return result.DataResponse.colItemLock;
        }
    }
}
