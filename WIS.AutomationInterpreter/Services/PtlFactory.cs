using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using WIS.AutomationInterpreter.Interfaces;
using WIS.Domain.Automatismo.Constants;

namespace WIS.AutomationInterpreter.Services
{
    public class PtlFactory : IPtlFactory
    {
        protected readonly ILogger<PtlFactory> _logger;
        protected readonly IConfiguration _configuration;
        protected readonly AutomatismoClientService _automatismoClientService;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public PtlFactory(ILogger<PtlFactory> logger,
            IConfiguration configuration,
            AutomatismoClientService automatismoClientService,
            IHttpContextAccessor httpContextAccessor)
        {
            _automatismoClientService = automatismoClientService;
            _logger = logger;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public IPtlClientService GetIntegrationService(int cdInterfazExterna)
        {
            switch (cdInterfazExterna)
            {
                case CodigoInterfazAutomatismoDb.TrunOnLigth:
                case CodigoInterfazAutomatismoDb.StartOfOperation:
                case CodigoInterfazAutomatismoDb.ResetOfOperation:
                case CodigoInterfazAutomatismoDb.ConfirmCommand:
                    return new PtlClientService(_logger, _configuration, _httpContextAccessor);

                case CodigoInterfazAutomatismoDb.TrunOnLigth_Smarlog:
                case CodigoInterfazAutomatismoDb.StartOfOperation_Smarlog:
                case CodigoInterfazAutomatismoDb.ResetOfOperation_Smarlog:
                case CodigoInterfazAutomatismoDb.ConfirmCommand_Smarlog:
                    return new SmartLogPtlClientService(_logger, _automatismoClientService);
                default:
                    return GetIntegrationServiceCustom(cdInterfazExterna);
            }
        }

        protected virtual IPtlClientService GetIntegrationServiceCustom(int cdInterfazExterna)
        {
            throw new NotImplementedException();
        }
    }
}
