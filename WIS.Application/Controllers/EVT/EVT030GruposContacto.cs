using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Expedicion;
using WIS.Domain.Eventos;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.EVT
{
    public class EVT030GruposContacto : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<EVT030GruposContacto> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        public EVT030GruposContacto(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            IFormValidationService formValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ILogger<EVT030GruposContacto> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "NU_CONTACTO_GRUPO",
            };

            this._uowFactory = uowFactory;
            this._session = session;
            this._identity = identity;
            this._formValidationService = formValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsRemoveEnabled = false;
            context.IsAddEnabled = true;
            context.IsEditingEnabled = true;

            grid.SetInsertableColumns(new List<string> {
                "NM_GRUPO"
            });

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton> {
                new GridButton("btnEditar", "General_Sec0_btn_Editar", "far fa-edit")
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            SortCommand defaultSort = new SortCommand("NU_CONTACTO_GRUPO", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            GruposContactos dbQuery = new GruposContactos();

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            SortCommand defaultSort = new SortCommand("NU_CONTACTO_GRUPO", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            GruposContactos dbQuery = null;

            dbQuery = new GruposContactos();

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            GruposContactos dbQuery = new GruposContactos();

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
            uow.CreateTransactionNumber("EVT030 Crear Grupo");
            var nuTransaccion = uow.GetTransactionNumber();

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

                        if (row.IsNew)
                        {
                            GrupoContacto grupo = this.CrearGrupo(uow, row, context);
                            grupo.NumeroTransaccion = nuTransaccion;

                            uow.DestinatarioRepository.AddGrupo(grupo);

                            uow.SaveChanges();
                        }
                    }
                }

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "EVT030GridCommit");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
                uow.Rollback();
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new EVT030GridValidationModule(uow), grid, row, context);
        }

        #region Metodos Auxiliares

        protected virtual GrupoContacto CrearGrupo(IUnitOfWork uow, GridRow row, GridFetchContext context)
        {
            GrupoContacto nuevoGrupo = new GrupoContacto();

            nuevoGrupo.Id = uow.DestinatarioRepository.GetNextNuGrupo();
            nuevoGrupo.Nombre = row.GetCell("NM_GRUPO").Value;
            nuevoGrupo.FechaAlta = DateTime.Now;

            return nuevoGrupo;
        }
        
        #endregion

    }
}
