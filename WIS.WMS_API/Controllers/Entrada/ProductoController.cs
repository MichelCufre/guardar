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
    public class ProductoController : ControllerBaseExtension
    {
        private readonly IProductoMapper _productoMapper;
        private readonly IProductoService _productoService;
        private readonly IEjecucionService _ejecucionService;
        private readonly ILogger<ProductoController> _logger;
        private readonly IValidationService _validationService;
        private const int _interfazExterna = CInterfazExterna.Producto;

        public ProductoController(ILogger<ProductoController> logger,
            IProductoService productoService,
            IProductoMapper productoMapper,
            IEjecucionService ejecucionService,
            IValidationService validationService)
        {
            _logger = logger;
            _productoService = productoService;
            _productoMapper = productoMapper;
            _ejecucionService = ejecucionService;
            _validationService = validationService;
        }

        /// <summary>swagger_summary_producto_createorupdate</summary>
        /// <remarks>swagger_remarks_producto_createorupdate</remarks>
        /// <returns>swagger_returns_producto_createorupdate</returns>
        /// <response code="200">swagger_response_200_producto_createorupdate</response>
        /// <response code="400">swagger_response_400_producto_createorupdate</response>
        [HttpPost("CreateOrUpdate")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductosResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [SwaggerRequestType(typeof(ProductosRequest))]
        public async Task<IActionResult> CreateOrUpdate()
        {
            try
            {
                var request = await GetRequest<ProductosRequest>();

                int empresa = request.Empresa;
                string archivo = request.Archivo;
                string dsReferencia = request.DsReferencia ?? "Manejo de Productos";
                string idRequest = request.IdRequest ?? "";

                var data = JsonConvert.SerializeObject(request);
                var loginName = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                InterfazEjecucion ejecucion = await _ejecucionService.AddEjecucion(_interfazExterna, empresa, dsReferencia, data, archivo, loginName, idRequest);

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
        ///     swagger_summary_producto_reprocess
        /// </summary>
        /// <remarks>swagger_remarks_producto_reprocess</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_producto_reprocess</returns>
        /// <response code="200">swagger_response_200_producto_reprocess</response>
        /// <response code="400">swagger_response_400_producto_reprocess</response>
        [HttpPost("Reprocess")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductosResponse))]
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

                return await CreateOrUpdate(JsonConvert.DeserializeObject<ProductosRequest>(Encoding.UTF8.GetString(itfzData.Data)), itfz);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                throw ex;
            }
        }

        private async Task<IActionResult> CreateOrUpdate(ProductosRequest request, InterfazEjecucion ejecucion)
        {
            try
            {
                List<Producto> productos = _productoMapper.Map(request);
                ValidationsResult result = await _productoService.AgregarProductos(productos, (ejecucion.UserId ?? 0));

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

            return Ok(new ProductosResponse(ejecucion));
        }

        /// <summary>
        ///     swagger_summary_producto_getproducto
        /// </summary>
        /// <remarks>swagger_remarks_producto_getproducto</remarks>
        /// <param name="codigo" example="PR1">swagger_param_codigo_producto_getproducto</param>
        /// <param name="empresa" example="1">swagger_param_empresa_producto_getproducto</param>
        /// <returns>swagger_returns_producto_getproducto</returns>
        /// <response code="200">swagger_response_200_producto_getproducto</response>
        /// <response code="401">swagger_response_401_producto_getproducto</response>
        /// <response code="404">swagger_response_404_producto_getproducto</response>
        [HttpGet("GetProducto")]
        [EmpresaAccessValidation(_interfazExterna, true)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductoResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetProducto([RequiredValidation] string codigo, [RequiredValidation] int empresa)
        {
            try
            {
                var producto = await _productoService.GetProducto(codigo, empresa);
                if (producto != null)
                    return Ok(_productoMapper.MapToResponse(producto));

                var error = new Error("WMSAPI_msg_Error_ProductoNoEncontrado", codigo, empresa);

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
