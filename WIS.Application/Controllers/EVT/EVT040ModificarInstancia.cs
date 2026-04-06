using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Expedicion;
using WIS.Domain.Eventos;
using WIS.Exceptions;
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
using WIS.Session;
using WIS.Sorting;


namespace WIS.Application.Controllers.EVT
{
    public class EVT040ModificarInstancia : AppController
    {
        protected readonly ISessionAccessor _session;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<EVT040ModificarInstancia> _logger;

        public EVT040ModificarInstancia(
            ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ILogger<EVT040ModificarInstancia> logger)
        {
            this._session = session;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._gridValidationService = gridValidationService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._formValidationService = formValidationService;
            this._logger = logger;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsRemoveEnabled = false;
            context.IsAddEnabled = false;
            context.IsEditingEnabled = true;
            context.IsCommitEnabled = false;

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var defaultSort = new SortCommand("CD_EVENTO_PARAMETRO", SortDirection.Descending);
            var tipoNotificacion = context.GetParameter("tipoNotificacion");

            if (int.TryParse(context.GetParameter("evento"), out int numeroEvento) && int.TryParse(context.GetParameter("instancia"), out int numeroInstancia))
            {
                var dbQuery = new ParametrosInstanciasEventosQuery(tipoNotificacion, numeroEvento, numeroInstancia);

                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, new List<string> { "CD_EVENTO_PARAMETRO" });

                grid.SetEditableCells(new List<string> { "VL_PARAMETRO" });
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var defaultSort = new SortCommand("CD_EVENTO_PARAMETRO", SortDirection.Descending);
            var dbQuery = new InstanciasEventosQuery();

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var defaultSort = new SortCommand("CD_EVENTO_PARAMETRO", SortDirection.Descending);
            var tipoNotificacion = context.GetParameter("tipoNotificacion");

            if (!int.TryParse(context.GetParameter("evento"), out int numeroEvento) || !int.TryParse(context.GetParameter("instancia"), out int numeroInstancia))
                return null;

            var dbQuery = new ParametrosInstanciasEventosQuery(tipoNotificacion, numeroEvento, numeroInstancia);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var empresa = context.GetParameter("empresa");
            var tipoAgente = context.GetParameter("tipoAgente");

            return this._gridValidationService.Validate(new EVT040ModificarInstanciaGridValidationModule(uow, empresa, tipoAgente, this._session), grid, row, context);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("EVT040 Modificar Instancia");
            var nuTransaccion = uow.GetTransactionNumber();

            try
            {
                Instancia objInstancia;

                if (int.TryParse(context.GetParameter("instancia"), out int nuInstancia))
                {
                    objInstancia = uow.EventoRepository.GetInstancia(nuInstancia);

                    if (objInstancia == null)
                        throw new MissingParameterException("EVT040_Sec0_Error_InstanciaNoExiste");
                }
                else
                {
                    throw new MissingParameterException("General_Sec0_Error_Error80");
                }

                objInstancia.NumeroEvento = int.Parse(context.GetParameter("evento"));
                objInstancia.Descripcion = context.GetParameter("descripcion");
                objInstancia.EsHabilitado = bool.Parse(context.GetParameter("habilitado"));
                objInstancia.IdTipoNotificacion = context.GetParameter("tipoNotificacion");
                objInstancia.Plantilla = context.GetParameter("plantilla");
                objInstancia.FechaModificacion = DateTime.Now;
                objInstancia.NumeroTransaccion = nuTransaccion;

                uow.EventoRepository.UpdateInstancia(objInstancia);

                grid.Rows.ForEach(row =>
                {
                    var cdEventoParametro = row.GetCell("CD_EVENTO_PARAMETRO").Value;
                    var vlParametro = row.GetCell("VL_PARAMETRO").Value;
                    var isRequired = row.GetCell("FL_REQUERIDO").Value == "S";

                    if (isRequired && string.IsNullOrEmpty(vlParametro))
                        throw new ValidationFailedException("EVT040_Sec0_Error_ParametroRequerido", new string[] { cdEventoParametro });

                    var obj = uow.EventoRepository.GetParametroInstancia(objInstancia.NumeroEvento, cdEventoParametro, nuInstancia, objInstancia.TipoNotificacion.ToString());

                    if (obj == null)
                    {
                        obj = new ParametroInstancia()
                        {
                            Codigo = cdEventoParametro,
                            NumeroInstancia = nuInstancia,
                            Valor = vlParametro,
                            TipoNotificacion = objInstancia.TipoNotificacion,
                            NuEvento = objInstancia.NumeroEvento,
                            FechaAlta = DateTime.Now,
                            NumeroTransaccion = nuTransaccion,
                        };

                        uow.EventoRepository.AddParametroInstancia(obj);
                    }
                    else
                    {
                        obj.Valor = vlParametro;
                        obj.FechaModificacion = DateTime.Now;
                        obj.NumeroTransaccion = nuTransaccion;

                        uow.EventoRepository.UpdateParametroInstancia(obj);
                    }
                });

                uow.SaveChanges();
                uow.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                uow.Rollback();
                throw ex;
            }

            return grid;
        }


        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuInstancia = int.Parse(context.GetParameter("instancia"));
            var instancia = uow.EventoRepository.GetInstancia(nuInstancia);

            this.InicializarSelects(uow, form, instancia);

            if (instancia != null)
            {
                form.GetField("evento").Value = instancia.NumeroEvento.ToString();
                form.GetField("instancia").Value = nuInstancia.ToString();
                form.GetField("descripcion").Value = instancia.Descripcion;
                form.GetField("habilitado").Value = instancia.EsHabilitado.ToString().ToLower();
                form.GetField("tipoNotificacion").Value = instancia.IdTipoNotificacion;
                form.GetField("plantilla").Value = instancia.Plantilla;
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            if (!form.IsValid())
            {
                context.AddInfoNotification("General_Form_Error_NoValido");
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new EVT040ModificarInstanciaFormValidationModule(uow), form, context);
        }

        #region Metodos Auxiliares

        protected virtual void InicializarSelects(IUnitOfWork uow, Form form, Instancia instancia)
        {
            FormField selectEvento = form.GetField("evento");
            selectEvento.Options = new List<SelectOption>();

            FormField selectTipoNotificacion = form.GetField("tipoNotificacion");
            selectTipoNotificacion.Options = new List<SelectOption>();

            FormField selectPlantilla = form.GetField("plantilla");
            selectPlantilla.Options = new List<SelectOption>();

            uow.EventoRepository.GetEventos()?.ForEach(w =>
            {
                selectEvento.Options.Add(new SelectOption(w.Id.ToString(), w.Nombre));
            });

            uow.DominioRepository.GetDominios(CodigoDominioDb.EventoTipoNotificacion)?.ForEach(w =>
            {
                selectTipoNotificacion.Options.Add(new SelectOption(w.Valor, w.Descripcion));
            });

            uow.EventoRepository.GeEventoTemplatoByEventoTipoNotificacion(instancia.NumeroEvento, instancia.TipoNotificacion.ToString())?.ForEach(w =>
            {
                selectPlantilla.Options.Add(new SelectOption(w.CdEstilo, w.CdEstilo + " " + w.dsEstilo));
            });
        }

        #endregion
    }
}

