using DocumentFormat.OpenXml.InkML;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Recepcion;
using WIS.Application.Validation.Modules.GridModules.Recepcion;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
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
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.REC
{
    public class REC275ModificarLogica : AppController
    {

        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<REC275ModificarLogica> _logger;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IRecepcionService _recepcionService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REC275ModificarLogica(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            ISecurityService security,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            ILogger<REC275ModificarLogica> logger,
            IGridValidationService gridValidationService,
            IRecepcionService recepcionService)
        {
            this.GridKeys = new List<string>
            {
                "NU_ALM_LOGICA_INSTANCIA_PARAM",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_ALM_LOGICA_INSTANCIA_PARAM", SortDirection.Ascending),
            };

            this._uowFactory = uowFactory;
            this._session = session;
            this._identity = identity;
            this._security = security;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._formValidationService = formValidationService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
            this._recepcionService = recepcionService;
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
                new GridButton("btnEditarParametro", "REC275_frm1_btn_MODIFICAR", "fas fa-edit"),
            }));

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (form.Id == "REC275EditarParametro_form_1")
            {
                form.GetField("codigoParametro").Value = context.Parameters.FirstOrDefault(s => s.Id == "codigoParametro")?.Value;
                form.GetField("descripcionParametro").Value = context.Parameters.FirstOrDefault(s => s.Id == "descripcionParametro")?.Value;
                form.GetField("valorParametro").Value = context.Parameters.FirstOrDefault(s => s.Id == "valorParametro")?.Value;
                form.GetField("logica").Value = context.Parameters.FirstOrDefault(s => s.Id == "logica")?.Value;

                this.InicializarSelectParametro(uow, form);
            }
            else
            {
                var cdEstrategia = context.Parameters.FirstOrDefault(s => s.Id == "codigoEstrategia")?.Value;

                form.GetField("codigoEstrategia").Value = cdEstrategia;
                var estrategia = uow.EstrategiaRepository.GetEstrategiaByCod(cdEstrategia);
                if (estrategia != null)
                    form.GetField("descripcionEstrategia").Value = estrategia.Descripcion;
                form.GetField("codigoInstancia").Value = context.Parameters.FirstOrDefault(s => s.Id == "codigoInstancia")?.Value;
                form.GetField("descripcionProceso").Value = context.Parameters.FirstOrDefault(s => s.Id == "descripcionProceso")?.Value;

                string codigoLogica = context.Parameters.FirstOrDefault(s => s.Id == "logica")?.Value;
                if (!string.IsNullOrEmpty(codigoLogica))
                {
                    Logica logica = uow.EstrategiaRepository.GetLogicaByCod(short.Parse(codigoLogica));
                    form.GetField("logica").Value = $"{logica.NumeroLogica} - {logica.Descripcion}";
                }

                string ordenAscendente = context.Parameters.FirstOrDefault(s => s.Id == "ordenAscendente")?.Value;

                if (!string.IsNullOrEmpty(ordenAscendente))
                {
                    if (ordenAscendente == "N")
                        form.GetField("ordenDeUbicaciones").Value = "false";
                    else
                        form.GetField("ordenDeUbicaciones").Value = "true";
                }
            }

            return form;
        }

        public virtual void InicializarSelectParametro(IUnitOfWork uow, Form form)
        {
            FormField selectorValorParametro = form.GetField("valorParametro");
            string codigoParametro = form.GetField("codigoParametro").Value;
            selectorValorParametro.Options = new List<SelectOption>();

            List<SelectOption> valores = SearchValorParametro(uow, form, selectorValorParametro.Value);

            if (codigoParametro == "GENERAR_PARCIAL" || codigoParametro == "UBIC_BAJAS_ALTAS" ||
                 codigoParametro == "UBIC_MULTIPRODUCTO" || codigoParametro == "UBIC_MULTILOTE" ||
                 codigoParametro == "RESPETA_FIFO" || codigoParametro == "PRODUCTOS_COINCIDENTES" ||
                 codigoParametro == "LOTES_COINCIDENTES" || codigoParametro == "UBIC_INICIO" ||
                 codigoParametro == "MODALIDAD_REABASTECIMIENTO" || codigoParametro == "RESPETA_LOTE" ||
                 codigoParametro == "IGNORAR_VENCIMIENTO_STOCK" || codigoParametro == "RESPETA_VENCIMIENTO" ||
                 codigoParametro == "TIPO_PICKING")
            {
                foreach (var valor in valores)
                {
                    selectorValorParametro.Options.Add(new SelectOption(valor.Value, valor.Label));
                }
            }
            else
            {
                foreach (var valor in valores)
                {
                    selectorValorParametro.Options.Add(new SelectOption(valor.Value, $"{valor.Value} - {valor.Label}"));
                }
            }

        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "valorParametro":
                    return this.SearchValorParametro(form, context);

                default:
                    return new List<SelectOption>();
            }
        }

        public virtual List<SelectOption> SearchValorParametro(Form form, FormSelectSearchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            return SearchValorParametro(uow, form, context.SearchValue);
        }

        public virtual List<SelectOption> SearchValorParametro(IUnitOfWork uow, Form form, string searchValue = null)
        {
            var codigoParametro = form.GetField("codigoParametro");
            return this._recepcionService.GetAllValues(uow, codigoParametro.Value, searchValue, this._identity.UserId);
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new REC275ModificarLogicaFormValidationModule(uow, this._identity), form, context);
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            List<GridRow> rowsEntrada = JsonConvert.DeserializeObject<List<GridRow>>(context.GetParameter("rowsEntrada"));
            List<ComponentParameter> parameters = this.ParametrosDeGrilla(rowsEntrada);

            return this._gridValidationService.Validate(new MantenimientoModificarLogicaGridValidationModule(uow, this._identity, parameters), grid, row, context);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int numeroInstancia = -1;
            if (!string.IsNullOrEmpty(query.GetParameter("codigoInstancia")))
            {
                numeroInstancia = int.Parse(query.GetParameter("codigoInstancia"));
            }

            InstanciasParametrosQuery dbQuery = new InstanciasParametrosQuery();

            if (numeroInstancia != -1)
            {
                dbQuery = new InstanciasParametrosQuery(numeroInstancia);
            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            MantenimientoPanelDeEstrategias mantenimientoEstrategias = new MantenimientoPanelDeEstrategias(uow, this._identity.UserId, this._identity.Application);
            List<GridRow> rowsEntrada = JsonConvert.DeserializeObject<List<GridRow>>(context.GetParameter("rowsEntrada"));

            try
            {
                string codigoInstancia = form.GetField("codigoInstancia").Value;
                string codigoLogica = form.GetField("logica").Value;
                string descripcion = form.GetField("descripcionProceso").Value;
                string ascendente = form.GetField("ordenDeUbicaciones").Value;

                codigoLogica = codigoLogica.Substring(0, codigoLogica.IndexOf("-")).Trim();

                var instancia = uow.EstrategiaRepository.GetInstanciaByCod(codigoInstancia);

                instancia.Descripcion = descripcion;
                instancia.FechaModificacion = DateTime.Now;

                if (ascendente == "false")
                    instancia.OrdenarAscendente = "N";
                else
                    instancia.OrdenarAscendente = "S";

                mantenimientoEstrategias.UpdateInstanciaLogica(instancia);

                uow.SaveChanges();

                var lineasParametros = this.MapFormulaEntrada(uow, rowsEntrada, instancia.Id, short.Parse(codigoLogica));

                foreach (InstanciaLogicaParametro parametro in lineasParametros)
                {
                    mantenimientoEstrategias.UpdateInstanciaParametro(parametro);
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw ex;
            }

            return form;
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int numeroInstancia = -1;
            if (!string.IsNullOrEmpty(query.GetParameter("codigoInstancia")))
            {
                numeroInstancia = int.Parse(query.GetParameter("codigoInstancia"));
            }

            if (numeroInstancia != -1)
            {
                InstanciasParametrosQuery dbQuery = new InstanciasParametrosQuery(numeroInstancia);
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

                foreach (var row in grid.Rows)
                {
                    if (!string.IsNullOrEmpty(row.GetCell("VL_ALM_PARAMETRO").Value))
                    {
                        var valorParametro = row.GetCell("VL_ALM_PARAMETRO");
                        valorParametro.ForceSetOldValue("-1");
                        valorParametro.Modified = true;
                    }
                }
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            int numeroInstancia = -1;
            if (!string.IsNullOrEmpty(query.GetParameter("codigoInstancia")))
            {
                numeroInstancia = int.Parse(query.GetParameter("codigoInstancia"));
            }

            if (numeroInstancia != -1)
            {
                InstanciasParametrosQuery dbQuery = new InstanciasParametrosQuery(numeroInstancia);
                uow.HandleQuery(dbQuery);
                return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
            }
            else
            {
                InstanciasParametrosQuery dbQuery = new InstanciasParametrosQuery();
                uow.HandleQuery(dbQuery);
                return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
            }
        }

        public virtual List<InstanciaLogicaParametro> MapFormulaEntrada(IUnitOfWork uow, List<GridRow> rowsEntrada, int numeroLogicaInstancia, short numeroLogica)
        {
            var lineasEntrada = new List<InstanciaLogicaParametro>();

            foreach (var row in rowsEntrada)
            {
                var codigoInstanciaParametro = int.Parse(row.GetCell("NU_ALM_LOGICA_INSTANCIA_PARAM").Value);
                InstanciaLogicaParametro instanciaParametro = uow.EstrategiaRepository.GetInstanciaParametroByCod(codigoInstanciaParametro);
                instanciaParametro.FechaModificacion = DateTime.Now;
                instanciaParametro.Valor = row.GetCell("VL_ALM_PARAMETRO").Value.Trim().ToUpper();

                lineasEntrada.Add(instanciaParametro);
            }

            return lineasEntrada;
        }

        public virtual List<ComponentParameter> ParametrosDeGrilla(List<GridRow> rowsEntrada)
        {
            List<ComponentParameter> parameters = new List<ComponentParameter>();

            foreach (var row in rowsEntrada)
            {
                string valor = row.GetCell("VL_ALM_PARAMETRO").Value;

                if (!string.IsNullOrEmpty(valor))
                {
                    string id = row.GetCell("NM_ALM_PARAMETRO").Value;
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
    }
}
