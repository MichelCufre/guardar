using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Porteria;
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

namespace WIS.Application.Controllers.POR
{
    public class POR080PreRegistroEntrada : AppController
    {
        protected readonly IIdentityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> Grid1Keys { get; }
        protected List<string> GridPersonasKeys { get; }
        protected List<string> GridContainersKeys { get; }

        public POR080PreRegistroEntrada(
            IIdentityService security,
            IGridService gridService,
            IGridExcelService excelService,
            IUnitOfWorkFactory uowFactory,
           IFilterInterpreter filterInterpreter)
        {
            this.Grid1Keys = new List<string>
            {
                "NU_PORTERIA_VEHICULO"
            };

            this.GridPersonasKeys = new List<string>
            {
                "NU_PORTERIA_VEHICULO"
            };

            this.GridContainersKeys = new List<string>
            {
                "NU_SEQ_CONTAINER"
            };

            this._security = security;
            this._gridService = gridService;
            this._excelService = excelService;
            this._uowFactory = uowFactory;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            switch (grid.Id)
            {
                case "POR080_grid_1": return this.Grid1FetchRows(grid, query.FetchContext);
                case "POR080_grid_Personas":
                    {

                        grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton> {

                        new GridButton("btnBorrar", "General_Sec0_btn_Borrar", "fas fa-trash-alt"),

                    }));

                        return this.GridPersonasFetchRows(grid, query.FetchContext);
                    }
                case "POR080_grid_Containers":
                    {

                        grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton> {

                        new GridButton("btnBorrar", "General_Sec0_btn_Borrar", "fas fa-trash-alt"),

                    }));

                        return this.GridContainersFetchRows(grid, query.FetchContext);
                    }
            }

            return grid;
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            switch (grid.Id)
            {
                case "POR080_grid_1": return this.Grid1FetchRows(grid, query);
                case "POR080_grid_Personas": return this.GridPersonasFetchRows(grid, query);
                case "POR080_grid_Containers": return this.GridContainersFetchRows(grid, query);
            }

            return grid;
        }

        public virtual Grid Grid1FetchRows(Grid grid, GridFetchContext query)
        {
            SortCommand defaultSort = new SortCommand("NU_PORTERIA_VEHICULO", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PorteriaPreRegistroEntradaQuery();

            uow.HandleQuery(dbQuery, true);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, defaultSort, this.Grid1Keys);

            return grid;
        }
        public virtual Grid GridPersonasFetchRows(Grid grid, GridFetchContext query)
        {
            SortCommand defaultSort = new SortCommand("NU_POTERIA_PERSONA", SortDirection.Descending);

            List<int> listaPersonas = JsonConvert.DeserializeObject<List<int>>(query.GetParameter("SelectionGridPersonas"));

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PorteriaRegistroEntradaPersonasQuery(listaPersonas);

            uow.HandleQuery(dbQuery, true);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, defaultSort, this.GridPersonasKeys);

            return grid;
        }
        public virtual Grid GridContainersFetchRows(Grid grid, GridFetchContext query)
        {
            SortCommand defaultSort = new SortCommand("NU_SEQ_CONTAINER", SortDirection.Descending);

            List<int> listaPersonas = JsonConvert.DeserializeObject<List<int>>(query.GetParameter("SelectionGridContainers"));

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PorteriaPreRegistroEntradaContainersQuery(listaPersonas);

            uow.HandleQuery(dbQuery, true);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, defaultSort, this.GridContainersKeys);

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            List<int> listaPersonas = JsonConvert.DeserializeObject<List<int>>(query.GetParameter("SelectionGridPersonas"));

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PorteriaRegistroEntradaPersonasQuery(listaPersonas);

            uow.HandleQuery(dbQuery, true);

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

            var dbQuery = new PorteriaPreRegistroEntradaQuery();

            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._security.Application}{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return _excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, defaultSort);
        }
        public override GridButtonActionContext GridButtonAction(GridButtonActionContext data)
        {

            if (data.GridId == "POR080_grid_Personas")
            {

                ComponentParameter parameterPersonas = data.Parameters.FirstOrDefault(w => w.Id == "SelectionGridPersonas");
                ComponentParameter parameterPersonasRemove = data.Parameters.FirstOrDefault(w => w.Id == "notSelectionGridPersonas");

                List<int> listaPersonas = JsonConvert.DeserializeObject<List<int>>(parameterPersonas.Value);
                List<int> listaPersonasRemove = JsonConvert.DeserializeObject<List<int>>(parameterPersonasRemove.Value);

                int nuPersona = data.Row.GetCell("NU_POTERIA_PERSONA").Value.ToNumber<int>();

                if (listaPersonas.Any(w => w == nuPersona))
                {
                    listaPersonas.Remove(nuPersona);
                }
                else
                {
                    if (!listaPersonasRemove.Any(w => w == nuPersona))
                        listaPersonasRemove.Add(nuPersona);

                }

                parameterPersonas.Value = JsonConvert.SerializeObject(listaPersonas);
                parameterPersonasRemove.Value = JsonConvert.SerializeObject(listaPersonasRemove);

            }
            else if (data.GridId == "POR080_grid_Containers")
            {
                ComponentParameter parameterContainers = data.Parameters.FirstOrDefault(w => w.Id == "SelectionGridContainers");

                List<int> listaContainers = JsonConvert.DeserializeObject<List<int>>(parameterContainers.Value);

                listaContainers.Remove(data.Row.Id.ToNumber<int>());

                parameterContainers.Value = JsonConvert.SerializeObject(listaContainers);

            }

            return data;
        }
    }
}
