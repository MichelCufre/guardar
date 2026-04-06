using NLog;
using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules.Produccion;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Domain.General;
using WIS.Domain.Produccion;
using WIS.Domain.StockEntities;
using WIS.Exceptions;
using WIS.Extension;
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
    public class PRD220 : AppController
    {
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFormatProvider _culture;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        public PRD220(ISessionAccessor session,
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
            string nroIngreso = this._session.GetValue<string>("PRD220_NU_PRDC_INGRESO");

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                IngresoBlackBox ingreso = uow.ProduccionRepository.GetIngresoBlackBox(nroIngreso);

                if (ingreso != null)
                {
                    data.AddParameter("NU_PRDC_INGRESO", ingreso.Id);
                    data.AddParameter("CD_EMPRESA", Convert.ToString(ingreso.Empresa));
                }
            }

            this._session.SetValue("PRD220_NU_PRDC_INGRESO", null);

            return data;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;

            grid.SetInsertableColumns(new List<string>
            {
                "CD_PRODUTO",
                "NU_IDENTIFICADOR",
                "DT_FABRICACAO",
                "QT_PRODUCIDO"
            });

            string nroIngreso = context.FetchContext.GetParameter("NU_PRDC_INGRESO");

            using (var uow = this._uowFactory.GetUnitOfWork())
            {

				IngresoBlackBox ingreso = uow.ProduccionRepository.GetIngresoBlackBox(nroIngreso);
				int empresaInt = (int)ingreso.Empresa;
                Empresa empresa = uow.EmpresaRepository.GetEmpresa(empresaInt);

                grid.GetColumn("CD_PRDC_DEFINICION").DefaultValue = ingreso.Formula.Id;
                grid.GetColumn("CD_EMPRESA").DefaultValue = Convert.ToString(ingreso.Empresa);
                grid.GetColumn("NM_EMPRESA").DefaultValue = empresa.Nombre;
                grid.GetColumn("NU_PREDIO").DefaultValue = ingreso.Linea.Predio;
                grid.GetColumn("NU_PRDC_INGRESO").DefaultValue = ingreso.Id;
                grid.GetColumn("NU_IDENTIFICADOR").DefaultValue = ingreso.Id;
            }

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                string nroIngreso = context.GetParameter("NU_PRDC_INGRESO");
                var dbQuery = new StockProducirBlackBoxProduccionQuery(nroIngreso);

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("NU_PRDC_INGRESO", Sorting.SortDirection.Descending);

                List<SortCommand> sorts = new List<SortCommand>
                {
                    new SortCommand("NU_PRDC_INGRESO", Sorting.SortDirection.Descending),
                    new SortCommand("CD_EMPRESA", Sorting.SortDirection.Ascending),
                    new SortCommand("CD_PRODUTO", Sorting.SortDirection.Ascending)
                };

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

                foreach (var row in grid.Rows)
                {
                    row.SetEditableCells(new List<string> { "QT_PRODUCIDO" });
                }
            }

            return grid;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            var culture = this._identity.GetFormatProvider();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                uow.CreateTransactionNumber(this._identity.Application);

                var nroIngreso = context.GetParameter("NU_PRDC_INGRESO");
                var ingreso = uow.ProduccionRepository.GetIngresoBlackBox(nroIngreso);
                var ajuste = new AjusteProducidoBlackBox(uow, ingreso, this._identity.UserId);

                if (grid.Rows.Count > 0 && ingreso.Consumidos.Count == 0)
                {
                    throw new ValidationFailedException("General_Sec0_Error_Error77");
                }

                if (grid.HasNewDuplicates(this.GridKeys))
                    throw new ValidationFailedException("DOC081_Sec0_Error_Error06");

                foreach (var row in grid.Rows)
                {
                    string producto = row.GetCell("CD_PRODUTO").Value;
                    string identificador = row.GetCell("NU_IDENTIFICADOR").Value;

                    DateTime? vencimiento = DateTimeExtension.ParseFromIso(row.GetCell("DT_FABRICACAO").Value);
                    decimal cantidad = decimal.Parse(row.GetCell("QT_PRODUCIDO").Value, culture);
                    int empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);

                    Producto prod = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(empresa, producto);

                    if (row.IsNew)
                    {
                        ajuste.AddLineaProducto(producto, prod.ParseIdentificador(identificador), 1, vencimiento, cantidad, empresa, out Stock stock);
                    }
                    else if (row.IsDeleted)
                    {
                        //decimal faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value, culture);
                        //ajuste.RemoveLineaProducto(producto, prod.ParseIdentificador(identificador), faixa, empresa);
                    }
                    else
                    {
                        decimal faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value, culture);
                        //ajuste.UpdateLineaProducto(producto, prod.ParseIdentificador(identificador), faixa, cantidad, empresa);
                    }
                }

                uow.SaveChanges();

                context.AddSuccessNotification("General_Sec0_Success_SavedChanges");
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                string nroIngreso = context.GetParameter("NU_PRDC_INGRESO");

                var dbQuery = new StockProducirBlackBoxProduccionQuery(nroIngreso);

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("NU_PRDC_INGRESO", Sorting.SortDirection.Descending);

                List<SortCommand> sorts = new List<SortCommand>
                {
                    new SortCommand("NU_PRDC_INGRESO", Sorting.SortDirection.Descending),
                    new SortCommand("CD_EMPRESA", Sorting.SortDirection.Ascending),
                    new SortCommand("CD_PRODUTO", Sorting.SortDirection.Ascending)
                };

                context.FileName = this._identity.Application +"-"+ DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new PRD220GridValidationModule(uow, this._culture), grid, row, context);
        }

        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            switch (context.ColumnId)
            {
                case "CD_PRODUTO":
                    return this.SearchProducto(grid, row, context);
            }

            return new List<SelectOption>();
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string nroIngreso = query.GetParameter("NU_PRDC_INGRESO");
            var dbQuery = new StockProducirBlackBoxProduccionQuery(nroIngreso);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        #region Metodos Auxiliares

        public virtual List<SelectOption> SearchProducto(Grid grid, GridRow row, GridSelectSearchContext context)
        {
            int cdEmpresa = int.Parse(context.GetParameter("CD_EMPRESA"));
            List<SelectOption> options = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                List<Producto> productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(cdEmpresa, context.SearchValue);

                foreach (var producto in productos)
                {
                    options.Add(new SelectOption(producto.Codigo, $"{producto.Codigo} {producto.Descripcion}"));
                }
            }

            return options;
        }

        #endregion
    }
}
