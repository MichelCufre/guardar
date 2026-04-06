using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WIS.Domain.Validation
{
    public class AutomatismoAccessValidation : TypeFilterAttribute
    {
        public AutomatismoAccessValidation() : base(typeof(AutomatismoActionFilter))
        {

        }

        private class AutomatismoActionFilter : IActionFilter
        {
            private IConfiguration _configuration;

            public AutomatismoActionFilter(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                var secret = _configuration.GetSection("AuthSettings:IntegrationSecret")?.Value;

                if (string.IsNullOrEmpty(secret))
                {
                    secret = _configuration.GetSection("IntegrationSettings:Secret")?.Value;
                }

                if (!string.IsNullOrEmpty(secret))
                {
                    var request = context.HttpContext.Request;
                    var payload = string.Empty;

                    request.Body.Position = 0;

                    using (var sr = new StreamReader(request.Body, Encoding.UTF8))
                    {
                        payload = sr.ReadToEnd();
                    }

                    if (request.Headers.TryGetValue("X-Hub-Signature", out StringValues xHubSignature))
                    {
                        if (xHubSignature.Count > 0)
                        {
                            var hash = ComputeHash(secret, payload);
                            var signature = Convert.FromBase64String(xHubSignature[0]);

                            if (!CryptographicOperations.FixedTimeEquals(hash, signature))
                            {
                                context.Result = new ObjectResult("Operación no autorizada") { StatusCode = 401 };
                            }
                        }
                    }
                    else
                    {
                        context.Result = new ObjectResult("Operación no autorizada") { StatusCode = 401 };
                    }
                }
            }

            public byte[] ComputeHash(string secret, string payload)
            {
                var bytes = Encoding.UTF8.GetBytes(secret);
                using (var hmac = new HMACSHA512(bytes))
                {
                    bytes = Encoding.UTF8.GetBytes(payload);
                    return hmac.ComputeHash(bytes);
                }
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {

            }
        }
    }
}
