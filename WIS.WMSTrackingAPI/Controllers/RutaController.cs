using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using WIS.Domain.Services;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Tracking.Models;
using WIS.Domain.Tracking.Validation;
using WIS.Domain.Validation;
using WIS.WMSTrackingAPI.Extensions;
using WIS.WMSTrackingAPI.Models.Mappers;
using WIS.WMSTrackingAPI.Models.Mappers.Interfaces;

namespace WIS.WMSTrackingAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class RutaController : ControllerBaseExtension
    {
        private readonly IRutaMapper _rutaMapper;
        private readonly IRutaService _rutaService;
        private readonly ILogger<RutaController> _logger;
        private readonly IValidationService _validationService;

        public RutaController(IRutaService rutaService, IRutaMapper rutaMapper, IValidationService validationService, ILogger<RutaController> logger)
        {
            this._logger = logger;
            this._rutaMapper = rutaMapper;
            this._rutaService = rutaService;
            this._validationService = validationService;
        }

        /// <summary>
        ///     swagger_summary_ruta_getrutabyzona
        /// </summary>
        /// <remarks>swagger_remarks_ruta_getrutabyzona</remarks>
        /// <param name="zona" example="S/G">swagger_param_zona_ruta_getrutabyzona</param>
        [HttpGet("GetRuta")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RutaResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetRutaByZona([RequiredValidation] string zona)
        {
            try
            {
                var ruta = await _rutaService.GetRutaByZona(zona);

                if (ruta != null)
                    return Ok(_rutaMapper.MapToResponse(ruta));

                var error = new Error("TRK_msg_Error_ZonaSinRutaAsociada", zona);

                return Problem404NotFound(_validationService.Translate(error));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                return Problem500InternalServerError();
            }
        }

        /// <summary>
        ///     swagger_summary_ruta_addruta
        /// </summary>
        /// <remarks>swagger_remarks_ruta_addruta</remarks>
        /// <param name="request"></param>
        [HttpPost("AddRuta")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddRutaByZona([FromBody] RutaZonaRequest request)
        {
            try
            {
                var loginName = GetLoginName();
                var ruta = _rutaMapper.Map(request);
                TrackingValidationResult result = await _rutaService.AddRutaByZona(ruta, loginName);

                if (result.HasError())
                    return BadRequest(JsonConvert.SerializeObject(result));

                return Ok();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                return Problem500InternalServerError();
            }
        }

        /// <summary>
        ///     swagger_summary_ruta_updateruta
        /// </summary>
        /// <remarks>swagger_remarks_ruta_updateruta</remarks>
        /// <param name="request"></param>
        [HttpPost("UpdateRuta")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateRutaByZona([FromBody] RutaZonaRequest request)
        {
            try
            {
                var loginName = GetLoginName();
                var ruta = _rutaMapper.Map(request);
                TrackingValidationResult result = await _rutaService.UpdateRutaByZona(ruta, loginName);

                if (result.HasError())
                    return BadRequest(JsonConvert.SerializeObject(result));

                return Ok();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, this.Url.ActionContext.ToString());
                return Problem500InternalServerError();
            }
        }
    }
}
