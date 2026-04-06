using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WIS.HttpLegacy
{
    public class HttpClientSingleton
    {
        private HttpClientSingleton()
        {
        }

        public static HttpClient Instance { get; } = new HttpClient();
    }
}
