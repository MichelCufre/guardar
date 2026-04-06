using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Automatizacion;
using WIS.Components.Common.Select;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Automatizacion;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.AUT
{
    public class AUT100PanelAutomatismo : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IAutomatismoFactory _automatismoFactory;

        public AUT100PanelAutomatismo(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ISecurityService security,
            IFormValidationService formValidationService,
            IAutomatismoFactory automatismoFactory)
        {
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._security = security;
            this._formValidationService = formValidationService;
            this._automatismoFactory = automatismoFactory;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            switch (form.Id)
            {
                case "AUT100_form_1":
                    form.GetField("automatismo").Options = this.SelectAutomatismo(uow);
                    break;

                case "AUT100_form_2":
                    form.GetField("tipoUbicacion").Options = this.SelectTipoUbicacion(uow);
                    break;

                default:
                    break;
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            return this._formValidationService.Validate(new PanelAutomatismoValidationModule(uow), form, context);
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            switch (grid.Id)
            {
                case "AUT100_grid_1":
                    this.GridFetchRowsStock(uow, grid, context);
                    break;
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            switch (grid.Id)
            {
                case "AUT100_grid_1":
                    return this.GridFetchStatsStock(grid, context);
            }

            return null;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            switch (grid.Id)
            {
                case "AUT100_grid_1":
                    return this.GridExportExcelStock(grid, context);
            }

            return null;
        }

        #region Metodos Auxiliares

        public virtual List<SelectOption> SelectAutomatismo(IUnitOfWork uow)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            AutomatismoQuery dbQuery = new AutomatismoQuery(_automatismoFactory);

            uow.HandleQuery(dbQuery);

            List<string> prediosUsuario = uow.PredioRepository.GetPredioUser(this._identity.UserId);

            List<IAutomatismo> automatismos = dbQuery.GetAutomatismos(this._identity.Predio, prediosUsuario);

            foreach (IAutomatismo automatismo in automatismos)
            {
                opciones.Add(new SelectOption(automatismo.ZonaUbicacion, automatismo.Codigo + " - " + automatismo.Descripcion));
            }

            return opciones;
        }

        public virtual List<SelectOption> SelectTipoUbicacion(IUnitOfWork uow)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            var dominios = uow.DominioRepository.GetDominios("TPAUTPOS");

            foreach (var dominio in dominios)
            {
                opciones.Add(new SelectOption(dominio.Id, dominio.Id + " - " + dominio.Descripcion));
            }

            return opciones;
        }

        public virtual Grid GridFetchRowsStock(IUnitOfWork uow, Grid grid, GridFetchContext context)
        {
            List<SortCommand> DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_ENDERECO",SortDirection.Ascending),
                new SortCommand("CD_PRODUTO",SortDirection.Ascending),
                new SortCommand("CD_FAIXA",SortDirection.Ascending),
                new SortCommand("CD_EMPRESA",SortDirection.Ascending),
                new SortCommand("NU_IDENTIFICADOR",SortDirection.Ascending)
            };

            List<string> GridKeys = new List<string> { "CD_ENDERECO", "CD_PRODUTO", "CD_FAIXA", "CD_EMPRESA", "NU_IDENTIFICADOR" };

            string tipoUbicacion = context.GetParameter("tipoUbicacion");

            StockAutomatismoQuery dbQuery = new StockAutomatismoQuery(tipoUbicacion);

            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, DefaultSort, GridKeys);

            return grid;
        }

        public virtual GridStats GridFetchStatsStock(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string tipoUbicacion = context.GetParameter("tipoUbicacion");

            StockAutomatismoQuery dbQuery = new StockAutomatismoQuery(tipoUbicacion);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public virtual byte[] GridExportExcelStock(Grid grid, GridExportExcelContext context)
        {
            List<SortCommand> DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_ENDERECO",SortDirection.Ascending),
                new SortCommand("CD_PRODUTO",SortDirection.Ascending),
                new SortCommand("CD_FAIXA",SortDirection.Ascending),
                new SortCommand("CD_EMPRESA",SortDirection.Ascending),
                new SortCommand("NU_IDENTIFICADOR",SortDirection.Ascending)
            };

            using var uow = this._uowFactory.GetUnitOfWork();

            string tipoUbicacion = context.GetParameter("tipoUbicacion");

            StockAutomatismoQuery dbQuery = new StockAutomatismoQuery(tipoUbicacion);

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application +"-"+ DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, DefaultSort);
        }

        #endregion
    }
}
