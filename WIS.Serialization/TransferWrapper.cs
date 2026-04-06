using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using WIS.Serialization.Converters;

namespace WIS.Serialization
{
    public abstract class TransferWrapper : ITransferWrapper
    {
        public string Application { get; set; }
        public int User { get; set; }
        public string Predio { get; set; }
        public string Data { get; set; }
        public string PageToken { get; set; }
        public TransferWrapperStatus Status { get; set; }
        public string Message { get; set; }
        public string[] MessageArguments { get; set; }
        public string SessionData { get; set; }

        public TransferWrapper()
        {
            this.Status = TransferWrapperStatus.Ok;
        }
        public TransferWrapper(ITransferWrapper wrapper)
        {
            this.Application = wrapper.Application;
            this.User = wrapper.User;
            this.Status = TransferWrapperStatus.Ok;
        }
        public TransferWrapper(string application, int user, string pageToken)
        {
            this.Application = application;
            this.User = user;
            this.Status = TransferWrapperStatus.Ok;
            this.PageToken = pageToken;
        }

        public virtual void SetData(Object data)
        {
            var settings = new JsonSerializerSettings
            {
                //PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                FloatParseHandling = FloatParseHandling.Decimal,
                //TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = this.GetSerializationBinder()
            };

            this.Data = JsonConvert.SerializeObject(data, settings);
        }
        public virtual T GetData<T>(bool preserveReferences = false, IList<JsonConverter> converters = null)
        {
            if (this.Data == null)
                return default(T);

            var settings = new JsonSerializerSettings
            {
                //TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = this.GetSerializationBinder()
            };

            if (converters != null)
            {
                foreach (var converter in converters)
                {
                    settings.Converters.Add(converter);
                }
            }

            if (preserveReferences)
            {
                //settings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                settings.FloatParseHandling = FloatParseHandling.Decimal;
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }

            settings.Error = delegate (object sender, ErrorEventArgs args)
            {
                args.ErrorContext.Handled = true;
            };

            return JsonConvert.DeserializeObject<T>(this.Data, settings);
        }
        public virtual void SetSessionData(Object data)
        {
            var settings = new JsonSerializerSettings
            {
                FloatParseHandling = FloatParseHandling.Decimal //TODO: Pasar a configuración de sistema
            };

            settings.Converters.Insert(0, new PrimitiveJsonConverter());

            this.SessionData = JsonConvert.SerializeObject(data, Formatting.None, settings);
        }
        public virtual Dictionary<string, object> GetSessionData()
        {
            if (this.SessionData == null)
                return default;

            var settings = new JsonSerializerSettings
            {
                FloatParseHandling = FloatParseHandling.Decimal
            };

            settings.Converters.Insert(0, new PrimitiveJsonConverter());

            return JsonConvert.DeserializeObject<Dictionary<string, object>>(this.SessionData, settings);
        }
        /// <summary>
        /// Resuelve las relaciones de objetos dentro del serializado y retorna un objeto sin relaciones
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual string GetResolvedData<T>(IList<JsonConverter> converters = null)
        {
            var data = this.GetData<T>(false, converters);

            var settings = new JsonSerializerSettings
            {
                FloatParseHandling = FloatParseHandling.Decimal,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var serializedData = JsonConvert.SerializeObject(data, settings);

            return serializedData;
        }
        public virtual void SetError(string message, string[] arguments = null)
        {
            this.Status = TransferWrapperStatus.Error;
            this.Message = message;
            this.MessageArguments = arguments;
        }

        public virtual ISerializationBinder GetSerializationBinder()
        {
            //Esto se define por seguridad, no se permite pasar tipos no esperados
            throw new NotImplementedException();
        }
    }
}
