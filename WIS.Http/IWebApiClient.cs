using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WIS.Http
{
    public interface IWebApiClient
    {
        Task<HttpResponseMessage> GetAsync(Uri address, CancellationToken cancelToken);
        Task<HttpResponseMessage> GetAsync(Uri address, Dictionary<string, string> parameters, CancellationToken cancelToken);

        Task<HttpResponseMessage> PostAsync<T>(string uri, int? timeout, string application, string operation, T transferObject, CancellationToken cancelToken);
        Task<HttpResponseMessage> PostAsync<T>(string uri, string application, string operation, T transferObject, CancellationToken cancelToken);
        Task<HttpResponseMessage> PostAsync<T>(string uri, int? timeout, string application, T transferObject, CancellationToken cancelToken);
        Task<HttpResponseMessage> PostAsync<T>(string uri, string application, T transferObject, CancellationToken cancelToken);
        Task<HttpResponseMessage> PostAsync<T>(Uri address, T transferObject, CancellationToken cancelToken);

        Task<HttpResponseMessage> DeleteAsync<T>(Uri address, T transferObject, CancellationToken cancelToken);
        Task<HttpResponseMessage> DeleteAsync<T>(HttpClient client, Uri address, T transferObject, CancellationToken cancelToken);

        Task<HttpResponseMessage> PutAsync<T>(HttpClient client, Uri address, T transferObject, CancellationToken cancelToken);

        HttpResponseMessage Get(Uri address);
        HttpResponseMessage Get(Uri address, Dictionary<string, string> parameters);
        HttpResponseMessage Post<T>(string uri, string application, string operation, T transferObject);
        HttpResponseMessage Post<T>(Uri address, T transferObject);
        HttpResponseMessage Delete<T>(Uri address, T transferObject);
        HttpResponseMessage Put<T>(Uri address, T transferObject);
        HttpResponseMessage Post(string endpoint, string method, string content);
    }
}
