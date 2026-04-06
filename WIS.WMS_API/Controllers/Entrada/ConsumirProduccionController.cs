using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Interfaces;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.Exceptions;
using WIS.WMS_API.Extensions;
using WIS.WMS_API.Models.Mappers.Interfaces;
using WIS.Domain.General.API.Dtos;

namespace WIS.WMS_API.Controllers.Entrada
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class ConsumirProduccionController : ControllerBaseExtension
    {
        protected readonly IConsumirProduccionMapper _mapper;
        protected readonly IEjecucionService _ejecucionService;
        protected readonly IConsumirProduccionService _service;
        protected readonly IValidationService _validationService;
        protected readonly ILogger<ConsumirProduccionController> _logger;
        protected const int _interfazExterna = CInterfazExterna.ConsumirProduccion;

        public ConsumirProduccionController(IConsumirProduccionMapper mapper,
            IEjecucionService ejecucionService,
            IConsumirProduccionService produccionService,
            IValidationService validationService,
            ILogger<ConsumirProduccionController> logger)
        {
            _mapper = mapper;
            _ejecucionService = ejecucionService;
            _service = produccionService;
            _validationService = validationService;
            _logger = logger;
        }

        /// <summary>swagger_summary_consumirproduccion_consume</summary>
        /// <remarks>swagger_remarks_consumirproduccion_consume</remarks>
        /// <returns>swagger_returns_consumirproduccion_consume</returns>
        /// <response code="200">swagger_response_200_consumirproduccion_consume</response>
        /// <response code="400">swagger_response_400_consumirproduccion_consume</response>
        [HttpPost("Consume")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [SwaggerRequestType(typeof(ConsumirProduccionRequest))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ConsumirProduccionResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public virtual async Task<IActionResult> Consume()
        {
            try
            {
                var request = await GetRequest<ConsumirProduccionRequest>();

                int empresa = request.Empresa;
                string archivo = request.Archivo;
                string ds_referencia = request.DsReferencia ?? "Producción";
                string idRequest = request.IdRequest ?? string.Empty;

                var data = JsonConvert.SerializeObject(request);
                var loginName = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                InterfazEjecucion ejecucion = await _ejecucionService.AddEjecucion(_interfazExterna, empresa, ds_referencia, data, archivo, loginName, idRequest);

                return await Consume(request, ejecucion);
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
        ///     swagger_summary_consumirproduccion_reprocess
        /// </summary>
        /// <remarks>swagger_remarks_consumirproduccion_reprocess</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_consumirproduccion_reprocess</returns>
        /// <response code="200">swagger_response_200_consumirproduccion_reprocess</response>
        /// <response code="400">swagger_response_400_consumirproduccion_reprocess</response>
        [HttpPost("Reprocess")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ConsumirProduccionResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public virtual async Task<IActionResult> Reprocess([FromBody] ReprocesamientoRequest request)
        {
            try
            {
                var nuInterfaz = request.Interfaz;
                var result = await _validationService.ValidateReprocess(nuInterfaz, _interfazExterna);

                if (!string.IsNullOrEmpty(result.Error))
                    return Problem400BadRequest(result.Error);

                var itfz = await _ejecucionService.IniciarReprocesamiento(result.Value);
                var itfzData = await _ejecucionService.GetEjecucionData(nuInterfaz);

                return await Consume(JsonConvert.DeserializeObject<ConsumirProduccionRequest>(Encoding.UTF8.GetString(itfzData.Data)), itfz);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                throw ex;
            }
        }

        private async Task<IActionResult> Consume(ConsumirProduccionRequest request, InterfazEjecucion ejecucion)
        {
            try
            {
                var consumo = _mapper.Map(request);
                var result = await _service.ProcesarConsumo(consumo, ejecucion.UserId ?? 0);

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

            return Ok(new ConsumirProduccionResponse(ejecucion));
        }

    }
}
