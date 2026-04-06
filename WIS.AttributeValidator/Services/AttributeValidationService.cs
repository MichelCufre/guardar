using System.Threading.Tasks;
using WIS.AttributeValidator.Interfaces;
using WIS.Common.API.Dtos;

namespace WIS.AttributeValidator.Services
{
    public class AttributeValidationService : IAttributeValidationService
    {
        public async Task<AttributeValidationResponse> Validate(AttributeValidationRequest request)
        {
            return new AttributeValidationResponse
            {
                IsValid = true,
            };
        }
    }
}
