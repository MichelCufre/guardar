using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Porteria;
using WIS.Domain.Porteria;
using WIS.Extension;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
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
    public class POR060AsociarAgendasVehiculo : AppController
    {
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IFilterInterpreter _filterInterpeter;

        protected List<string> Grid1Keys { get; }
        protected List<string> GridAgregarKeys { get; }
        protected List<string> GridQuitarKeys { get; }

        public POR060AsociarAgendasVehiculo(
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IUnitOfWorkFactory uowFactory,
            IFilterInterpreter filterInterpeter)
        {
            this.Grid1Keys = new List<string>
            {
                "NU_PORTERIA_VEHICULO"
            };

            this.GridAgregarKeys = new List<string>
            {
                "NU_AGENDA"
            };

            this.GridQuitarKeys = new List<string>
            {
                "NU_AGENDA"
            };

            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._uowFactory = uowFactory;
            this._filterInterpeter = filterInterpeter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            switch (grid.Id)
            {
                case "POR060_grid_1": return this.Grid1Initialize(grid, query);
            }

            return grid;
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            switch (grid.Id)
            {
                case "POR060_grid_1": return this.Grid1FetchRows(grid, query);
                case "POR060_grid_Agregar": return this.GridAgregarFetchRows(grid, query);
                case "POR060_grid_Quitar": return this.GridQuitarFetchRows(grid, query);
            }

            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            switch (grid.Id)
            {
                case "POR060_grid_1":
                    {
                        var dbQuery = new PorteriaAsociarAgendasVehiculoQuery();

                        uow.HandleQuery(dbQuery);
                        dbQuery.ApplyFilter(this._filterInterpeter, query.Filters);

                        return new GridStats
                        {
                            Count = dbQuery.GetCount()
                        };
                    }
                case "POR060_grid_Agregar":
                    {
                        FiltrosAsociarAgendasVehiculoAgenda filtros = JsonConvert.DeserializeObject<FiltrosAsociarAgendasVehiculoAgenda>(query.GetParameter("FILTROS"));

                        bool isFilter = query.GetParameter("FL_FILTER") == "S";
                        var dbQuery = new PorteriaAsociarAgendasVehiculoAgendasQuery(filtros, isFilter, true);

                        uow.HandleQuery(dbQuery, true);
                        dbQuery.ApplyFilter(this._filterInterpeter, query.Filters);

                        return new GridStats
                        {
                            Count = dbQuery.GetCount()
                        };
                    }
                case "POR060_grid_Quitar":
                    {
                        FiltrosAsociarAgendasVehiculoAgenda filtros = JsonConvert.DeserializeObject<FiltrosAsociarAgendasVehiculoAgenda>(query.GetParameter("FILTROS"));
                        bool isFilter = query.GetParameter("FL_FILTER") == "S";

                        var dbQuery = new PorteriaAsociarAgendasVehiculoAgendasQuery(filtros, isFilter);

                        uow.HandleQuery(dbQuery);
                        dbQuery.ApplyFilter(this._filterInterpeter, query.Filters);

                        return new GridStats
                        {
                            Count = dbQuery.GetCount()
                        };
                    }
            }

            return null;
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            SortCommand defaultSort = new SortCommand("NU_PORTERIA_VEHICULO", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PorteriaAsociarAgendasVehiculoQuery();

            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return _excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, defaultSort);
        }
        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext selection)
        {
            switch (selection.GridId)
            {
                case "POR060_grid_Agregar": return this.GridAgregarMenuItemAction(selection);
                case "POR060_grid_Quitar": return this.GridQuitarMenuItemAction(selection);
            }
            return selection;
        }

        public virtual Grid Grid1Initialize(Grid grid, GridInitializeContext query)
        {
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("btnEditar", "General_Sec0_btn_Editar", "fas fa-edit"),
            }));

            return this.Grid1FetchRows(grid, query.FetchContext);
        }
        public virtual Grid Grid1FetchRows(Grid grid, GridFetchContext query)
        {
            SortCommand defaultSort = new SortCommand("NU_PORTERIA_VEHICULO", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PorteriaAsociarAgendasVehiculoQuery();

            uow.HandleQuery(dbQuery, true);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, defaultSort, this.Grid1Keys);

            return grid;
        }

        public virtual Grid GridAgregarInitialize(Grid grid, GridInitializeContext query)
        {
            return this.GridAgregarFetchRows(grid, query.FetchContext);
        }
        public virtual Grid GridAgregarFetchRows(Grid grid, GridFetchContext query)
        {
            SortCommand defaultSort = new SortCommand("NU_AGENDA", SortDirection.Descending);

            FiltrosAsociarAgendasVehiculoAgenda filtros = JsonConvert.DeserializeObject<FiltrosAsociarAgendasVehiculoAgenda>(query.GetParameter("FILTROS"));

            bool isFilter = query.GetParameter("FL_FILTER") == "S";

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PorteriaAsociarAgendasVehiculoAgendasQuery(filtros, isFilter, true);

            uow.HandleQuery(dbQuery, true);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, defaultSort, this.GridAgregarKeys);

            return grid;
        }
        public virtual GridMenuItemActionContext GridAgregarMenuItemAction(GridMenuItemActionContext context)
        {
            FiltrosAsociarAgendasVehiculoAgenda filtros = JsonConvert.DeserializeObject<FiltrosAsociarAgendasVehiculoAgenda>(context.GetParameter("FILTROS"));
            bool isFilter = context.GetParameter("FL_FILTER") == "S";
            int nuPorteriaVehiculo = filtros.NU_PORTERIA_VEHICULO.ToNumber<int>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PorteriaAsociarAgendasVehiculoAgendasQuery(filtros, isFilter, true);

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpeter, context.Filters);

            var rowSelected = dbQuery.GetKeysRowsSelected(context.Selection.AllSelected, context.Selection.Keys);

            rowSelected.ForEach(w =>
            {

                uow.PorteriaRepository.AddPorteriaVehiculoAgenda(new PorteriaVehiculoAgenda
                {
                    NU_PORTERIA_VEHICULO = nuPorteriaVehiculo,
                    NU_AGENDA = w.ToNumber<int>(),

                });

            });

            uow.SaveChanges();

            return context;
        }

        public virtual Grid GridQuitarInitialize(Grid grid, GridInitializeContext query)
        {
            return this.GridQuitarFetchRows(grid, query.FetchContext);
        }
        public virtual Grid GridQuitarFetchRows(Grid grid, GridFetchContext query)
        {
            SortCommand defaultSort = new SortCommand("NU_AGENDA", SortDirection.Descending);

            FiltrosAsociarAgendasVehiculoAgenda filtros = JsonConvert.DeserializeObject<FiltrosAsociarAgendasVehiculoAgenda>(query.GetParameter("FILTROS"));
            bool isFilter = query.GetParameter("FL_FILTER") == "S";

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PorteriaAsociarAgendasVehiculoAgendasQuery(filtros, isFilter);

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, defaultSort, this.GridQuitarKeys);

            return grid;
        }
        public virtual GridMenuItemActionContext GridQuitarMenuItemAction(GridMenuItemActionContext selection)
        {
            FiltrosAsociarAgendasVehiculoAgenda filtros = JsonConvert.DeserializeObject<FiltrosAsociarAgendasVehiculoAgenda>(selection.GetParameter("FILTROS"));
            bool isFilter = selection.GetParameter("FL_FILTER") == "S";
            int nuPorteriaVehiculo = filtros.NU_PORTERIA_VEHICULO.ToNumber<int>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PorteriaAsociarAgendasVehiculoAgendasQuery(filtros, isFilter);

            var rowSelected = dbQuery.GetKeysRowsSelected(selection.Selection.AllSelected, selection.Selection.Keys);

            rowSelected.ForEach(w =>
            {
                uow.PorteriaRepository.RemovePorteriaVehiculoAgenda(nuPorteriaVehiculo, w.ToNumber<int>());
            });

            uow.SaveChanges();

            return selection;
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            form.GetField("FL_FILTER").Options = new List<SelectOption> {
                new SelectOption("S","POR060_Sec0_lbl_OptConFiltrosVehiculo"),
                new SelectOption("N","General_Sec0_lbl_OptTodo"),
            };

            return form;
        }
    }
}
