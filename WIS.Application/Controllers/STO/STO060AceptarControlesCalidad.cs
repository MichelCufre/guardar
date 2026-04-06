using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Stock;
using WIS.Domain.General;
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
    public class STO060AceptarControlesCalidad : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        public STO060AceptarControlesCalidad(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_CTR_CALIDAD_PENDIENTE"
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems.Add(new GridButton("btnAceptarControl", "STO060_grid1_btn_AprobarControles", string.Empty, new ConfirmMessage("STO060_grid1_btn_ConfirmarAprobarControles")));

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ACTIONS", new List<GridButton>
            {
                new GridButton("btnAtributos", "STO060_Sec0_btn_Atributos", "fas fa-list"),
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Id == "STO060_grid_1") //Grid Etiquetas
            {
                var dbQuery = new StockControlCalidadAceptarEtiqQuery();

                uow.HandleQuery(dbQuery);

                var sort = new SortCommand("NU_CTR_CALIDAD_PENDIENTE", SortDirection.Descending);

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

            }
            else if (grid.Id == "STO060_grid_2") //Grid Ubicaciones
            {
                var dbQuery = new StockControlCalidadAceptarUbicQuery();

                uow.HandleQuery(dbQuery);

                var sort = new SortCommand("NU_CTR_CALIDAD_PENDIENTE", SortDirection.Descending);

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
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Id == "STO060_grid_1") //Grid Etiquetas
            {
                var dbQuery = new StockControlCalidadAceptarEtiqQuery();

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else //Grid Ubicaciones
            {
                var dbQuery = new StockControlCalidadAceptarUbicQuery();

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Id == "STO060_grid_1") //Grid Etiquetas
            {
                var dbQuery = new StockControlCalidadAceptarEtiqQuery();

                uow.HandleQuery(dbQuery);

                var sort = new SortCommand("NU_CTR_CALIDAD_PENDIENTE", SortDirection.Descending);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, sort);

            }
            else //Grid Ubicaciones
            {
                var dbQuery = new StockControlCalidadAceptarUbicQuery();

                uow.HandleQuery(dbQuery);

                var sort = new SortCommand("NU_CTR_CALIDAD_PENDIENTE", SortDirection.Descending);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, sort);
            }
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber("STO060AceptarControlesCalidad - GridMenuItemAction");
            uow.BeginTransaction();

            try
            {
                if (context.ButtonId == "btnAceptarControl")
                {
                    if (context.GridId == "STO060_grid_1") //Grid Etiquetas
                    {
                        var controles = this.GetControlesSelection(context);

                        if (context.Selection.AllSelected)
                            controles = this.GetControlesAllSelected(uow, controles, context);

                        var aceptacion = new AceptacionControlesCalidad(uow, controles, this._identity.UserId);

                        aceptacion.AceptarControlesEtiqueta();
                    }
                    else //Grid Ubicaciones
                    {
                        var controles = this.GetControlesSelection(context);

                        if (context.Selection.AllSelected)
                            controles = this.GetControlesAllSelected(uow, controles, context);

                        var aceptacion = new AceptacionControlesCalidad(uow, controles, this._identity.UserId);

                        aceptacion.AceptarControlesUbicacion();
                    }

                    uow.SaveChanges();
                    context.AddSuccessNotification("STO060_Sec0_Error_ControlAceptadoCorrectamente");
                }

                uow.Commit();
            }
            catch (Exception ex)
            {
                uow.Rollback();
            }

            return context;
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
                throw;
            }

            return context;
        }

        #region Metodos Auxiliares

        public virtual List<int> GetControlesAllSelected(IUnitOfWork uow, List<int> controlesExcluir, GridMenuItemActionContext context)
        {

            if (context.GridId == "STO060_grid_1")
            {
                var dbQuery = new StockControlCalidadAceptarEtiqQuery();

                uow.HandleQuery(dbQuery);

                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                dbQuery.ExcludeSelection(controlesExcluir);

                return dbQuery.GetIdControles();
            }
            else
            {
                var dbQuery = new StockControlCalidadAceptarUbicQuery();

                uow.HandleQuery(dbQuery);

                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                dbQuery.ExcludeSelection(controlesExcluir);

                return dbQuery.GetIdControles();
            }
        }
        
        public virtual List<int> GetControlesSelection(GridMenuItemActionContext context)
        {
            List<int> controles = new List<int>();

            foreach (var id in context.Selection.Keys)
            {
                controles.Add(int.Parse(id));
            }

            return controles;
        }

        #endregion

    }
}
