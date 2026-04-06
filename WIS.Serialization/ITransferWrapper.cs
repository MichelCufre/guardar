using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Serialization
{
    public interface ITransferWrapper
    {
        string Application { get; set; }
        int User { get; set; }
        string Data { get; set; }
        string Predio { get; set; }
        string PageToken { get; set; }
        TransferWrapperStatus Status { get; set; }
        string Message { get; set; }
        string[] MessageArguments { get; set; }
        string SessionData { get; set; }

        void SetData(Object data);
        T GetData<T>(bool preserveReferences = false, IList<JsonConverter> converters = null);
        void SetSessionData(Object data);
        Dictionary<string, object> GetSessionData();
        /// <summary>
        /// Resuelve las relaciones de objetos dentro del serializado y retorna un objeto sin relaciones
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string GetResolvedData<T>(IList<JsonConverter> converters = null);
        void SetError(string message, string[] arguments = null);

        ISerializationBinder GetSerializationBinder();
    }
}
