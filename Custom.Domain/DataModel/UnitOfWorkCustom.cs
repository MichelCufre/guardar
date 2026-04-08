using Custom.Domain.DataModel.Repositories;
using Custom.Domain.DataModel.Repositories;
using Custom.Persistence.Database;
using Microsoft.Extensions.Options;
using WIS.Configuration;
using WIS.Data;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Middlewares;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence;

namespace Custom.Domain.DataModel
{
    public class UnitOfWorkCustom : UnitOfWork, IUnitOfWorkCustom
    {
        #region Methods

        public UnitOfWorkCustom(IDatabaseConfigurationService dbConfigService,
             string application,
             int userId,
             string predio,
             IOptions<DatabaseSettings> databaseSettings,
             IDapper dapper,
             IFactoryService factoryService,
             IDatabaseFactory dbFactory) : base(dbConfigService, application, userId, predio, databaseSettings, dapper, factoryService, dbFactory)
        {
            this.SetContext(new CUSTOMDB(dbConfigService, databaseSettings.Value.ConnectionString, databaseSettings.Value.Schema));
        }

        public virtual void HandleQuery<T>(IQueryObject<T, CUSTOMDB> query)
        {
            query.BuildQuery((CUSTOMDB)this._context);
            ApplySecurityMiddleware(query, allowNullEmpresa: true, allowNullGrupos: false, filterEmpresa: true);
        }

        public virtual void HandleQuery<T>(IQueryObject<T, CUSTOMDB> query, bool allowNullEmpresas = true, bool allowNullGrupos = false, bool filterEmpresa = true, string predioNullMapValue = null)
        {
            query.BuildQuery((CUSTOMDB)this._context);
            ApplySecurityMiddleware(query, allowNullEmpresas, allowNullGrupos, filterEmpresa, predioNullMapValue);
        }

        private void ApplySecurityMiddleware<T>(IQueryObject<T, CUSTOMDB> query, bool allowNullEmpresa, bool allowNullGrupos, bool filterEmpresa, string predioNullMapValue = null)
        {
            if (filterEmpresa)
                query.ApplyMiddleware(new EmpresaQueryMiddleware(base._context, _userId, allowNullEmpresa));

            query.ApplyMiddleware(new GrupoQueryMiddleware(base._context, _userId, allowNullGrupos));
            query.ApplyMiddleware(new PredioQueryMiddleware(base._context, _userId, _predio, predioNullMapValue));
        }

        #endregion

        #region Repositories
        private AgendaCustomRepository _agendaCustomRepository; public AgendaCustomRepository AgendaCustomRepository => this._agendaCustomRepository ?? (this._agendaCustomRepository = new AgendaCustomRepository((CUSTOMDB)this._context, this._application, this._userId, this._dapper));
        private MiddlewareColaRepository _middlewareColaRepository; public MiddlewareColaRepository MiddlewareColaRepository => this._middlewareColaRepository ?? (this._middlewareColaRepository = new MiddlewareColaRepository(this._dapper));
        #endregion Repositories
    }
}
