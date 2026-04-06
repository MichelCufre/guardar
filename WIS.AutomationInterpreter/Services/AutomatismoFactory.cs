using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using WIS.AutomationInterpreter.Interfaces;
using WIS.AutomationInterpreter.Models.Mappers.Interfaces;
using WIS.Domain.Automatismo.Constants;

namespace WIS.AutomationInterpreter.Services
{
    public class AutomatismoFactory : IAutomatismoFactory
    {
        protected readonly ILogger<AutomatismoFactory> _logger;
        protected readonly IConfiguration _configuration;
        protected readonly IGalysMapper _galysMapper;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public AutomatismoFactory(ILogger<AutomatismoFactory> logger,
            IConfiguration configuration,
            IGalysMapper galysMapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _configuration = configuration;
            _galysMapper = galysMapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public IAutoStoreClientService GetIntegrationService(int cdInterfazExterna)
        {
            switch (cdInterfazExterna)
            {
                case CodigoInterfazAutomatismoDb.CD_INTERFAZ_PRODUCTOS:
                case CodigoInterfazAutomatismoDb.CD_INTERFAZ_ENTRADAS:
                case CodigoInterfazAutomatismoDb.CD_INTERFAZ_CODIGO_BARRAS:
                case CodigoInterfazAutomatismoDb.CD_INTERFAZ_SALIDA:
                    return new GalysClientService(_logger, _configuration, _galysMapper, _httpContextAccessor);
                default:
                    return GetIntegrationServiceCustom(cdInterfazExterna);
            }
        }

        protected virtual IAutoStoreClientService GetIntegrationServiceCustom(int cdInterfazExterna)
        {
            throw new NotImplementedException();
        }

    }
}
