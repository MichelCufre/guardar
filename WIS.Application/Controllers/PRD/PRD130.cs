using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules.Produccion;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Produccion;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Domain.Picking;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRD
{
    public class PRD130 : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected List<string> GridKeys { get; set; }

        public PRD130(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFormValidationService formValidationService,
            IGridValidationService gridValidationService,
            IFilterInterpreter filterInterpreter)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._formValidationService = formValidationService;
            this._gridValidationService = gridValidationService;
            this._filterInterpreter = filterInterpreter;

            this.GridKeys = new List<string>
            {
                "NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA"
            };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = false;
            context.IsAddEnabled = false;

            grid.SetInsertableColumns(new List<string>
            {
                "NU_PRDC_INGRESO"
            });
            using var uow = this._uowFactory.GetUnitOfWork();
            grid.AddOrUpdateColumn(new GridColumnSelect("NU_PRDC_INGRESO", this.SelectNumerosIngresoProduccion(uow)));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                this.GridKeys = new List<string>() { "NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA" };

                var dbQuery = new PedidosOrdenesProduccionQuery();

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

                foreach (var row in grid.Rows)
                {
                    row.SetEditableCells(new List<string> { "NU_PRDC_INGRESO" });
                }
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var dbQuery = new PedidosOrdenesProduccionQuery();

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                uow.CreateTransactionNumber(this._identity.Application);

                FormulaAccionMapper mapper = new FormulaAccionMapper();

                foreach (var row in grid.Rows)
                {
                    string nuPedido = row.GetCell("NU_PEDIDO").Value;
                    int cdEmpresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                    string cdCliente = row.GetCell("CD_CLIENTE").Value;

                    Pedido pedido = uow.PedidoRepository.GetPedido(cdEmpresa, cdCliente, nuPedido);

                    if (pedido == null)
                        throw new ValidationFailedException("KIT130_Sec0_Error_WB003_PedidoNoExiste");

                    if (pedido.Origen == null || pedido.Origen != "PRD110")
                        throw new ValidationFailedException("KIT130_Sec0_Error_WB004_PedidoNoValido");

                    pedido.IngresoProduccion = row.GetCell("NU_PRDC_INGRESO").Value;
                    pedido.Transaccion = uow.GetTransactionNumber();

                    uow.PedidoRepository.UpdatePedido(pedido);
                }

                uow.SaveChanges();

                context.AddSuccessNotification("General_Sec0_Success_SavedChanges");
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new PRD130GridValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            this.GridKeys = new List<string>() { "NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA" };

            var dbQuery = new PedidosOrdenesProduccionQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        #region Metodos Auxiliares

        public virtual List<SelectOption> SelectNumerosIngresoProduccion(IUnitOfWork uow)
        {
            return uow.ProduccionRepository.GetNumerosIngresos()
                .Select(numeroIngreso => new SelectOption(numeroIngreso, numeroIngreso))
                .ToList();
        }

        #endregion
    }
}


