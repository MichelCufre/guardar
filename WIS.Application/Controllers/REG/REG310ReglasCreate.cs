using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Application.Validation.Modules.GridModules.Registro;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
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
    public class REG310ReglasCreate : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<REG310ReglasCreate> _logger;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGrupoService _grupoService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }
        public REG310ReglasCreate(IUnitOfWorkFactory uowFactory, 
            IIdentityService identity, 
            IGridService gridService, 
            IGridExcelService gridExcelService, 
            IFormValidationService formValidationService, 
            IFilterInterpreter filterInterpreter, 
            ILogger<REG310ReglasCreate> logger, 
            IGridValidationService gridValidationService,
            IGrupoService grupoService)
        {
            this.GridKeys = new List<string>
            {
                "NU_PARAM",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_ORDEN", SortDirection.Ascending),
            };

            _uowFactory = uowFactory;
            _identity = identity;
            _gridService = gridService;
            _gridExcelService = gridExcelService;
            _formValidationService = formValidationService;
            _filterInterpreter = filterInterpreter;
            _logger = logger;
            _gridValidationService = gridValidationService;
            _grupoService = grupoService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            query.IsAddEnabled = false;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = false;
            query.IsCommitEnabled = false;

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("btnEditarParametro", "General_Sec0_btn_Editar", "fas fa-edit"),
            }));

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new GruposParametrosQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new GruposParametrosQuery();
            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new GruposParametrosQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            List<GridRow> rowsEntrada = JsonConvert.DeserializeObject<List<GridRow>>(context.GetParameter("rowsEntrada"));
            List<ComponentParameter> parameters = this.ParametrosDeGrilla(rowsEntrada);

            return this._gridValidationService.Validate(new GrupoParametroValidationModule(uow, parameters, _identity.GetFormatProvider()), grid, row, context);
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (form.Id == "REG310ReglasParametros_form_1")
            {
                form.GetField("codigoParametro").Value = context.GetParameter("codigoParametro");
                form.GetField("descripcionParametro").Value = context.GetParameter("descripcionParametro");
                form.GetField("tipoParametro").Value = context.GetParameter("tipoParametro");
                this.InicializarSelectParametro(uow, form, context);
            }
            else
            {
                form.GetField("nuRegla").Value = string.Empty;
                form.GetField("nuRegla").ReadOnly = true;
                form.GetField("descripcion").Value = string.Empty;
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                var cdGrupo = form.GetField("grupo").Value;
                var descripcion = form.GetField("descripcion").Value;
                var rowsEntrada = JsonConvert.DeserializeObject<List<GridRow>>(context.GetParameter("rowsEntrada"));

                var regla = CrearRegla(uow, cdGrupo, descripcion);
                CrearParametros(uow, rowsEntrada, regla.Id);

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
            }
            catch (ValidationFailedException ex)
            {
                this._logger.LogError(ex, ex.Message);
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                uow.Rollback();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            return this._formValidationService.Validate(new REG310ReglaFormValidationModule(uow, this._identity), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "valorParametro":
                    return this.SearchValorParametro(form, context);
                case "grupo":
                    return this.SearchGrupo(form, context);
                default:
                    return new List<SelectOption>();
            }
        }

        #region Metodos Auxiliares

        public virtual List<ComponentParameter> ParametrosDeGrilla(List<GridRow> rowsEntrada)
        {
            List<ComponentParameter> parameters = new List<ComponentParameter>();

            foreach (var row in rowsEntrada)
            {
                string valor = row.GetCell("VL_PARAM").Value;

                if (!string.IsNullOrEmpty(valor))
                {
                    string id = row.GetCell("NM_PARAM").Value;
                    ComponentParameter genericParam = new ComponentParameter()
                    {
                        Id = id,
                        Value = valor
                    };

                    parameters.Add(genericParam);
                }
            }

            return parameters;
        }

        public virtual void InicializarSelectParametro(IUnitOfWork uow, Form form, FormInitializeContext context)
        {
            var tipoParametro = form.GetField("tipoParametro").Value;
            var codigoParametro = form.GetField("codigoParametro").Value;
            form.GetField("valorParametro").Options = SearchValorParametro(uow, codigoParametro, tipoParametro);
        }

        public virtual List<SelectOption> SearchValorParametro(Form form, FormSelectSearchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var tipoParametro = form.GetField("tipoParametro").Value;
            var codigoParametro = form.GetField("codigoParametro").Value;
            return SearchValorParametro(uow, codigoParametro, tipoParametro, context.SearchValue);
        }

        public virtual List<SelectOption> SearchGrupo(Form form, FormSelectSearchContext FormQuery)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Grupo> grupos = uow.GrupoRepository.GetGrupoByNameOrCodePartial(FormQuery.SearchValue);

            foreach (var grupo in grupos)
            {
                opciones.Add(new SelectOption(grupo.Id.ToString(), $"{grupo.Id} - {grupo.Descripcion}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchValorParametro(IUnitOfWork uow, string codigoParametro, string tipoParametro, string searchValue = null)
        {
            return _grupoService.GetOptionsByParam(uow, codigoParametro, tipoParametro, searchValue, this._identity.UserId);
        }

        public virtual GrupoRegla CrearRegla(IUnitOfWork uow, string cdGrupo, string descripcion)
        {
            var orderUltimaRegla = uow.GrupoRepository.GetOrdenUltimaRegla();

            var regla = new GrupoRegla()
            {
                Descripcion = descripcion,
                CodigoGrupo = cdGrupo,
                Orden = orderUltimaRegla + 1,
                FechaInsercion = DateTime.Now,
            };

            uow.GrupoRepository.AddGrupoRegla(regla);
            return regla;
        }

        public virtual void CrearParametros(IUnitOfWork uow, List<GridRow> rowsEntrada, long nuRegla)
        {
            var parametrosDefaultRegla = uow.GrupoRepository.GetDefaultParamRegla(nuRegla);
            var parametros = rowsEntrada.Select(row => new GrupoReglaParametro()
            {
                NroRegla = nuRegla,
                NroParametro = int.Parse(row.GetCell("NU_PARAM").Value),
                Valor = row.GetCell("VL_PARAM").Value?.ToUpper(),
                FechaInsercion = DateTime.Now
            }).ToList() ?? new List<GrupoReglaParametro>();

            foreach (var p in parametrosDefaultRegla)
            {
                var param = parametros.FirstOrDefault(x => x.NroParametro == p.NroParametro);
                if (param != null)
                    uow.GrupoRepository.AddGrupoReglaParametro(param);
                else
                    uow.GrupoRepository.AddGrupoReglaParametro(p);
            }
        }

        #endregion
    }
}
