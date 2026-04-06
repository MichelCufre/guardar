using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Security;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Columns;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.GridComponent.Build.Configuration;
using WIS.Sorting;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.General;
using WIS.Exceptions;
using Microsoft.Extensions.Logging;
using WIS.Application.Validation;
using WIS.Filtering;

namespace WIS.Application.Controllers.REG
{
    public class REG035PanelClases : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ILogger<REG035PanelClases> _logger;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REG035PanelClases(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            ILogger<REG035PanelClases> logger,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "CD_CLASSE"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("DT_UPDROW", SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsAddEnabled = true;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = false;

            grid.AddOrUpdateColumn(new GridColumnSelect("CD_SUB_CLASSE", this.OptionSelectSuperClase()));

            grid.SetInsertableColumns(new List<string>
            {
                "CD_CLASSE",
                "DS_CLASSE",
                "CD_SUB_CLASSE"

            });

            return this.GridFetchRows(grid, query.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new UbicacionClaseQuery();
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, DefaultSort, this.GridKeys);

            foreach (var row in grid.Rows)
            {
                row.SetEditableCells(new List<string> { "DS_CLASSE", "CD_SUB_CLASSE" });
            }

            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var dbQuery = new UbicacionClaseQuery();

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

            var dbQuery = new UbicacionClaseQuery();
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
                        var codigo = row.GetCell("CD_CLASSE").Value;
                        var descripcion = row.GetCell("DS_CLASSE").Value;
                        var subClase = row.GetCell("CD_SUB_CLASSE").Value;

                        if (row.IsNew)
                        {
                            this.CrearClase(uow, codigo, descripcion, subClase);
                        }
                        else if (row.IsDeleted)
                        {
                            throw new OperationNotAllowedException("General_Sec0_msg_DeleteNotImplemented");
                        }
                        else
                        {
                            this.EditarClase(uow, codigo, descripcion, subClase);
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
                this._logger.LogError(ex, "REG035GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoUbicacionClaseGridValidationModule(uow), grid, row, context);
        }

        public virtual Clase CrearClase(IUnitOfWork uow, string codigo, string descripcion, string codigoSuperClase)
        {
            var clase = new Clase()
            {
                Id = codigo.ToUpper(),
                Descripcion = descripcion.ToUpper(),
                IdSuperClase = codigoSuperClase
            };

            uow.ClaseRepository.AddClase(clase);

            return clase;
        }
        public virtual Clase EditarClase(IUnitOfWork uow, string codigo, string descripcion, string codigoSuperClase)
        {
            var clase = uow.ClaseRepository.GetClaseById(codigo);

            if (clase == null)
                throw new ValidationFailedException("REG035_Frm1_Error_ClaseUbicacionNoExiste");

            clase.Descripcion = descripcion.ToUpper();
            clase.IdSuperClase = codigoSuperClase;

            uow.ClaseRepository.UpdateClase(clase);
            return clase;
        }

        public virtual List<SelectOption> OptionSelectSuperClase()
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<SuperClase> superClases = uow.ClaseRepository.GetSuperClases();

            foreach (var superClase in superClases)
            {
                opciones.Add(new SelectOption(superClase.Id, superClase.Descripcion));
            }

            return opciones;
        }
    }
}