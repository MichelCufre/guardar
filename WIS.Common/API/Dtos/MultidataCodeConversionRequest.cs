
using System.Collections.Generic;
using WIS.Common.API.Attributes;

namespace WIS.Common.API.Dtos
{
    public class MultidataCodeConversionRequest
    {      
        [ApiDtoExample("REC501")]
        public string Application { get; set; }
        
        [ApiDtoExample("EAN128")]
        public string MultidataCodeType { get; set; }
        
        [ApiDtoExample("01012345678901233102123456")]
        public string MultidataCode { get; set; }
        public List<MutidataCodeConversionAIRequest> AIs { get; set; }
        public List<MutidataCodeConversionContextKeyValuePairRequest> Context { get; set; }
    }

    public class MutidataCodeConversionAIRequest
    {
        [ApiDtoExample("01234567890123")]
        public string Value { get; set; }

        [ApiDtoExample("01")]
        public string AI { get; set; }
    }

    public class MutidataCodeConversionContextKeyValuePairRequest
    {
        [ApiDtoExample("1")]
        public string Value { get; set; }

        [ApiDtoExample("NU_PREDIO")]
        public string Key { get; set; }
    }
}
