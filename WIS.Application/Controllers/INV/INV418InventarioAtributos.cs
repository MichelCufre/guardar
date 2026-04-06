using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Inventario;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.INV
{
    public class INV418InventarioAtributos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSorting { get; }

        public INV418InventarioAtributos(IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            _uowFactory = uowFactory;
            _identity = identity;
            _gridService = gridService;
            _excelService = excelService;
            _filterInterpreter = filterInterpreter;

            GridKeys = new List<string>
            {
                "NU_INVENTARIO_ENDERECO_DET",
                "ID_ATRIBUTO"
            };

            DefaultSorting = new List<SortCommand>
            {
                new SortCommand("ID_ATRIBUTO", SortDirection.Ascending),
            };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!decimal.TryParse(context.GetParameter("nuInventarioDetalle"), _identity.GetFormatProvider(), out decimal nuInventarioDetalle))
                return grid;

            var dbQuery = new InventarioAtributosQuery(nuInventarioDetalle);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, DefaultSorting, GridKeys);


            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!decimal.TryParse(context.GetParameter("nuInventarioDetalle"), _identity.GetFormatProvider(), out decimal nuInventarioDetalle))
                return null;

            var dbQuery = new InventarioAtributosQuery(nuInventarioDetalle);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!decimal.TryParse(context.GetParameter("nuInventarioDetalle"), _identity.GetFormatProvider(), out decimal nuInventarioDetalle))
                return null;

            var dbQuery = new InventarioAtributosQuery(nuInventarioDetalle);
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, DefaultSorting);
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            return context;
        }
    }
}
