using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Serialization.Converters;

namespace WIS.WebApplication.Extensions
{
    public static class SessionExtension
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };

            settings.Converters.Insert(0, new PrimitiveJsonConverter());

            session.SetString(key, JsonConvert.SerializeObject(value, settings));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };

            settings.Converters.Insert(0, new PrimitiveJsonConverter());

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value, settings);
        }
    }
}
