using System.Threading.Tasks;
using WIS.Common.API.Dtos;

namespace WIS.AttributeValidator.Interfaces
{
    public interface IAttributeValidationService
    {
        Task<AttributeValidationResponse> Validate(AttributeValidationRequest request);
    }
}
