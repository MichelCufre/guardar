using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.DataModel.Queries.Stock;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.STO
{
    public class STO750ConsultaUT : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IIdentityService _identity;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public STO750ConsultaUT(IUnitOfWorkFactory uowFactory, IGridService gridService, IFilterInterpreter filterInterpreter, IIdentityService identity, IGridExcelService excelService)
        {
            this.GridKeys = new List<string>
            {
                "NU_UNIDAD_TRANSPORTE"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_UNIDAD_TRANSPORTE", SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._filterInterpreter = filterInterpreter;
            this._identity = identity;
            this._excelService = excelService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems = new List<IGridItem>
            {
                new GridButton("btnImprimir", "IMP050_grid1_btn_imprimir")
            };

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int? nroUt = null;
            var paramNroUt = context.GetParameter("nroUT");

            if (!string.IsNullOrEmpty(paramNroUt) && int.TryParse(paramNroUt, out int parsedValue))
                nroUt = parsedValue;

            var dbQuery = new ConsultaUTQuery(nroUt);
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int? nroUt = null;
            var paramNroUt = context.GetParameter("nroUT");

            if (!string.IsNullOrEmpty(paramNroUt) && int.TryParse(paramNroUt, out int parsedValue))
                nroUt = parsedValue;

            var dbQuery = new ConsultaUTQuery(nroUt);

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

            int? nroUt = null;
            var paramNroUt = context.GetParameter("nroUT");

            if (!string.IsNullOrEmpty(paramNroUt) && int.TryParse(paramNroUt, out int parsedValue))
                nroUt = parsedValue;

            var dbQuery = new ConsultaUTQuery(nroUt);

            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var selectedKeys = GetSelectedKeys(uow, context);

            context.AddParameter("selectedKeys", JsonConvert.SerializeObject(selectedKeys));

            return context;
        }

        public virtual List<int> GetSelectedKeys(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            int? nroUt = null;
            var paramNroUt = context.GetParameter("nroUT");

            if (!string.IsNullOrEmpty(paramNroUt) && int.TryParse(paramNroUt, out int parsedValue))
                nroUt = parsedValue;

            var dbQuery = new ConsultaUTQuery(nroUt);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys);

            return dbQuery.GetSelectedKeys(context.Selection.Keys);
        }
    }
}
