using WIS.Common.API.Attributes;

namespace WIS.Common.API.Dtos
{
    public class AttributeValidationResponse
    {
        [ApiDtoExample("false")]
        public bool IsValid { get; set; }

        [ApiDtoExample("Formato inválido")]
        public string Error { get; set; }
    }
}
