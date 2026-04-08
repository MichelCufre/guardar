using Microsoft.AspNetCore.Mvc;
using Custom.Domain.DataModel;
using Custom.Domain.DataModel.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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

        protected IActionResult Encolar(string tipo, object wmsRequest)
        {
            try
            {
                var payload = JsonConvert.SerializeObject(wmsRequest);

                using (var uow = (UnitOfWorkCustom)_uowFactory.GetUnitOfWork())
                {
                    // Inserta en T_MIDDLEWARE_COLA de la BD de WIS via Dapper
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
