using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WIS.Common.Extensions
{
    public static class JsonExtension
    {
        public static T Deserialize<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string Serialize(this object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public static JToken GetField(JObject obj, string name) =>
            obj.Properties().FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase))?.Value;
    }
}
