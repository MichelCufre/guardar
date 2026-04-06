using WIS.Common.API.Attributes;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Entrada
{
	public class ProduccionInsumoRequestWithKeyProduccion : ProduccionInsumoRequest
	{
        [ApiDtoExample("Id 1")]
        [StringLengthValidation(100, MinimumLength = 0)]
        public string IdProduccionExterno { get; set; }
	}
}
