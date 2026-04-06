using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace WIS.HttpLegacy.WebApi
{
    public class WebApiClientLegacy : IWebApiClientLegacy
    {
        private readonly HttpClient _client;

        public WebApiClientLegacy(HttpClient client)
        {
            this._client = client;
        }

        public HttpResponseMessage Get(Uri address)
        {
            return this._client.GetAsync(address).Result;
        }
        public HttpResponseMessage Get(Uri address, Dictionary<string, string> parameters)
        {
            var queryItems = new List<string>();

            foreach (var parameter in parameters)
            {
                queryItems.Add($"{parameter.Key}={parameter.Value}");
            }

            string query = "?" + string.Join("&", queryItems);

            var addressAndQuery = new Uri(address, query);

            return this._client.GetAsync(addressAndQuery).Result;
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string uri, string application, string operation, T transferObject, CancellationToken cancelToken)
        {
            this._client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var address = new Uri(new Uri(uri), "api/" + application + "/" + operation);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, address);

            request.Content = new StringContent(JsonConvert.SerializeObject(transferObject), Encoding.UTF8, "application/json");

            request.Headers.ConnectionClose = true;

            var response = await this._client.SendAsync(request, cancelToken);

            return response;
        }
        public async Task<HttpResponseMessage> PostAsync<T>(string uri, string application, T transferObject, CancellationToken cancelToken)
        {
            this._client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var address = new Uri(new Uri(uri), "api/" + application);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, address);

            request.Content = new StringContent(JsonConvert.SerializeObject(transferObject), Encoding.UTF8, "application/json");

            request.Headers.ConnectionClose = true;

            var response = await this._client.SendAsync(request, cancelToken);

            return response;
        }
        public async Task<HttpResponseMessage> PostAsync<T>(Uri address, T transferObject, CancellationToken cancelToken)
        {
            this._client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, address);

            request.Content = new StringContent(JsonConvert.SerializeObject(transferObject), Encoding.UTF8, "application/json");

            request.Headers.ConnectionClose = true;

            var response = await this._client.SendAsync(request, cancelToken);

            return response;
        }

        public HttpResponseMessage Post<T>(string uri, string application, string operation, T transferObject)
        {
            this._client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var address = new Uri(new Uri(uri), "api/" + application + "/" + operation);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, address);

            request.Method = HttpMethod.Post;
            request.Content = new StringContent(JsonConvert.SerializeObject(transferObject), Encoding.UTF8, "application/json");

            request.Headers.ConnectionClose = true;

            var response = this._client.SendAsync(request).Result;

            return response;
        }
        public HttpResponseMessage Post<T>(Uri address, T transferObject)
        {
            this._client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, address);

            request.Method = HttpMethod.Post;
            request.Content = new StringContent(JsonConvert.SerializeObject(transferObject), Encoding.UTF8, "application/json");

            request.Headers.ConnectionClose = true;

            var response = this._client.SendAsync(request).Result;

            return response;
        }

        public HttpResponseMessage Delete<T>(Uri address, T transferObject)
        {
            this._client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, address);

            request.Content = new StringContent(JsonConvert.SerializeObject(transferObject), Encoding.UTF8, "application/json");

            request.Headers.ConnectionClose = true;

            var response = this._client.SendAsync(request).Result;

            return response;
        }

        public HttpResponseMessage Put<T>(Uri address, T transferObject)
        {
            this._client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, address);

            request.Content = new StringContent(JsonConvert.SerializeObject(transferObject), Encoding.UTF8, "application/json");

            request.Headers.ConnectionClose = true;

            var response = this._client.SendAsync(request).Result;

            return response;
        }
    }
}
