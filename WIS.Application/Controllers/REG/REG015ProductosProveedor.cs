using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Queries.Registro;
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
using WIS.Persistence.Database;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.REG
{
    public class REG015ProductosProveedor : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<REG015ProductosProveedor> _logger;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REG015ProductosProveedor(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<REG015ProductosProveedor> logger)
        {
            this.GridKeys = new List<string>
            {
                "CD_PRODUTO",
                "CD_CLIENTE",
                "CD_EMPRESA",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_PRODUTO", SortDirection.Descending),
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsAddEnabled = false;
            context.IsCommitEnabled = true;
            context.IsRemoveEnabled = true;
            context.IsEditingEnabled = true;

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_EDITAR", new List<GridButton>
            {
                new GridButton("btnEditar", "General_Sec0_btn_Editar", "fas fa-edit")
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ProductoProveedorQuery();
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ProductoProveedorQuery();
            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ProductoProveedorQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                if (grid.Rows.Any())
                {
                    foreach (var row in grid.Rows)
                    {
                        if (row.IsDeleted)
                        {
                            var empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                            var cliente = row.GetCell("CD_CLIENTE").Value;
                            var producto = row.GetCell("CD_PRODUTO").Value;

                            var prodProveedor = uow.ProductoRepository.GetProductoProveedor(empresa, producto, cliente);
                            if (prodProveedor == null)
                                throw new ValidationFailedException("REG015_Sec0_Error_ProdProveedorNoEncontrado", [empresa.ToString(), cliente, producto]);

                            uow.ProductoRepository.RemoveProductoProveedor(prodProveedor);
                        }
                    }
                }

                uow.SaveChanges();

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                _logger.LogError(ex, ex.Message);
            }
            catch (Exception ex)
            {
                context.AddErrorNotification("General_Sec0_Error_ErrorOperacionGrilla");
                _logger.LogError(ex, "General_Sec0_Error_ErrorOperacionGrilla");
            }

            return grid;
        }
    }
}
