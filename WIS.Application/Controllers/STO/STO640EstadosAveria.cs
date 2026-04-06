using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Stock;
using WIS.Domain.Eventos;
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
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.STO
{
    public class STO640EstadosAveria : AppController
    {
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly IGridService service;
        protected readonly IGridExcelService _excelService;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        public STO640EstadosAveria(
            ISessionAccessor session, 
            IIdentityService identity, 
            IGridService service, 
            IGridExcelService excelService, 
            IUnitOfWorkFactory uowFactory, 
            IFilterInterpreter filterInterpreter)
        {
            this._session = session;
            this._identity = identity;

            this.GridKeys = new List<string>
            {
                "NU_LOG_CLASIF_STOCK"
            };
            this.service = service;
            this._excelService = excelService;
            this._uowFactory = uowFactory;
            this._filterInterpreter = filterInterpreter;
        }
        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            grid.MenuItems = new List<IGridItem> {
                new GridButton("btnNotificar", "General_Sec0_btn_Notificar"),
            };

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("btnAdjuntarArchivo", "General_Sec0_btn_AdjuntarArchivo", "fas fa-paperclip"),
            }));

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            SortCommand defaultSort = new SortCommand("CD_PRODUTO", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new EstadosAveriaQuery();

            uow.HandleQuery(dbQuery);

            grid.Rows = service.GetRows(dbQuery, grid.Columns, query, defaultSort, this.GridKeys);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            SortCommand defaultSort = new SortCommand("CD_PRODUTO", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new EstadosAveriaQuery();

            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, defaultSort);
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext query)
        {
            if (query.ButtonId == "btnAdjuntarArchivo")
            {
                _session.SetValue("EVT000_JSON", JsonConvert.SerializeObject(new JsonArchivoAdjunto
                {
                    CD_EMPRESA = query.Row.GetCell("CD_EMPRESA").Value,
                    CD_MANEJO = "AVE",
                    DS_REFERENCIA = $"{query.Row.GetCell("NU_LOG_CLASIF_STOCK").Value}",
                    DATA_INFO = JsonConvert.SerializeObject(query.Row.Cells.Select(w => new AuxCell20 { datafield = w.Column.Id, value = w.Value }).ToList())

                }));

                query.Redirect("/evento/EVT000");
                return query;
            }

            return query;
        }
        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            EstadosAveriaQuery dbQuery = new EstadosAveriaQuery();

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            var keysRowSelected = dbQuery.GetKeysRowsSelected(query.Selection.AllSelected, query.Selection.Keys);

            _session.SetValue("EVT000_AVERIAS", JsonConvert.SerializeObject(keysRowSelected));

            query.Redirect = "/evento/EVT000";

            return query;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new EstadosAveriaQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
    }
}
