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
    public class LpnController : ControllerBaseExtension
    {
        private readonly ILpnMapper _lpnMapper;
        private readonly ILpnService _lpnService;
        private readonly ILogger<LpnController> _logger;
        private readonly IEjecucionService _ejecucionService;
        private readonly IValidationService _validationService;
        private const int _interfazExterna = CInterfazExterna.Lpn;

        public LpnController(ILpnMapper lpnMapper,
            ILpnService lpnService,
            ILogger<LpnController> logger,
            IEjecucionService ejecucionService,
            IValidationService validationService)
        {
            _lpnMapper = lpnMapper;
            _lpnService = lpnService;
            _logger = logger;
            _ejecucionService = ejecucionService;
            _validationService = validationService;
        }

        /// <summary>swagger_summary_lpn_create</summary>
		/// <remarks>swagger_remarks_lpn_create</remarks>
		/// <returns>swagger_returns_lpn_create</returns>
		/// <response code="200">swagger_response_200_lpn_create</response>
		/// <response code="400">swagger_response_400_lpn_create</response>
        [HttpPost("Create")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [SwaggerRequestType(typeof(LpnsRequest))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LpnsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Create()
        {
            try
            {
                var request = await GetRequest<LpnsRequest>();

                int empresa = request.Empresa;
                string archivo = request.Archivo;
                string ds_referencia = request.DsReferencia ?? "Manejo de Lpn";
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
		///     swagger_summary_lpn_reprocess
		/// </summary>
		/// <remarks>swagger_remarks_lpn_reprocess</remarks>
		/// <param name="request"></param>
		/// <returns>swagger_returns_lpn_reprocess</returns>
		/// <response code="200">swagger_response_200_lpn_reprocess</response>
		/// <response code="400">swagger_response_400_lpn_reprocess</response>
        [HttpPost("Reprocess")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LpnsResponse))]
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

                return await Create(JsonConvert.DeserializeObject<LpnsRequest>(Encoding.UTF8.GetString(itfzData.Data)), itfz);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                throw ex;
            }
        }

        private async Task<IActionResult> Create(LpnsRequest request, InterfazEjecucion ejecucion)
        {
            var ids = new List<CreateLpnResponse>();

            try
            {
                var lpns = _lpnMapper.Map(request);
                var result = await _lpnService.AgregarLpns(lpns, request.Empresa, (ejecucion.UserId ?? 0));

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
                    ids.AddRange(lpns.Select(a => new CreateLpnResponse(a.Tipo, a.IdExterno, a.NumeroLPN)));
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

            return Ok(new LpnsResponse(ejecucion, ids));
        }

        /// <summary>
        ///     swagger_summary_lpn_getlpn
        /// </summary>
        /// <remarks>swagger_remarks_lpn_getlpn</remarks>
        /// <param name="nuLpn"></param>
        /// <returns>swagger_returns_lpn_getlpn</returns>
        /// <response code="200">swagger_response_200_lpn_getlpn</response>
        /// <response code="401">swagger_response_401_lpn_getlpn</response>
        /// <response code="404">swagger_response_404_lpn_getlpn</response>
        [HttpGet("GetLpn")]
        [LpnAccessValidation(_interfazExterna)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LpnResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetLpn([RequiredValidation] long nuLpn)
        {
            try
            {
                var lpn = await _lpnService.GetLpn(nuLpn);

                if (lpn != null)
                {
                    return Ok(_lpnMapper.MapToResponse(lpn));
                }

                var error = new Error("WMSAPI_msg_Error_LpnNoEncontrado", nuLpn);

                return Problem404NotFound(_validationService.Translate(error));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                return Problem500InternalServerError();
            }
        }

        /// <summary>
        ///     swagger_summary_lpn_getlpn
        /// </summary>
        /// <remarks>swagger_remarks_lpn_getlpnbyidexternotipo</remarks>
        /// <param name="idExterno"></param>
        /// <param name="tpLpn"></param>
        /// <returns>swagger_returns_lpn_getlpnbyidexternotipo</returns>
        /// <response code="200">swagger_response_200_lpn_getlpn</response>
        /// <response code="401">swagger_response_401_lpn_getlpn</response>
        /// <response code="404">swagger_response_404_lpn_getlpn</response>
        [HttpGet("GetLpnByIdExternoTipo")]
        [LpnAccessValidation(_interfazExterna)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LpnResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetLpnByIdExternoTipo([RequiredValidation] string idExterno, [RequiredValidation] string tpLpn)
        {
            try
            {
                var lpn = await _lpnService.GetLpn(idExterno, tpLpn);

                if (lpn != null)
                {
                    return Ok(_lpnMapper.MapToResponse(lpn));
                }

                var error = new Error("WMSAPI_msg_Error_LpnExternoTipoNoEncontrado", new object[] { idExterno, tpLpn });

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
