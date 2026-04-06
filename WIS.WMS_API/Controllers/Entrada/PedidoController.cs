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
using WIS.Domain.Picking;
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
    public class PedidoController : ControllerBaseExtension
    {
        protected readonly IPedidoMapper _pedidoMapper;
        protected readonly IPedidoService _pedidoService;
        protected readonly ILogger<PedidoController> _logger;
        protected readonly IEjecucionService _ejecucionService;
        protected readonly IValidationService _validationService;
		protected const int _interfazExterna = CInterfazExterna.Pedidos;

        public PedidoController(IPedidoMapper pedidoMapper, 
            IPedidoService pedidoService, 
            ILogger<PedidoController> logger,
            IEjecucionService ejecucionService,
            IValidationService validationService)
        {
            _pedidoMapper = pedidoMapper;
            _pedidoService = pedidoService;
            _logger = logger;
            _ejecucionService = ejecucionService;
            _validationService = validationService;
        }

        /// <summary>swagger_summary_pedido_create</summary>
        /// <remarks>swagger_remarks_pedido_create</remarks>
        /// <returns>swagger_returns_pedido_create</returns>
        /// <response code="200">swagger_response_200_pedido_create</response>
        /// <response code="400">swagger_response_400_pedido_create</response>
        [HttpPost("Create")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [SwaggerRequestType(typeof(PedidosRequest))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PedidosResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Create()
        {
            try
            {
                var request = await GetRequest<PedidosRequest>();

                var empresa = request.Empresa;
                var archivo = request.Archivo;
                var dsReferencia = request.DsReferencia ?? "Manejo de Pedidos";
				var idRequest = request.IdRequest ?? string.Empty;

				var data = JsonConvert.SerializeObject(request);
                var loginName = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

				InterfazEjecucion ejecucion = await _ejecucionService.AddEjecucion(_interfazExterna, empresa, dsReferencia, data, archivo, loginName, idRequest);

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
        ///     swagger_summary_pedido_reprocess
        /// </summary>
        /// <remarks>swagger_remarks_pedido_reprocess</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_pedido_reprocess</returns>
        /// <response code="200">swagger_response_200_pedido_reprocess</response>
        /// <response code="400">swagger_response_400_pedido_reprocess</response>
        [HttpPost("Reprocess")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PedidosResponse))]
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

                return await Create(JsonConvert.DeserializeObject<PedidosRequest>(Encoding.UTF8.GetString(itfzData.Data)), itfz);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                throw ex;
            }
        }

        private async Task<IActionResult> Create(PedidosRequest request, InterfazEjecucion ejecucion)
        {
            try
            {
                var pedidos = _pedidoMapper.Map(request);
                var result = await _pedidoService.AgregarPedidos(pedidos, (ejecucion.UserId ?? 0));

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

            return Ok(new PedidosResponse(ejecucion));
        }

        /// <summary>
        ///     swagger_summary_pedido_getpedido
        /// </summary>
        /// <remarks>swagger_remarks_pedido_getpedido</remarks>
        /// <param name="numero" example="PED100">swagger_param_numero_pedido_getpedido</param>
        /// <param name="empresa" example="1">swagger_param_empresa_pedido_getpedido</param>
        /// <param name="tipoAgente" example="CLI">swagger_param_tipoagente_pedido_getpedido</param>
        /// <param name="codigoAgente" example="AGE01">swagger_param_codigoagente_pedido_getpedido</param>
        /// <returns>swagger_returns_pedido_getpedido</returns>
        /// <response code="200">swagger_response_200_pedido_getpedido</response>
        /// <response code="401">swagger_response_401_pedido_getpedido</response>
        /// <response code="404">swagger_response_404_pedido_getpedido</response>
        [HttpGet("GetPedido")]
        [EmpresaAccessValidation(_interfazExterna, true)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PedidoResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetPedido([RequiredValidation] string numero, [RequiredValidation] int empresa, [RequiredValidation] string tipoAgente, [RequiredValidation] string codigoAgente)
        {
            try
            {
                var pedido = await _pedidoService.GetPedido(numero, empresa, tipoAgente, codigoAgente);

                if (pedido != null)
                    return Ok(_pedidoMapper.MapToResponse(pedido, tipoAgente, codigoAgente));

                var error = new Error("WMSAPI_msg_Error_PedidoNoEncontrado", numero, tipoAgente, codigoAgente, empresa);

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
