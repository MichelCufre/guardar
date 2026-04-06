using Microsoft.Extensions.Options;
using WIS.Configuration;
using WIS.Domain.DataModel;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence;
using WIS.Security;

namespace Custom.Domain.DataModel
{
    public class UnitOfWorkCustomFactory : IUnitOfWorkFactory
    {
        private readonly IDatabaseConfigurationService _dbConfigService;
        private readonly IIdentityService _identityService;
        private readonly IOptions<DatabaseSettings> _databaseSettings;
        private readonly IDapper _dapper;
        private readonly IFactoryService _factoryService;
        private readonly IDatabaseFactory _dbFactory;

        public UnitOfWorkCustomFactory(IDatabaseConfigurationService dbConfigService,
            IIdentityService identityService,
            IOptions<DatabaseSettings> databaseSettings,
            IDapper dapper,
            IFactoryService factoryService,
            IDatabaseFactory dbFactory)
        {
            _dbConfigService = dbConfigService;
            _identityService = identityService;
            _databaseSettings = databaseSettings;
            _dapper = dapper;
            _factoryService = factoryService;
            _dbFactory = dbFactory;
        }

        public UnitOfWork GetUnitOfWork()
        {
            return new UnitOfWorkCustom(
                _dbConfigService,
                _identityService.Application,
                _identityService.UserId,
                _identityService.Predio,
                _databaseSettings,
                _dapper,
                _factoryService,
                _dbFactory
            );
        }
    }
}
