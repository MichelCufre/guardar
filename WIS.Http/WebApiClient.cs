using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WIS.Http
{
    public class WebApiClient : IWebApiClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly int _timeout;

        public WebApiClient(IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._httpClientFactory = httpClientFactory;
            this._timeout = int.Parse(configuration.GetSection("AppSettings:InternalTimeout")?.Value ?? "30");
        }

        public async Task<HttpResponseMessage> GetAsync(Uri address, CancellationToken cancelToken)
        {
            var client = this._httpClientFactory.CreateClient();

            await SetAccessToken(client);

            client.Timeout = TimeSpan.FromMinutes(_timeout);

            return await client.GetAsync(address, cancelToken);
        }

        public async Task<HttpResponseMessage> GetAsync(Uri address, Dictionary<string, string> parameters, CancellationToken cancelToken)
        {
            var client = this._httpClientFactory.CreateClient();
            var queryItems = new List<string>();

            await SetAccessToken(client);

            foreach (var parameter in parameters)
            {
                queryItems.Add($"{parameter.Key}={parameter.Value}");
            }

            string query = string.Join("&", queryItems);

            var addressAndQuery = new Uri(address, query);

            client.Timeout = TimeSpan.FromMinutes(_timeout);

            return await client.GetAsync(addressAndQuery, cancelToken);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string uri, int? timeout, string application, string operation, T transferObject, CancellationToken cancelToken)
        {
            var client = this._httpClientFactory.CreateClient();
            var address = new Uri(new Uri(uri), "api/" + application + "/" + operation);
            var request = new HttpRequestMessage(HttpMethod.Post, address);

            await SetAccessToken(client);

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(JsonConvert.SerializeObject(transferObject), Encoding.UTF8, "application/json");

            request.Headers.ConnectionClose = true;

            client.Timeout = TimeSpan.FromMinutes(timeout ?? _timeout);

            return await client.SendAsync(request, cancelToken);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string uri, string application, string operation, T transferObject, CancellationToken cancelToken)
        {
            return await PostAsync<T>(uri, (int?)null, application, operation, transferObject, cancelToken);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string uri, int? timeout, string application, T transferObject, CancellationToken cancelToken)
        {
            var client = this._httpClientFactory.CreateClient();
            var address = new Uri(new Uri(uri), "api/" + application);
            var request = new HttpRequestMessage(HttpMethod.Post, address);

            await SetAccessToken(client);

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(JsonConvert.SerializeObject(transferObject), Encoding.UTF8, "application/json");

            request.Headers.ConnectionClose = true;

            client.Timeout = TimeSpan.FromMinutes(timeout ?? _timeout);

            return await client.SendAsync(request, cancelToken);
        }

        private async Task SetAccessToken(HttpClient client)
        {
            if (this._httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false)
            {
                var access_token = await this._httpContextAccessor.HttpContext.GetTokenAsync("access_token");
                client.DefaultRequestHeaders.Add("access_token", access_token);
            }
            else if (this._httpContextAccessor.HttpContext?.Request?.Headers?.ContainsKey("access_token") ?? false)
            {
                var access_token = this._httpContextAccessor.HttpContext.Request.Headers["access_token"][0];
                client.DefaultRequestHeaders.Add("access_token", access_token);
            }
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string uri, string application, T transferObject, CancellationToken cancelToken)
        {
            return await PostAsync(uri, (int?)null, application, transferObject, cancelToken);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(Uri address, T transferObject, CancellationToken cancelToken)
        {
            var client = this._httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, address);

            await SetAccessToken(client);

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(JsonConvert.SerializeObject(transferObject), Encoding.UTF8, "application/json");

            request.Headers.ConnectionClose = true;

            client.Timeout = TimeSpan.FromMinutes(_timeout);

            return await client.SendAsync(request, cancelToken);
        }

        public async Task<HttpResponseMessage> DeleteAsync<T>(Uri address, T transferObject, CancellationToken cancelToken)
        {
            var client = this._httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Delete, address);

            await SetAccessToken(client);

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(JsonConvert.SerializeObject(transferObject), Encoding.UTF8, "application/json");

            request.Headers.ConnectionClose = true;

            client.Timeout = TimeSpan.FromMinutes(_timeout);

            return await client.SendAsync(request, cancelToken);
        }

        public async Task<HttpResponseMessage> DeleteAsync<T>(HttpClient client, Uri address, T transferObject, CancellationToken cancelToken)
        {
            await SetAccessToken(client);

            client.Timeout = TimeSpan.FromMinutes(_timeout);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var request = new HttpRequestMessage(HttpMethod.Delete, address);

            request.Content = new StringContent(JsonConvert.SerializeObject(transferObject), Encoding.UTF8, "application/json");

            request.Headers.ConnectionClose = true;

            return await client.SendAsync(request, cancelToken);
        }

        public async Task<HttpResponseMessage> PutAsync<T>(HttpClient client, Uri address, T transferObject, CancellationToken cancelToken)
        {
            client.Timeout = TimeSpan.FromMinutes(_timeout);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            await SetAccessToken(client);

            var request = new HttpRequestMessage(HttpMethod.Put, address);

            request.Content = new StringContent(JsonConvert.SerializeObject(transferObject), Encoding.UTF8, "application/json");

            request.Headers.ConnectionClose = true;

            return await client.SendAsync(request, cancelToken);
        }

        public HttpResponseMessage Get(Uri address)
        {
            var client = this._httpClientFactory.CreateClient();

            SetAccessToken(client).Wait();

            client.Timeout = TimeSpan.FromMinutes(_timeout);

            var task = client.GetAsync(address);
            task.Wait();

            return task.Result;
        }

        public HttpResponseMessage Get(Uri address, Dictionary<string, string> parameters)
        {
            var client = this._httpClientFactory.CreateClient();
            var queryItems = new List<string>();

            SetAccessToken(client).Wait();

            foreach (var parameter in parameters)
            {
                queryItems.Add($"{parameter.Key}={parameter.Value}");
            }

            string query = "?" + string.Join("&", queryItems);

            var addressAndQuery = new Uri(address, query);

            client.Timeout = TimeSpan.FromMinutes(_timeout);

            var task = client.GetAsync(addressAndQuery);
            task.Wait();

            return task.Result;
        }

        public HttpResponseMessage Post<T>(string uri, string application, string operation, T transferObject)
        {
            var client = this._httpClientFactory.CreateClient();
            var address = new Uri(new Uri(uri), "api/" + application + "/" + operation);
            var request = new HttpRequestMessage(HttpMethod.Post, address);

            SetAccessToken(client).Wait();

            request.Method = HttpMethod.Post;
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(JsonConvert.SerializeObject(transferObject), Encoding.UTF8, "application/json");

            request.Headers.ConnectionClose = true;

            client.Timeout = TimeSpan.FromMinutes(_timeout);

            var task = client.SendAsync(request);
            task.Wait();

            return task.Result;
        }

        public HttpResponseMessage Post<T>(Uri address, T transferObject)
        {
            var client = this._httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, address);

            SetAccessToken(client).Wait();

            request.Method = HttpMethod.Post;
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(JsonConvert.SerializeObject(transferObject), Encoding.UTF8, "application/json");

            request.Headers.ConnectionClose = true;

            client.Timeout = TimeSpan.FromMinutes(_timeout);

            var task = client.SendAsync(request);
            task.Wait();

            return task.Result;
        }

        public HttpResponseMessage Post(string endpoint, string method, string content)
        {
            var address = new Uri(new Uri(endpoint), method);
            var client = this._httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, address);

            SetAccessToken(client).Wait();

            request.Method = HttpMethod.Post;
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(content, Encoding.UTF8, "application/json");

            request.Headers.ConnectionClose = true;

            client.Timeout = TimeSpan.FromMinutes(_timeout);

            var task = client.SendAsync(request);
            task.Wait();

            return task.Result;
        }

        public HttpResponseMessage Delete<T>(Uri address, T transferObject)
        {
            var client = this._httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Delete, address);

            SetAccessToken(client).Wait();

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(JsonConvert.SerializeObject(transferObject), Encoding.UTF8, "application/json");

            request.Headers.ConnectionClose = true;

            client.Timeout = TimeSpan.FromMinutes(_timeout);

            var task = client.SendAsync(request);
            task.Wait();

            return task.Result;
        }

        public HttpResponseMessage Put<T>(Uri address, T transferObject)
        {
            var client = this._httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Put, address);

            SetAccessToken(client).Wait();

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(JsonConvert.SerializeObject(transferObject), Encoding.UTF8, "application/json");

            request.Headers.ConnectionClose = true;

            client.Timeout = TimeSpan.FromMinutes(_timeout);

            var task = client.SendAsync(request);
            task.Wait();

            return task.Result;
        }
    }
}
