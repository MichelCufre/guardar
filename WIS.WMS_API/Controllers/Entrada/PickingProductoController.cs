using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.Automatismo;
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
    public class PickingProductoController : ControllerBaseExtension
    {
        private readonly IPickingProductoMapper _pickingProductoMapper;
        private readonly IEjecucionService _ejecucionService;
        private readonly IPickingProductoService _pickingProductoService;
        private readonly ILogger<PickingProductoController> _logger;
        private readonly IValidationService _validationService;
        private const int _interfazExterna = CInterfazExterna.PickingProducto;

        public PickingProductoController(IPickingProductoMapper pickingProductoMapper,
            IPickingProductoService pickingProductoService,
            ILogger<PickingProductoController> logger,
            IEjecucionService ejecucionService,
            IValidationService validationService)
        {
            _logger = logger;
            _pickingProductoMapper = pickingProductoMapper;
            _pickingProductoService = pickingProductoService;
            _ejecucionService = ejecucionService;
            _validationService = validationService;
        }

        /// <summary>
        ///     swagger_summary_pickingproducto_createupdateordelete
        /// </summary>
        /// <remarks>swagger_remarks_pickingproducto_createupdateordelete</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_pickingproducto_createupdateordelete</returns>
        /// <response code="200">swagger_response_200_pickingproducto_createupdateordelete</response>
        /// <response code="400">swagger_response_400_pickingproducto_createupdateordelete</response>
        [HttpPost("CreateUpdateOrDelete")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PickingProductosResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [SwaggerRequestType(typeof(PickingProductosRequest))]
        public async Task<IActionResult> CreateUpdateOrDelete()        
        {
            try
            {
                var request = await GetRequest<PickingProductosRequest>();

                int empresa = request.Empresa;
                string archivo = request.Archivo;
                string dsReferencia = request.DsReferencia ?? "Manejo de ubicaciones de picking";
                string idRequest = request.IdRequest ?? "";

                var data = JsonConvert.SerializeObject(request);
                var loginName = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                InterfazEjecucion ejecucion = await _ejecucionService.AddEjecucion(_interfazExterna, empresa, dsReferencia, data, archivo, loginName, idRequest);

                return await CreateUpdateOrDelete(request, ejecucion);
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
        ///     swagger_summary_pickingproducto_reprocess
        /// </summary>
        /// <remarks>swagger_remarks_pickingproducto_reprocess</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_pickingproducto_reprocess</returns>
        /// <response code="200">swagger_response_200_pickingproducto_reprocess</response>
        /// <response code="400">swagger_response_400_pickingproducto_reprocess</response>
        [HttpPost("Reprocess")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PickingProductosResponse))]
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

                return await CreateUpdateOrDelete(JsonConvert.DeserializeObject<PickingProductosRequest>(Encoding.UTF8.GetString(itfzData.Data)), itfz);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                throw ex;
            }
        }

        private async Task<IActionResult> CreateUpdateOrDelete(PickingProductosRequest request, InterfazEjecucion ejecucion)
        {
            try
            {
                List<UbicacionPickingProducto> codigos = _pickingProductoMapper.Map(request);
                ValidationsResult result = await _pickingProductoService.AgregarUbicacionesDePicking(codigos, (ejecucion.UserId ?? 0));

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
            catch (AutomatismoException ex)
            {
                ejecucion.Situacion = SituacionDb.ErrorNotificacionAutomatismo;
                await _ejecucionService.UpdateEjecucion(ejecucion);

            }
            catch (Exception ex)
            {
                ejecucion.Situacion = SituacionDb.ProcesadoConError;
                ejecucion.ErrorProcedimiento = "S";

                await _ejecucionService.AddError(ejecucion, 0, ex.Message);
                await _ejecucionService.UpdateEjecucion(ejecucion);

                throw ex;
            }
            if (ejecucion.Situacion != SituacionDb.ErrorNotificacionAutomatismo)
            {
                ejecucion.Situacion = SituacionDb.ProcesadoOK;
                await _ejecucionService.UpdateEjecucion(ejecucion);
            }

            ejecucion.Situacion = SituacionDb.ProcesadoOK;
            await _ejecucionService.UpdateEjecucion(ejecucion);

            return Ok(new PickingProductosResponse(ejecucion));
        }
    }
}
