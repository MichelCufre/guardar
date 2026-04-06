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
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General.Enums;
using WIS.Domain.Liberacion;
using WIS.Exceptions;
using WIS.GridComponent.Columns;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.General;
using WIS.Domain.DataModel.Mappers.Constants;
using Microsoft.Extensions.Logging;
using WIS.Application.Validation;
using WIS.Filtering;

namespace WIS.Application.Controllers.REG
{
    public class REG140PanelOndasPreparacion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ILogger<REG140PanelOndasPreparacion> _logger;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        public REG140PanelOndasPreparacion(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            ILogger<REG140PanelOndasPreparacion> logger,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "CD_ONDA"
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
            query.IsRemoveEnabled = false;

            grid.AddOrUpdateColumn(new GridColumnSelect("NU_PREDIO", this.OptionSelectPredio()));

            grid.SetInsertableColumns(new List<string>
            {
                "CD_ONDA",
                "DS_ONDA",
                "NU_PREDIO",
            });

            return this.GridFetchRows(grid, query.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new OndasDePreparacionQuery();

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("CD_ONDA", SortDirection.Ascending);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, defaultSort, this.GridKeys);

            foreach (var row in grid.Rows)
            {
                row.SetEditableCells(new List<string>
                {
                     "DS_ONDA",
                     "ACTIVO",
                     "NU_PREDIO",
                });
            }

            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new OndasDePreparacionQuery();

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

            var dbQuery = new OndasDePreparacionQuery();

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("CD_ONDA", SortDirection.Ascending);

            query.FileName = $"{this._identity.Application}{DateTime.Now:yyyy-MM-dd_HH:mm}.xlsx";

            return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, defaultSort);
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

                    if (grid.HasDuplicates(new List<string>() { "DS_ONDA" }))
                        throw new ValidationFailedException("REG140_Grid_Error_SuperClaseUbicacionNoExiste");

                    OndaMapper ondaMapper = new OndaMapper();

                    foreach (var row in grid.Rows)
                    {
                        if (row.IsNew)
                        {
                            this.CrearOnda(uow, row);
                        }
                        else if (row.IsDeleted)
                        {
                            throw new OperationNotAllowedException("General_Sec0_msg_DeleteNotImplemented");
                        }
                        else
                        {
                            // rows editadas
                            this.EditarOnda(uow, ondaMapper, row);

                        }
                    }

                }

                uow.SaveChanges();

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ExpectedException ex)
            {
                this._logger.LogWarning(ex, "GridCommit");
                query.AddErrorNotification(ex.Message);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "REG140GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoOndasDePreparacionValidationModule(uow, this._identity.UserId), grid, row, context);
        }

        public virtual Onda CrearOnda(IUnitOfWork uow, GridRow row)
        {
            var id = row.GetCell("CD_ONDA").Value;
            var descripcion = row.GetCell("DS_ONDA").Value;
            var predio = row.GetCell("NU_PREDIO").Value;

            Onda onda = new Onda()
            {
                Id = short.Parse(id),
                Descripcion = descripcion.ToUpper(),
                Estado = SituacionDb.Activo,
                Predio = !string.IsNullOrEmpty(predio) ? predio : null,
            };

            uow.OndaRepository.AddOnda(onda);

            return onda;
        }
        public virtual Onda EditarOnda(IUnitOfWork uow, OndaMapper ondaMapper, GridRow row)
        {
            Onda onda = null;

            if (short.TryParse(row.GetCell("CD_ONDA").Value, out short idOnda))
                onda = uow.OndaRepository.GetOnda(idOnda);

            if (onda == null)
                throw new EntityNotFoundException("REG140_Grid_Error_OndaNoExiste", new string[] { row.GetCell("CD_ONDA").Value });

            onda.Descripcion = row.GetCell("DS_ONDA").Value.ToUpper();

            bool activo = ondaMapper.MapStringToBoolean(row.GetCell("ACTIVO").Value);
            if (ondaMapper.MapEstadoBooleanToShort(activo) != onda.Estado)
            {
                if (activo)
                    onda.Enable();
                else
                    onda.Disable();
            }

            var predio = row.GetCell("NU_PREDIO").Value;
            onda.Predio = !string.IsNullOrEmpty(predio) ? predio : null;

            uow.OndaRepository.UpdateOnda(onda);

            return onda;
        }

        public virtual List<SelectOption> OptionSelectPredio()
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new GetPrediosUsuarioQuery();
            uow.HandleQuery(dbQuery);

            List<Predio> predios = dbQuery.GetPrediosUsuario(_identity.UserId);
            foreach (var predio in predios)
            {
                opciones.Add(new SelectOption(predio.Numero, $"{predio.Numero} - { predio.Descripcion}")); ;
            }

            return opciones;
        }

    }
}