using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
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
    public class AgenteController : ControllerBaseExtension
    {
        private readonly IAgenteMapper _agenteMapper;
        private readonly IAgenteService _agenteService;
        private readonly ILogger<AgenteController> _logger;
        private readonly IEjecucionService _ejecucionService;
        private readonly IValidationService _validationService;
        private const int _interfazExterna = CInterfazExterna.Agentes;

        public AgenteController(ILogger<AgenteController> logger,
            IAgenteService agenteService,
            IAgenteMapper agenteMapper,
            IEjecucionService ejecucionService,
            IValidationService validationService)
        {
            this._logger = logger;
            this._agenteMapper = agenteMapper;
            this._agenteService = agenteService;
            this._ejecucionService = ejecucionService;
            _validationService = validationService;
        }

        /// <summary>swagger_summary_agente_createorupdate</summary>
        /// <remarks>swagger_remarks_agente_createorupdate</remarks>
        /// <returns>swagger_returns_agente_createorupdate</returns>
        /// <response code="200">swagger_response_200_agente_createorupdate</response>
        /// <response code="400">swagger_response_400_agente_createorupdate</response>
        [HttpPost("CreateOrUpdate")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [SwaggerRequestType(typeof(AgentesRequest))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AgentesResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> CreateOrUpdate()
        {
            try
            {
                var request = await GetRequest<AgentesRequest>();

                int empresa = request.Empresa;
                string archivo = request.Archivo;
                string ds_referencia = request.DsReferencia ?? "Manejo de Agentes";
                string idRequest = request.IdRequest ?? "";
                var data = JsonConvert.SerializeObject(request);
                var loginName = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                InterfazEjecucion ejecucion = await _ejecucionService.AddEjecucion(_interfazExterna, empresa, ds_referencia, data, archivo, loginName, idRequest);

                return await CreateOrUpdate(request, ejecucion);
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
        ///     swagger_summary_agente_reprocess
        /// </summary>
        /// <remarks>swagger_remarks_agente_reprocess</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_agente_reprocess</returns>
        /// <response code="200">swagger_response_200_agente_reprocess</response>
        /// <response code="400">swagger_response_400_agente_reprocess</response>
        [HttpPost("Reprocess")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AgentesResponse))]
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

                return await CreateOrUpdate(JsonConvert.DeserializeObject<AgentesRequest>(Encoding.UTF8.GetString(itfzData.Data)), itfz);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                throw ex;
            }
        }

        private async Task<IActionResult> CreateOrUpdate(AgentesRequest request, InterfazEjecucion ejecucion)
        {
            try
            {
                List<Agente> agentes = _agenteMapper.Map(request);
                ValidationsResult result = await _agenteService.AgregarAgentes(agentes, (ejecucion.UserId ?? 0));

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

            return Ok(new AgentesResponse(ejecucion));
        }

        /// <summary>
        ///     swagger_summary_agente_getagente
        /// </summary>
        /// <remarks>swagger_remarks_agente_getagente</remarks>
        /// <param name="codigo" example="CLI">swagger_param_codigo_agente_getagente</param>
        /// <param name="empresa" example="1">swagger_param_empresa_agente_getagente</param>
        /// <param name="tipo" example="CLI">swagger_param_tipo_agente_getagente</param>
        /// <returns>swagger_returns_agente_getagente</returns>
        /// <response code="200">swagger_response_200_agente_getagente</response>
        /// <response code="401">swagger_response_401_agente_getagente</response>
        /// <response code="404">swagger_response_404_agente_getagente</response>
        [HttpGet("GetAgente")]
        [EmpresaAccessValidation(_interfazExterna, true)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AgenteResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetAgente([RequiredValidation] string codigo, [RequiredValidation] int empresa, [RequiredValidation] string tipo)
        {
            try
            {
                var agente = await _agenteService.GetAgente(codigo, empresa, tipo);

                if (agente != null)
                    return Ok(_agenteMapper.MapToResponse(agente));

                var error = new Error("WMSAPI_msg_Error_AgenteNoEncontrado", codigo, empresa.ToString(), tipo);

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
