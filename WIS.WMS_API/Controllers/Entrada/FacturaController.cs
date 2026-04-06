using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.API.Dtos;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.General.API.Dtos.Salida;
using WIS.Domain.Interfaces;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.Exceptions;
using WIS.WMS_API.Extensions;
using WIS.WMS_API.Models.Mappers.Interfaces;

namespace WIS.WMS_API.Controllers.Entrada
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class FacturaController : ControllerBaseExtension
    {
        private readonly ILogger<FacturaController> _logger;
        private readonly IEjecucionService _ejecucionService;
        private readonly IFacturaMapper _facturaMapper;
        private readonly IFacturaService _facturaService;
        private readonly IValidationService _validationService;
        private const int _interfazExterna = CInterfazExterna.Facturas;

        public FacturaController(
            ILogger<FacturaController> logger,
            IEjecucionService ejecucionService,
            IFacturaMapper facturaMapper,
            IValidationService validationService,
            IFacturaService facturaService)
        {
            _facturaMapper = facturaMapper;
            _logger = logger;
            _ejecucionService = ejecucionService;
            _facturaService = facturaService;
            _validationService = validationService;
        }

        /// <summary>
        ///     swagger_summary_factura_create
        /// </summary>
        /// <remarks>swagger_remarks_factura_create</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_create</returns>
        /// <response code="200">swagger_response_200_factura_create</response>
        /// <response code="400">swagger_response_400_factura_create</response>

        [HttpPost("Create")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [SwaggerRequestType(typeof(FacturasRequest))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FacturasResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Create()
        {
            try
            {
                var request = await GetRequest<FacturasRequest>();

                int empresa = request.Empresa;
                string archivo = request.Archivo;
                string dsReferencia = request.DsReferencia ?? "Manejo de Facturas";
                string idRequest = request.IdRequest ?? "";

                var data = JsonConvert.SerializeObject(request);
                var loginName = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var ejecucion = await _ejecucionService.AddEjecucion(_interfazExterna, empresa, dsReferencia, data, archivo, loginName, idRequest);

                return await Create(request, ejecucion);
            }
            catch (ValidationFailedException ex)
            {
                return Problem400BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                throw ex;
            }
        }

        /// <summary>
        ///     swagger_summary_factura_reprocess
        /// </summary>
        /// <remarks>swagger_remarks_factura_reprocess</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_reprocess</returns>
        /// <response code="200">swagger_response_200_factura_reprocess</response>
        /// <response code="400">swagger_response_400_factura_reprocess</response>
        [HttpPost("Reprocess")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FacturasResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Reprocess([FromBody] ReprocesamientoRequest request)
        {
            try
            {
                var nuInterfaz = request.Interfaz;
                var result = await _validationService.ValidateReprocess(nuInterfaz, _interfazExterna);

                if (!string.IsNullOrEmpty(result.Error))
                    return Problem400BadRequest(result.Error);

                var itfz = await _ejecucionService.IniciarReprocesamiento(result.Value);
                var itfzData = await _ejecucionService.GetEjecucionData(nuInterfaz);

                return await Create(JsonConvert.DeserializeObject<FacturasRequest>(Encoding.UTF8.GetString(itfzData.Data)), itfz);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                throw ex;
            }
        }

        private async Task<IActionResult> Create(FacturasRequest request, InterfazEjecucion ejecucion)
        {
            try
            {
                var facturas = _facturaMapper.Map(request);
                var result = await _facturaService.AgregarFacturas(facturas, ejecucion.Empresa ?? 1);

                if (result.HasError())
                {
                    ejecucion.Situacion = SituacionDb.ProcesadoConError;
                    ejecucion.ErrorCarga = result.HasProceduralError() ? "N" : "S";
                    ejecucion.ErrorProcedimiento = result.HasProceduralError() ? "S" : "N";

                    await _ejecucionService.AddErrores(ejecucion, result.Errors);

                    string errorDetail = JsonConvert.SerializeObject(result.Errors);
                    var errorTitle = new Error("WMSAPI_msg_Error_ErrorInterfaz", ejecucion.Id);

                    return Problem400BadRequest(errorDetail, _validationService.Translate(errorTitle), ejecucion.Id);
                }
            }
            catch (Exception ex)
            {
                ejecucion.Situacion = SituacionDb.ProcesadoConError;
                ejecucion.ErrorProcedimiento = "S";

                await _ejecucionService.AddError(ejecucion, 0, ex.Message);
                await _ejecucionService.UpdateEjecucion(ejecucion);
                throw ex;
            }

            ejecucion.Situacion = SituacionDb.ProcesadoOK;
            await _ejecucionService.UpdateEjecucion(ejecucion);

            return Ok(new FacturasResponse(ejecucion));
        }

        /// <summary>
        ///     swagger_summary_factura_getfactura
        /// </summary>
        /// <remarks>swagger_remarks_factura_getfactura</remarks>
        /// <param name="nuFactura" example="FAC1">swagger_param_nufactura_factura_getfactura</param>
        /// <param name="empresa" example="1">swagger_param_empresa_factura_getfactura</param>
        /// <param name="serie" example="A">swagger_param_serie_factura_getfactura</param>
        /// <param name="codigoAgente" example="AGE01">swagger_param_codigoagente_factura_getfactura</param>
        /// <param name="tipoAgente" example="PRO">swagger_param_tipoagente_factura_getfactura</param>
        /// <returns>swagger_returns_factura_getfactura</returns>
        /// <response code="200">swagger_response_200_factura_getfactura</response>
        /// <response code="401">swagger_response_401_factura_getfactura</response>
        /// <response code="404">swagger_response_404_factura_getfactura</response>
        [HttpGet("GetFactura")]
        [EmpresaAccessValidation(_interfazExterna, true)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FacturaResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetFactura([RequiredValidation] string nuFactura, [RequiredValidation, ExisteEmpresaValidation] int empresa, [RequiredValidation] string serie, [RequiredValidation] string codigoAgente, [RequiredValidation] string tipoAgente)
        {
            try
            {
                var factura = await _facturaService.GetFactura(nuFactura, empresa, serie, codigoAgente, tipoAgente);
                if (factura != null)
                    return Ok(_facturaMapper.MapToResponse(factura));

                var error = new Error("WMSAPI_msg_Error_FacturaNoEncontrada", nuFactura, empresa);

                return Problem404NotFound(_validationService.Translate(error));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                return Problem500InternalServerError();
            }
        }
    }
}
