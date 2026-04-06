using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Interfaces;
using WIS.Domain.Picking.Dtos;
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
    public class PreparacionController : ControllerBaseExtension
    {
        private readonly IPreparacionMapper _mapper;
        private readonly IPreparacionService _service;
        private readonly ILogger<PreparacionController> _logger;
        private readonly IEjecucionService _ejecucionService;
        private readonly IValidationService _validationService;

        public PreparacionController(ILogger<PreparacionController> logger, IPreparacionService service, IPreparacionMapper mapper, IEjecucionService ejecucionService, IValidationService validationService)
        {
            this._logger = logger;
            this._mapper = mapper;
            this._service = service;
            this._ejecucionService = ejecucionService;
            _validationService = validationService;
        }

        #region Picking
        /// <summary>swagger_summary_preparacion_picking</summary>
        /// <remarks>swagger_remarks_preparacion_picking</remarks>
        /// <returns>swagger_returns_preparacion_picking</returns>
        /// <response code="200">swagger_response_200_preparacion_picking</response>
        /// <response code="400">swagger_response_400_preparacion_picking</response>
        [HttpPost("Picking")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(CInterfazExterna.Picking)]
        [SwaggerRequestType(typeof(PickingRequest))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PickingResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Picking()
        {
            try
            {
                var request = await GetRequest<PickingRequest>();

                int empresa = request.Empresa;
                string archivo = request.Archivo;
                string ds_referencia = request.DsReferencia ?? "Picking";
				string idRequest = request.IdRequest ?? "";

				var data = JsonConvert.SerializeObject(request);
                var loginName = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

				if (request.Usuario != null)
                {
                    if (_ejecucionService.IsValidUser(request.Usuario))
                        loginName = request.Usuario.LoginName;
                    else
                    {
                        var error = new Error("WMSAPI_msg_Error_UsuarioNoValido", request.Usuario.LoginName);
                        return Problem404NotFound(_validationService.Translate(error));
                    }
                }

                var ejecucion = await _ejecucionService.AddEjecucion(CInterfazExterna.Picking, empresa, ds_referencia, data, archivo, loginName, idRequest);

                return await Pickear(request, ejecucion);
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
        ///     swagger_summary_preparacion_reprocesarpicking
        /// </summary>
        /// <remarks>swagger_remarks_preparacion_reprocesarpicking</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_preparacion_reprocesarpicking</returns>
        /// <response code="200">swagger_response_200_preparacion_reprocesarpicking</response>
        /// <response code="400">swagger_response_400_preparacion_reprocesarpicking</response>
        [HttpPost("ReprocesarPicking")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(CInterfazExterna.Picking)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PickingResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> ReprocesarPicking([FromBody] ReprocesamientoRequest request)
        {
            try
            {
                var nuInterfaz = request.Interfaz;

                if (!await _ejecucionService.ExisteEjecucion(nuInterfaz))
				{
                    var error = new Error("WMSAPI_msg_Error_InterfazNoExiste", nuInterfaz);
                    return Problem400BadRequest(_validationService.Translate(error));
                }

                var itfz = await _ejecucionService.GetEjecucion(nuInterfaz);

                if (itfz.CdInterfazExterna != CInterfazExterna.Picking)
                {
                    var error = new Error("WMSAPI_msg_Error_InterfazExternaInvalida", nuInterfaz, CInterfazExterna.Picking);
                    return Problem400BadRequest(_validationService.Translate(error));
                }

                if (itfz.Situacion != SituacionDb.ProcesadoConError)
				{
                    var error = new Error("WMSAPI_msg_Error_InterfazSinEstado", nuInterfaz, SituacionDb.ProcesadoConError);
                    return Problem400BadRequest(_validationService.Translate(error));
                }

                itfz = await _ejecucionService.IniciarReprocesamiento(itfz);

                var itfzData = await _ejecucionService.GetEjecucionData(nuInterfaz);

                return await Pickear(JsonConvert.DeserializeObject<PickingRequest>(Encoding.UTF8.GetString(itfzData.Data)), itfz);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                throw ex;
            }
        }

        private async Task<IActionResult> Pickear(PickingRequest request, InterfazEjecucion ejecucion)
        {
            try
            {
                var pickeos = _mapper.Map(request, (ejecucion.UserId ?? 0));
                var result = await _service.ProcesarPicking(pickeos, (ejecucion.UserId ?? 0));

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

            return Ok(new TransferenciaStockResponse(ejecucion));
        }

        #endregion

        #region AnularPickingPedidoPendiente
        /// <summary>swagger_summary_preparacion_anularpickingpedidopendiente</summary>
        /// <remarks>swagger_remarks_preparacion_anularpickingpedidopendiente</remarks>
        /// <returns>swagger_returns_preparacion_anularpickingpedidopendiente</returns>
        /// <response code="200">swagger_response_200_preparacion_anularpickingpedidopendiente</response>
        /// <response code="400">swagger_response_400_preparacion_anularpickingpedidopendiente</response>
        [HttpPost("AnularPickingPedidoPendiente")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(CInterfazExterna.AnularPickingPedidoPendiente)]
        [SwaggerRequestType(typeof(AnularPickingPedidoPendienteRequest))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnularPickingPedidoPendienteResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> AnularPickingPedidoPendiente()
        {
            try
            {
                var request = await GetRequest<AnularPickingPedidoPendienteRequest>();

                int empresa = request.Empresa;
                string archivo = request.Archivo;
                string ds_referencia = request.DsReferencia ?? "CrossDocking";
				string idRequest = request.IdRequest ?? "";

				var data = JsonConvert.SerializeObject(request);
                var loginName = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (request.Usuario != null)
                {
                    if (_ejecucionService.IsValidUser(request.Usuario))
                        loginName = request.Usuario.LoginName;
                    else
                    {
                        var error = new Error("WMSAPI_msg_Error_UsuarioNoValido", request.Usuario.LoginName);
                        return Problem404NotFound(_validationService.Translate(error));
                    }
                }

				var ejecucion = await _ejecucionService.AddEjecucion(CInterfazExterna.AnularPickingPedidoPendiente, empresa, ds_referencia, data, archivo, loginName, idRequest);

                return await AnularPickingPedidoPendiente(request, ejecucion);
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
        ///     swagger_summary_preparacion_reprocesaranulacionpreparacionpedidopicking
        /// </summary>
        /// <remarks>swagger_remarks_preparacion_reprocesaranulacionpreparacionpedidopicking</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_preparacion_reprocesaranulacionpreparacionpedidopicking</returns>
        /// <response code="200">swagger_response_200_preparacion_reprocesaranulacionpreparacionpedidopicking</response>
        /// <response code="400">swagger_response_400_preparacion_reprocesaranulacionpreparacionpedidopicking</response>
        [HttpPost("ReprocesarAnularPickingPedidoPendiente")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(CInterfazExterna.AnularPickingPedidoPendiente)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnularPickingPedidoPendienteResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> ReprocesarAnulacionPreparacionPedidoPicking([FromBody] ReprocesamientoRequest request)
        {
            try
            {
                var nuInterfaz = request.Interfaz;

                if (!await _ejecucionService.ExisteEjecucion(nuInterfaz))
                {
                    var error = new Error("WMSAPI_msg_Error_InterfazNoExiste", nuInterfaz);
                    return Problem400BadRequest(_validationService.Translate(error));
                }

                var itfz = await _ejecucionService.GetEjecucion(nuInterfaz);

                if (itfz.CdInterfazExterna != CInterfazExterna.AnularPickingPedidoPendiente)
				{
                    var error = new Error("WMSAPI_msg_Error_InterfazExternaInvalida", nuInterfaz, CInterfazExterna.AnularPickingPedidoPendiente);
                    return Problem400BadRequest(_validationService.Translate(error));
                }
            
                if (itfz.Situacion != SituacionDb.ProcesadoConError)
                {
                    var error = new Error("WMSAPI_msg_Error_InterfazSinEstado", nuInterfaz, SituacionDb.ProcesadoConError);
                    return Problem400BadRequest(_validationService.Translate(error));
                }

                itfz = await _ejecucionService.IniciarReprocesamiento(itfz);

                var itfzData = await _ejecucionService.GetEjecucionData(nuInterfaz);

                return await AnularPickingPedidoPendiente(JsonConvert.DeserializeObject<AnularPickingPedidoPendienteRequest>(Encoding.UTF8.GetString(itfzData.Data)), itfz);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                throw ex;
            }
        }

        private async Task<IActionResult> AnularPickingPedidoPendiente(AnularPickingPedidoPendienteRequest request, InterfazEjecucion ejecucion)
        {
            try
            {
                List<AnularPickingPedidoPendiente> ajustes = _mapper.Map(request);
                ValidationsResult result = await _service.AnularPickingPedidoPendiente(ajustes, (ejecucion.UserId ?? 0));

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

            return Ok(new AnularPickingPedidoPendienteResponse(ejecucion));
        }

        #endregion

        #region AnularPickingPedidoPendienteAutomatismo
        /// <summary>
        ///     swagger_summary_preparacion_anularpickingpedidopendiente
        /// </summary>
        /// <remarks>swagger_remarks_preparacion_anularpickingpedidopendiente</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_preparacion_anularpickingpedidopendiente</returns>
        /// <response code="200">swagger_response_200_preparacion_anularpickingpedidopendiente</response>
        /// <response code="400">swagger_response_400_preparacion_anularpickingpedidopendiente</response>
        [HttpPost("AnularPickingPedidoPendienteAutomatismo")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(CInterfazExterna.AnularPickingPedidoPendiente)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnularPickingPedidoPendienteResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> AnularPickingPedidoPendienteAutomatismo([FromBody] AnularPickingPedidoPendienteRequest request)
        {
            try
            {
                int empresa = request.Empresa;
                string archivo = request.Archivo;
                string ds_referencia = request.DsReferencia ?? "AnularPicking AUTOMATISMO";
                string idRequest = request.IdRequest ?? "";

                var data = JsonConvert.SerializeObject(request);
                var loginName = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (request.Usuario != null)
                {
                    if (_ejecucionService.IsValidUser(request.Usuario))
                        loginName = request.Usuario.LoginName;
                    else
                    {
                        var error = new Error("WMSAPI_msg_Error_UsuarioNoValido", request.Usuario.LoginName);
                        return Problem404NotFound(_validationService.Translate(error));
                    }
                }

                var ejecucion = await _ejecucionService.AddEjecucion(CInterfazExterna.AnularPickingPedidoPendiente, empresa, ds_referencia, data, archivo, loginName, idRequest);

                return await AnularPickingPedidoPendienteAutomatismo(request, ejecucion);
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


        private async Task<IActionResult> AnularPickingPedidoPendienteAutomatismo(AnularPickingPedidoPendienteRequest request, InterfazEjecucion ejecucion)
        {
            try
            {
                List<AnularPickingPedidoPendienteAutomatismo> ajustes = _mapper.MapAutomatismo(request);
                ValidationsResult result = await _service.AnularPickingPedidoPendienteAutomatismo(ajustes, (ejecucion.UserId ?? 0));

                if (result.HasError())
                {
                    ejecucion.Situacion = SituacionDb.ProcesadoConError;
                    ejecucion.ErrorCarga = result.HasProceduralError() ? "N" : "S";
                    ejecucion.ErrorProcedimiento = result.HasProceduralError() ? "S" : "N";

                    await _ejecucionService.AddErrores(ejecucion, result.Errors);

                    var errorDetail = JsonConvert.SerializeObject(result.Errors);
                    var errorTitle = new Error("WMSAPI_msg_Error_ErrorInterfaz", ejecucion.Id);

                    return Problem400BadRequest(errorDetail, _validationService.Translate(errorTitle));
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

            return Ok(new AnularPickingPedidoPendienteResponse(ejecucion));
        }

        #endregion

        //AnularPicking
        //ReprocesarAnulacionPicking

        //SepararPicking (PTL)
        //ReprocesarSeparacion

        //CambiarContenedor (PTL)
        //ReprocesarCambioContenedor
    }
}
