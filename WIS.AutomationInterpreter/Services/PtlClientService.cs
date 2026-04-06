using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WIS.AutomationInterpreter.Interfaces;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.Integracion;
using WIS.Domain.Integracion.Dtos;

namespace WIS.AutomationInterpreter.Services
{
    public class PtlClientService : ClientIntegrationService, IPtlClientService
    {
        public PtlClientService(ILogger logger,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor) : base(logger, configuration, httpContextAccessor)
        {
        }

        public PtlCommandResponse TurnLigthOnOrOff(AutomatismoInterpreterRequest request)
        {
            _logger.LogTrace(JsonSerializer.Serialize(request));

            //TODO:

            return new PtlCommandResponse();
        }

        public PtlCommandResponse ResetOfOperation(AutomatismoInterpreterRequest request)
        {
            _logger.LogTrace(JsonSerializer.Serialize(request));

            //TODO:

            return new PtlCommandResponse();
        }

        public PtlCommandResponse StartOfOperation(AutomatismoInterpreterRequest request)
        {
            _logger.LogTrace(JsonSerializer.Serialize(request));

            //TODO:

            return new PtlCommandResponse();
        }
    }
}
