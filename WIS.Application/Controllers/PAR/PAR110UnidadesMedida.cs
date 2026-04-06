using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Parametrizacion;
using WIS.Domain.General;
using WIS.Domain.Parametrizacion;
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

namespace WIS.Application.Controllers.PAR
{
    public class PAR110UnidadesMedida : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<PAR110UnidadesMedida> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PAR110UnidadesMedida(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<PAR110UnidadesMedida> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "CD_UNIDADE_MEDIDA",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_UNIDADE_MEDIDA", SortDirection.Ascending),
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
                "CD_UNIDADE_MEDIDA",
                "DS_UNIDADE_MEDIDA",
                "FG_ACEITA_DECIMAL",
                "CD_UNIDAD_MEDIDA_EXTERNA"
            });

            return this.GridFetchRows(grid, query.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            UnidadesMedidaQuery dbQuery = new UnidadesMedidaQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> {
                "DS_UNIDADE_MEDIDA",
                "FG_ACEITA_DECIMAL",
                "CD_UNIDAD_MEDIDA_EXTERNA"
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
                    RegistroModificacionUnidadesMedida registroModificacionUM = new RegistroModificacionUnidadesMedida(uow, this._identity.UserId, this._identity.Application);

                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {

                        if (row.IsNew)
                        {
                            UnidadMedida lenguajeImpresion = this.CrearUnidadMedida(uow, row, query);
                            registroModificacionUM.RegistrarUnidadesMedida(lenguajeImpresion);
                        }
                        else if (row.IsDeleted)
                        {
                            // rows delete
                            this.DeleteUnidadMedida(uow, row, query);
                        }
                        else
                        {
                            // rows editadas
                            UnidadMedida lenguajeImpresion = this.UpdateUnidadMedida(uow, row, query);
                            registroModificacionUM.ModificarUnidadesMedida(lenguajeImpresion);
                        }
                    }
                }

                uow.SaveChanges();

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "PAR110GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoUnidadesMedidaGridValidationModule(uow), grid, row, context);
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            UnidadesMedidaQuery dbQuery = new UnidadesMedidaQuery();
            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            UnidadesMedidaQuery dbQuery = new UnidadesMedidaQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public virtual UnidadMedida CrearUnidadMedida(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            UnidadMedida nuevaUnidadMedida = new UnidadMedida();

            nuevaUnidadMedida.Id = row.GetCell("CD_UNIDADE_MEDIDA").Value;
            nuevaUnidadMedida.Descripcion = row.GetCell("DS_UNIDADE_MEDIDA").Value;
            nuevaUnidadMedida.IdExterno = row.GetCell("CD_UNIDAD_MEDIDA_EXTERNA").Value;
            nuevaUnidadMedida.FechaInsercion = DateTime.Now;
            nuevaUnidadMedida.FechaModificacion = DateTime.Now;

            string aceptaDecimal = row.GetCell("FG_ACEITA_DECIMAL").Value.ToUpper();

            if (aceptaDecimal == "S")
                nuevaUnidadMedida.AceptaDecimal = true;
            else if (aceptaDecimal == "N")
                nuevaUnidadMedida.AceptaDecimal = false;

            return nuevaUnidadMedida;
        }
        public virtual UnidadMedida UpdateUnidadMedida(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            string idUnidadMedida = row.GetCell("CD_UNIDADE_MEDIDA").Value;

            UnidadMedida unidadMedida = new UnidadMedida();
            unidadMedida = uow.UnidadMedidaRepository.GetById(idUnidadMedida);

            unidadMedida.Descripcion = row.GetCell("DS_UNIDADE_MEDIDA").Value;
            unidadMedida.IdExterno = row.GetCell("CD_UNIDAD_MEDIDA_EXTERNA").Value;
            unidadMedida.FechaModificacion = DateTime.Now;

            string aceptaDecimal = row.GetCell("FG_ACEITA_DECIMAL").Value.ToUpper();

            if (aceptaDecimal == "S")
                unidadMedida.AceptaDecimal = true;
            else if (aceptaDecimal == "N")
                unidadMedida.AceptaDecimal = false;

            return unidadMedida;
        }
        public virtual void DeleteUnidadMedida(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            string idUnidadMedida = row.GetCell("CD_UNIDADE_MEDIDA").Value;

            if (uow.UnidadMedidaRepository.ExisteUnidadMedida(idUnidadMedida))
            {
                uow.UnidadMedidaRepository.DeleteUnidadMedida(idUnidadMedida);
            }
            else
            {
                throw new EntityNotFoundException("PAR110_Sec0_Error_Er001_UnidadMedidaNoExisteEliminar");
            }
        }
    }
}
