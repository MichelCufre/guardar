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
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.Exceptions;
using WIS.WMS_API.Extensions;
using WIS.WMS_API.Models.Mappers.Interfaces;

namespace Custom.WMS_API.Controllers.Entrada
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class ReferenciaRecepcionController : ControllerBaseExtension
    {
        private readonly IEjecucionService _ejecucionService;
        private readonly IReferenciaRecepcionMapper _referenciaMapper;
        private readonly IAgendaMapper _agendaMapper;
        private readonly IReferenciaRecepcionService _referenciaService;
        private readonly IAgendaService _agendaService;
        private readonly IAgenteService _agenteService;
        private readonly ILogger<ReferenciaRecepcionController> _logger;
        private readonly IValidationService _validationService;
        private const int _interfazExterna = CInterfazExterna.ReferenciaDeRecepcion;

        public ReferenciaRecepcionController(
            ILogger<ReferenciaRecepcionController> logger,
            IReferenciaRecepcionService referenciaService,
            IAgendaService agendaService,
            IAgenteService agenteService,
            IReferenciaRecepcionMapper referenciaMapper,
            IAgendaMapper agendaMapper,
            IEjecucionService ejecucionService,
            IValidationService validationService)
        {
            _logger = logger;
            _referenciaService = referenciaService;
            _agendaService = agendaService;
            _agenteService = agenteService;
            _referenciaMapper = referenciaMapper;
            _agendaMapper = agendaMapper;
            _ejecucionService = ejecucionService;
            _validationService = validationService;
        }

        /// <summary>swagger_summary_referenciarecepcion_create</summary>
        /// <remarks>swagger_remarks_referenciarecepcion_create</remarks>
        /// <returns>swagger_returns_referenciarecepcion_create</returns>
        /// <response code="200">swagger_response_200_referenciarecepcion_create</response>
        /// <response code="400">swagger_response_400_referenciarecepcion_create</response>
        [HttpPost("Create")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [SwaggerRequestType(typeof(ReferenciasConAgendaRequest))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReferenciasRecepcionResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Create()
        {
            try
            {
                var request = await GetRequest<ReferenciasConAgendaRequest>();

                int empresa = request.Empresa;
                string archivo = request.Archivo;
                string ds_referencia = request.DsReferencia ?? "Manejo de Referencias de Recepción";
                string idRequest = request.IdRequest ?? "";

                var data = JsonConvert.SerializeObject(request);
                var loginName = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                InterfazEjecucion ejecucion = await _ejecucionService.AddEjecucion(_interfazExterna, empresa, ds_referencia, data, archivo, loginName, idRequest);

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

        /// <summary>swagger_summary_referenciarecepcion_reprocess</summary>
        /// <remarks>swagger_remarks_referenciarecepcion_reprocess</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_referenciarecepcion_reprocess</returns>
        /// <response code="200">swagger_response_200_referenciarecepcion_reprocess</response>
        /// <response code="400">swagger_response_400_referenciarecepcion_reprocess</response>
        [HttpPost("Reprocess")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReferenciasRecepcionResponse))]
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

                return await Create(JsonConvert.DeserializeObject<ReferenciasConAgendaRequest>(Encoding.UTF8.GetString(itfzData.Data)), itfz);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                throw ex;
            }
        }

        /// <summary>swagger_summary_referenciarecepcion_getreferencia</summary>
        /// <param name="nuReferencia" example="REF1">swagger_param_nureferencia_referenciarecepcion_getreferencia</param>
        /// <param name="empresa" example="1">swagger_param_empresa_referenciarecepcion_getreferencia</param>
        /// <param name="tipo" example="OC">swagger_param_tipo_referenciarecepcion_getreferencia</param>
        /// <param name="tipoAgente" example="PRO">swagger_param_tipoagente_referenciarecepcion_getreferencia</param>
        /// <param name="codigoAgente" example="AGE01">swagger_param_codigoagente_referenciarecepcion_getreferencia</param>
        /// <returns>swagger_returns_referenciarecepcion_getreferencia</returns>
        /// <response code="200">swagger_response_200_referenciarecepcion_getreferencia</response>
        /// <response code="401">swagger_response_401_referenciarecepcion_getreferencia</response>
        /// <response code="404">swagger_response_404_referenciarecepcion_getreferencia</response>
        [HttpGet("GetReferencia")]
        [EmpresaAccessValidation(_interfazExterna, true)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReferenciaRecepcionResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetReferencia([RequiredValidation] string nuReferencia, [RequiredValidation, ExisteEmpresaValidation] int empresa, [RequiredValidation] string tipo, [RequiredValidation] string tipoAgente, [RequiredValidation] string codigoAgente)
        {
            try
            {
                var referencia = await _referenciaService.GetReferencia(nuReferencia, empresa, tipo, tipoAgente, codigoAgente);
                if (referencia != null)
                    return Ok(_referenciaMapper.MapToResponse(referencia, tipoAgente, codigoAgente));

                var error = new Error("WMSAPI_msg_Error_ReferenciaNoEncontrada", nuReferencia, empresa);

                return Problem404NotFound(_validationService.Translate(error));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                return Problem500InternalServerError();
            }
        }

        /// <summary>swagger_summary_referenciarecepcion_getreferenciabyid</summary>
        /// <param name="idReferencia" example="1146">swagger_param_idreferencia_referenciarecepcion_getreferenciabyid</param>
        /// <returns>swagger_returns_referenciarecepcion_getreferenciabyid</returns>
        /// <response code="200">swagger_response_200_referenciarecepcion_getreferenciabyid</response>
        /// <response code="401">swagger_response_401_referenciarecepcion_getreferenciabyid</response>
        /// <response code="404">swagger_response_404_referenciarecepcion_getreferenciabyid</response>
        [HttpGet("GetReferenciaById")]
        [ReferenciaAccessValidation(_interfazExterna)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReferenciaRecepcionResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetReferenciaById([RequiredValidation] int idReferencia)
        {
            try
            {
                var referencia = await _referenciaService.GetReferenciaById(idReferencia);
                if (referencia != null)
                {
                    var agente = await _agenteService.GetAgente(referencia.CodigoCliente, referencia.IdEmpresa);
                    return Ok(_referenciaMapper.MapToResponse(referencia, agente.Tipo, agente.Codigo));
                }

                var error = new Error("WMSAPI_msg_Error_ReferenciaIDNoEncontrada", idReferencia);

                return Problem404NotFound(_validationService.Translate(error));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                return Problem500InternalServerError();
            }
        }

        private async Task<IActionResult> Create(ReferenciasConAgendaRequest request, InterfazEjecucion ejecucion)
        {
            try
            {
                // 1. Crear agendas primero
                var agendasRequest = request.ToAgendasRequest();
                var agendas = _agendaMapper.Map(agendasRequest);
                var agendaResult = await _agendaService.AgregarAgendas(request.Empresa, agendas, (ejecucion.UserId ?? 0));

                if (agendaResult.HasError())
                {
                    ejecucion.Situacion = SituacionDb.ProcesadoConError;
                    ejecucion.ErrorCarga = agendaResult.HasProceduralError() ? "N" : "S";
                    ejecucion.ErrorProcedimiento = agendaResult.HasProceduralError() ? "S" : "N";

                    await _ejecucionService.AddErrores(ejecucion, agendaResult.Errors);

                    var errorDetail = JsonConvert.SerializeObject(agendaResult.Errors);
                    var errorTitle = new Error("WMSAPI_msg_Error_ErrorInterfaz", ejecucion.Id);

                    return Problem400BadRequest(errorDetail, _validationService.Translate(errorTitle), ejecucion.Id);
                }

                // 2. Crear referencias
                var referenciasRequest = request.ToReferenciasRecepcionRequest();
                List<ReferenciaRecepcion> referencias = _referenciaMapper.Map(referenciasRequest);
                ValidationsResult result = await _referenciaService.AgregarReferencias(referencias, (ejecucion.UserId ?? 0));

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

            return Ok(new ReferenciasRecepcionResponse(ejecucion));
        }
    }
}
