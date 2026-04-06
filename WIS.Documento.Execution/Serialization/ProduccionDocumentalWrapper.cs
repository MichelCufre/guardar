using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using WIS.Serialization;
using WIS.Serialization.Binders;

namespace WIS.Documento.Execution.Serialization
{
    public class ProduccionDocumentalWrapper : TransferWrapper, IProduccionDocumentalWrapper
    {
        public override ISerializationBinder GetSerializationBinder()
        {
            return new CustomSerializationBinder(new List<Type> {
                 typeof(ProduccionDocumentalRequest),
                 typeof(ProduccionDocumentalResponse),
            });
        }
    }
}
