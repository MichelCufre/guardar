using System;
using System.Collections.Generic;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.Automatismo.Logic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Automatizacion;
using WIS.Domain.Services.Interfaces;
using WIS.Extension;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.AUT
{
    public class AUT100Ejecuciones : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IAutomatismoAutoStoreClientService _autoStoreClientService;
        protected readonly IAutomatismoFactory _automatismoFactory;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public AUT100Ejecuciones(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            IAutomatismoAutoStoreClientService autoStoreClientService,
            IAutomatismoFactory automatismoFactory)
        {
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._autoStoreClientService = autoStoreClientService;
            this._automatismoFactory = automatismoFactory;

            this.GridKeys = new List<string>
            {
                "NU_AUTOMATISMO_EJECUCION"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_AUTOMATISMO_EJECUCION", SortDirection.Descending)
            };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ACTIONS", new List<GridButton>
            {
                new GridButton("btnEditar", "EXP040_grid1_btn_Editar", "fas fa-edit"),
                new GridButton("btnVerErrores", "AUT100Ejecuciones_grid1_btn_VerErrores", "fa fa-eye"),
                new GridButton("btnReprocesar", "AUT100Ejecuciones_grid1_btn_Reprocesar", "fas fa-undo-alt"),
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuAutomatismo = context.GetParameter("AUT100_NU_AUTOMATISMO");

            var query = (string.IsNullOrEmpty(nuAutomatismo) || !int.TryParse(nuAutomatismo, out int numeroAutomatismo)) ? new AutomatismoEjecucionQuery() : new AutomatismoEjecucionQuery(numeroAutomatismo);

            uow.HandleQuery(query);

            grid.Rows = _gridService.GetRows(query, grid.Columns, context, this.DefaultSort, this.GridKeys);

            this.ComprobarPermisosEnBotones(uow, grid);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuAutomatismo = context.GetParameter("AUT100_NU_AUTOMATISMO");

            var dbQuery = (string.IsNullOrEmpty(nuAutomatismo) || !int.TryParse(nuAutomatismo, out int numeroAutomatismo)) ? new AutomatismoEjecucionQuery() : new AutomatismoEjecucionQuery(numeroAutomatismo);

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuAutomatismo = context.GetParameter("AUT100_NU_AUTOMATISMO");

            var query = (string.IsNullOrEmpty(nuAutomatismo) || !int.TryParse(nuAutomatismo, out int numeroAutomatismo)) ? new AutomatismoEjecucionQuery() : new AutomatismoEjecucionQuery(numeroAutomatismo);

            uow.HandleQuery(query);

            query.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats()
            {
                Count = query.GetCount()
            };
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var logic = new AutomatismoLogic(uow, _automatismoFactory);

            if (context.ButtonId == "btnReprocesar")
            {
                var ejec = context.Row.GetCell("NU_AUTOMATISMO_EJECUCION").Value.ToNumber<int>();

                logic.Reprocesar(ejec, _autoStoreClientService);

                context.AddInfoNotification("AUT100Ejecuciones_form1_Success_AutomatismoReprocesado");
            }

            return context;
        }

        #region Metodos Auxiliares

        public virtual void ComprobarPermisosEnBotones(IUnitOfWork uow, Grid grid)
        {
            foreach (var row in grid.Rows)
            {
                var automatismoEjecucion = int.Parse(row.GetCell("NU_AUTOMATISMO_EJECUCION").Value);

                string estado = row.GetCell("ND_SITUACION").Value;

                if (InterfazMarcadaConError(estado))
                    row.CssClass = "red";

                #region >> btnVerErrores
                if (!InterfazMarcadaConError(estado))
                    row.DisabledButtons.Add("btnVerErrores");
                #endregion

                #region >> btnReprocesar
                if (!InterfazMarcadaConError(estado) || InterfazYaReprocesada(estado))
                    row.DisabledButtons.Add("btnReprocesar");
                #endregion
            }
        }

        public virtual bool InterfazYaReprocesada(string estado)
        {
            return estado == EstadoAutomatismoEjecucionDb.ESTPROCREP;
        }

        public virtual bool AutomatismoAllowsEdition(IUnitOfWork uow, AutomatismoEjecucion ejecucion)
        {
            if (ejecucion.IdAutomatismo == null) return false;

            var automatismo = uow.AutomatismoRepository.GetAutomatismoById((int)ejecucion.IdAutomatismo);

            return automatismo.AllowEditionEjecucion();
        }

        public virtual bool InterfazMarcadaConError(string estado)
        {
            return estado == EstadoAutomatismoEjecucionDb.PROCESADO_CON_ERROR_API || estado == EstadoAutomatismoEjecucionDb.PROCESADO_CON_ERROR_AUTOMATISMO;
        }

        public virtual bool EjecucionTieneDatos(IUnitOfWork uow, AutomatismoEjecucion ejecucion)
        {
            return uow.AutomatismoEjecucionRepository.EjecucionTieneDatos(ejecucion.Id);
        }

        #endregion
    }
}
