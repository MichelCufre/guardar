using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using WIS.Serialization;
using WIS.Serialization.Binders;

namespace WIS.Documento.Execution.Serialization
{
    public class TransferenciaDocumentalWrapper : TransferWrapper, ITransferenciaDocumentalWrapper
    {
        public override ISerializationBinder GetSerializationBinder()
        {
            return new CustomSerializationBinder(new List<Type> {
                 typeof(TransferenciaDocumentalRequest),
                 typeof(TransferenciaDocumentalResponse),
            });
        }
    }
}
