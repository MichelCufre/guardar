using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Security;

namespace WIS.Domain.Services
{
    public class AnulacionService : IAnulacionService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IOptions<AnulacionSettings> _settings;
        protected readonly ILogger<AnulacionService> _logger;
        protected readonly IDapper _dapper;
        protected readonly ITaskQueueService _taskQueue;
        protected readonly IIdentityService _identity;
        protected readonly ITrackingService _trackingService;

        public AnulacionService(IUnitOfWorkFactory uowFactory,
            IOptions<AnulacionSettings> settings,
            ITaskQueueService taskQueue,
            ILogger<AnulacionService> logger,
            IDapper dapper,
            IIdentityService identity,
            ITrackingService trackingService)
        {
            _uowFactory = uowFactory;
            _settings = settings;
            _taskQueue = taskQueue;
            _logger = logger;
            _dapper = dapper;
            _identity = identity;
            _trackingService = trackingService;
        }

        public virtual void Start()
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var logic = new AnulacionDePreparaciones(uow, _logger, _dapper, _taskQueue, _identity, _trackingService);
                logic.IniciarProcesoAnulacion();
            }
        }

    }
}
