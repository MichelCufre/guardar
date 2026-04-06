using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.AutomationManager.Extensions;
using WIS.AutomationManager.Interfaces;
using WIS.AutomationManager.Models.Mappers.Interfaces;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Validation;

namespace WIS.AutomationManager.Controllers.Salida
{
    [ApiController]
    [PtlAccessValidation]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]")]
    [Produces("application/json")]
    public class PtlController : AutomatismoBaseController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IPtlFactory _ptlFactory;
        protected readonly IPtlAutomatismoMapper _ptlAutomatismoMapper;
        protected readonly IAutomatismoValidationService _validationService;

        public PtlController(IUnitOfWorkFactory uowFactory,
            IAutomatismoService automatismoService,
            IAutomatismoEjecucionService automatismoEjecucionService,
            IPtlFactory ptlFactory,
            IPtlAutomatismoMapper ptlAutomatismoMapper,
            IAutomatismoValidationService validationService,
            ILogger<PtlController> logger) : base(logger, automatismoEjecucionService, automatismoService)
        {
            _uowFactory = uowFactory;
            _ptlFactory = ptlFactory;
            _ptlAutomatismoMapper = ptlAutomatismoMapper;
            _validationService = validationService;
        }

        [HttpPost("GetColor")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PtlColorResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetColor([FromBody] PtlColorRequest request)
        {
            try
            {
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    var result = await _validationService.ValidateAutomatismo(uow, request.Ptl);

                    if (result.HasError())
                        return await ProcessError(null, request, result);

                    var ptl = _ptlFactory.GetPtl(_automatismoService.GetByZona(uow, request.Ptl));
                    var service = _ptlFactory.GetService(ptl);
                    var response = service.GetColor(request.UserId);

                    return await ProcessOk(null, request, response);
                }
            }
            catch (Exception ex)
            {
                return await ProcessError(null, request, ex);
            }
        }

        [HttpPost("GetColors")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PtlColorResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetColors([FromBody] PtlColorRequest request)
        {
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var result = await _validationService.ValidateAutomatismo(uow, request.Ptl);

                if (result.HasError())
                    return await ProcessError(null, request, result);

                var ptl = _ptlFactory.GetPtl(_automatismoService.GetByZona(uow, request.Ptl));
                var service = _ptlFactory.GetService(ptl);
                var response = service.GetColores();

                return await ProcessOk(null, request, response);
            }
        }

        [HttpPost("ClearColor")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> ClearColor([FromBody] PtlColorRequest request)
        {
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var result = await _validationService.ValidateAutomatismo(uow, request.Ptl);

                if (result.HasError())
                    return await ProcessError(null, request, result);

                var ptl = _ptlFactory.GetPtl(_automatismoService.GetByZona(uow, request.Ptl));
                var service = _ptlFactory.GetService(ptl);

                service.ClearColor(request.UserId);

                return await ProcessOk(null, request, true);
            }
        }

        [HttpPost("FinishOperation")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> FinishOperation([FromBody] PtlFinishOperationRequest request)
        {
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var result = await _validationService.ValidateAutomatismo(uow, request.Ptl);

                if (result.HasError())
                    return await ProcessError(null, request, result);

                var ptl = _ptlFactory.GetPtl(_automatismoService.GetByZona(uow, request.Ptl));
                var service = _ptlFactory.GetService(ptl);

                service.FinishOperation(request.UserId, request.Color);

                return await ProcessOk(null, request, true);
            }
        }

        [HttpPost("ValidateColor")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> ValidateColor([FromBody] PtlColorRequest request)
        {
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                ValidationsResult result = await _validationService.ValidateAutomatismo(uow, request.Ptl);

                if (result.HasError())
                    return await ProcessError(null, request, result);

                IPtl ptl = _ptlFactory.GetPtl(_automatismoService.GetByZona(uow, request.Ptl));
                IPtlService service = _ptlFactory.GetService(ptl);

                result = service.ValidarColor(request.Color, request.UserId);

                if (result.HasError())
                    return await ProcessError(null, request, result);

                return await ProcessOk(null, request, result.IsValid());
            }
        }

        [HttpPost("ValidarOperacion")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> ValidarOperacion([FromBody] PtlProductRequest request)
        {
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var result = await _validationService.ValidateAutomatismo(uow, request.Ptl);

                if (result.HasError())
                    return await ProcessError(null, request, result);

                var ptl = _ptlFactory.GetPtl(_automatismoService.GetByZona(uow, request.Ptl));
                var service = _ptlFactory.GetService(ptl);

                result = service.ValidarOperacion(request.Color, request.Company, request.Product);

                if (result.HasError())
                    return await ProcessError(null, request, result);

                return await ProcessOk(null, request, result.IsValid());
            }
        }

        [HttpGet("GetPtl")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PtlResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetPtl(string ptl)
        {
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var result = await _validationService.ValidateAutomatismo(uow, ptl);

                if (result.HasError())
                    return await ProcessError(null, ptl, result);

                var currPtl = _automatismoService.GetByZona(uow, ptl);

                return await ProcessOk(null, ptl, _ptlAutomatismoMapper.Map(currPtl));
            }
        }

        [HttpGet("GetAllPtl")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PtlResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetAllPtl()
        {
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var ptl = _automatismoService.GetAllByTipo(uow, AutomatismoTipo.Ptl);
                return await ProcessOk(null, null, _ptlAutomatismoMapper.Map(ptl));
            }
        }

        [HttpPost("StartOfOperation")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> StartOfOperation([FromBody] PtlRequest request)
        {
            AutomatismoEjecucion ejecucion = null;

            try
            {
                ejecucion = await AddEjecucion(null, CodigoInterfazAutomatismoDb.StartOfOperation, "", null, GetLoginName());

                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    var result = await _validationService.ValidateEnvioInterfaz(uow, request.Ptl, ejecucion.InterfazExterna);

                    if (result.HasError())
                        return await ProcessError(ejecucion, request, result);

                    var automatismo = _automatismoService.GetByZona(uow, request.Ptl);

                    ejecucion.SetAutomatismo(automatismo);

                    var ptl = _ptlFactory.GetPtl(automatismo);
                    var service = _ptlFactory.GetService(ptl);

                    result = service.StartOfOperation();

                    if (result.HasError())
                        return await ProcessError(ejecucion, request, result);

                    return await ProcessOk(ejecucion, request, result.SuccessMessage);
                }
            }
            catch (Exception ex)
            {
                return await ProcessError(ejecucion, request, ex);
            }
        }

        [HttpPost("ResetOfOperation")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> ResetOfOperation([FromBody] PtlRequest request)
        {
            AutomatismoEjecucion ejecucion = null;

            try
            {
                ejecucion = await AddEjecucion(null, CodigoInterfazAutomatismoDb.ResetOfOperation, "", null, GetLoginName());

                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    var result = await _validationService.ValidateEnvioInterfaz(uow, request.Ptl, ejecucion.InterfazExterna);

                    if (result.HasError())
                        return await ProcessError(ejecucion, request, result);

                    var automatismo = _automatismoService.GetByZona(uow, request.Ptl);
                    ejecucion.SetAutomatismo(automatismo);

                    var ptl = _ptlFactory.GetPtl(automatismo);
                    var service = _ptlFactory.GetService(ptl);

                    result = service.ResetOfOperation();

                    if (result.HasError())
                        return await ProcessError(ejecucion, request, result);

                    return await ProcessOk(ejecucion, request, result.SuccessMessage);
                }
            }
            catch (Exception ex)
            {
                return await ProcessError(ejecucion, request, ex);
            }
        }

        [HttpPost("TrunOnLigth")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> TrunOnLigth([FromBody] PtlActionRequest request)
        {
            AutomatismoEjecucion ejecucion = null;

            try
            {
                ejecucion = await AddEjecucion(null, CodigoInterfazAutomatismoDb.TrunOnLigth, $"TrunOnLigth$Color:{request.Color}$Prod:{request.Product}$Emp:{request.Company}", null, "");

                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    var result = await _validationService.ValidateEnvioInterfaz(uow, request.Ptl, ejecucion.InterfazExterna);

                    if (result.HasError())
                        return await ProcessError(ejecucion, request, result);

                    var automatismo = _automatismoService.GetByZona(uow, request.Ptl);

                    ejecucion.SetAutomatismo(automatismo);

                    var ptl = _ptlFactory.GetPtl(automatismo);
                    var service = _ptlFactory.GetService(ptl);

                    request.DisplayFn = request.DisplayFn ?? ptl.GetCodigoCancelacion();

                    var accion = _ptlAutomatismoMapper.Map(ptl, request, ejecucion.Id);

                    result = service.PrenderLuces(accion);

                    if (result.HasError())
                        return await ProcessError(ejecucion, request, result);

                    return await ProcessOk(ejecucion, request, result.SuccessMessage);
                }
            }
            catch (Exception ex)
            {
                return await ProcessError(ejecucion, request, ex);
            }
        }

        [HttpPost("CloseLocation")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> CloseLocation([FromBody] PtlActionRequest request)
        {
            AutomatismoEjecucion ejecucion = null;

            try
            {
                ejecucion = await AddEjecucion(null, CodigoInterfazAutomatismoDb.TrunOnLigth, $"CloseLocation$Posicion:{request.Position}$Color:{request.Color}", null, "");

                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    var result = await _validationService.ValidateEnvioInterfaz(uow, request.Ptl, ejecucion.InterfazExterna);

                    if (result.HasError())
                        return await ProcessError(null, request, result);

                    var automatismo = _automatismoService.GetByZona(uow, request.Ptl);

                    ejecucion.SetAutomatismo(automatismo);

                    var ptl = _ptlFactory.GetPtl(automatismo);
                    var service = _ptlFactory.GetService(ptl);

                    request.DisplayFn = request.DisplayFn ?? ptl.GetCodigoCancelacion();
                    request.Display = request.Display ?? ptl.GetCodigoCierre();

                    var accion = _ptlAutomatismoMapper.Map(ptl, request, ejecucion.Id);

                    result = service.CerrarUbicacion(accion);

                    if (result.HasError())
                        return await ProcessError(ejecucion, request, result);

                    return await ProcessOk(ejecucion, request, result.SuccessMessage);
                }
            }
            catch (Exception ex)
            {
                return await ProcessError(ejecucion, request, ex);
            }
        }

        [HttpGet("GetPositionByPtlAndUbicacion")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AutomatismoPosicion))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetPositionByPtlAndUbicacion(string ptl, string ubicacion)
        {
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var result = await _validationService.ValidateAutomatismo(uow, ptl);

                if (result.HasError())
                    return await ProcessError(null, ptl, result);

                var automatismo = _automatismoService.GetByZona(uow, ptl);
                var ptlCurr = _ptlFactory.GetPtl(automatismo);
                var service = _ptlFactory.GetService(ptlCurr);

                return await ProcessOk(null, ptl, ptlCurr.GetPosicionByUbicacion(ubicacion));
            }
        }

        [HttpGet("GetLightsOnByPtl")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PtlCommandConfirmRequest>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetLightsOnByPtl(string ptl)
        {
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var result = await _validationService.ValidateAutomatismo(uow, ptl);

                if (result.HasError())
                    return await ProcessError(null, ptl, result);

                var automatismo = _automatismoService.GetByZona(uow, ptl);
                var ptlCurr = _ptlFactory.GetPtl(automatismo);
                var service = _ptlFactory.GetService(ptlCurr);

                return await ProcessOk(null, ptl, _ptlAutomatismoMapper.Map(ptlCurr, service.GetLightsOn()));
            }
        }

        [HttpGet("GetLightsOnByPtlAndColor")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PtlCommandConfirmRequest>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetLightsOnByPtlAndColor(string ptl, string color)
        {
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var result = await _validationService.ValidateAutomatismo(uow, ptl);

                if (result.HasError())
                    return await ProcessError(null, ptl, result);

                var automatismo = _automatismoService.GetByZona(uow, ptl);
                var ptlCurr = _ptlFactory.GetPtl(automatismo);
                var service = _ptlFactory.GetService(ptlCurr);

                return await ProcessOk(null, ptl, _ptlAutomatismoMapper.Map(ptlCurr, service.GetLightsOn().Where(w => w.Color == color).ToList()));
            }
        }

        [HttpGet("GetPtlByTipoAutomatismo")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetPtlByTipoAutomatismo(string tipoPTL)
        {
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var service = _ptlFactory.GetService(tipoPTL);
                var ptls = service.GetPtlByTipo();
                var response = ptls != null ? JsonConvert.SerializeObject(ptls) : string.Empty;

                return await ProcessOk(null, tipoPTL, response);
            }
        }

        [HttpPost("UpdateLuzByPtlColor")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateLuzByPtlColor([FromBody] PtlColorActivoRequest request)
        {
            try
            {
                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    var result = await _validationService.ValidateAutomatismo(uow, request.IdPtl);

                    if (result.HasError())
                        return await ProcessError(null, request, result);

                    var automatismo = _automatismoService.GetByZona(uow, request.IdPtl);
                    var ptl = _ptlFactory.GetPtl(automatismo);
                    var service = _ptlFactory.GetService(ptl);

                    result = service.UpdateLuzByPtlColor(request);

                    if (result.HasError())
                        return await ProcessError(null, request, result);

                    return await ProcessOk(null, request, true);
                }
            }
            catch (Exception ex)
            {
                return await ProcessError(null, request, ex);
            }
        }

        [HttpPost("ValidatePtlReferencia")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> ValidatePtlReferencia([FromBody] PtlRequest request)
        {
            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var result = await _validationService.ValidateAutomatismo(uow, request.Ptl);

                if (result.HasError())
                    return await ProcessError(null, request, result);

                var ptl = _ptlFactory.GetPtl(_automatismoService.GetByZona(uow, request.Ptl));
                var service = _ptlFactory.GetService(ptl);

                result = service.ValidatePtlReferencia(request.Referencia);

                if (result.HasError())
                    return await ProcessOk(null, request, result.IsValid());

                return await ProcessOk(null, request, result.IsValid());
            }
        }
    }
}
