using Custom.Domain.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using WIS.Domain.DataModel;

namespace WIS.MiddlewareAPI.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public abstract class MiddlewareControllerBase : ControllerBase
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ILogger _logger;

        protected MiddlewareControllerBase(IUnitOfWorkFactory uowFactory, ILogger logger)
        {
            _uowFactory = uowFactory;
            _logger     = logger;
        }

        // Encola el payload crudo tal como llega del cliente (XML o JSON)
        protected IActionResult Encolar(string tipo, string payload)
        {
            try
            {
                using (var uow = (UnitOfWorkCustom)_uowFactory.GetUnitOfWork())
                {
                    uow.MiddlewareColaRepository.Encolar(tipo, payload);
                }

                _logger.LogInformation("Encolado tipo={Tipo}", tipo);
                return Accepted(new { mensaje = "Solicitud encolada correctamente.", tipo });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al encolar tipo={Tipo}", tipo);
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}
