using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using WIS.Serialization;
using WIS.Serialization.Binders;

namespace WIS.Documento.Execution.Serialization
{
    public class ReservaDocumentalWrapper : TransferWrapper, IReservaDocumentalWrapper
    {
        public override ISerializationBinder GetSerializationBinder()
        {
            return new CustomSerializationBinder(new List<Type> {
                 typeof(ReservaDocumentalRequest),
                 typeof(ReservaDocumentalResponse),
            });
        }
    }
}
