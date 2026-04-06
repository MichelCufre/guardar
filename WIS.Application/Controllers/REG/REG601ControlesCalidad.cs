using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Security;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Build.Configuration;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Sorting;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Excel;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Exceptions;
using WIS.Domain.General;
using Microsoft.Extensions.Logging;
using WIS.Application.Validation;
using WIS.Filtering;

namespace WIS.Application.Controllers.REG
{
    public class REG601ControlesCalidad : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ILogger<REG601ControlesCalidad> _logger;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected ControlDeCalidadMapper _mapper = new ControlDeCalidadMapper();

        protected List<string> GridKeys { get; }

        public REG601ControlesCalidad(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            ILogger<REG601ControlesCalidad> logger,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "CD_CONTROL"
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsAddEnabled = true;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = true;
            query.IsCommitEnabled = true;

            grid.SetInsertableColumns(new List<string>
            {
                "CD_CONTROL",
                "DS_CONTROL",
                "SG_CONTROL",
                "ID_BLOQUEIO",
            });

            return this.GridFetchRows(grid, query.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ControlesDeCalidadQuery();

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("CD_CONTROL", SortDirection.Ascending);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

            foreach (var row in grid.Rows)
            {
                row.SetEditableCells(new List<string> { "DS_CONTROL", "SG_CONTROL", "ID_BLOQUEIO", });
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ControlesDeCalidadQuery();

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("CD_CONTROL", SortDirection.Ascending);

            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ControlesDeCalidadQuery();
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

                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {
                        var codigo = row.GetCell("CD_CONTROL").Value;
                        var descripcion = row.GetCell("DS_CONTROL").Value;
                        var sigla = row.GetCell("SG_CONTROL").Value;
                        var bloqueo = row.GetCell("ID_BLOQUEIO").Value;

                        if (row.IsNew)
                            this.CrearControlDeCalidadClase(uow, codigo, descripcion, sigla, bloqueo);
                        else if (row.IsDeleted)
                            this.EliminarControlDeCalidadClase(uow, codigo);
                        else
                            this.EditarControlDeCalidadClase(uow, codigo, descripcion, sigla, bloqueo);
                    }
                }

                uow.SaveChanges();

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ValidationFailedException ex)
            {
                this._logger.LogWarning(ex, "GridCommit");
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "REG601GridCommit");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoControlDeCalidadClaseGridValidationModule(uow), grid, row, context);
        }

        #region Metodos auxiliares

        public virtual ControlDeCalidad CrearControlDeCalidadClase(IUnitOfWork uow, string codigo, string descripcion, string sigla, string bloqueo)
        {
            var clase = new ControlDeCalidad()
            {
                Id = int.Parse(codigo),
                Descripcion = descripcion.ToUpper(),
                Sigla = sigla.ToUpper(),
                EsBloqueante = _mapper.MapStringToBoolean(bloqueo),
            };

            uow.ControlDeCalidadRepository.AddTipoControlDeCalidad(clase);

            return clase;
        }

        public virtual ControlDeCalidad EditarControlDeCalidadClase(IUnitOfWork uow, string codigo, string descripcion, string sigla, string bloqueo)
        {
            var clase = uow.ControlDeCalidadRepository.GetTipoControlDeCalidad(Int32.Parse(codigo));

            if (clase == null)
                throw new ValidationFailedException("REG601_Sec0_Error_Er001_TPCtrlCalidadNoExiste");

            clase.Descripcion = descripcion.ToUpper();
            clase.Sigla = sigla.ToUpper();
            clase.EsBloqueante = _mapper.MapStringToBoolean(bloqueo);

            uow.ControlDeCalidadRepository.UpdateControlDeCalidadClase(clase);
            return clase;
        }

        public virtual void EliminarControlDeCalidadClase(IUnitOfWork uow, string codigo)
        {
            var clase = uow.ControlDeCalidadRepository.GetTipoControlDeCalidad(int.Parse(codigo));

            if (clase == null)
                throw new ValidationFailedException("REG601_Sec0_Error_Er001_TPCtrlCalidadNoExiste");

            if (uow.ControlDeCalidadRepository.QuedaControlPendiente(int.Parse(codigo)))
                throw new ValidationFailedException("REG601_Sec0_Error_Er002_ExisteCtrlCalidadAsociado");

            uow.ControlDeCalidadRepository.RemoveControlDeCalidadClase(clase);
        }

        #endregion
    }
}