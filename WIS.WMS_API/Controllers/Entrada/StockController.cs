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
    public class StockController : ControllerBaseExtension
    {
        private readonly IStockMapper _stockMapper;
        private readonly IStockService _stockService;
        private readonly ILogger<StockController> _logger;
        private readonly IEjecucionService _ejecucionService;
        private readonly IValidationService _validationService;
        public StockController(ILogger<StockController> logger, IStockService stockService, IStockMapper stockMapper, IEjecucionService ejecucionService, IValidationService validationService)
        {
            this._logger = logger;
            this._stockMapper = stockMapper;
            this._stockService = stockService;
            this._ejecucionService = ejecucionService;
            _validationService = validationService;
        }

        #region Ajuste

        /// <summary>swagger_summary_stock_ajustar</summary>
        /// <remarks>swagger_remarks_stock_ajustar</remarks>
        /// <returns>swagger_returns_stock_ajustar</returns>
        /// <response code="200">swagger_response_200_stock_ajustar</response>
        /// <response code="400">swagger_response_400_stock_ajustar</response>
        [HttpPost("Ajustar")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(CInterfazExterna.AjustarStock)]
        [SwaggerRequestType(typeof(AjustesDeStockRequest))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AjustesDeStockResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Ajustar()
        {
            try
            {
                var request = await GetRequest<AjustesDeStockRequest>();

                var empresa = request.Empresa;
                var archivo = request.Archivo;
                var ds_referencia = request.DsReferencia ?? "Ajuste Stock";
				string idRequest = request.IdRequest ?? "";

				var data = JsonConvert.SerializeObject(request);
                var loginName = GetLoginName(); 

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

				var ejecucion = await _ejecucionService.AddEjecucion(CInterfazExterna.AjustarStock, empresa, ds_referencia, data, archivo, loginName, idRequest);

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
        ///     swagger_summary_stock_reprocesarajuste
        /// </summary>
        /// <remarks>swagger_remarks_stock_reprocesarajuste</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_stock_reprocesarajuste</returns>
        /// <response code="200">swagger_response_200_stock_reprocesarajuste</response>
        /// <response code="400">swagger_response_400_stock_reprocesarajuste</response>
        [HttpPost("ReprocesarAjuste")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(CInterfazExterna.AjustarStock)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AjustesDeStockResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> ReprocesarAjuste([FromBody] ReprocesamientoRequest request)
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

                if (itfz.CdInterfazExterna != CInterfazExterna.AjustarStock)
                {
                    var error = new Error("WMSAPI_msg_Error_InterfazExternaInvalida", nuInterfaz, CInterfazExterna.AjustarStock);
                    return Problem400BadRequest(_validationService.Translate(error));
                }

                if (itfz.Situacion != SituacionDb.ProcesadoConError)
                {
                    var error = new Error("WMSAPI_msg_Error_InterfazSinEstado", nuInterfaz, SituacionDb.ProcesadoConError);
                    return Problem400BadRequest(_validationService.Translate(error));
                }

                itfz = await _ejecucionService.IniciarReprocesamiento(itfz);

                var itfzData = await _ejecucionService.GetEjecucionData(nuInterfaz);

                return await CreateOrUpdate(JsonConvert.DeserializeObject<AjustesDeStockRequest>(Encoding.UTF8.GetString(itfzData.Data)), itfz);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                throw ex;
            }
        }

        private async Task<IActionResult> CreateOrUpdate(AjustesDeStockRequest request, InterfazEjecucion ejecucion)
        {
            try
            {
                var ajustes = _stockMapper.Map(request, ejecucion);
                var result = await _stockService.ProcesarAjuste(ajustes, (ejecucion.UserId ?? 0));

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

            return Ok(new AjustesDeStockResponse(ejecucion));
        }

        #endregion

        #region Transferencia
        /// <summary>swagger_summary_stock_transferir</summary>
        /// <remarks>swagger_remarks_stock_transferir</remarks>
        /// <returns>swagger_returns_stock_transferir</returns>
        /// <response code="200">swagger_response_200_stock_transferir</response>
        /// <response code="400">swagger_response_400_stock_transferir</response>
        [HttpPost("Transferir")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(CInterfazExterna.TransferenciaStock)]
        [SwaggerRequestType(typeof(TransferenciaStockRequest))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TransferenciaStockResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Transferir()
        {
            try
            {
                var request = await GetRequest<TransferenciaStockRequest>();

                var empresa = request.Empresa;
                var archivo = request.Archivo;
                var ds_referencia = request.DsReferencia ?? "Transferir Stock";
				string idRequest = request.IdRequest ?? "";

				var data = JsonConvert.SerializeObject(request);
                var loginName = GetLoginName();

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

				var ejecucion = await _ejecucionService.AddEjecucion(CInterfazExterna.TransferenciaStock, empresa, ds_referencia, data, archivo, loginName, idRequest);

                return await Transferencia(request, ejecucion);
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
        ///     swagger_summary_stock_reprocesartransferencia
        /// </summary>
        /// <remarks>swagger_remarks_stock_reprocesartransferencia</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_stock_reprocesartransferencia</returns>
        /// <response code="200">swagger_response_200_stock_reprocesartransferencia</response>
        /// <response code="400">swagger_response_400_stock_reprocesartransferencia</response>
        [HttpPost("ReprocesarTransferencia")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(CInterfazExterna.TransferenciaStock)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TransferenciaStockResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> ReprocesarTransferencia([FromBody] ReprocesamientoRequest request)
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

                if (itfz.CdInterfazExterna != CInterfazExterna.TransferenciaStock)
                {
                    var error = new Error("WMSAPI_msg_Error_InterfazExternaInvalida", nuInterfaz, CInterfazExterna.TransferenciaStock);
                    return Problem400BadRequest(_validationService.Translate(error));
                }

                if (itfz.Situacion != SituacionDb.ProcesadoConError)
                {
                    var error = new Error("WMSAPI_msg_Error_InterfazSinEstado", nuInterfaz, SituacionDb.ProcesadoConError);
                    return Problem400BadRequest(_validationService.Translate(error));
                }

                itfz = await _ejecucionService.IniciarReprocesamiento(itfz);

                var itfzData = await _ejecucionService.GetEjecucionData(nuInterfaz);

                return await Transferencia(JsonConvert.DeserializeObject<TransferenciaStockRequest>(Encoding.UTF8.GetString(itfzData.Data)), itfz);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                throw ex;
            }
        }

        private async Task<IActionResult> Transferencia(TransferenciaStockRequest request, InterfazEjecucion ejecucion)
        {
            try
            {
                var transferencias = _stockMapper.Map(request);
                var result = await _stockService.ProcesarTransferencia(transferencias, (ejecucion.UserId ?? 0));

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

        #region MovimientoStockAutomatismo
        /// <summary>
        ///     swagger_summary_stock_MovimientoStockAutomatismo
        /// </summary>
        /// <remarks>swagger_remarks_MovimientoStockAutomatismo</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_MovimientoStockAutomatismo</returns>
        /// <response code="200">swagger_response_200_MovimientoStockAutomatismo</response>
        /// <response code="400">swagger_response_400_MovimientoStockAutomatismo</response>
        [HttpPost("MovimientoStockAutomatismo")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(CInterfazExterna.TransferenciaStock)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TransferenciaStockResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> MovimientoStockAutomatismo([FromBody] TransferenciaStockRequest request)
        {
            try
            {
                var empresa = request.Empresa;
                var archivo = request.Archivo;
                var ds_referencia = request.DsReferencia ?? "Movimiento Entrada AS";
                string idRequest = request.IdRequest ?? "";

                var data = JsonConvert.SerializeObject(request);
                var loginName = GetLoginName();

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

                var ejecucion = await _ejecucionService.AddEjecucion(CInterfazExterna.TransferenciaStock, empresa, ds_referencia, data, archivo, loginName, idRequest);

                return await MovimientoStockAutomatismo(request, ejecucion);
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



        private async Task<IActionResult> MovimientoStockAutomatismo(TransferenciaStockRequest request, InterfazEjecucion ejecucion)
        {
            try
            {
                var transferencias = _stockMapper.Map(request);
                var result = await _stockService.ProcesarTransferenciaAutomatismo(transferencias, (ejecucion.UserId ?? 0));

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

            return Ok(new TransferenciaStockResponse(ejecucion));
        }
        #endregion
    }
}
