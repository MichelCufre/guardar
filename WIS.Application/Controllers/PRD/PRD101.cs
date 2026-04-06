using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Domain.General;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRD
{
    public class PRD101 : AppController
    {
        protected readonly IGridService _gridService;
        protected readonly ISessionAccessor _session;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IIdentityService _identity;
        protected readonly IGridExcelService _excelService;

        public PRD101(
            IGridService gridService,
            IUnitOfWorkFactory uowFactory,
            IFilterInterpreter filterInterpreter,
            ISessionAccessor session,
            IGridValidationService gridValidationService,
            IIdentityService identity,
            IGridExcelService excelService)
        {
            this._gridService = gridService;
            this._uowFactory = uowFactory;
            this._filterInterpreter = filterInterpreter;
            this._session = session;
            this._gridValidationService = gridValidationService;
            this._identity = identity;
            this._excelService = excelService;
        }

        protected readonly List<string> GridKeysEntrada = new List<string>
        {
            "CD_PRDC_DEFINICION", "CD_COMPONENTE", "CD_EMPRESA", "CD_PRODUTO", "CD_FAIXA"
        };

        protected readonly List<string> GridKeysSalida = new List<string>
        {
            "CD_PRDC_DEFINICION", "CD_EMPRESA", "CD_PRODUTO", "CD_FAIXA", "ID_PRODUTO_FINAL"
        };

        protected readonly List<SortCommand> GridSortEntrada = new List<SortCommand>
        {
           new SortCommand("DS_PRODUTO", SortDirection.Descending)
        };

        protected readonly List<SortCommand> GridSortSalida = new List<SortCommand>
        {
           new SortCommand("DS_PRODUTO", SortDirection.Descending)
        };

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            string formula = context.FetchContext.GetParameter("formula");
            this._session.SetValue("PRD101_FORMULA", formula);

            using var uow = this._uowFactory.GetUnitOfWork();

            context.IsEditingEnabled = true;
            context.IsAddEnabled = true;
            context.IsCommitEnabled = false;
            context.IsRemoveEnabled = false;

            if (string.IsNullOrEmpty(formula) || !uow.ProduccionRepository.AnyProduccionParaFormula(formula))
            {
                context.IsCommitEnabled = true;
                context.IsRemoveEnabled = true;
            }

            grid.SetInsertableColumns(new List<string>
            {
                "CD_PRODUTO", "QT_CONSUMIDA_LINEA"
            });

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string formula = context.GetParameter("formula");

            if (grid.Id == "PRD101_grid_1")
            {
                var dbQuery = new FormulaProduccionEntradaQuery(formula);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.GridSortEntrada, this.GridKeysEntrada);

                grid.SetEditableCells(new List<string>
                {
                    "QT_CONSUMIDA_LINEA"
                });
            }
            else if (grid.Id == "PRD101_grid_2")
            {
                var dbQuery = new FormulaProduccionSalidaQuery(formula);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.GridSortSalida, this.GridKeysSalida);

                grid.SetEditableCells(new List<string>
                {
                    "QT_CONSUMIDA_LINEA"
                });
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string formula = query.GetParameter("formula");

            if (grid.Id == "PRD101_grid_1")
            {
                var dbQuery = new FormulaProduccionEntradaQuery(formula);

                uow.HandleQuery(dbQuery);

                dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else if (grid.Id == "PRD101_grid_2")
            {
                var dbQuery = new FormulaProduccionSalidaQuery(formula);

                uow.HandleQuery(dbQuery);

                dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }

            return null;
        }

        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            switch (context.ColumnId)
            {
                case "CD_PRODUTO":
                    return this.SearchProducto(grid, context);
            }

            return new List<SelectOption>();
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Id == "PRD101_grid_1")
                return this._gridValidationService.Validate(new DefinicionFormulaLineaEntradaValidationModule(uow, _identity.GetFormatProvider()), grid, row, context);
            else if (grid.Id == "PRD101_grid_2")
                return this._gridValidationService.Validate(new DefinicionFormulaLineaSalidaValidationModule(uow, _identity.GetFormatProvider()), grid, row, context);

            return null;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var codigoFormula = _session.GetValue<string>("PRD101_FORMULA");

            if (uow.ProduccionRepository.AnyProduccionParaFormula(codigoFormula))
                throw new ValidationFailedException("PRD100_grid1_error_ExisteProduccion");

            foreach (var row in grid.Rows)
            {
                if (grid.Id == "PRD101_grid_1")
                {
                    if (row.IsDeleted)
                    {
                        var codigoProducto = row.GetCell("CD_PRODUTO").Value;
                        BorrarLineaEntrada(codigoProducto, codigoFormula, uow);
                    }
                }
                else if (grid.Id == "PRD101_grid_2")
                {
                    if (row.IsDeleted)
                    {
                        var codigoProducto = row.GetCell("CD_PRODUTO").Value;
                        BorrarLineaSalida(codigoProducto, codigoFormula, uow);
                    }
                }
            }

            uow.SaveChanges();

            context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string formula = context.GetParameter("formula");

            if (grid.Id == "PRD101_grid_1")
            {
                var dbQuery = new FormulaProduccionEntradaQuery(formula);

                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.GridSortEntrada);
            }

            if (grid.Id == "PRD101_grid_2")
            {
                var dbQuery = new FormulaProduccionSalidaQuery(formula);

                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.GridSortSalida);
            }

            return null;
        }

        #region Metodos Auxiliares

        public virtual List<SelectOption> SearchProducto(Grid grid, GridSelectSearchContext context)
        {
            string paramEmpresa = context.GetParameter("empresa");

            List<SelectOption> opciones = new List<SelectOption>();

            if (string.IsNullOrEmpty(paramEmpresa))
                return opciones;

            int empresa = int.Parse(paramEmpresa);

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                List<Producto> productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(empresa, context.SearchValue);

                foreach (Producto producto in productos)
                {
                    opciones.Add(new SelectOption(producto.Codigo, $"{producto.Codigo} - {producto.Descripcion}"));
                }
            }

            return opciones;
        }

        public virtual void BorrarLineaEntrada(string codigoProducto, string codigoFormula, IUnitOfWork uow)
        {
            var formula = uow.FormulaRepository.GetFormula(codigoFormula);
            var formulaEntrada = formula.Entrada.FirstOrDefault(e => e.Producto == codigoProducto);

            uow.FormulaRepository.DeleteFormulaLineaEntrada(formula, formulaEntrada);
        }

        public virtual void BorrarLineaSalida(string codigoProducto, string codigoFormula, IUnitOfWork uow)
        {
            var formula = uow.FormulaRepository.GetFormula(codigoFormula);
            var formulaSalida = formula.Salida.FirstOrDefault(e => e.Producto == codigoProducto);

            uow.FormulaRepository.DeleteFormulaLineaSalida(formula, formulaSalida);
        }

        #endregion
    }
}
