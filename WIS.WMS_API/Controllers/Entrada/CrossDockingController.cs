using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
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
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class CrossDockingController : ControllerBaseExtension
    {
        private readonly ICrossDockingMapper _CrossDockingMapper;
        private readonly ICrossDockingService _crossDockingService;
        private readonly ILogger<StockController> _logger;
        private readonly IEjecucionService _ejecucionService;
		private readonly IValidationService _validationService;

		public CrossDockingController(ILogger<StockController> logger, 
            ICrossDockingService crossDockingService, 
            ICrossDockingMapper stockMapper, 
            IEjecucionService ejecucionService,
			IValidationService validationService)
        {
            this._logger = logger;
            this._CrossDockingMapper = stockMapper;
            this._crossDockingService = crossDockingService;
            this._ejecucionService = ejecucionService;
			_validationService = validationService;
		}

        #region CrossDocking

        /// <summary>
        ///     CrossDocking en una fase
        /// </summary>
        /// <remarks>
        /// 
        /// Descripción:
        /// 
        ///     Permite recepcionar una agenda, atendiendo un cliente del CrossDocking asociado.
        ///     
        /// Notas:
        ///     Los decimales se deben enviar con '.'. 
        ///     Los formatos de fechas aceptados son: [yyyy-MM-dd], [yyyy/MM/dd], [MM/dd/yyyy] y [MM - dd yyyy]
        ///     
        /// </remarks>
        /// <returns>Detalle de la interfaz generada</returns>
        /// <response code="200">Operación exitosa.</response>
        /// <response code="400">Error de validación.</response>
        [HttpPost("CrossDockingUnaFase")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(CInterfazExterna.CrossDockingUnaFase)]
        [SwaggerRequestType(typeof(CrossDockingUnaFaseRequest))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CrossDockingUnaFaseResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> CrossDockingUnaFase()
        {
            try
            {
                var request = await GetRequest<CrossDockingUnaFaseRequest>();

                int empresa = request.Empresa;
                string archivo = request.Archivo;
                string ds_referencia = request.DsReferencia ?? "CrossDocking";
				string idRequest = request.IdRequest ?? "";

				var data = JsonConvert.SerializeObject(request);
                var loginName = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

				var ejecucion = await _ejecucionService.AddEjecucion(CInterfazExterna.CrossDockingUnaFase, empresa, ds_referencia, data, archivo, loginName, idRequest);

                return await CrossDockingUnaFase(request, ejecucion);
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
        ///     Reprocesa el CrossDocking
        /// </summary>
        /// <remarks>
        /// 
        /// Descripción:
        /// 
        ///     Reprocesa la interfaz en error generada previamente el CrossDocking.  
        ///     
        /// </remarks>
        /// <param name="request"></param>
        /// <returns>Detalle de la interfaz reprocesada</returns>
        /// <response code="200">Operación exitosa.</response>
        /// <response code="400">Error de validación.</response>
        [HttpPost("ReprocesarCrossDockingUnaFase")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(CInterfazExterna.CrossDockingUnaFase)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CrossDockingUnaFaseResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> ReprocesarCrossDockingUnaFase([FromBody] ReprocesamientoRequest request)
        {
            try
            {
                var nuInterfaz = request.Interfaz;

                if (!await _ejecucionService.ExisteEjecucion(nuInterfaz))
                    return Problem400BadRequest($"La interfaz {nuInterfaz} no existe");

                var itfz = await _ejecucionService.GetEjecucion(nuInterfaz);

                if (itfz.CdInterfazExterna != CInterfazExterna.CrossDockingUnaFase)
                    return Problem400BadRequest($"La interfaz {nuInterfaz} no corresponde a la interfaz externa {CInterfazExterna.CrossDockingUnaFase}");

                if (itfz.Situacion != SituacionDb.ProcesadoConError)
                    return Problem400BadRequest($"La interfaz {nuInterfaz} no tiene estado {SituacionDb.ProcesadoConError}");

                itfz = await _ejecucionService.IniciarReprocesamiento(itfz);

                var itfzData = await _ejecucionService.GetEjecucionData(nuInterfaz);

                return await CrossDockingUnaFase(JsonConvert.DeserializeObject<CrossDockingUnaFaseRequest>(Encoding.UTF8.GetString(itfzData.Data)), itfz);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                throw ex;
            }
        }

        private async Task<IActionResult> CrossDockingUnaFase(CrossDockingUnaFaseRequest request, InterfazEjecucion ejecucion)
        {
            try
            {
                List<CrossDockingUnaFase> ajustes = _CrossDockingMapper.Map(request);
                ValidationsResult result = await _crossDockingService.CrossDockingUnaFase(ajustes, (ejecucion.UserId ?? 0));

                if (result.HasError())
                {
                    ejecucion.Situacion = SituacionDb.ProcesadoConError;
                    ejecucion.ErrorCarga = result.HasProceduralError() ? "N" : "S";
                    ejecucion.ErrorProcedimiento = result.HasProceduralError() ? "S" : "N";

                    await _ejecucionService.AddErrores(ejecucion, result.Errors);

                    string finalMessage = JsonConvert.SerializeObject(result.Errors);
                    return Problem400BadRequest(finalMessage, $"Error Interfaz Nro {ejecucion.Id}", ejecucion.Id);
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

            return Ok(new CrossDockingUnaFaseResponse(ejecucion));
        }

        #endregion
    }
}
