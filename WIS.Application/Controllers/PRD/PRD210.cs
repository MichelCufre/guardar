using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules.Produccion;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Domain.Produccion;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRD
{
    public class PRD210 : AppController
    {
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFormatProvider _culture;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        public PRD210(ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IGridValidationService gridValidationService,
            IFilterInterpreter filterInterpreter)
        {
            this._session = session;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._gridValidationService = gridValidationService;
            this._culture = identity.GetFormatProvider();
            this._filterInterpreter = filterInterpreter;

            this.GridKeys = new List<string>
            {
                "CD_EMPRESA",
                "CD_PRODUTO",
                "CD_FAIXA",
                "NU_IDENTIFICADOR",
                "CD_ENDERECO"
            };
        }

        public override PageContext PageLoad(PageContext data)
        {
            string nroIngreso = this._session.GetValue<string>("PRD210_NU_PRDC_INGRESO");

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                IngresoBlackBox ingreso = uow.ProduccionRepository.GetIngresoBlackBox(nroIngreso);

                if (ingreso != null)
                {
                    int empresaInt = (int)ingreso.Empresa;

                    string nombreEmpresa = uow.EmpresaRepository.GetNombre(empresaInt);

                    Formula formula = uow.FormulaRepository.GetFormula(ingreso.Formula.Id);

                    data.AddParameter("NU_PRDC_INGRESO", ingreso.Id);
                    data.AddParameter("CD_PRDC_DEFINICION", ingreso.Formula.Id);
                    data.AddParameter("NM_PRDC_DEFINICION", formula.Nombre);
                    data.AddParameter("DS_PRDC_DEFINICION", formula.Descripcion);
                    data.AddParameter("CD_EMPRESA", Convert.ToString(ingreso.Empresa));
                    data.AddParameter("NM_EMPRESA", nombreEmpresa);
                    data.AddParameter("NU_PREDIO", ingreso.Linea.Predio);
                }
            }

            this._session.SetValue("PRD210_NU_PRDC_INGRESO", null);

            return data;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = false;
            context.IsAddEnabled = false;

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                string nroIngreso = context.GetParameter("NU_PRDC_INGRESO");
                var dbQuery = new StockConsumirBlackBoxProduccionQuery(nroIngreso);

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("NU_PRDC_INGRESO", SortDirection.Descending);

                List<SortCommand> sorts = new List<SortCommand>
                {
                    new SortCommand("NU_PRDC_INGRESO", SortDirection.Descending),
                    new SortCommand("CD_EMPRESA", SortDirection.Ascending),
                    new SortCommand("CD_PRODUTO", SortDirection.Ascending)
                };

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

                foreach (var row in grid.Rows)
                {
                    row.SetEditableCells(new List<string> { "QT_CONSUMIDO" });
                }
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                string nroIngreso = context.GetParameter("NU_PRDC_INGRESO");
                var dbQuery = new StockConsumirBlackBoxProduccionQuery(nroIngreso);

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("NU_PRDC_INGRESO", SortDirection.Descending);

                List<SortCommand> sorts = new List<SortCommand>
                {
                    new SortCommand("NU_PRDC_INGRESO", SortDirection.Descending),
                    new SortCommand("CD_EMPRESA", SortDirection.Ascending),
                    new SortCommand("CD_PRODUTO", SortDirection.Ascending)
                };

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new PRD210GridValidationModule(uow, this._culture), grid, row, context);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext contnext)
        {
            var culture = this._identity.GetFormatProvider();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                uow.CreateTransactionNumber(this._identity.Application);

                var nroIngreso = contnext.GetParameter("NU_PRDC_INGRESO");
                var ingreso = uow.ProduccionRepository.GetIngresoBlackBox(nroIngreso);
                var ajuste = new AjusteConsumidoBlackBox(uow, ingreso, this._identity.UserId);

                foreach (var row in grid.Rows)
                {
                    string producto = row.GetCell("CD_PRODUTO").Value;
                    string identificador = row.GetCell("NU_IDENTIFICADOR").Value;
                    decimal faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value, culture);
                    int empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                    decimal cantidad = decimal.Parse(row.GetCell("QT_CONSUMIDO").Value, culture);
                    DetallePedidoE detalle = uow.LineaRepository.GetDetallePedido(empresa, producto, identificador, faixa, nroIngreso);

                    //if (detalle != null && detalle.Semiacabado == "S")
                    //{
                    //    ajuste.AjustarProducto(empresa, producto, identificador, faixa, cantidad, "S", "N");
                    //}

                    //if (detalle != null && detalle.Consumible == "S")
                    //{
                    //    ajuste.AjustarProducto(empresa, producto, identificador, faixa, cantidad, "N", "S");
                    //}
                    //else
                    //{
                    //    ajuste.AjustarProducto(empresa, producto, identificador, faixa, cantidad, "N", "N");
                    //}
                }

                uow.SaveChanges();

                contnext.AddSuccessNotification("General_Sec0_Success_SavedChanges");
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string nroIngreso = query.GetParameter("NU_PRDC_INGRESO");
            var dbQuery = new StockConsumirBlackBoxProduccionQuery(nroIngreso);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
    }
}
