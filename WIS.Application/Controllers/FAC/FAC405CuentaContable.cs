using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Facturacion;
using WIS.Domain.Facturacion;
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

namespace WIS.Application.Controllers.FAC
{
    public class FAC405CuentaContable : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<FAC405CuentaContable> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public FAC405CuentaContable(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<FAC405CuentaContable> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "NU_CUENTA_CONTABLE",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_CUENTA_CONTABLE", SortDirection.Ascending),
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            _filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsAddEnabled = true;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = false;

            grid.SetInsertableColumns(new List<string> {
                "NU_CUENTA_CONTABLE",
                "DS_CUENTA_CONTABLE"
            });

            return this.GridFetchRows(grid, query.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            CuentaContableQuery dbQuery = new CuentaContableQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> {
                "DS_CUENTA_CONTABLE"
            });

            return grid;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                if (grid.Rows.Any())
                {
                    RegistroModificacionCuentaContable registroModificacionCC = new RegistroModificacionCuentaContable(uow, this._identity.UserId, this._identity.Application);

                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {

                        if (row.IsNew)
                        {
                            CuentaContable cuentaContable = this.CrearCuentaContable(uow, row, query);
                            registroModificacionCC.RegistrarLenguajeImpresion(cuentaContable);
                        }
                        else
                        {
                            // rows editadas
                            CuentaContable cuentaContable = this.UpdateCuentaContable(uow, row, query);
                            registroModificacionCC.ModificarLenguajeImpresion(cuentaContable);
                        }
                    }

                }

                uow.SaveChanges();

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "FAC405GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoCuentaContableGridValidationModule(uow), grid, row, context);
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            CuentaContableQuery dbQuery = new CuentaContableQuery();
            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            CuentaContableQuery dbQuery = new CuentaContableQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public virtual CuentaContable CrearCuentaContable(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            CuentaContable nuevaCuenta = new CuentaContable();

            nuevaCuenta.Id = row.GetCell("NU_CUENTA_CONTABLE").Value;
            nuevaCuenta.Descripcion = row.GetCell("DS_CUENTA_CONTABLE").Value.ToUpper();

            return nuevaCuenta;
        }

        public virtual CuentaContable UpdateCuentaContable(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            string idCuentaContable = row.GetCell("NU_CUENTA_CONTABLE").Value;

            CuentaContable cuentaContable = new CuentaContable();
            cuentaContable = uow.CuentaContableRepository.GetCuentaContable(idCuentaContable);

            cuentaContable.Descripcion = row.GetCell("DS_CUENTA_CONTABLE").Value.ToUpper();

            return cuentaContable;
        }
    }
}
