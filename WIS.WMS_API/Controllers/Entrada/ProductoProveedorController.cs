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
    public class ProductoProveedorController : ControllerBaseExtension
    {
        private readonly IProductoMapper _productoMapper;
        private readonly IProductoService _productoService;
        private readonly IEjecucionService _ejecucionService;
        private readonly ILogger<ProductoProveedorController> _logger;
        private readonly IValidationService _validationService;
        private const int _interfazExterna = CInterfazExterna.ProductoProveedor;

        public ProductoProveedorController(IProductoMapper productoMapper,
            IProductoService productoService, 
            IEjecucionService ejecucionService, 
            ILogger<ProductoProveedorController> logger,
            IValidationService validationService)
        {
            _productoMapper = productoMapper;
            _productoService = productoService;
            _ejecucionService = ejecucionService;
            _logger = logger;
            _validationService = validationService;
        }

        /// <summary>swagger_summary_productoproveedor_createordelete</summary>
        /// <remarks>swagger_remarks_productoproveedor_createordelete</remarks>
        /// <returns>swagger_returns_productoproveedor_createordelete</returns>
        /// <response code="200">swagger_response_200_productoproveedor_createordelete</response>
        /// <response code="400">swagger_response_400_productoproveedor_createordelete</response>
        [HttpPost("CreateOrDelete")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [SwaggerRequestType(typeof(ProductosProveedorRequest))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductosProveedorResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> CreateOrDelete()
        {
            try
            {
                var request = await GetRequest<ProductosProveedorRequest>();

                int empresa = request.Empresa;
                string archivo = request.Archivo;
                string ds_referencia = request.DsReferencia ?? "Manejo de Productos proveedor";
				string idRequest = request.IdRequest ?? "";

				var data = JsonConvert.SerializeObject(request);
                var loginName = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

				InterfazEjecucion ejecucion = await _ejecucionService.AddEjecucion(_interfazExterna, empresa, ds_referencia, data, archivo, loginName, idRequest);

                return await CreateOrDelete(request, ejecucion);
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
        ///     swagger_summary_productoproveedor_reprocess
        /// </summary>
        /// <remarks>swagger_remarks_productoproveedor_reprocess</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_productoproveedor_reprocess</returns>
        /// <response code="200">swagger_response_200_productoproveedor_reprocess</response>
        /// <response code="400">swagger_response_400_productoproveedor_reprocess</response>
        [HttpPost("Reprocess")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductosProveedorResponse))]
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

                return await CreateOrDelete(JsonConvert.DeserializeObject<ProductosProveedorRequest>(Encoding.UTF8.GetString(itfzData.Data)), itfz);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                throw ex;
            }
        }

        private async Task<IActionResult> CreateOrDelete(ProductosProveedorRequest request, InterfazEjecucion ejecucion)
        {
            try
            {
                List<ProductoProveedor> codigos = _productoMapper.Map(request);
                ValidationsResult result = await _productoService.AgregarProductosProveedor(codigos, (ejecucion.UserId ?? 0));

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

            return Ok(new ProductosProveedorResponse(ejecucion));
        }

        /// <summary>
        ///     swagger_summary_productoproveedor_getproductoproveedor
        /// </summary>
        /// <remarks>swagger_remarks_productoproveedor_getproductoproveedor</remarks>
        /// <param name="producto" example="PR1">swagger_param_producto_productoproveedor_getproductoproveedor</param>
        /// <param name="empresa" example="1">swagger_param_empresa_productoproveedor_getproductoproveedor</param>
        /// <param name="tipoAgente" example="CLI">swagger_param_tipoagente_productoproveedor_getproductoproveedor</param>
        /// <param name="codigoAgente" example="AGE01">swagger_param_codigoagente_productoproveedor_getproductoproveedor</param>
        /// <returns>swagger_returns_productoproveedor_getproductoproveedor</returns>
        /// <response code="200">swagger_response_200_productoproveedor_getproductoproveedor</response>
        /// <response code="401">swagger_response_401_productoproveedor_getproductoproveedor</response>
        /// <response code="404">swagger_response_404_productoproveedor_getproductoproveedor</response>
        [HttpGet("GetProductoProveedor")]
        [EmpresaAccessValidation(_interfazExterna, true)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductoProveedorResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetProductoProveedor([RequiredValidation] string producto, [RequiredValidation] int empresa, [RequiredValidation] string tipoAgente, [RequiredValidation] string codigoAgente)
        {
            try
            {
                var productoProveedor = await _productoService.GetProductoProveedor(producto, empresa, tipoAgente, codigoAgente);

                if (productoProveedor != null)
                    return Ok(_productoMapper.MapToResponse(productoProveedor, tipoAgente, codigoAgente));

                var error = new Error("WMSAPI_msg_Error_ProductoProveedorNoEncontrado", producto, empresa, tipoAgente, codigoAgente);

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
