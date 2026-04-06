using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using WIS.Configuration;
using WIS.Data;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence;
using WIS.Persistence.Database;
using WIS.Security;

namespace WIS.Domain.DataModel
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IDatabaseConfigurationService _dbConfigService;
        private readonly IIdentityService _identityService;
        private readonly IOptions<DatabaseSettings> _databaseSettings;
        private readonly IDapper _dapper;
        private readonly IFactoryService _factoryService;
        private readonly IDatabaseFactory _dbFactory;

        public UnitOfWorkFactory(IDatabaseConfigurationService dbConfigService, 
            IIdentityService identityService, 
            IOptions<DatabaseSettings> databaseSettings, 
            IDapper dapper,
            IFactoryService factoryService,
            IDatabaseFactory dbFactory)
        {
            this._dbConfigService = dbConfigService;
            this._identityService = identityService;
            this._databaseSettings = databaseSettings;
            this._dapper = dapper;
            this._factoryService = factoryService;
            this._dbFactory = dbFactory;
        }

        public UnitOfWork GetUnitOfWork()
        {
            return new UnitOfWork(
                this._dbConfigService,
                this._identityService.Application,
                this._identityService.UserId, 
                this._identityService.Predio,
                this._databaseSettings,
                this._dapper,
                this._factoryService,
                this._dbFactory
            );
        }
    }
}
