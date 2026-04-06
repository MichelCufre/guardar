using NLog;
using System;
using System.Collections.Generic;
using WIS.Application.Security;
using WIS.Application.Validation;
using WIS.Domain.DataModel;
using WIS.Domain.Produccion.Queries;
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

namespace WIS.Application.Controllers.PRD
{
    public class PRD111PanelEspacioProduccion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ISecurityService _security;

        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRD111PanelEspacioProduccion(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IGridValidationService gridValidationService,
            IFilterInterpreter filterInterpreter,
            ISecurityService security)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._gridValidationService = gridValidationService;
            this._filterInterpreter = filterInterpreter;
            this._security = security;

            this.GridKeys = new List<string>
            {
                "CD_PRDC_LINEA"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("DT_ADDROW", SortDirection.Descending)
            };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_ARRAY", new List<IGridItem>
            {       
                new GridButton("btnConsumirStock", "PRD111_grid1_btn_ConsumirStock", "fa-solid fa-circle-chevron-down"),
                new GridButton("btnProducirStock", "PRD111_grid1_btn_ProducirStock", "fa-solid fa-circle-chevron-up"),
                new GridButton("btnExpulsarStock", "PRD111_grid1_btn_ExpulsarStock", "fa-solid fa-circle-minus"),
            }));

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ACTIONS", new List<GridButton>
            {
                new GridButton("btnEditar", "General_Sec0_btn_Editar", "fas fa-edit")
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var dbQuery = new EspacioProduccionQuery();
            uow.HandleQuery(dbQuery);

            grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, DefaultSort, this.GridKeys);

            DisableButtons(grid, uow);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var dbQuery = new EspacioProduccionQuery();
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var dbQuery = new EspacioProduccionQuery();
            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        #region Metodos Auxiliares

        public virtual void DisableButtons(Grid grid, IUnitOfWork uow)
        {
            var result = this._security.CheckPermissions(new List<string>
            {
                SecurityResources.PRD111_grid1_btn_ConsumirStock,
                SecurityResources.PRD111_grid1_btn_ProducirStock
            });

            foreach (GridRow row in grid.Rows)
            {
                row.DisabledButtons = new List<string>() { };

                var codigoEspacio = row.GetCell("CD_PRDC_LINEA").Value;
                var ingresoActivo = uow.EspacioProduccionRepository.AnyIngresoActivoEspacio(codigoEspacio);

                if (ingresoActivo)
                    row.DisabledButtons.Add("btnEditar");

                if (!result[SecurityResources.PRD111_grid1_btn_ConsumirStock])
                    row.DisabledButtons.Add("btnConsumirStock");

                if (!result[SecurityResources.PRD111_grid1_btn_ConsumirStock])
                    row.DisabledButtons.Add("btnProducirStock");
            }
        }

        #endregion

    }
}
