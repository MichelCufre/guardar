using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.REC
{
    public class REC500GrillaDetalleFactura : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REC500GrillaDetalleFactura(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            IGridValidationService gridValidationService)
        {
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._gridValidationService = gridValidationService;

            this.GridKeys = new List<string>
        {
            "NU_RECEPCION_FACTURA",
            "CD_EMPRESA",
            "CD_FAIXA",
            "NU_IDENTIFICADOR",
            "CD_PRODUTO"
        };

            this.DefaultSort = new List<SortCommand>
        {
            new SortCommand("NU_AGENDA", SortDirection.Ascending)
        };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = false;
            context.IsAddEnabled = false;
            context.IsRollbackEnabled = false;
            context.IsCommitEnabled = false;
            context.IsRemoveEnabled = false;

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new FacturaDetalleQuery();
            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("CD_PRODUTO", SortDirection.Ascending);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new FacturaDetalleQuery();
            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("CD_PRODUTO", SortDirection.Ascending);

            context.FileName = $"{_identity.Application}_{DateTime.Now:yyyy-MM-dd_HH-mm}.xlsx";

            return _excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }

        public override Grid GridImportExcel(Grid grid, GridImportExcelContext context)
        {
            if (context.Payload == null)
                throw new MissingParameterException("Datos nulos");

            using (var excelImporter = new GridExcelImporter(context.Translator, context.FileName, grid.Columns, context.Payload))
            {
                try
                {
                    var rowsExcel = excelImporter.BuildRows();
                    int rowId = 0;

                    foreach (var row in rowsExcel)
                    {
                        foreach (var column in grid.Columns)
                        {
                            if (!row.Cells.Any(c => c.Column.Id == column.Id))
                            {
                                row.AddCell(new GridCell()
                                {
                                    Column = column,
                                });
                            }
                        }
                        rowId--;
                        row.Id = rowId.ToString();

                        var validationContext = new GridValidationContext
                        {
                            Parameters = context.FetchContext.Parameters
                        };
                        grid = this.GridValidateRow(row, grid, validationContext);
                    }
                    if (grid.Rows.Any(r => !r.IsDeleted && !r.IsValid()))
                        throw new ValidationFailedException("General_Sec0_Error_Error07");

                    grid.Rows.AddRange(rowsExcel);

                    grid = this.GridFetchRows(grid, context.FetchContext);
                }
                catch (ValidationFailedException ex)
                {
                    var payload = Convert.ToBase64String(excelImporter.GetAsByteArray());

                    throw new GridExcelImporterException(ex.Message, payload, ex.StrArguments);
                }
                catch (Exception ex)
                {
                    context.AddErrorNotification(ex.Message);
                    throw;
                }
            }
            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new FacturaDetalleQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return grid;
        }
    }
}
