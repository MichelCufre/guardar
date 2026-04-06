using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WIS.HttpLegacy.WebApi
{
    public interface IWebApiClientLegacy
    {
        HttpResponseMessage Get(Uri address);
        HttpResponseMessage Get(Uri address, Dictionary<string, string> parameters);
        Task<HttpResponseMessage> PostAsync<T>(string uri, string application, string operation, T transferObject, CancellationToken cancelToken);
        Task<HttpResponseMessage> PostAsync<T>(string uri, string application, T transferObject, CancellationToken cancelToken);
        Task<HttpResponseMessage> PostAsync<T>(Uri address, T transferObject, CancellationToken cancelToken);
        HttpResponseMessage Post<T>(string uri, string application, string operation, T transferObject);
        HttpResponseMessage Post<T>(Uri address, T transferObject);

        HttpResponseMessage Delete<T>(Uri address, T transferObject);

        HttpResponseMessage Put<T>(Uri address, T transferObject);
    }
}
