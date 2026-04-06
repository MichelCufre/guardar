using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WIS.Serialization.Binders
{
    public class CustomSerializationBinder : ISerializationBinder
    {
        public IList<Type> KnownTypes { get; set; }

        public CustomSerializationBinder(IList<Type> knownTypes)
        {
            this.KnownTypes = knownTypes;
        }

        public Type BindToType(string assemblyName, string typeName)
        {
            return KnownTypes.SingleOrDefault(t => t.Name == typeName);
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.Name;
        }

    }
}
