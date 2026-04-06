using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WIS.Domain.Integracion.HTTPMethod;

namespace WIS.Domain.Integracion.Factories
{
    public class HTTPMethodServiceFactory
    {
        public HTTPMethodService Create(ServiceHttpProtocol method, ILogger logger, IHttpContextAccessor httpContextAccessor, int timeout, string secret)
        {
            switch (method)
            {
                case ServiceHttpProtocol.POST:
                    return new HTTPMethodServicePOST(httpContextAccessor, logger, timeout, secret);
                case ServiceHttpProtocol.GET:
                    return new HTTPMethodServiceGET(httpContextAccessor, logger, timeout);
                case ServiceHttpProtocol.PUT:
                    return new HTTPMethodServicePUT(httpContextAccessor, logger, timeout, secret);
                case ServiceHttpProtocol.DELETE:
                    return new HTTPMethodServiceDELETE(httpContextAccessor, logger, timeout);
            }

            return null;
        }
    }
}
