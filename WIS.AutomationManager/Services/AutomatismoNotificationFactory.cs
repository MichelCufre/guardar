
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WIS.AutomationManager.Interfaces;
using WIS.AutomationManager.Models;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.DataModel;
using WIS.Security;

namespace WIS.AutomationManager.Services
{
    public class AutomatismoNotificationFactory : IAutomatismoNotificationFactory
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IAutomatismoInterpreterClientService _interpretService;
        protected readonly IAutomatismoWmsApiClientService _wmsApiClientService;
        protected readonly IAutomatismoValidationService _validationService;
        protected readonly ILogger<AutomatismoNotificationFactory> _logger;

        protected IIdentityService _identity;
        protected readonly IOptions<AutomationSettings> _configuration;

        public AutomatismoNotificationFactory(IUnitOfWorkFactory uowFactory,
            IAutomatismoWmsApiClientService wmsApiClientService,
            IAutomatismoValidationService validationService,
            IAutomatismoInterpreterClientService interpretService,
            IIdentityService identity,
            IOptions<AutomationSettings> configuration,
            ILogger<AutomatismoNotificationFactory> logger)
        {
            _uowFactory = uowFactory;
            _interpretService = interpretService;
            _identity = identity;
            _configuration = configuration;
            _wmsApiClientService = wmsApiClientService;
            _validationService = validationService;
            _logger = logger;
        }

        public IAutomatismoNotificationService Create(IAutomatismo automatismo)
        {
            switch (automatismo.Tipo)
            {
                case AutomatismoTipo.Vertical:
                case AutomatismoTipo.AutoStore:
                    return new AutomatismoAutoStoreNotificationService(_wmsApiClientService, _interpretService, _logger);
            }

            return null;
        }
    }
}
