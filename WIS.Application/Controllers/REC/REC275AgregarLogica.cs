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
    public class REC275AgregarLogica : AppController
    {

        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<REC275AgregarLogica> _logger;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IRecepcionService _recepcionService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REC275AgregarLogica(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            ISecurityService security,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            ILogger<REC275AgregarLogica> logger,
            IGridValidationService gridValidationService,
            IRecepcionService recepcionService)
        {
            this.GridKeys = new List<string>
            {
                "NM_ALM_PARAMETRO",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_ALM_PARAMETRO", SortDirection.Ascending),
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
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            short numeroLogica = -1;
            if (!string.IsNullOrEmpty(query.GetParameter("numeroLogica")))
            {
                numeroLogica = short.Parse(query.GetParameter("numeroLogica"));
            }

            ParametrosQuery dbQuery = new ParametrosQuery(numeroLogica);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            return grid;
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            short numeroLogica = -1;
            if (!string.IsNullOrEmpty(query.GetParameter("numeroLogica")))
            {
                numeroLogica = short.Parse(query.GetParameter("numeroLogica"));
            }

            if (numeroLogica != -1)
            {
                ParametrosQuery dbQuery = new ParametrosQuery(numeroLogica);
                uow.HandleQuery(dbQuery);
                return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
            }
            else
            {
                ParametrosQuery dbQuery = new ParametrosQuery();
                uow.HandleQuery(dbQuery);
                return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
            }
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (query.Parameters.Any(x => x.Id == "numeroLogica"))
            {
                string numeroLogica = query.Parameters.FirstOrDefault(s => s.Id == "numeroLogica").Value;

                if (!string.IsNullOrEmpty(numeroLogica))
                {
                    ParametrosQuery dbQuery = new ParametrosQuery(short.Parse(numeroLogica));

                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                    return new GridStats
                    {
                        Count = dbQuery.GetCount()
                    };
                }
                else
                {
                    ParametrosQuery dbQuery = new ParametrosQuery();

                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                    return new GridStats
                    {
                        Count = 0
                    };
                }
            }

            return null;
        }
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            List<GridRow> rowsEntrada = JsonConvert.DeserializeObject<List<GridRow>>(context.GetParameter("rowsEntrada"));
            List<ComponentParameter> parameters = this.ParametrosDeGrilla(rowsEntrada);

            return this._gridValidationService.Validate(new MantenimientoAgregarLogicaGridValidationModule(uow, this._identity, parameters), grid, row, context);
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
                if (context.Parameters.Any(x => x.Id == "numeroEstrategia"))
                {
                    string codigoEstrategia = context.Parameters.FirstOrDefault(s => s.Id == "numeroEstrategia").Value;

                    form.GetField("codigoEstrategia").Value = codigoEstrategia;
                    var estrategia = uow.EstrategiaRepository.GetEstrategiaByCod(codigoEstrategia);
                    if (estrategia != null)
                        form.GetField("descripcionEstrategia").Value = estrategia.Descripcion;
                }

                this.InicializarSelect(uow, form);
            }

            return form;
        }
        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            MantenimientoPanelDeEstrategias mantenimientoEstrategias = new MantenimientoPanelDeEstrategias(uow, this._identity.UserId, this._identity.Application);
            List<GridRow> rowsEntrada = JsonConvert.DeserializeObject<List<GridRow>>(context.GetParameter("rowsEntrada"));

            try
            {
                string codigoEstrategia = form.GetField("codigoEstrategia").Value;
                string codigoLogica = form.GetField("logica").Value;
                string descripcion = form.GetField("descripcionProceso").Value;
                string ascendente = form.GetField("ordenDeUbicaciones").Value;

                InstanciaLogica instanciaLogica = this.CreateInstancia(codigoEstrategia, codigoLogica, descripcion, ascendente, uow);
                mantenimientoEstrategias.RegistrarInstanciaLogica(instanciaLogica);

                uow.SaveChanges();

                List<InstanciaLogicaParametro> lineasParametros = this.MapFormulaEntrada(rowsEntrada, instanciaLogica.Id, short.Parse(codigoLogica));
                List<ParametroDefault> parametrosDefault = uow.EstrategiaRepository.GetAllParametrosByCodLogica(codigoLogica);
                List<InstanciaLogicaParametro> parametrosDefaultMapeados = this.MapParametrosDefaultToInstancia(parametrosDefault, instanciaLogica.Id, short.Parse(codigoLogica));

                foreach (InstanciaLogicaParametro parametro in parametrosDefaultMapeados)
                {
                    if (!lineasParametros.Any(x => x.Parametro.Id == parametro.Parametro.Id))
                    {
                        mantenimientoEstrategias.RegistrarInstanciaParametro(parametro);
                    }
                    else
                    {
                        InstanciaLogicaParametro instanciaParametro = lineasParametros.Where(x => x.Parametro.Id == parametro.Parametro.Id).FirstOrDefault();
                        mantenimientoEstrategias.RegistrarInstanciaParametro(instanciaParametro);
                    }
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
        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new REC275AgregarLogicaFormValidationModule(uow, this._identity), form, context);
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

        #region Aux
        public virtual void InicializarSelect(IUnitOfWork uow, Form form)
        {
            FormField selectorLogica = form.GetField("logica");

            selectorLogica.Options = new List<SelectOption>();

            List<Logica> logicas = uow.EstrategiaRepository.GetAllLogicas();
            foreach (var logica in logicas)
            {
                selectorLogica.Options.Add(new SelectOption(logica.NumeroLogica.ToString(), $"{logica.NumeroLogica} - {logica.Descripcion}"));
            }
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
        public virtual List<ComponentParameter> ParametrosDeGrilla(List<GridRow> rowsEntrada)
        {
            List<ComponentParameter> parameters = new List<ComponentParameter>();

            foreach (var row in rowsEntrada)
            {
                string valor = row.GetCell("VL_ALM_PARAMETRO_DEFAULT").Value;

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
        public virtual List<SelectOption> SearchValorParametro(Form form, FormSelectSearchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            return SearchValorParametro(uow, form, context.SearchValue);
        }
        public virtual List<SelectOption> SearchValorParametro(IUnitOfWork uow, Form form, string searchValue = null)
        {
            FormField codigoParametro = form.GetField("codigoParametro");
            return this._recepcionService.GetAllValues(uow, codigoParametro.Value, searchValue, this._identity.UserId);
        }

        public virtual List<InstanciaLogicaParametro> MapFormulaEntrada(List<GridRow> rowsEntrada, int numeroLogicaInstancia, short numeroLogica)
        {
            var lineasEntrada = new List<InstanciaLogicaParametro>();

            foreach (var row in rowsEntrada)
            {
                lineasEntrada.Add(new InstanciaLogicaParametro
                {
                    Instancia = numeroLogicaInstancia,
                    Logica = numeroLogica,
                    Parametro = new LogicaParametro
                    {
                        Id = short.Parse(row.GetCell("NU_ALM_PARAMETRO").Value)
                    },
                    Valor = row.GetCell("VL_ALM_PARAMETRO_DEFAULT").Value.Trim().ToUpper(),
                    FechaRegistro = DateTime.Now,
                });
            }

            return lineasEntrada;
        }
        public virtual List<InstanciaLogicaParametro> MapParametrosDefaultToInstancia(List<ParametroDefault> rowsEntrada, int numeroLogicaInstancia, short numeroLogica)
        {
            var lineasParametrosDefault = new List<InstanciaLogicaParametro>();

            foreach (var row in rowsEntrada)
            {
                lineasParametrosDefault.Add(new InstanciaLogicaParametro
                {
                    Instancia = numeroLogicaInstancia,
                    Logica = numeroLogica,
                    Parametro = new LogicaParametro
                    {
                        Id = row.NumeroParametro
                    },
                    Valor = row.Valor,
                    FechaRegistro = DateTime.Now,
                });
            }

            return lineasParametrosDefault;
        }
        public virtual InstanciaLogica CreateInstancia(string codigoEstrategia, string codigoLogica, string Descripcion, string ascendente, IUnitOfWork uow)
        {
            InstanciaLogica logica = new InstanciaLogica();

            //Obtenemos las instancias de las estrategias para obtener el numero de orden que se le va a dar
            List<InstanciaLogica> listaInstancias = uow.EstrategiaRepository.GetAllInstanciaByCodEstrategia(codigoEstrategia);

            logica.Descripcion = Descripcion;
            logica.Estrategia = int.Parse(codigoEstrategia);
            logica.FechaRegistro = DateTime.Now;
            logica.Logica = short.Parse(codigoLogica);
            logica.Orden = Convert.ToInt16(listaInstancias.Count() + 1);

            if (ascendente == "false")
                logica.OrdenarAscendente = "N";
            else
                logica.OrdenarAscendente = "S";

            return logica;
        }

        #endregion
    }
}
