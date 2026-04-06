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
    public class PAR604TipoCodigoBarra : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<PAR604TipoCodigoBarra> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PAR604TipoCodigoBarra(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<PAR604TipoCodigoBarra> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "TP_CODIGO_BARRAS",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("TP_CODIGO_BARRAS", SortDirection.Ascending),
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
                "TP_CODIGO_BARRAS",
                "DS_CODIGO_BARRAS"
            });

            return this.GridFetchRows(grid, query.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            TipoCodigoBarraQuery dbQuery = new TipoCodigoBarraQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> {
                "DS_CODIGO_BARRAS"
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
                    RegistroModificacionTipoCodigoBarra registroModificacionTCB = new RegistroModificacionTipoCodigoBarra(uow, this._identity.UserId, this._identity.Application);

                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {

                        if (row.IsNew)
                        {
                            ProductoCodigoBarraTipo tipoCodigoBarra = this.CrearTipoCodigoBarra(uow, row, query);
                            registroModificacionTCB.RegistrarTipoCodigoBarra(tipoCodigoBarra);
                        }
                        else if (row.IsDeleted)
                        {
                            // rows delete
                            this.DeleteTipoCodigoBarra(uow, row, query);
                        }
                        else
                        {
                            // rows editadas
                            ProductoCodigoBarraTipo tipoCodigoBarra = this.UpdateTipoCodigoBarra(uow, row, query);
                            registroModificacionTCB.ModificarTipoCodigoBarra(tipoCodigoBarra);
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

            return this._gridValidationService.Validate(new MantenimientoTipoCodigoBarraGridValidationModule(uow), grid, row, context);
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            TipoCodigoBarraQuery dbQuery = new TipoCodigoBarraQuery();
            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            TipoCodigoBarraQuery dbQuery = new TipoCodigoBarraQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public virtual ProductoCodigoBarraTipo CrearTipoCodigoBarra(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            ProductoCodigoBarraTipo nuevaTipoCodigoBarra = new ProductoCodigoBarraTipo();

            nuevaTipoCodigoBarra.Id = int.Parse(row.GetCell("TP_CODIGO_BARRAS").Value);
            nuevaTipoCodigoBarra.Descripcion = row.GetCell("DS_CODIGO_BARRAS").Value;
            nuevaTipoCodigoBarra.FechaInsercion = DateTime.Now;
            nuevaTipoCodigoBarra.FechaModificacion = DateTime.Now;

            return nuevaTipoCodigoBarra;
        }
        public virtual ProductoCodigoBarraTipo UpdateTipoCodigoBarra(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            int idTipoCodigoBarra = int.Parse(row.GetCell("TP_CODIGO_BARRAS").Value);

            ProductoCodigoBarraTipo tipoCodigoBarra = new ProductoCodigoBarraTipo();
            tipoCodigoBarra = uow.ProductoCodigoBarraRepository.GetProductoCodigoBarraTipo(idTipoCodigoBarra);

            tipoCodigoBarra.Descripcion = row.GetCell("DS_CODIGO_BARRAS").Value;
            tipoCodigoBarra.FechaModificacion = DateTime.Now;

            return tipoCodigoBarra;
        }
        public virtual void DeleteTipoCodigoBarra(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            int idTipoCodigoBarra = int.Parse(row.GetCell("TP_CODIGO_BARRAS").Value);

            if (uow.ProductoCodigoBarraRepository.ExisteTipoCodigoBarras(idTipoCodigoBarra))
            {
                uow.ProductoCodigoBarraRepository.DeleteTipoCodigoBarra(idTipoCodigoBarra);
            }
            else
            {
                throw new EntityNotFoundException("PAR604_Sec0_Error_Er001_TipoCodBarraNoExisteEliminar");
            }
        }
    }
}
