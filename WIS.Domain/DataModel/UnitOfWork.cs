using Microsoft.Extensions.Options;
using System;
using WIS.Configuration;
using WIS.Data;
using WIS.Domain.DataModel.Middlewares;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence;
using WIS.Persistence.Database;


namespace WIS.Domain.DataModel
{
    public class UnitOfWork : UnitOfWorkCore, IUnitOfWork, IDisposable
    {
        protected readonly string _predio;

        public UnitOfWork(IDatabaseConfigurationService dbConfigService,
            string application,
            int userId,
            string predio,
            IOptions<DatabaseSettings> databaseSettings,
            IDapper dapper,
            IFactoryService factoryService,
            IDatabaseFactory dbFactory,
            bool openContext = true) : base(dbConfigService, application, userId, databaseSettings, dapper, factoryService, dbFactory, openContext)
        {
            this._predio = predio;
        }

        public override void HandleQuery<T>(IQueryObject<T, WISDB> query)
        {
            base.HandleQuery(query);
            this.ApplySecurityMiddleware(query, true, false, true);
        }

        public void HandleQuery<T>(IQueryObject<T, WISDB> query, bool allowNullEmpresas = true, bool allowNullGrupos = false, bool filterEmpresa = true, string predioNullMapValue = null)
        {
            base.HandleQuery(query);
            this.ApplySecurityMiddleware(query, allowNullEmpresas, allowNullGrupos, filterEmpresa, predioNullMapValue);
        }

        private void ApplySecurityMiddleware<T>(IQueryObject<T, WISDB> query, bool allowNullEmpresa, bool allowNullGrupos, bool filterEmpresa, string predioNullMapValue = null)
        {
            if (filterEmpresa)
                query.ApplyMiddleware(new EmpresaQueryMiddleware(this._context, this._userId, allowNullEmpresa));

            query.ApplyMiddleware(new GrupoQueryMiddleware(this._context, this._userId, allowNullGrupos));
            query.ApplyMiddleware(new PredioQueryMiddleware(this._context, this._userId, this._predio, predioNullMapValue));
        }
    }
}
