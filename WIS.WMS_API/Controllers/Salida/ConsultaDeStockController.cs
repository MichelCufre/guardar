using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.API.Dtos.Salida;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.WMS_API.Extensions;
using WIS.WMS_API.Models.Mappers.Interfaces;

namespace WIS.WMS_API.Controllers.Salida
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class ConsultaDeStockController : ControllerBaseExtension
    {
        private readonly IStockMapper _stockMapper;
        private readonly IStockService _stockService;
        private readonly ILogger<ConsultaDeStockController> _logger;
        private readonly IValidationService _validationService;
        private const int _interfazExterna = CInterfazExterna.ConsultaDeStock;

        public ConsultaDeStockController(ILogger<ConsultaDeStockController> logger, 
            IStockMapper stockMapper, 
            IStockService stockService,
            IValidationService validationService)
        {
            _logger = logger;
            _stockMapper = stockMapper;
            _stockService = stockService;
            _validationService = validationService;
        }

        /// <summary>
        ///     swagger_summary_consultadestock_getdata
        /// </summary>
        /// <remarks>swagger_remarks_consultadestock_getdata</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_consultadestock_getdata</returns>
        /// <response code="200">swagger_response_200_consultadestock_getdata</response>
        /// <response code="401">swagger_response_401_consultadestock_getdata</response>
        /// <response code="404">swagger_response_404_consultadestock_getdata</response>
        [HttpPost("GetData")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ConsultaStockResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetData([FromBody] StockRequest request)
        {
            try
            {
                var loginName = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var filtros = _stockMapper.Map(request);
                var result = await _stockService.GetStock(filtros, loginName, request.Empresa);

                if (result.ValidationsResult.HasError())
                {
                    string finalMessage = JsonConvert.SerializeObject(result.ValidationsResult.Errors);
                    return Problem400BadRequest(finalMessage, $"Errores en la consulta");
                }
                return Ok(result.StockResult);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                return Problem500InternalServerError();
            }

        }
    }
}
