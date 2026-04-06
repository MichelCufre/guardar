using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Stock;
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

namespace WIS.Application.Controllers.STO
{
    public class STO110ControlesCalidad : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IIdentityService _identity;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        public STO110ControlesCalidad(IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IIdentityService identity, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_CTR_CALIDAD_PENDIENTE"
            };

            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            _identity = identity;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ACTIONS", new List<GridButton>
            {
                new GridButton("btnAtributos", "STO110_Sec0_btn_Atributos", "fas fa-list"),
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new StockControlCalidadQuery();

            uow.HandleQuery(dbQuery);

            var sort = new SortCommand("CD_PRODUTO", SortDirection.Ascending);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, sort, this.GridKeys);

            foreach (var row in grid.Rows)
            {
                var nuLpn = row.GetCell("NU_LPN").Value;
                var idLpnDet = row.GetCell("ID_LPN_DET").Value;

                if (string.IsNullOrEmpty(nuLpn) || string.IsNullOrEmpty(idLpnDet) || !uow.ManejoLpnRepository.AnyDetalleAtributo(int.Parse(idLpnDet)))
                {
                    row.DisabledButtons.Add("btnAtributos");
                }
            }

            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new StockControlCalidadQuery();

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

            var dbQuery = new StockControlCalidadQuery();

            uow.HandleQuery(dbQuery);

            var sort = new SortCommand("CD_PRODUTO", SortDirection.Ascending);

            context.FileName = this._identity.Application +"-"+ DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, sort);
        }
        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                switch (context.ButtonId)
                {
                    case "btnAtributos":
                        context.Redirect("/stock/STO710", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "numeroLPN", Value = context.Row.GetCell("NU_LPN").Value },
                            new ComponentParameter(){ Id = "detalle", Value = "true" },
                            new ComponentParameter(){ Id = "idDetalle", Value = context.Row.GetCell("ID_LPN_DET").Value },
                        });
                        break;

                    
                }
            }
            catch (ValidationFailedException ex)
            {       
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return context;
        }
    }
}
