using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Expedicion;
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

namespace WIS.Application.Controllers.EXP
{
    public class EXP150VerContenedores : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public EXP150VerContenedores(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
               "CD_EMPRESA","CD_CLIENTE","NU_PEDIDO","NU_CONTENEDOR","NU_PREPARACION"
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
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
                    {
                      //  new GridButton("BtnAsignar", "General_Sec0_btn_Asignar", "fa fa-plus-circle"),
                        new GridButton("BtnVerDetPickContenedor", "EXP150_Sec0_btn_VerDetallePickingCont", "fas fa-list"),
                    }));


            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            string pedido = context.GetParameter("NU_PEDIDO");
            int empresa = int.Parse(context.GetParameter("CD_EMPRESA"));
            string cliente = context.GetParameter("CD_CLIENTE");

            var dbQuery = new EXP150VerContenedoresQuery(pedido, cliente, empresa);
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);


            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string pedido = context.GetParameter("NU_PEDIDO");
            int empresa = int.Parse(context.GetParameter("CD_EMPRESA"));
            string cliente = context.GetParameter("CD_CLIENTE");
            var dbQuery = new EXP150VerContenedoresQuery(pedido, cliente, empresa);

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

            var dbQuery = new EXP150VerContenedoresQuery(pedido, cliente, empresa);

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);

        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {

            context.AddParameter("BTNID", context.ButtonId);
            context.AddParameter("NU_CONTENEDOR", context.Row.GetCell("NU_CONTENEDOR").Value);
            context.AddParameter("NU_PREPARACION", context.Row.GetCell("NU_PREPARACION").Value);

            return context;
        }
    }



}