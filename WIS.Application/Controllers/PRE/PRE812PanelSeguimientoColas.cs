using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Exceptions;
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

namespace WIS.Application.Controllers.PRE
{
    public class PRE812PanelSeguimientoColas : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridExcelService _excelService;
        protected readonly IIdentityService _identity;
        protected readonly Logger _logger;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE812PanelSeguimientoColas(
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IFilterInterpreter filterInterpreter,
            IGridExcelService excelService,
            IIdentityService identity)
        {
            this.GridKeys = new List<string>
            {
                "NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA"
            };

            this.DefaultSort = new List<SortCommand>() {
                new SortCommand("NU_PUNTUACION",SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._filterInterpreter = filterInterpreter;
            this._excelService = excelService;
            this._identity = identity;
            this._logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            if (this._identity.Predio == GeneralDb.PredioSinDefinir)
            {
                context.FetchContext.AddParameter("redirect", JsonConvert.SerializeObject(true));
                context.FetchContext.AddErrorNotification("EXP330_form1_Error_NoAccesoPantalla");

                return grid;
            }

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_LIST", new List<GridButton>
            {
                new GridButton("btnEditar", "PRE812_grid1_btn_Puntuaciones", "fas fa-list"),
                new GridButton("btnDetalle", "PRE812_grid1_btn_Detalle", "fas fa-search"),
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            if (this._identity.Predio == GeneralDb.PredioSinDefinir)
            {
                context.AddParameter("redirect", JsonConvert.SerializeObject(true));
                context.AddErrorNotification("EXP330_form1_Error_NoAccesoPantalla");

                return grid;
            }

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new SeguimientoColaDeTrabajoQuery();

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            if (this._identity.Predio == GeneralDb.PredioSinDefinir)
                return new GridStats { Count = 0 };

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new SeguimientoColaDeTrabajoQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);
            dbQuery.ApplyFilter(this._filterInterpreter, context.ExplicitFilter);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            if (this._identity.Predio == GeneralDb.PredioSinDefinir)
                throw new EntityNotFoundException("General_Sec0_Error_Error08");

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new SeguimientoColaDeTrabajoQuery();

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }
    }
}
