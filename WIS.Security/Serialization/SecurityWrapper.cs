using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using WIS.Serialization;
using WIS.Serialization.Binders;

namespace WIS.Security.Serialization
{
    public class SecurityWrapper : TransferWrapper, ITransferWrapper, ISecurityWrapper
    {
        public override ISerializationBinder GetSerializationBinder()
        {
            //Esto se define por seguridad, no se permite pasar tipos no esperados
            return new CustomSerializationBinder(new List<Type> {
                typeof(SecurityRequest),
                typeof(SecurityContent)
            });
        }
    }
}
