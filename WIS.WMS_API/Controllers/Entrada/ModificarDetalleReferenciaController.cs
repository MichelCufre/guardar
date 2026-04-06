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

namespace WIS.WMS_API.Controllers.Entrada
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class ModificarDetalleReferenciaController : ControllerBaseExtension
    {
        private readonly IReferenciaRecepcionMapper _referenciaMapper;
        private readonly IReferenciaRecepcionService _referenciaService;
        private readonly ILogger<ModificarDetalleReferenciaController> _logger;
        private readonly IEjecucionService _ejecucionService;
        private readonly IValidationService _validationService;
        private const int _interfazExterna = CInterfazExterna.ModificarDetalleReferenciaRecepcion;

        public ModificarDetalleReferenciaController(IReferenciaRecepcionMapper referenciaMapper,
            IReferenciaRecepcionService referenciaService,
            ILogger<ModificarDetalleReferenciaController> logger, 
            IEjecucionService ejecucionService,
            IValidationService validationService)
        {
            _referenciaMapper = referenciaMapper;
            _referenciaService = referenciaService;
            _logger = logger;
            _ejecucionService = ejecucionService;
            _validationService = validationService;
        }

        /// <summary>swagger_summary_modificardetallereferencia_update</summary>
        /// <remarks>swagger_remarks_modificardetallereferencia_update</remarks>
        /// <returns>swagger_returns_modificardetallereferencia_update</returns>
        /// <response code="200">swagger_response_200_modificardetallereferencia_update</response>
        /// <response code="400">swagger_response_400_modificardetallereferencia_update</response>
        [HttpPost("Update")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [SwaggerRequestType(typeof(ModificacionDetalleReferenciaRequest))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModificacionDetalleReferenciaResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Update()
        {
            try
            {
                var request = await GetRequest<ModificacionDetalleReferenciaRequest>();

                int empresa = request.Empresa;
                string archivo = request.Archivo;
                string ds_referencia = request.DsReferencia ?? "Modificar detalles de referencias";
				string idRequest = request.IdRequest ?? "";

				var data = JsonConvert.SerializeObject(request);
                var loginName = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

				InterfazEjecucion ejecucion = await _ejecucionService.AddEjecucion(_interfazExterna, empresa, ds_referencia, data, archivo, loginName, idRequest);

                return await Update(request, ejecucion);
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
        ///     swagger_summary_modificardetallereferencia_reprocess
        /// </summary>
        /// <remarks>swagger_remarks_modificardetallereferencia_reprocess</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_modificardetallereferencia_reprocess</returns>
        /// <response code="200">swagger_response_200_modificardetallereferencia_reprocess</response>
        /// <response code="400">swagger_response_400_modificardetallereferencia_reprocess</response>
        [HttpPost("Reprocess")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModificacionDetalleReferenciaResponse))]
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

                return await Update(JsonConvert.DeserializeObject<ModificacionDetalleReferenciaRequest>(Encoding.UTF8.GetString(itfzData.Data)), itfz);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                throw ex;
            }
        }

        private async Task<IActionResult> Update(ModificacionDetalleReferenciaRequest request, InterfazEjecucion ejecucion)
        {
            try
            {
                List<ReferenciaRecepcion> referencias = _referenciaMapper.Map(request);
                ValidationsResult result = await _referenciaService.ModificarReferencias(referencias, (ejecucion.UserId ?? 0));

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

            return Ok(new ModificacionDetalleReferenciaResponse(ejecucion));
        }
    }
}
