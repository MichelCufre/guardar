using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WIS.HttpLegacy.WebApi
{
    public static class HttpClientAccessor
    {
        public static Func<HttpClient> ValueFactory = () => {
            return new HttpClient() 
            {
                Timeout = TimeSpan.FromMinutes(30),
            };
        };

        private static readonly Lazy<HttpClient> client = new Lazy<HttpClient>(ValueFactory);

        public static HttpClient HttpClient
        {
            get { return client.Value; }
        }
    }
}
