using System;
using System.Collections.Generic;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Domain.Produccion;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRD
{
    public class PRD102 : AppController
    {
        protected readonly IGridService _gridService;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IFilterInterpreter _filterInterpreter;

        public PRD102(IUnitOfWorkFactory uowFactory, IGridService gridService, IFilterInterpreter filterInterpreter)
        {
            this._gridService = gridService;
            _uowFactory = uowFactory;
            this._filterInterpreter = filterInterpreter;
        }

        protected readonly List<string> GridKeys = new List<string>
        {
             "CD_PRDC_DEFINICION", "CD_ACCION_INSTANCIA"
        };

        protected readonly List<SortCommand> GridSort = new List<SortCommand>
        {
           new SortCommand("NU_ORDEN", SortDirection.Ascending)
        };

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            string formula = context.FetchContext.GetParameter("formula");

            context.IsEditingEnabled = true;
            context.IsAddEnabled = true;
            context.IsCommitEnabled = false;
            context.IsRemoveEnabled = !string.IsNullOrEmpty(formula);

            grid.SetInsertableColumns(new List<string>
            {
                "CD_ACCION_INSTANCIA", "QT_PASADAS", "NU_ORDEN"
            });

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string formula = context.GetParameter("formula");

            var dbQuery = new FormulaProduccionConfigAccionQuery(formula);

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.GridSort, this.GridKeys);

            grid.SetEditableCells(new List<string>
                {
                    "QT_PASADAS", "NU_ORDEN"
                });

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string formula = query.GetParameter("formula");

            var dbQuery = new FormulaProduccionConfigAccionQuery(formula);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        
        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            switch (context.ColumnId)
            {
                case "CD_ACCION_INSTANCIA":
                    return this.SearchAccionInstancia(grid, context);
            }

            return new List<SelectOption>();
        }

        #region Metodos Auxiliares

        public virtual List<SelectOption> SearchAccionInstancia(Grid grid, GridSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<FormulaAccion> acciones = uow.FormulaAccionRepository.GetAccionByDescriptionPartial(context.SearchValue);

            foreach (FormulaAccion accion in acciones)
            {
                opciones.Add(new SelectOption(Convert.ToString(accion.Id), accion.Descripcion));
            }

            return opciones;
        }

        #endregion
    }
}
