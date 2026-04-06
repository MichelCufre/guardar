using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace WIS.Domain.Integracion.HTTPMethod
{
    public class HTTPMethodServiceDELETE : HTTPMethodService
    {
        public HTTPMethodServiceDELETE(IHttpContextAccessor httpContextAccessor, ILogger logger, int timeout)
            : base(httpContextAccessor, logger, timeout)
        {
        }

        public override async Task<(TOut, HttpResponseMessage)> ExecuteAsync<TOut>(object requestData)
        {
            try
            {
                var baseAddress = new Uri(this._baseAddress);
                var strContent = (string)null;

                if (requestData != null)
                {
                    strContent = (requestData.GetType() == typeof(string)) ? (string)requestData : JsonConvert.SerializeObject(requestData);
                }

                using (var httpClient = new HttpClient())
                {
                    if (this._integracionServicio.Authorization != null)
                    {
                        httpClient.DefaultRequestHeaders.Authorization = this._integracionServicio.Authorization.GetAuthorizationHeaderValue(_httpContextAccessor);
                    }

                    var requestUri = new Uri(this._baseAddress);
                    var builder = new UriBuilder(requestUri);
                    var query = HttpUtility.ParseQueryString(builder.Query);

                    if (this._queryParams != null)
                    {
                        foreach (var key in this._queryParams.AllKeys)
                        {
                            query[key] = this._queryParams[key];
                        }
                    }

                    builder.Query = query.ToString();
                    requestUri = builder.Uri;

                    httpClient.Timeout = TimeSpan.FromMinutes(_timeout);

                    var response = await httpClient.DeleteAsync(requestUri);
                    var responseContent = await response?.Content?.ReadAsStringAsync();
                    var result = (responseContent == null) ? default(TOut) : JsonConvert.DeserializeObject<TOut>(responseContent);

                    return (result, response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "** ERROR ** Get GetAsync: " + ex.Message);
                throw ex;
            }
        }
    }
}
