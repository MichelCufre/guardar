using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WIS.AttributeValidator.Interfaces;
using WIS.Common.API.Dtos;

namespace WIS.AttributeValidator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class AttributeValidatorController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IAttributeValidationService _attributeValidationService;

        public AttributeValidatorController(ILogger<AttributeValidatorController> logger,
            IAttributeValidationService attributeValidationService)
        {
            _logger = logger;
            _attributeValidationService = attributeValidationService;
        }

        /// <summary>
        ///     swagger_summary_attributevalidator_validate
        /// </summary>
        /// <remarks>swagger_remarks_attributevalidator_validate</remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">swagger_response_200_attributevalidator_validate</response>
        /// <response code="400">swagger_response_400_attributevalidator_validate</response>
        [HttpPost("Validate")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AttributeValidationResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Validate([FromBody] AttributeValidationRequest request)
        {
            _logger.LogDebug($"Id: {request.Id}, Value: {request.Value}");

            return Ok(await this._attributeValidationService.Validate(request));
        }
    }
}
