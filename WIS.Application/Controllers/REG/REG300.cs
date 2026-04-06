using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules.Registro;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Registro;
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

namespace WIS.Application.Controllers.REG
{
    public class REG300 : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REG300(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "CD_GRUPO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("DT_ADDROW", SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._gridValidationService = gridValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            query.IsAddEnabled = false;
            query.IsCommitEnabled = true;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = true;

            grid.AddOrUpdateColumn(new GridColumnSelect("CD_CLASSE", this.SelectClase(uow)));

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new GruposQuery();
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            var gruposModificables = uow.GrupoRepository.GetGruposModificables();

            foreach (var row in grid.Rows)
            {
                var camposEditables = new List<string> { "DS_GRUPO" };

                var cdGrupo = row.GetCell("CD_GRUPO").Value;
                if (gruposModificables.Contains(cdGrupo))
                    camposEditables.Add("CD_CLASSE");

                row.SetEditableCells(camposEditables);
            }

            return grid;
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new GruposQuery();
            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new GruposQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();
            try
            {
                bool inhabilitado = false;
                if (grid.Rows.Any())
                {
                    foreach (var row in grid.Rows)
                    {
                        var cdGrupo = row.GetCell("CD_GRUPO").Value;
                        var grupo = uow.GrupoRepository.GetGrupo(cdGrupo);

                        if (grupo == null)
                            throw new ValidationFailedException("REG300_msg_Error_GrupoNoExiste", new string[] { cdGrupo });

                        if (row.IsDeleted)
                        {
                            if (uow.GrupoRepository.GrupoEliminable(grupo.Id))
                                uow.GrupoRepository.RemoveGrupo(grupo);
                            else
                            {
                                if (grid.Rows.Count == 1)
                                    throw new ValidationFailedException("REG300_msg_Error_GrupoNoEliminable", new string[] { grupo.Id });
                                else
                                    inhabilitado = true;
                            }
                        }
                        else
                        {
                            grupo.Descripcion = row.GetCell("DS_GRUPO").Value;
                            grupo.CodigoClase = row.GetCell("CD_CLASSE").Value;
                            grupo.FechaModificacion = DateTime.Now;
                            uow.GrupoRepository.UpdateGrupo(grupo);
                        }
                    }
                }

                uow.SaveChanges();
                uow.Commit();

                if (inhabilitado)
                    context.AddInfoNotification("REG300_msg_Error_RemoveParcial");
                else
                    context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                uow.Rollback();
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }
            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            return this._gridValidationService.Validate(new GruposValidationModule(uow), grid, row, context);
        }

        public virtual List<SelectOption> SelectClase(IUnitOfWork uow)
        {
            return uow.ClaseRepository.GetClases()
                .Select(c => new SelectOption(c.Id.ToString(), $"{c.Id} - { c.Descripcion}")).ToList();
        }
    }
}
