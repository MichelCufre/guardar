using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.API.Dtos.Salida;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.Exceptions;
using WIS.WMS_API.Extensions;
using WIS.WMS_API.Helpers;
using WIS.WMS_API.Models.Mappers.Interfaces;

namespace WIS.WMS_API.Controllers.Salida
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class SalidaController : ControllerBaseExtension
    {
        private readonly IEjecucionMapper _ejecucionMapper;
        private readonly ILogger<SalidaController> _logger;
        private readonly IEjecucionService _ejecucionService;
        private readonly IParameterService _parameterService;

        public SalidaController(IEjecucionMapper ejecucionMapper, ILogger<SalidaController> logger, IEjecucionService ejecucionService, IParameterService parameterService)
        {
            _logger = logger;
            _ejecucionMapper = ejecucionMapper;
            _ejecucionService = ejecucionService;
            _parameterService = parameterService;
        }

		/// <summary>
		///     swagger_summary_salida_confirmarlectura
		/// </summary>
		/// <remarks>swagger_remarks_salida_confirmarlectura</remarks>
		/// <param name="request"></param>
		/// <returns>swagger_returns_salida_confirmarlectura</returns>
		/// <response code="200">swagger_response_200_salida_confirmarlectura</response>
		/// <response code="400">swagger_response_400_salida_confirmarlectura</response>
		/// <response code="404">swagger_response_404_salida_confirmarlectura</response>
		/// <response code="401">swagger_response_401_salida_confirmarlectura</response>
		[HttpPost("ConfirmarLectura")]
        [Consumes("application/json")]
        [EjecucionAccessValidation]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> ConfirmarLectura([FromBody] ConfirmarLecturaRequest request)
        {
            var empresa = request.Empresa;
            var nroEjecucion = request.NumeroInterfazEjecucion;
            var estado = request.Resultado;
            var errores = request.Errores;

            try
            {
                var gruposConsulta = UserHelper.GetGruposConsulta(HttpContext, empresa, _ejecucionService, _parameterService);
                await _ejecucionService.ConfirmarLectura(nroEjecucion, empresa, gruposConsulta, estado, errores);

                return Ok("Operación realizada con éxito.");
            }
            catch (EntityNotFoundException ex)
            {
                return Problem404NotFound(ex.Message);
            }
            catch (ValidationFailedException ex)
            {
                return Problem400BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                return Problem500InternalServerError();
            }
        }

		/// <summary>
		///     swagger_summary_salida_getejecucionespendientes
		/// </summary>
		/// <remarks>swagger_remarks_salida_getejecucionespendientes</remarks>
		/// <param name="empresa">swagger_param_empresa_salida_getejecucionespendientes</param>
		/// <returns>swagger_returns_salida_getejecucionespendientes</returns>
		/// <response code="200">swagger_response_200_salida_getejecucionespendientes</response>
		/// <response code="401">swagger_response_401_salida_getejecucionespendientes</response>
		[HttpGet("GetEjecucionesPendientes")]
        [EmpresaAccessValidation(CInterfazExterna.SinEspecificar, true, false)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EjecucionesPendientesResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetEjecucionesPendientes([RequiredValidation] int empresa)
        {
            try
            {
                var gruposConsulta = UserHelper.GetGruposConsulta(HttpContext, empresa, _ejecucionService, _parameterService);
                var interfaces = await _ejecucionService.GetSalidasPendientes(empresa, gruposConsulta);

                return Ok(_ejecucionMapper.MapToResponse(interfaces));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                return Problem500InternalServerError();
            }
        }

		/// <summary>
		///     swagger_summary_salida_consultarestado
		/// </summary>
		/// <remarks>swagger_remarks_salida_consultarestado</remarks>
		/// <param name="nroEjecucion" example="4564">swagger_param_nroejecucion_salida_getejecucionespendientes</param>
		/// <param name="empresa" example="1">swagger_param_empresa_salida_getejecucionespendientes</param>
		/// <returns>swagger_returns_salida_getejecucionespendientes</returns>
		/// <response code="200">swagger_response_200_salida_getejecucionespendientes</response>
		/// <response code="400">swagger_response_400_salida_getejecucionespendientes</response>
		/// <response code="404">swagger_response_404_salida_getejecucionespendientes</response>
		/// <response code="401">swagger_response_401_salida_getejecucionespendientes</response>
		[HttpGet("ConsultarEstado")]
        [EmpresaAccessValidation(CInterfazExterna.SinEspecificar, true, false)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EstadoEjecucionResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> ConsultarEstado([RequiredValidation] long nroEjecucion, [RequiredValidation] int empresa)
        {
            try
            {
                var gruposConsulta = UserHelper.GetGruposConsulta(HttpContext, empresa, _ejecucionService, _parameterService);
                var estado = await _ejecucionService.ConsultarEstado(nroEjecucion, empresa, gruposConsulta);

                return Ok(_ejecucionMapper.MapToResponse(estado));
            }
            catch (EntityNotFoundException ex)
            {
                return Problem404NotFound(ex.Message);
            }
            catch (ValidationFailedException ex)
            {
                return Problem400BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                return Problem500InternalServerError();
            }
        }
    }
}
