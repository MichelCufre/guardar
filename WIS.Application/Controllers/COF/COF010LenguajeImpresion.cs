using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Configuracion;
using WIS.Domain.Impresiones;
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

namespace WIS.Application.Controllers.COF
{
    public class COF010LenguajeImpresion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<COF010LenguajeImpresion> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public COF010LenguajeImpresion(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<COF010LenguajeImpresion> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "CD_LENGUAJE_IMPRESION",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_LENGUAJE_IMPRESION", SortDirection.Ascending),
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            _filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsAddEnabled = true;
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = true;

            grid.SetInsertableColumns(new List<string> {
                "CD_LENGUAJE_IMPRESION",
                "DS_LENGUAJE_IMPRESION"
            });

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new LenguajeImpresionQuery();
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> {
                "DS_LENGUAJE_IMPRESION"
            });

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new LenguajeImpresionQuery();
            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new LenguajeImpresionQuery();

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

            return this._gridValidationService.Validate(new MantenimientoLenguajeImpresionGridValidationModule(uow), grid, row, context);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                if (grid.Rows.Any())
                {
                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {
                        var idLenguajeImpresion = row.GetCell("CD_LENGUAJE_IMPRESION").Value;
                        var lenguajeImpresion = uow.LenguajeImpresionRepository.GetLenguajeImpresion(idLenguajeImpresion);

                        if (lenguajeImpresion == null && !row.IsNew)
                            throw new ValidationFailedException("COF010_msg_Error_LenguajeImpresionNoExiste", [idLenguajeImpresion]);

                        if (row.IsNew)
                        {

                            lenguajeImpresion = CrearLenguajeImpresion(uow, row);
                            uow.LenguajeImpresionRepository.AddLenguajeImpresion(lenguajeImpresion);
                        }
                        else if (row.IsDeleted)
                        {
                            uow.LenguajeImpresionRepository.DeleteLenguajeImpresion(idLenguajeImpresion);
                        }
                        else
                        {
                            UpdateLenguajeImpresion(uow, row, lenguajeImpresion);
                            uow.LenguajeImpresionRepository.UpdateLenguajeImpresion(lenguajeImpresion);
                        }
                    }

                }

                uow.SaveChanges();

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "COF010GridCommit");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        #region Metodos Auxiliares

        public virtual LenguajeImpresion CrearLenguajeImpresion(IUnitOfWork uow, GridRow row)
        {
            return new LenguajeImpresion()
            {
                Id = row.GetCell("CD_LENGUAJE_IMPRESION").Value,
                Descripcion = row.GetCell("DS_LENGUAJE_IMPRESION").Value
            };
        }

        public virtual LenguajeImpresion UpdateLenguajeImpresion(IUnitOfWork uow, GridRow row, LenguajeImpresion lenguajeImpresion)
        {
            lenguajeImpresion.Descripcion = row.GetCell("DS_LENGUAJE_IMPRESION").Value;
            return lenguajeImpresion;
        }

        #endregion
    }
}
