using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Http.Extensions
{
    public static class HttpContentExtension
    {
        public static async Task<T> ReadAsStringAndDeserializeAsync<T>(this HttpContent message)
        {
            return JsonConvert.DeserializeObject<T>(await message.ReadAsStringAsync());
        }
    }
}
