using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
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
    [SwaggerTag("Gestion de control de calidad")]
    public class ControlDeCalidadController : ControllerBaseExtension
    {
        protected readonly IControlCalidadMapper _controlCalidadMapper;
        protected readonly IControlCalidadService _controlCalidadService;
        protected readonly IEjecucionService _ejecucionService;
        protected readonly ILogger<ControlDeCalidadController> _logger;
        protected readonly IValidationService _validationService;
        protected const int _interfazExterna = CInterfazExterna.ControlDeCalidad;

        public ControlDeCalidadController(
            IControlCalidadMapper controlCalidadMapper,
            IControlCalidadService controlCalidadService,
            IEjecucionService ejecucionService,
            ILogger<ControlDeCalidadController> logger,
            IValidationService validationService)
        {
            _controlCalidadMapper = controlCalidadMapper;
            _ejecucionService = ejecucionService;
            _logger = logger;
            _validationService = validationService;
            _controlCalidadService = controlCalidadService;
        }

        /// <summary>
        ///     swagger_summary_controldecalidad_reprocess
        /// </summary>
        /// <remarks>swagger_remarks_controldecalidad_reprocess</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_controldecalidad_reprocess</returns>
        /// <response code="200">swagger_response_200_controldecalidad_reprocess</response>
        /// <response code="400">swagger_response_400_controldecalidad_reprocess</response>
        [HttpPost("Reprocess")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ControlCalidadResponse))]
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

                return await CreateOrUpdateProcess(JsonConvert.DeserializeObject<ControlCalidadRequest>(Encoding.UTF8.GetString(itfzData.Data)), itfz);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                throw ex;
            }
        }

        /// <summary>swagger_summary_controldecalidad_createorupdate</summary>
        /// <remarks>swagger_remarks_controldecalidad_createorupdate</remarks>
        /// <returns>swagger_returns_controldecalidad_createorupdate</returns>
        /// <response code="200">swagger_response_200_controldecalidad_createorupdate</response>
        /// <response code="400">swagger_response_400_controldecalidad_createorupdate</response>
        [HttpPost("CreateOrUpdate")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [SwaggerRequestType(typeof(ControlCalidadRequest))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ControlCalidadResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> CreateOrUpdate()
        {
            try
            {
                var request = await GetRequest<ControlCalidadRequest>();

                int empresa = request.Empresa;
                string archivo = request.Archivo;
                string dsRef = request.DsReferencia ?? "Manejo de control de calidad";
                string idRequest = request.IdRequest ?? "";
                string data = JsonConvert.SerializeObject(request);
                string loginName = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var ejecucion = await _ejecucionService.AddEjecucion(_interfazExterna, empresa, dsRef, data, archivo, loginName, idRequest);

                return await this.CreateOrUpdateProcess(request, ejecucion);
            }
            catch (ValidationFailedException ex)
            {
                return Problem400BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                throw;
            }
        }

        private async Task<IActionResult> CreateOrUpdateProcess(ControlCalidadRequest request, InterfazEjecucion ejecucion)
        {
            try
            {
                var items = _controlCalidadMapper.Map(request);
                var result = await _controlCalidadService.AsignarControlCalidad(items, (ejecucion.UserId ?? 0), request.Empresa);

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
            catch (Exception e)
            {
                ejecucion.Situacion = SituacionDb.ProcesadoConError;
                ejecucion.ErrorProcedimiento = "S";

                await _ejecucionService.AddError(ejecucion, 0, e.Message);
                await _ejecucionService.UpdateEjecucion(ejecucion);

                throw;
            }

            ejecucion.Situacion = SituacionDb.ProcesadoOK;
            await _ejecucionService.UpdateEjecucion(ejecucion);

            return Ok(_controlCalidadService.GetResponse(ejecucion));
        }
    }
}
