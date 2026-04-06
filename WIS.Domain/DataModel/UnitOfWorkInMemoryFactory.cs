using WIS.Persistence.InMemory;
using WIS.Security;

namespace WIS.Domain.DataModel
{
    public class UnitOfWorkInMemoryFactory : IUnitOfWorkInMemoryFactory
    {
        private readonly IDatabaseOptionProvider _dbConfigService;
        private readonly IIdentityService _identityService;

        public UnitOfWorkInMemoryFactory(IDatabaseOptionProvider dbConfigService, IIdentityService identityService)
        {
            this._dbConfigService = dbConfigService;
            this._identityService = identityService;
        }

        public UnitOfWorkCoreInMemory GetUnitOfWork()
        {
            return new UnitOfWorkCoreInMemory(
                this._dbConfigService,
                this._identityService.Application,
                this._identityService.UserId
            );
        }
    }
}
