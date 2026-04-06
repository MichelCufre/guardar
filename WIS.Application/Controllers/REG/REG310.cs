using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.General;
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
using WIS.Sorting;

namespace WIS.Application.Controllers.REG
{
    public class REG310 : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly ILogger<REG310> _logger;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REG310(IUnitOfWorkFactory uowFactory, IIdentityService identity, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter, IGridValidationService gridValidationService, ILogger<REG310> logger)
        {
            this.GridKeys = new List<string>
            {
                "NU_GRUPO_REGLA"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_ORDEN", SortDirection.Ascending)
            };

            _uowFactory = uowFactory;
            _identity = identity;
            _gridService = gridService;
            _excelService = excelService;
            _filterInterpreter = filterInterpreter;
            _gridValidationService = gridValidationService;
            _logger = logger;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = true;
            query.IsAddEnabled = false;
            query.IsCommitEnabled = true;

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("btnEditar", "General_Sec0_btn_Editar", "fas fa-edit"),
            }));

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_UP", new List<GridButton>
            {
                new GridButton("btnUp", "General_Sec0_btn_Subir", "fas fa-arrow-up"),
            }));

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_DOWN", new List<GridButton>
            {
                new GridButton("btnDown", "General_Sec0_btn_Bajar", "fas fa-arrow-down"),
            }));
            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new GruposReglasQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, DefaultSort, GridKeys);

            var reglas = uow.GrupoRepository.GetReglasAgrupacion();
            foreach (var row in grid.Rows)
            {
                DisableButtons(uow, row, reglas, grid.Rows.Count());
            }
            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new GruposReglasQuery();
            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";
            return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new GruposReglasQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            try
            {
                if (grid.Rows.Any())
                {
                    foreach (var row in grid.Rows)
                    {
                        if (row.IsDeleted)
                        {
                            var nuRegla = long.Parse(row.GetCell("NU_GRUPO_REGLA").Value);
                            uow.GrupoRepository.EliminarRegla(nuRegla);
                        }
                    }
                }
                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ValidationFailedException ex)
            {
                this._logger.LogError(ex, "REG301Reglas_GridCommit");
                query.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "REG301Reglas_GridCommit");
                query.AddErrorNotification(ex.Message);
            }

            return grid;
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();
            try
            {
                GrupoRegla reglaSubir = null;
                GrupoRegla reglaBajar = null;

                int numeroOrden = -1;
                var cdGrupo = context.Row.GetCell("CD_GRUPO").Value;
                var nuRegla = long.Parse(context.Row.GetCell("NU_GRUPO_REGLA").Value);

                if (context.ButtonId == "btnUp")
                {
                    reglaSubir = uow.GrupoRepository.GetGrupoRegla(nuRegla);

                    if (reglaSubir == null)
                        throw new ValidationFailedException("REG300_msg_Error_GrupoReglaNoExiste", new string[] { nuRegla.ToString() });

                    numeroOrden = reglaSubir.Orden - 1;
                    reglaBajar = uow.GrupoRepository.GetGrupoReglaForChangeOrder(numeroOrden);
                }
                else if (context.ButtonId == "btnDown")
                {
                    reglaBajar = uow.GrupoRepository.GetGrupoRegla(nuRegla);
                    if (reglaBajar == null)
                        throw new ValidationFailedException("REG300_msg_Error_GrupoReglaNoExiste", new string[] { nuRegla.ToString() });

                    numeroOrden = reglaBajar.Orden + 1;
                    reglaSubir = uow.GrupoRepository.GetGrupoReglaForChangeOrder(numeroOrden);
                }

                if (reglaSubir != null && reglaBajar != null)
                {
                    reglaSubir.Orden--;
                    reglaBajar.Orden++;

                    uow.GrupoRepository.UpdateGrupoRegla(reglaSubir);
                    uow.GrupoRepository.UpdateGrupoRegla(reglaBajar);
                }

                uow.SaveChanges();
                uow.Commit();
            }
            catch (ValidationFailedException ex)
            {
                this._logger.LogError(ex, "REG301Reglas_GridButtonAction");
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                uow.Rollback();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "REG301Reglas_GridButtonAction");
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }
            return context;
        }

        public virtual void DisableButtons(IUnitOfWork uow, GridRow row, IEnumerable<GrupoRegla> reglas, int cantidadFilas)
        {
            var nuRegla = long.Parse(row.GetCell("NU_GRUPO_REGLA").Value);
            var regla = reglas.FirstOrDefault(r => r.Id == nuRegla);

            //Si es la primera desactivamos el boton
            if (regla.Orden == 1)
                row.DisabledButtons.Add("btnUp");

            //Si es la ultima desactivamos
            if (regla.Orden == cantidadFilas)
                row.DisabledButtons.Add("btnDown");
        }
    }
}
