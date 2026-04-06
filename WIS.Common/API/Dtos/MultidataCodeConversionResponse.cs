using System.Collections.Generic;
using WIS.Common.API.Attributes;

namespace WIS.Common.API.Dtos
{
    public class MultidataCodeConversionResponse
    {
        public string Error {  get; set; }
        public List<MultidataCodeConversionAIResponse> AIs { get; set; } = new List<MultidataCodeConversionAIResponse>();
    }

    public class MultidataCodeConversionAIResponse
    {
        [ApiDtoExample("01234567890123")]
        public string Value { get; set; }

        [ApiDtoExample("01")]
        public string AI { get; set; }
    }
}
