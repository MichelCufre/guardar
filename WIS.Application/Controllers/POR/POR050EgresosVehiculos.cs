using System;
using System.Collections.Generic;
using WIS.Application.Security;
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
    public class POR050EgresosVehiculos : AppController
    {
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        public POR050EgresosVehiculos(
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IUnitOfWorkFactory uowFactory,
            ISecurityService security,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_PORTERIA_VEHICULO","NU_PORTERIA_VEHICULO_CAMION","CD_CAMION"
            };

            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._uowFactory = uowFactory;
            this._security = security;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            List<IGridItem> items = new List<IGridItem>();

            using var uow = this._uowFactory.GetUnitOfWork();

            if (this._security.IsUserAllowed(SecurityResources.WEXP040_grid1_btn_PedidosExpedidos))
            {
                items.Add(new GridButton("btnPedidosExpedidos", "POR050_Sec0_btn_PedidosExpedidos"));
            }

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", items));

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            SortCommand defaultSort = new SortCommand("NU_PORTERIA_VEHICULO", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PorteriaEgresosVehiculosQuery();

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, defaultSort, this.GridKeys);

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PorteriaEgresosVehiculosQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            SortCommand defaultSort = new SortCommand("NU_PORTERIA_VEHICULO", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PorteriaEgresosVehiculosQuery();

            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}{DateTime.Now:yyyy-MM-dd_HH:mm}.xlsx";

            return _excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, defaultSort);
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext data)
        {
            if (data.ButtonId == "btnPedidosExpedidos")
            {
                string cdCamion = data.Row.GetCell("CD_CAMION").Value;

                if (!string.IsNullOrEmpty(cdCamion))
                {
                    //this._auditor.SetUserValue(this._identity.UserId, "WPOR050_ORIGEN", "WPOR050");
                    //this._auditor.SetUserValue(this._identity.UserId, "WPOR050_CD_CAMION", cdCamion);

                    data.Redirect("/expedicion/EXP041?camion=" + cdCamion);
                }
            }

            return data;
        }
    }
}
