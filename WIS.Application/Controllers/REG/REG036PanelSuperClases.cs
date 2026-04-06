using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Security;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Build.Configuration;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Sorting;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Excel;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Domain.General;
using WIS.Exceptions;
using Microsoft.Extensions.Logging;
using WIS.Application.Validation;
using WIS.Filtering;

namespace WIS.Application.Controllers.REG
{
    public class REG036PanelSuperClases : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ILogger<REG036PanelSuperClases> _logger;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REG036PanelSuperClases(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            ILogger<REG036PanelSuperClases> logger,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "CD_SUB_CLASSE"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("DT_UPDROW", SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._logger = logger;
            this._identity = identity;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsAddEnabled = true;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = false;

            grid.SetInsertableColumns(new List<string>
            {
                "CD_SUB_CLASSE",
                "DS_SUB_CLASSE"
            });

            return this.GridFetchRows(grid, query.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new UbicacionSuperClaseQuery();
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, DefaultSort, this.GridKeys);

            foreach (var row in grid.Rows)
            {
                row.SetEditableCells(new List<string> { "DS_SUB_CLASSE" });
            }

            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new UbicacionSuperClaseQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new UbicacionSuperClaseQuery();
            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";
            return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, DefaultSort);
        }
        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                if (grid.Rows.Any())
                {

                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {
                        var codigo = row.GetCell("CD_SUB_CLASSE").Value;
                        var descripcion = row.GetCell("DS_SUB_CLASSE").Value;

                        if (row.IsNew)
                        {
                            this.CrearSuperClase(uow, codigo, descripcion);
                        }
                        else if (row.IsDeleted)
                        {
                            throw new ValidationFailedException("General_Sec0_msg_DeleteNotImplemented");
                        }
                        else
                        {
                            this.EditarSuperClase(uow, codigo, descripcion);
                        }
                    }

                }

                uow.SaveChanges();
                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ValidationFailedException ex)
            {
                query.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "REG036GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoUbicacionSuperClaseGridValidationModule(uow), grid, row, context);
        }

        public virtual SuperClase CrearSuperClase(IUnitOfWork uow, string codigo, string descripcion)
        {
            SuperClase clase = new SuperClase()
            {
                Id = codigo.ToUpper(),
                Descripcion = descripcion.ToUpper(),
            };

            uow.ClaseRepository.AddSuperClase(clase);

            return clase;
        }

        public virtual SuperClase EditarSuperClase(IUnitOfWork uow, string codigo, string descripcion)
        {
            var clase = uow.ClaseRepository.GetSuperClaseById(codigo);

            if (clase == null)
                throw new ValidationFailedException("REG036_Frm1_Error_SuperClaseUbicacionNoExiste");

            clase.Descripcion = descripcion.ToUpper();

            uow.ClaseRepository.UpdateSuperClase(clase);

            return clase;
        }
    }
}