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
    public class AgendaController : ControllerBaseExtension
    {
        private readonly IAgendaMapper _agendaMapper;
        private readonly IAgenteService _agenteService;
        private readonly IAgendaService _agendaService;
        private readonly ILogger<AgendaController> _logger;
        private readonly IEjecucionService _ejecucionService;
        private readonly IValidationService _validationService;
        private const int _interfazExterna = CInterfazExterna.Agendas;

        public AgendaController(IAgendaMapper agendaMapper,
            IAgendaService agendaService,
            ILogger<AgendaController> logger,
            IEjecucionService ejecucionService,
            IAgenteService agenteService,
            IValidationService validationService)
        {
            _agendaMapper = agendaMapper;
            _agendaService = agendaService;
            _logger = logger;
            _ejecucionService = ejecucionService;
            _agenteService = agenteService;
            _validationService = validationService;
        }

        /// <summary>swagger_summary_agenda_create</summary>
        /// <remarks>swagger_remarks_agenda_create</remarks>
        /// <returns>swagger_returns_create</returns>
        /// <response code="200">swagger_response_200_agenda_create</response>
        /// <response code="400">swagger_response_400_agenda_create</response>
        [HttpPost("Create")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [SwaggerRequestType(typeof(AgendasRequest))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AgendasResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Create()
        {
            try
            {
                var request = await GetRequest<AgendasRequest>();

                int empresa = request.Empresa;
                string archivo = request.Archivo;
                string dsReferencia = request.DsReferencia ?? "Manejo de Agendas";
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
		///     swagger_summary_agenda_reprocess
		/// </summary>
		/// <remarks>swagger_remarks_agenda_reprocess</remarks>
		/// <param name="request"></param>
		/// <returns>swagger_returns_reprocess</returns>
		/// <response code="200">swagger_response_200_agenda_reprocess</response>
		/// <response code="400">swagger_response_400_agenda_reprocess</response>
        [HttpPost("Reprocess")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AgendasResponse))]
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

                return await Create(JsonConvert.DeserializeObject<AgendasRequest>(Encoding.UTF8.GetString(itfzData.Data)), itfz);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                throw ex;
            }
        }

        private async Task<IActionResult> Create(AgendasRequest request, InterfazEjecucion ejecucion)
        {
            var agendaIds = new List<int>();

            try
            {
                var agendas = _agendaMapper.Map(request);
                var result = await _agendaService.AgregarAgendas(request.Empresa, agendas, (ejecucion.UserId ?? 0));

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
                {
                    agendaIds.AddRange(agendas.Select(a => a.Id));
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

            return Ok(new AgendasResponse(ejecucion, agendaIds));
        }

        /// <summary>
        ///     swagger_summary_agenda_getagenda
        /// </summary>
        /// <remarks>swagger_remarks_agenda_getagenda</remarks>
        /// <param name="nuAgenda"></param>
        /// <returns>swagger_returns_getagenda</returns>
        /// <response code="200">swagger_response_200_agenda_getagenda</response>
        /// <response code="401">swagger_response_401_agenda_getagenda</response>
        /// <response code="404">swagger_response_404_agenda_getagenda</response>
        [HttpGet("GetAgenda")]
        [AgendaAccessValidation(_interfazExterna)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AgendaResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetAgenda([RequiredValidation] int nuAgenda)
        {
            try
            {
                var agenda = await _agendaService.GetAgenda(nuAgenda);

                if (agenda != null)
                {
                    var agente = await _agenteService.GetAgente(agenda.CodigoInternoCliente, agenda.IdEmpresa);
                    return Ok(_agendaMapper.MapToResponse(agenda, agente.Codigo, agente.Tipo));
                }

                var error = new Error("WMSAPI_msg_Error_AgendaNoEncontrada", nuAgenda);

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
