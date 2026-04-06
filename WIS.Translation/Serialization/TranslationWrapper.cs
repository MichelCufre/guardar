using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using WIS.Serialization;
using WIS.Serialization.Binders;

namespace WIS.Translation.Serialization
{
    public class TranslationWrapper : TransferWrapper, ITransferWrapper
    {
        public override ISerializationBinder GetSerializationBinder()
        {
            //Esto se define por seguridad, no se permite pasar tipos no esperados
            return new CustomSerializationBinder(new List<Type> {
                typeof(TranslatedValue),
                typeof(TranslationVersion),
            });
        }
    }
}
