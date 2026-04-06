using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class STO810PanelMapeoProductos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();
        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public STO810PanelMapeoProductos(IUnitOfWorkFactory uowFactory, IIdentityService identity, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "CD_EMPRESA",
                "CD_PRODUTO",
                "CD_FAIXA",
                "CD_EMPRESA_DESTINO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("DT_ADDROW", SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = true;
            context.IsAddEnabled = false;
            context.IsCommitEnabled = true;

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("btnEditar", "STO810_grid1_btn_Editar", "fas fa-edit")
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new TraspasoMapeoProductosQuery();

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new TraspasoMapeoProductosQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new TraspasoMapeoProductosQuery();

            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}_{DateTime.Now:yyyy-MM-dd_HH:mm}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("STO810 Eliminación Mapeo de Productos");
            uow.BeginTransaction();

            try
            {
                if (grid.Rows.Any())
                {
                    foreach (var row in grid.Rows)
                    {
                        if (row.IsDeleted)
                        {
                            var empresaOrigen = int.Parse(row.GetCell("CD_EMPRESA").Value);
                            var cdProducto = row.GetCell("CD_PRODUTO").Value;
                            var faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value);
                            var empresaDestino = int.Parse(row.GetCell("CD_EMPRESA_DESTINO").Value);

                            if (!uow.TraspasoEmpresasRepository.AnyMapeoProducto(empresaOrigen, cdProducto, faixa, empresaDestino))
                                throw new ValidationFailedException("STO810_Sec0_Error_MapeoNoExiste");

                            uow.TraspasoEmpresasRepository.DeleteMapeoProducto(empresaOrigen, cdProducto, faixa, empresaDestino);
                        }
                    }
                }

                uow.SaveChanges();
                uow.Commit();
                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ValidationFailedException ex)
            {
                _logger.Error($"Error {ex.Message} - {ex}");
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                uow.Rollback();
            }
            catch (Exception ex)
            {
                _logger.Error($"Error {ex.Message} - {ex}");
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }

            return grid;
        }
    }
}
