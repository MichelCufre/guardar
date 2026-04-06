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
using WIS.Domain.Produccion.DTOs;
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
    public class ProduccionController : ControllerBaseExtension
    {
        protected readonly IProduccionMapper _mapper;
        protected readonly IEjecucionService _ejecucionService;
        protected readonly IProduccionService _produccionService;
        protected readonly IValidationService _validationService;
        protected readonly ILogger<ProduccionController> _logger;
        protected const int _interfazExterna = CInterfazExterna.Produccion;

        public ProduccionController(IProduccionMapper mapper,
            IEjecucionService ejecucionService,
            IProduccionService produccionService,
            IValidationService validationService,
            ILogger<ProduccionController> logger)
        {
            _mapper = mapper;
            _ejecucionService = ejecucionService;
            _produccionService = produccionService;
            _validationService = validationService;
            _logger = logger;
        }

        /// <summary>swagger_summary_produccion_create</summary>
        /// <remarks>swagger_remarks_produccion_create</remarks>
        /// <returns>swagger_returns_produccion_create</returns>
        /// <response code="200">swagger_response_200_produccion_create</response>
        /// <response code="400">swagger_response_400_produccion_create</response>
        [HttpPost("Create")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [SwaggerRequestType(typeof(ProduccionesRequest))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProduccionesResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public virtual async Task<IActionResult> Create()
        {
            try
            {
                var request = await GetRequest<ProduccionesRequest>();

                int empresa = request.Empresa;
                string archivo = request.Archivo;
                string ds_referencia = request.DsReferencia ?? "Ingresos de producciones";
                string idRequest = request.IdRequest ?? string.Empty;

                var data = JsonConvert.SerializeObject(request);
                var loginName = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                InterfazEjecucion ejecucion = await _ejecucionService.AddEjecucion(_interfazExterna, empresa, ds_referencia, data, archivo, loginName, idRequest);

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
        ///     swagger_summary_produccion_reprocess
        /// </summary>
        /// <remarks>swagger_remarks_produccion_reprocess</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_produccion_reprocess</returns>
        /// <response code="200">swagger_response_200_produccion_reprocess</response>
        /// <response code="400">swagger_response_400_produccion_reprocess</response>
        [HttpPost("Reprocess")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProduccionesResponse))]
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

                return await Create(JsonConvert.DeserializeObject<ProduccionesRequest>(Encoding.UTF8.GetString(itfzData.Data)), itfz);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                throw ex;
            }
        }

        private async Task<IActionResult> Create(ProduccionesRequest request, InterfazEjecucion ejecucion)
        {
            List<IngresosGeneradosApiProduccion> ingresosGenerados = new List<IngresosGeneradosApiProduccion>();

            try
            {
                var ingresos = _mapper.Map(request, ejecucion);               
                var result = await _produccionService.AgregarIngresos(ingresos, (ejecucion.UserId ?? 0), ingresosGenerados);

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

            return Ok(new ProduccionesResponse(ejecucion, ingresosGenerados));
        }

        /// <summary>
        ///     swagger_summary_produccion_getproduccion
        /// </summary>
        /// <remarks>swagger_remarks_produccion_getproduccion</remarks>
        /// <param name="nroIngresoProduccion"></param>
        /// <returns>swagger_returns_produccion_getproduccion</returns>
        /// <response code="200">swagger_response_200_produccion_getproduccion</response>
        /// <response code="401">swagger_response_401_produccion_getproduccion</response>
        /// <response code="404">swagger_response_404_produccion_getproduccion</response>
        [HttpGet("GetProduccion")]
        [ProduccionAccessValidation(_interfazExterna)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProduccionResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public virtual async Task<IActionResult> GetProduccion([RequiredValidation] string nroIngresoProduccion)
        {
            try
            {
                var ingreso = await _produccionService.GetProduccion(nroIngresoProduccion);

                if (ingreso != null)
                {
                    return Ok(_mapper.MapToResponse(ingreso));
                }

                var error = new Error("General_Sec0_Error_ProduccionXNotFound", nroIngresoProduccion);

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
