using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Inventario;
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

namespace WIS.Application.Controllers.INV
{
    public class INV060AsignacionMotivoAjusteStock : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public INV060AsignacionMotivoAjusteStock(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridValidationService gridValidationService,
            IGridService gridService, IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_AJUSTE_STOCK"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_AJUSTE_STOCK", SortDirection.Ascending)
            };

            this._uowFactory = uowFactory;
            this._gridValidationService = gridValidationService;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsAddEnabled = false;
            context.IsRemoveEnabled = false;

            grid.AddOrUpdateColumn(new GridColumnSelect("CD_MOTIVO_AJUSTE", this.OptionSelectMotivo()));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new INV060GridQuery();
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> { "CD_MOTIVO_AJUSTE", "DS_MOTIVO" });

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new INV060GridQuery();
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

            grid.Rows.ForEach(w =>
            {
                var ajuste = uow.AjusteRepository.GetAjusteStock(int.Parse(w.GetCell("NU_AJUSTE_STOCK").Value));

                var dsMotivo = w.GetCell("DS_MOTIVO").Value;
                var cdMotivoAjuste = w.GetCell("CD_MOTIVO_AJUSTE").Value;

                ajuste.CdMotivoAjuste = cdMotivoAjuste;
                ajuste.DescMotivo = dsMotivo;
                ajuste.FechaModificacion = DateTime.Now;

                uow.AjusteRepository.UpdateAjusteStock(ajuste);

                uow.SaveChanges();

            });

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new INV060GridQuery();
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, DefaultSort);
        }

        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            switch (context.ColumnId)
            {
                case "CD_EMPRESA":
                    return this.SearchEmpresa(grid, row, context);
            }

            return new List<SelectOption>();
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new INV060GridValidationModule(uow), grid, row, context);
        }

        #region Metodos Auxiliares

        public virtual List<SelectOption> SearchEmpresa(Grid grid, GridRow row, GridSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var empresas = uow.EmpresaRepository.GetByNombreOrCodePartial(context.SearchValue);

            foreach (var empresa in empresas)
            {
                opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} -  {empresa.Nombre}"));
            }

            return opciones;
        }
        public virtual List<SelectOption> OptionSelectMotivo()
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var motivos = uow.AjusteRepository.GetsMotivosAjuste();

            foreach (var motivo in motivos)
            {
                opciones.Add(new SelectOption(motivo.Codigo, $"{motivo.Codigo} - {motivo.Descripcion}"));
            }

            return opciones;
        }

        #endregion
    }
}
