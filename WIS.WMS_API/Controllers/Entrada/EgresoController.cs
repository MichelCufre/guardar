using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.API.Dtos;
using WIS.Domain.General.API.Dtos.Entrada;
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
    public class EgresoController : ControllerBaseExtension
    {
        protected readonly IEgresoMapper _egresoMapper;
        protected readonly IEgresoService _egresoService;
        protected readonly IAgenteService _agenteService;
        protected readonly ILogger<EgresoController> _logger;
        protected readonly IEjecucionService _ejecucionService;
        protected readonly IValidationService _validationService;
        protected const int _interfazExterna = CInterfazExterna.Egresos;

        public EgresoController(ILogger<EgresoController> logger, 
            IEgresoMapper egresoMapper, 
            IEgresoService egresoService, 
            IAgenteService agenteService, 
            IEjecucionService ejecucionService,
            IValidationService validationService)
        {
            _logger = logger;
            _egresoMapper = egresoMapper;
            _egresoService = egresoService;
            _agenteService = agenteService;
            _ejecucionService = ejecucionService;
            _validationService = validationService;
        }

        /// <summary>swagger_summary_egreso_create</summary>
        /// <remarks>swagger_remarks_egreso_create</remarks>
        /// <returns>swagger_returns_egreso_create</returns>
        /// <response code="200">swagger_response_200_egreso_create</response>
        /// <response code="400">swagger_response_400_egreso_create</response>
        [HttpPost("Create")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [SwaggerRequestType(typeof(EgresoRequest))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EgresosResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Create()
        {
            try
            {
                var request = await GetRequest<EgresoRequest>();

                int empresa = request.Empresa;
                string archivo = request.Archivo;
                string ds_referencia = request.DsReferencia ?? "Egresos";
				string idRequest = request.IdRequest ?? "";

				var data = JsonConvert.SerializeObject(request);
                var loginName = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

				var ejecucion = await _ejecucionService.AddEjecucion(_interfazExterna, empresa, ds_referencia, data, archivo, loginName, idRequest);

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
        ///     swagger_summary_egreso_reprocess
        /// </summary>
        /// <remarks>swagger_remarks_egreso_reprocess</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_egreso_reprocess</returns>
        /// <response code="200">swagger_response_200_egreso_reprocess</response>
        /// <response code="400">swagger_response_400_egreso_reprocess</response>
        [HttpPost("Reprocess")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EgresosResponse))]
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

                return await Create(JsonConvert.DeserializeObject<EgresoRequest>(Encoding.UTF8.GetString(itfzData.Data)), itfz);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                throw ex;
            }
        }

        protected async Task<IActionResult> Create(EgresoRequest request, InterfazEjecucion ejecucion)
        {
            var ids = new List<DetalleResponse>();

            try
            {
                var egresos = _egresoMapper.Map(request);
                var result = await _egresoService.AgregarEgresos(request.Empresa, egresos, (ejecucion.UserId ?? 0));
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
                else
                    ids.AddRange(egresos.Select(a => new DetalleResponse(a.IdExterno, a.Id)));
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

            return Ok(new EgresosResponse(ejecucion, ids));
        }

        /// <summary>
        ///     swagger_summary_egreso_getegresoporcodigocamion
        /// </summary>
        /// <remarks>swagger_remarks_egreso_getegresoporcodigocamion</remarks>
        /// <param name="cdCamion" example="455">swagger_param_cdCamion_egreso_getegresoporcodigocamion</param>
        /// <param name="empresa" example="455">swagger_param_empresa_egreso_getegresoporcodigocamion</param>
        /// <returns>swagger_returns_egreso_getegresoporcodigocamion</returns>
        /// <response code="200">swagger_response_200_egreso_getegresoporcodigocamion</response>
        /// <response code="401">swagger_response_401_egreso_getegresoporcodigocamion</response>
        /// <response code="404">swagger_response_404_egreso_getegresoporcodigocamion</response>
        [HttpGet("GetEgresoPorCodigoCamion")]
        [EmpresaAccessValidation(_interfazExterna, true)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EgresoResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetEgresoPorCodigoCamion([RequiredValidation] int cdCamion, [RequiredValidation] int empresa)
        {
            try
            {
                var camion = await _egresoService.GetCamion(cdCamion);

                if (camion != null)
                {
                    var agentes = await _agenteService.GetAgentesEgreso(camion.Id);
                    return Ok(_egresoMapper.MapToResponse(camion, agentes));
                }

                var error = new Error("WMSAPI_msg_Error_EgresoNoEncontrado", cdCamion);

                return Problem404NotFound(_validationService.Translate(error));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                return Problem500InternalServerError();
            }
        }

        /// <summary>
        ///     swagger_summary_egreso_getegresoporidexterno
        /// </summary>
        /// <remarks>swagger_remarks_egreso_getegresoporidexterno</remarks>
        /// <param name="idExterno" example="455">swagger_param_idexterno_egreso_getegresoporidexterno</param>
        /// <param name="empresa" example="1">swagger_param_empresa_egreso_getegresoporidexterno</param>
        /// <returns>swagger_returns_egreso_getegresoporidexterno</returns>
        /// <response code="200">swagger_response_200_egreso_getegresoporidexterno</response>
        /// <response code="401">swagger_response_401_egreso_getegresoporidexterno</response>
        /// <response code="404">swagger_response_404_egreso_getegresoporidexterno</response>
        [HttpGet("GetEgresoPorIdExterno")]
        [EmpresaAccessValidation(_interfazExterna, true)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EgresoResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetEgresoPorIdExterno([RequiredValidation] string idExterno, [RequiredValidation] int empresa)
        {
            try
            {
                var camion = await _egresoService.GetCamionByIdExterno(idExterno, empresa);
                if (camion != null)
                {
                    var agentes = await _agenteService.GetAgentesEgreso(camion.Id);
                    return Ok(_egresoMapper.MapToResponse(camion, agentes));
                }
                var error = new Error("WMSAPI_msg_Error_EgresoExternoNoEncontrado", idExterno, empresa);

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
