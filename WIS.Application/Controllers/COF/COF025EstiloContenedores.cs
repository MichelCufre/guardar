using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Impresion;
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

namespace WIS.Application.Controllers.COF
{
    public class COF025EstiloContenedores : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<COF025EstiloContenedores> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public COF025EstiloContenedores(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<COF025EstiloContenedores> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "CD_LABEL_ESTILO", "TP_CONTENEDOR"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_LABEL_ESTILO", SortDirection.Ascending),
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
            query.IsRemoveEnabled = true;

            grid.SetInsertableColumns(new List<string> {
                "CD_LABEL_ESTILO",
                "TP_CONTENEDOR"
            });

            using var uow = this._uowFactory.GetUnitOfWork();
            grid.AddOrUpdateColumn(new GridColumnSelect("CD_LABEL_ESTILO", this.SelectCodigoEstiloContenedores(uow)));
            grid.AddOrUpdateColumn(new GridColumnSelect("TP_CONTENEDOR", this.SelectTipoEstiloContenedores(uow)));

            return this.GridFetchRows(grid, query.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            EstilosEtiquetaContenedoresQuery dbQuery = new EstilosEtiquetaContenedoresQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            return grid;
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

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {
                        string idEstiloContenedor = row.GetCell("CD_LABEL_ESTILO").Value;
                        string tipoContenedor = row.GetCell("TP_CONTENEDOR").Value;

                        if (row.IsNew)
                        {

                            uow.EstiloContenedorRepository.AddTipoContenedor(idEstiloContenedor, tipoContenedor);
                        }
                        else if (row.IsDeleted)
                        {
                            // rows delete
                            uow.EstiloContenedorRepository.DeleteTipoContenedor(idEstiloContenedor, tipoContenedor);
                        }
                    }

                }
                uow.SaveChanges();

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "COF020GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoEstiloContenedoresGridValidationModule(uow), grid, row, context);
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            EstilosEtiquetaContenedoresQuery dbQuery = new EstilosEtiquetaContenedoresQuery();
            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            EstilosEtiquetaContenedoresQuery dbQuery = new EstilosEtiquetaContenedoresQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public virtual List<SelectOption> SelectCodigoEstiloContenedores(IUnitOfWork uow)
        {
            return uow.EstiloEtiquetaRepository.GetEtiquetaEstilos(EstiloEtiquetaDb.Contenedor)
                .Select(w => new SelectOption(w.Id, w.Id + " - " + w.Descripcion + " - " + w.Tipo))
                .ToList();
        }
        public virtual List<SelectOption> SelectTipoEstiloContenedores(IUnitOfWork uow)
        {
            return uow.ContenedorRepository.GetTiposContenedores()
                .Select(w => new SelectOption(w.Id, w.Id + " - " + w.Descripcion + " - " + (w.ClientePredefinido ? "S" : "N")))
                .ToList();
        }
    }
}
