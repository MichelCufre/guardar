using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Porteria;
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

namespace WIS.Application.Controllers.POR
{
    public class POR040AgendasVehiculo : AppController
    {
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IFilterInterpreter _filterInterpeter;

        protected List<string> GridKeys { get; }

        public POR040AgendasVehiculo(
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IUnitOfWorkFactory uowFactory,
            ISecurityService security,
              IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_PORTERIA_VEHICULO","NU_PORTERIA_VEHICULO_AGENDA","NU_AGENDA"
            };

            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._uowFactory = uowFactory;
            this._security = security;
            _filterInterpeter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            List<IGridItem> items = new List<IGridItem>();

            using var uow = this._uowFactory.GetUnitOfWork();

            if (this._security.IsUserAllowed("WREC010_Page_Access_RefRecepcion"))
            {
                items.Add(new GridButton("btnReferencias", "POR040_Sec0_btn_Referencias"));
            }

            if (this._security.IsUserAllowed("WREC170_grid1_btn_DetallesAgenda"))
            {
                items.Add(new GridButton("btnDetalles", "General_Sec0_btn_Detalles"));
            }

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", items));

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            SortCommand defaultSort = new SortCommand("NU_PORTERIA_VEHICULO", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PorteriaAgendasVehiculoQuery();

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, defaultSort, this.GridKeys);

            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PorteriaAgendasVehiculoQuery dbQuery = null;

            dbQuery = new PorteriaAgendasVehiculoQuery();

            uow.HandleQuery(dbQuery, true);
            dbQuery.ApplyFilter(this._filterInterpeter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            SortCommand defaultSort = new SortCommand("NU_PORTERIA_VEHICULO", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PorteriaAgendasVehiculoQuery();

            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}{DateTime.Now:yyyy-MM-dd_HH:mm}.xlsx";

            return _excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, defaultSort);
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext data)
        {

            string nuAgenda = data.Row.GetCell("NU_AGENDA").Value;

            if (data.ButtonId == "btnReferencias")
            {
                //this._auditor.SetUserValue(this._security.UserId, "WREC170_FROM", "WPOR040");
                //this._auditor.SetUserValue(this._security.UserId, "WREC170_NU_AGENDA", nuAgenda);

                data.Redirect("/recepcion/REC010?agenda=" + nuAgenda);
            }
            else if (data.ButtonId == "btnDetalles")
            {
                //this._auditor.SetUserValue(this._security.UserId, "WREC170_FROM", "WREC170");
                //this._auditor.SetUserValue(this._security.UserId, "WREC170_NU_AGENDA", nuAgenda);

                data.Redirect("/recepcion/REC171?agenda=" + nuAgenda);
            }

            return data;
        }
    }
}
