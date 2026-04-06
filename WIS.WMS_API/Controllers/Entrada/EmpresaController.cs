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
    public class EmpresaController : ControllerBaseExtension
    {
        private readonly IEmpresaMapper _empresaMapper;
        private readonly IEmpresaService _empresaService;
        private readonly ILogger<EmpresaController> _logger;
        private readonly IEjecucionService _ejecucionService;
        private readonly IValidationService _validationService;
        private const int _interfazExterna = CInterfazExterna.Empresas;

        public EmpresaController(IEmpresaMapper empresaMapper, 
            IEmpresaService empresaService, 
            ILogger<EmpresaController> logger, 
            IEjecucionService ejecucionService,
            IValidationService validationService)
        {
            _logger = logger;
            _empresaMapper = empresaMapper;
            _empresaService = empresaService;
            _ejecucionService = ejecucionService;
            _validationService = validationService;
        }

        /// <summary>swagger_summary_empresa_createorupdate</summary>
        /// <remarks>swagger_remarks_empresa_createorupdate</remarks>
        /// <returns>swagger_returns_empresa_createorupdate</returns>
        /// <response code="200">swagger_response_200_empresa_createorupdate</response>
        /// <response code="400">swagger_response_400_empresa_createorupdate</response>
        [HttpPost("CreateOrUpdate")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [SwaggerRequestType(typeof(EmpresasRequest))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmpresasResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> CreateOrUpdate()
        {
            try
            {
                var request = await GetRequest<EmpresasRequest>();

                int empresa = request.Empresa;
                string archivo = request.Archivo;
                string ds_referencia = request.DsReferencia ?? "Manejo de empresas";
				string idRequest = request.IdRequest ?? "";

				var data = JsonConvert.SerializeObject(request);
                var loginName = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

				InterfazEjecucion ejecucion = await _ejecucionService.AddEjecucion(_interfazExterna, empresa, ds_referencia, data, archivo, loginName, idRequest);

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
        ///     swagger_summary_empresa_reprocess
        /// </summary>
        /// <remarks>swagger_remarks_empresa_reprocess</remarks>
        /// <param name="request"></param>
        /// <returns>swagger_returns_empresa_reprocess</returns>
        /// <response code="200">swagger_response_200_empresa_reprocess</response>
        /// <response code="400">swagger_response_400_empresa_reprocess</response>
        [HttpPost("Reprocess")]
        [Consumes("application/json")]
        [EmpresaAccessValidation(_interfazExterna)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmpresasResponse))]
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

                return await CreateOrUpdate(JsonConvert.DeserializeObject<EmpresasRequest>(Encoding.UTF8.GetString(itfzData.Data)), itfz);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                throw ex;
            }
        }

        private async Task<IActionResult> CreateOrUpdate(EmpresasRequest request, InterfazEjecucion ejecucion)
        {
            try
            {
                List<Empresa> empresas = _empresaMapper.Map(request);
                ValidationsResult result = await _empresaService.AgregarEmpresas(empresas, request.Empresa, (ejecucion.UserId ?? 0));

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

            return Ok(new EmpresasResponse(ejecucion));
        }

        /// <summary>
        ///     swagger_summary_empresa_getempresa
        /// </summary>
        /// <remarks>swagger_remarks_empresa_getempresa</remarks>
        /// <param name="empresa" example="1">swagger_param_empresa_empresa_getempresa</param>
        /// <returns>swagger_returns_empresa_getempresa</returns>
        /// <response code="200">swagger_response_200_empresa_getempresa</response>
        /// <response code="401">swagger_response_401_empresa_getempresa</response>
        /// <response code="404">swagger_response_404_empresa_getempresa</response>
        [HttpGet("GetEmpresa")]
        [EmpresaAccessValidation(_interfazExterna, true)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmpresaResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetEmpresa([RequiredValidation] int empresa)
        {
            try
            {
                var emp = await _empresaService.GetEmpresa(empresa);

                if (emp != null)
                    return Ok(_empresaMapper.MapToResponse(emp));

                var error = new Error("WMSAPI_msg_Error_ExisteEmpresaValidation", empresa);

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
