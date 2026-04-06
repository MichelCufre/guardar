using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using WIS.Common.API.Dtos;
using WIS.MultidataCodeConverter.Interfaces;

namespace WIS.MultidataCodeConverter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class MultidataCodeConverterController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMultidataCodeConverterService _multidataCodeConverterService;

        public MultidataCodeConverterController(ILogger<MultidataCodeConverterController> logger,
            IMultidataCodeConverterService multidataCodeConverterService)
        {
            _logger = logger;
            _multidataCodeConverterService = multidataCodeConverterService;
        }

        /// <summary>
        ///     swagger_summary_multidatacodeconverter_convert
        /// </summary>
        /// <remarks>swagger_remarks_multidatacodeconverter_convert</remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <response code="200">swagger_response_200_multidatacodeconverter_convert</response>
        /// <response code="400">swagger_response_400_multidatacodeconverter_convert</response>
        [HttpPost("Convert")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MultidataCodeConversionResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Convert([FromBody] MultidataCodeConversionRequest request)
        {
            var ais = request.AIs != null ? $"[{string.Join(',', request.AIs.Select(x => $"{x.AI}:{x.Value}"))}]" : "[]";
            var context = request.Context != null ? $"[{string.Join(',', request.Context.Select(x => $"{x.Key}:{x.Value}"))}]" : "[]"; ;

            _logger.LogDebug($"Aplicacion: {request.Application}, TipoCodigo: {request.MultidataCodeType}, Codigo: {request.MultidataCode}, AIs: {ais}, Contexto: {context}");

            return Ok(await this._multidataCodeConverterService.Convert(request));
        }
    }
}
