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
using WIS.Domain.Impresiones;
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
    public class COF020EstiloEtiqueta : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<COF020EstiloEtiqueta> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public COF020EstiloEtiqueta(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<COF020EstiloEtiqueta> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "CD_LABEL_ESTILO",
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

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsAddEnabled = true;
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = true;

            grid.SetInsertableColumns(new List<string> {
                "CD_LABEL_ESTILO",
                "DS_LABEL_ESTILO",
                "TP_LABEL"
            });

            using var uow = this._uowFactory.GetUnitOfWork();

            grid.AddOrUpdateColumn(new GridColumnSelect("TP_LABEL", this.SelectTipoEstiloEtiqueta(uow)));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new EstilosEtiquetaQuery();
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> {
                "DS_LABEL_ESTILO",
                "TP_LABEL",
                "FL_HABILITADO"
            });

            grid.AddOrUpdateColumn(new GridColumnSelect("TP_LABEL", this.SelectTipoEstiloEtiqueta(uow)));

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new EstilosEtiquetaQuery();
            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new EstilosEtiquetaQuery();
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

            return this._gridValidationService.Validate(new MantenimientoEstiloEtiquetaGridValidationModule(uow), grid, row, context);
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
                        var idEstiloEtiqueta = row.GetCell("CD_LABEL_ESTILO").Value;
                        var etiquetaEstilo = uow.EstiloEtiquetaRepository.GetEtiquetaEstilo(idEstiloEtiqueta);

                        if (etiquetaEstilo == null && !row.IsNew)
                            throw new ValidationFailedException("COF020_msg_Error_EstiloEtiquetaNoExiste", [idEstiloEtiqueta]);

                        if (row.IsNew)
                        {
                            etiquetaEstilo = CrearEstiloEtiqueta(uow, row);
                            uow.EstiloEtiquetaRepository.AddEtiquetaEstilo(etiquetaEstilo);
                        }
                        else if (row.IsDeleted)
                        {
                            uow.EstiloEtiquetaRepository.DeleteEtiquetaEstilo(idEstiloEtiqueta);
                        }
                        else
                        {
                            UpdateEstiloEtiqueta(uow, row, etiquetaEstilo);
                            uow.EstiloEtiquetaRepository.UpdateEstiloEtiqueta(etiquetaEstilo);
                        }
                    }

                }

                uow.SaveChanges();

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "COF020GridCommit");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        #region Metodos Auxiliares
        public virtual EtiquetaEstilo CrearEstiloEtiqueta(IUnitOfWork uow, GridRow row)
        {
            return new EtiquetaEstilo
            {
                Id = row.GetCell("CD_LABEL_ESTILO").Value,
                Descripcion = row.GetCell("DS_LABEL_ESTILO").Value,
                Tipo = row.GetCell("TP_LABEL").Value.Remove(0, 4),
                Habilitado = "S"
            };
        }

        public virtual EtiquetaEstilo UpdateEstiloEtiqueta(IUnitOfWork uow, GridRow row, EtiquetaEstilo etiquetaEstilo)
        {
            etiquetaEstilo.Descripcion = row.GetCell("DS_LABEL_ESTILO").Value;
            etiquetaEstilo.Habilitado = row.GetCell("FL_HABILITADO").Value;

            var dominio = row.GetCell("TP_LABEL").Value;

            if (dominio.Length <= 3)
                etiquetaEstilo.Tipo = dominio;
            else
                etiquetaEstilo.Tipo = dominio.Remove(0, 4);

            return etiquetaEstilo;
        }

        public virtual List<SelectOption> SelectTipoEstiloEtiqueta(IUnitOfWork uow)
        {
            return uow.DominioRepository.GetDominios(CodigoDominioDb.TipoEtiquetas)
                .Select(w => new SelectOption(w.Id, w.Valor + " - " + w.Descripcion))
                .ToList();
        }

        #endregion
    }
}
