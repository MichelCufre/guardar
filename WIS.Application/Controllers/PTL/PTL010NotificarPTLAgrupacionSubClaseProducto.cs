using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.Automatismo;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Ptl;
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

namespace WIS.Application.Controllers.PTL
{
    public class PTL010NotificarPTLAgrupacionSubClaseProducto : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PTL010NotificarPTLAgrupacionSubClaseProducto(
          ISecurityService security,
          IUnitOfWorkFactory uowFactory,
          IIdentityService identity,
          IGridService gridService,
          IGridExcelService excelService,
          IFilterInterpreter filterInterpreter)
        {

            this.GridKeys = new List<string>
            {
               "NU_PREPARACION", "CD_CLIENTE", "CD_EMPRESA","CD_SUB_CLASSE"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_PREPARACION", SortDirection.Descending),
                new SortCommand("CD_CLIENTE", SortDirection.Descending),
                new SortCommand("CD_EMPRESA", SortDirection.Descending),
                new SortCommand("CD_SUB_CLASSE", SortDirection.Descending)
            };

            _security = security;
            _uowFactory = uowFactory;
            _identity = identity;
            _gridService = gridService;
            _excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        #region Grid functions        

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_SELECT", new List<GridButton> { new GridButton("btnSelectRow", "General_Sec0_btn_SelectRow", "fas fa-hand-point-right") }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string preparacion = context.GetParameter("preparacion");
            string cliente = context.GetParameter("cliente");
            string empresa = context.GetParameter("empresa");
            string numeroAutomatismo = context.GetParameter("numeroAutomatismo");
            string vlComparteContenedorPicking = context.GetParameter("GRID_VL_COMP_CONT_PICK_PROD_ROW_SELECTED")?.Split('$')[3];

            if (string.IsNullOrEmpty(preparacion) ||
                string.IsNullOrEmpty(cliente) ||
                string.IsNullOrEmpty(empresa) ||
                string.IsNullOrEmpty(numeroAutomatismo) ||
                string.IsNullOrEmpty(vlComparteContenedorPicking))
                return grid;
            PTL010NotificarPTLAgrupacionSubclaseQuery dbQuery = null;
            if (!string.IsNullOrEmpty(context.GetParameter("AGRUPACION_CONTENEDOR_LEIDO")))
            {
                var agrupacionContentedor = JsonConvert.DeserializeObject<PtlAgrupacionContenedor>(context.GetParameter("AGRUPACION_CONTENEDOR_LEIDO"));
                dbQuery = new PTL010NotificarPTLAgrupacionSubclaseQuery(preparacion.ToNumber<int>(), numeroAutomatismo.ToNumber<int>(), cliente, empresa.ToNumber<int>(), vlComparteContenedorPicking, agrupacionContentedor.SubClase);
            }
            else
                dbQuery = new PTL010NotificarPTLAgrupacionSubclaseQuery(preparacion.ToNumber<int>(), numeroAutomatismo.ToNumber<int>(), cliente, empresa.ToNumber<int>(), vlComparteContenedorPicking);

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            if (grid.Rows.Count == 1)
            {
                grid.Rows.FirstOrDefault().DisabledButtons.Add("btnSelectRow");
                context.AddOrUpdateParameter("GRID_SUBCLASE_PROD_ROW_SELECTED", grid.Rows.FirstOrDefault().Id);
                return grid;
            }

            string rowSelected = context.GetParameter("GRID_SUBCLASE_PROD_ROW_SELECTED");

            if (grid.Rows.Count > 1 && !string.IsNullOrEmpty(rowSelected))
            {
                var keys = rowSelected.Split('$');
                grid.Rows.FirstOrDefault(x => x.GetCell("NU_PREPARACION").Value == keys[0] &&
                                              x.GetCell("CD_CLIENTE").Value == keys[1] &&
                                              x.GetCell("CD_EMPRESA").Value == keys[2] &&
                                              x.GetCell("CD_SUB_CLASSE").Value == keys[3]).CssClass = "blue";
            }

            return grid;
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PTL010NotificarPTLAgrupacionSubclaseQuery();
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "_AgrupacionSubClase_" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }


        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PTL010NotificarPTLAgrupacionSubclaseQuery();

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.ButtonId == "btnSelectRow")
            {
                context.AddOrUpdateParameter("GRID_SUBCLASE_PROD_ROW_SELECTED", context.Row.Id);
            }

            return context;
        }
        #endregion

    }
}
