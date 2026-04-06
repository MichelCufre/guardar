using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Linq;
using System.Threading.Tasks;
using WIS.AutomationManager.Extensions;
using WIS.AutomationManager.Interfaces;
using WIS.AutomationManager.Models.Mappers.Interfaces;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Validation;

namespace WIS.AutomationManager.Controllers.Entrada
{
    [ApiController]
    [AutomatismoAccessValidation]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]")]
    [Produces("application/json")]
    public class PtlConfirmationController : AutomatismoBaseController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IPtlFactory _ptlFactory;
        protected readonly IPtlAutomatismoMapper _ptlAutomatismoMapper;
        protected readonly IAutomatismoValidationService _validationService;

        public PtlConfirmationController(IUnitOfWorkFactory uowFactory,
            IAutomatismoService automatismoService,
            IAutomatismoEjecucionService automatismoEjecucionService,
            IPtlFactory ptlFactory,
            IPtlAutomatismoMapper ptlAutomatismoMapper,
            IAutomatismoValidationService validationService,
            ILogger<PtlConfirmationController> logger) : base(logger, automatismoEjecucionService, automatismoService)
        {
            _uowFactory = uowFactory;
            _ptlFactory = ptlFactory;
            _ptlAutomatismoMapper = ptlAutomatismoMapper;
            _validationService = validationService;
        }


        [HttpPost("ConfirmCommand")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PtlCommandResponse))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
		public async Task<IActionResult> ConfirmCommand([FromBody] PtlCommandConfirmRequest request)
        {
            var response = new PtlCommandResponse()
            {
                CommandId = request.CommandId
            };

            AutomatismoEjecucion ejecucion = null;

            try
            {
                ejecucion = await AddEjecucion(null, CodigoInterfazAutomatismoDb.ConfirmCommand, $"Posicion:{request.Address}$Color:{request.Color}", null, "");

                using (var uow = _uowFactory.GetUnitOfWork())
                {
                    var validationsResult = await _validationService.ValidateEnvioInterfazByCodigo(uow, request.Id, ejecucion.InterfazExterna);

                    if (validationsResult.HasError())
                    {
                        response.SetError(validationsResult.GetErrors().FirstOrDefault());

						return await ProcessError(ejecucion, request, validationsResult);
					}

					var automatismo = _automatismoService.GetByCodigo(uow, request.Id);
                    ejecucion.SetAutomatismo(automatismo);

                    var ptl = _ptlFactory.GetPtl(automatismo);
                    var service = _ptlFactory.GetService(ptl);
                    var accion = _ptlAutomatismoMapper.Map(ptl, request);

                    switch (request.CommandType)
                    {
                        case PtlTipoComandoDb.Confirmacion:
                            validationsResult = service.ProcesarConfirmacion(accion);
                            break;
                        case PtlTipoComandoDb.Cancelacion:
                            validationsResult = service.DescartarLuz(accion.Ubicacion, accion.Color);
                            break;
                        case PtlTipoComandoDb.CierrePosicion:
                            validationsResult = service.ConfirmarCerrarUbicacion(accion);
                            break;
                        default:
                            throw new Exception("CommandType not found");
                    }

                    if (validationsResult.HasError())
                    {
						return await ProcessError(ejecucion, request, validationsResult);
					}

					return await ProcessOk(ejecucion, request, response);
                }
            }
            catch (Exception ex)
            {
                response.SetError(ex.Message);

                var validationsResult = new ValidationsResult();
                validationsResult.AddError(ex.Message);

				return await ProcessError(ejecucion, request, validationsResult);
			}
		}
    }
}
