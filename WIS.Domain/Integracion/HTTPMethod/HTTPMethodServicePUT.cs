using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace WIS.Domain.Integracion.HTTPMethod
{
    public class HTTPMethodServicePUT : HTTPMethodService
    {
        protected readonly string _secret;

        public HTTPMethodServicePUT(IHttpContextAccessor httpContextAccessor, ILogger logger, int timeout, string secret)
            : base(httpContextAccessor, logger, timeout)
        {
            _secret = secret;
        }

        public override Task<(TOut, HttpResponseMessage)> ExecuteAsync<TOut>(object requestData)
        {
            throw new System.NotImplementedException();
        }
    }
}
