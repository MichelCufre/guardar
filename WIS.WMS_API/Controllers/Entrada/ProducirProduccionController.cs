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
    public class ProducirProduccionController : ControllerBaseExtension
    {
        protected readonly IProducirProduccionMapper _mapper;
        protected readonly IEjecucionService _ejecucionService;
        protected readonly IProducirProduccionService _service;
        protected readonly IValidationService _validationService;
        protected readonly ILogger<ProducirProduccionController> _logger;
        protected const int _interfazExterna = CInterfazExterna.ProducirProduccion;

        public ProducirProduccionController(IProducirProduccionMapper mapper,
            IEjecucionService ejecucionService,
            IProducirProduccionService produccionService,
            IValidationService validationService,
            ILogger<ProducirProduccionController> logger)
        {
            _mapper = mapper;
            _ejecucionService = ejecucionService;
            _service = produccionService;
            _validationService = validationService;
            _logger = logger;
        }

        /// <summary>swagger_summary_producirproduccion_produce</summary>
        /// <remarks>swagger_remarks_producirproduccion_produce</remarks>
        /// <returns>swagger_returns_producirproduccion_produce</returns>
        /// <response code="200">swagger_response_200_producirproduccion_produce</response>
        /// <response code="400">swagger_response_400_producirproduccion_produce</response>
        [HttpPost("Produce")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [SwaggerRequestType(typeof(ProducirProduccionRequest))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProducirProduccionResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public virtual async Task<IActionResult> Produce()
        {
            try
            {
                var request = await GetRequest<ProducirProduccionRequest>();

                int empresa = request.Empresa;
                string archivo = request.Archivo;
                string ds_referencia = request.DsReferencia ?? "Producción";
                string idRequest = request.IdRequest ?? string.Empty;

                var data = JsonConvert.SerializeObject(request);
                var loginName = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                InterfazEjecucion ejecucion = await _ejecucionService.AddEjecucion(_interfazExterna, empresa, ds_referencia, data, archivo, loginName, idRequest);

                return await Produce(request, ejecucion);
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
        ///     swagger_summary_producirproduccion_reprocess
        /// </summary>
        /// <remarks>swagger_remarks_producirproduccion_reprocess</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_producirproduccion_reprocess</returns>
        /// <response code="200">swagger_response_200_producirproduccion_reprocess</response>
        /// <response code="400">swagger_response_400_producirproduccion_reprocess</response>
        [HttpPost("Reprocess")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProducirProduccionResponse))]
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

                return await Produce(JsonConvert.DeserializeObject<ProducirProduccionRequest>(Encoding.UTF8.GetString(itfzData.Data)), itfz);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                throw ex;
            }
        }

        private async Task<IActionResult> Produce(ProducirProduccionRequest request, InterfazEjecucion ejecucion)
        {
            try
            {
                var produccion = _mapper.Map(request);
                var result = await _service.ProcesarProduccion(produccion, ejecucion.UserId ?? 0);

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

            return Ok(new ProducirProduccionResponse(ejecucion));
        }

    }
}
