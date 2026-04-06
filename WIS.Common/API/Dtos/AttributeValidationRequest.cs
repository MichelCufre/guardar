
using System.ComponentModel.DataAnnotations;
using WIS.Common.API.Attributes;

namespace WIS.Common.API.Dtos
{
    public class AttributeValidationRequest
    {
        [ApiDtoExample("1")]
        [Required]
        public int Id { get; set; }

        [ApiDtoExample("Test")]
        [Required]
        public string Value { get; set; }
    }
}
