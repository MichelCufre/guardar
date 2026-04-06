using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.Automatismo;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Interfaces;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.Exceptions;
using WIS.Persistence.Database;
using WIS.WMS_API.Extensions;
using WIS.WMS_API.Models.Mappers.Interfaces;

namespace WIS.WMS_API.Controllers.Entrada
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class CodigoBarrasController : ControllerBaseExtension
    {
        private readonly ICodigoBarrasMapper _barcodeMapper;
        private readonly IEjecucionService _ejecucionService;
        private readonly ICodigoBarrasService _barcodeService;
        private readonly ILogger<CodigoBarrasController> _logger;
        private readonly IValidationService _validationService;
        private const int _interfazExterna = CInterfazExterna.CodigoDeBarras;

        public CodigoBarrasController(ICodigoBarrasMapper barcodeMapper,
            ICodigoBarrasService barcodeService,
            ILogger<CodigoBarrasController> logger,
            IEjecucionService ejecucionService,
            IValidationService validationService)
        {
            _logger = logger;
            _barcodeMapper = barcodeMapper;
            _barcodeService = barcodeService;
            _ejecucionService = ejecucionService;
            _validationService = validationService;
        }

        /// <summary>swagger_summary_codigobarras_createupdateordelete</summary>
        /// <remarks>swagger_remarks_codigobarras_createupdateordelete</remarks>
        /// <returns>swagger_returns_codigobarras_createupdateordelete</returns>
        /// <response code="200">swagger_response_200_codigobarras_createupdateordelete</response>
        /// <response code="400">swagger_response_400_codigobarras_createupdateordelete</response>
        [HttpPost("CreateUpdateOrDelete")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [SwaggerRequestType(typeof(CodigosBarrasRequest))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CodigosBarrasResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> CreateUpdateOrDelete()
        {
            try
            {
                var request = await GetRequest<CodigosBarrasRequest>();

                int empresa = request.Empresa;
                string archivo = request.Archivo;
                string ds_referencia = request.DsReferencia ?? "Manejo de código de barras";
                string idRequest = request.IdRequest ?? "";
                var data = JsonConvert.SerializeObject(request);
                var loginName = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                InterfazEjecucion ejecucion = await _ejecucionService.AddEjecucion(_interfazExterna, empresa, ds_referencia, data, archivo, loginName, idRequest);

                return await CreateUpdateOrDelete(request, ejecucion);
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
        ///     swagger_summary_codigobarras_reprocess
        /// </summary>
        /// <remarks>swagger_remarks_codigobarras_reprocess</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_codigobarras_reprocess</returns>
        /// <response code="200">swagger_response_200_codigobarras_reprocess</response>
        /// <response code="400">swagger_response_400_codigobarras_reprocess</response>
        [HttpPost("Reprocess")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CodigosBarrasResponse))]
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

                return await CreateUpdateOrDelete(JsonConvert.DeserializeObject<CodigosBarrasRequest>(Encoding.UTF8.GetString(itfzData.Data)), itfz);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                throw ex;
            }
        }

        private async Task<IActionResult> CreateUpdateOrDelete(CodigosBarrasRequest request, InterfazEjecucion ejecucion)
        {
            try
            {
                List<CodigoBarras> codigos = _barcodeMapper.Map(request);
                ValidationsResult result = await _barcodeService.AgregarCodigosDeBarras(codigos, (ejecucion.UserId ?? 0));

                if (result.HasError())
                {
                    ejecucion.Situacion = SituacionDb.ProcesadoConError;
                    ejecucion.ErrorCarga = result.HasProceduralError() ? "N" : "S";
                    ejecucion.ErrorProcedimiento = result.HasProceduralError() ? "S" : "N";

                    await _ejecucionService.AddErrores(ejecucion, result.Errors);

                    var errorDetail = JsonConvert.SerializeObject(result.Errors);
                    var errorTitle = new Error("WMSAPI_msg_Error_ErrorInterfaz", ejecucion.Id);

                    return Problem400BadRequest(errorDetail, _validationService.Translate(errorTitle), ejecucion.Id);
                }
            }
            catch (AutomatismoException ex)
            {
                ejecucion.Situacion = SituacionDb.ErrorNotificacionAutomatismo;
                await _ejecucionService.UpdateEjecucion(ejecucion);

            }
            catch (Exception ex)
            {
                ejecucion.Situacion = SituacionDb.ProcesadoConError;
                ejecucion.ErrorProcedimiento = "S";

                await _ejecucionService.AddError(ejecucion, 0, ex.Message);
                await _ejecucionService.UpdateEjecucion(ejecucion);

                throw ex;
            }
            if (ejecucion.Situacion != SituacionDb.ErrorNotificacionAutomatismo)
            {
                ejecucion.Situacion = SituacionDb.ProcesadoOK;
                await _ejecucionService.UpdateEjecucion(ejecucion);
            }

            ejecucion.Situacion = SituacionDb.ProcesadoOK;
            await _ejecucionService.UpdateEjecucion(ejecucion);

            return Ok(new CodigosBarrasResponse(ejecucion));
        }

        /// <summary>
        ///     swagger_summary_codigobarras_getcodigobarras
        /// </summary>
        /// <remarks>swagger_remarks_codigobarras_getcodigobarras</remarks>
        /// <param name="codigo" example="PR1">swagger_param_codigo_codigobarras_getcodigobarras</param>
        /// <param name="empresa" example="1">swagger_param_empresa_codigobarras_getcodigobarras</param>
        /// <returns>swagger_returns_codigobarras_getcodigobarras</returns>
        /// <response code="200">swagger_response_200_codigobarras_getcodigobarras</response>
        /// <response code="401">swagger_response_401_codigobarras_getcodigobarras</response>
        /// <response code="404">swagger_response_404_codigobarras_getcodigobarras</response>
        [HttpGet("GetCodigoBarras")]
        [EmpresaAccessValidation(_interfazExterna, true)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CodigoBarrasResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetCodigoBarras([RequiredValidation] string codigo, [RequiredValidation] int empresa)
        {
            try
            {
                var barcode = await _barcodeService.GetCodigoDeBarras(codigo, empresa);

                if (barcode != null)
                    return Ok(_barcodeMapper.MapToResponse(barcode));

                var error = new Error("WMSAPI_msg_Error_CodigoDeBarrasNoEncontrado", codigo, empresa);

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
