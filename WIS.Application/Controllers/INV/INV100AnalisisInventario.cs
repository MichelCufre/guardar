using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Inventario;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
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
    public class INV100AnalisisInventario : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public INV100AnalisisInventario(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_INVENTARIO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_INVENTARIO", SortDirection.Descending)
            };

            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            var nuInventario = context.GetParameter("inventario");
            if (!string.IsNullOrEmpty(nuInventario) && !decimal.TryParse(nuInventario, _identity.GetFormatProvider(), out decimal i))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuInventario = decimal.TryParse(context.GetParameter("inventario"), this._identity.GetFormatProvider(), out decimal n) ? n : default(decimal?);

            var dbQuery = new INV100GridQuery(nuInventario);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, DefaultSort, this.GridKeys);

            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuInventario = decimal.TryParse(context.GetParameter("inventario"), this._identity.GetFormatProvider(), out decimal n) ? n : default(decimal?);

            var dbQuery = new INV100GridQuery(nuInventario);
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

            var nuInventario = decimal.TryParse(context.GetParameter("inventario"), this._identity.GetFormatProvider(), out decimal n) ? n : default(decimal?);

            var dbQuery = new INV100GridQuery(nuInventario);
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, DefaultSort);
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            return form;
        }
    }
}
