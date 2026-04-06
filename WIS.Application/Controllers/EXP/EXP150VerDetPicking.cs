using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Expedicion;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.EXP
{
    public class EXP150VerDetPicking : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public EXP150VerDetPicking(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
               "CD_EMPRESA","CD_CLIENTE","NU_PEDIDO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_PEDIDO", SortDirection.Descending),

            };
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            string pedido = context.GetParameter("NU_PEDIDO");
            int empresa = int.Parse(context.GetParameter("CD_EMPRESA"));
            string cliente = context.GetParameter("CD_CLIENTE");
            int cont = int.Parse(context.GetParameter("NU_CONTENEDOR"));
            int prep = int.Parse(context.GetParameter("NU_PREPARACION"));

            var dbQuery = new EXP150VerDetPickingContenedoresQuery(pedido, cliente, empresa, cont, prep);
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            grid.Rows.ForEach(row =>
            {

            });

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string pedido = context.GetParameter("NU_PEDIDO");
            int empresa = int.Parse(context.GetParameter("CD_EMPRESA"));
            string cliente = context.GetParameter("CD_CLIENTE");
            int cont = int.Parse(context.GetParameter("NU_CONTENEDOR"));
            int prep = int.Parse(context.GetParameter("NU_PREPARACION"));
            var dbQuery = new EXP150VerDetPickingContenedoresQuery(pedido, cliente, empresa, cont, prep);

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

            string pedido = context.GetParameter("NU_PEDIDO");
            int empresa = int.Parse(context.GetParameter("CD_EMPRESA"));
            string cliente = context.GetParameter("CD_CLIENTE");
            int cont = int.Parse(context.GetParameter("NU_CONTENEDOR"));
            int prep = int.Parse(context.GetParameter("NU_PREPARACION"));

            var dbQuery = new EXP150VerDetPickingContenedoresQuery(pedido, cliente, empresa, cont, prep);

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);

        }
    }
}