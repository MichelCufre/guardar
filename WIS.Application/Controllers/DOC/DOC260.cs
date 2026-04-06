using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Documento;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Documento;
using WIS.Domain.Documento;
using WIS.Domain.Documento.Constants;
using WIS.Domain.Documento.Integracion.Recepcion;
using WIS.Domain.ManejoStock;
using WIS.Domain.ManejoStock.AjusteStockDocumental;
using WIS.Domain.Recepcion;
using WIS.Domain.Recepcion.Enums;
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
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.DOC
{
    public class DOC260 : AppController
    {
        protected readonly ISessionAccessor _session;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IFactoryService _factoryService;
        protected readonly IDocumentoService _documentoService;
        protected readonly IParameterService _parameterService;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected List<string> GridKeys { get; }

        public DOC260(ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFormValidationService formValidationService,
            IGridValidationService gridValidationService,
            IFilterInterpreter filterInterpreter,
            IDocumentoService documentoService,
            IFactoryService factoryService,
            IParameterService parameterService)
        {
            this._session = session;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._formValidationService = formValidationService;
            this._gridValidationService = gridValidationService;
            this._filterInterpreter = filterInterpreter;
            this._documentoService = documentoService;
            this._factoryService = factoryService;
            this._parameterService = parameterService;

            this.GridKeys = new List<string>
            {
                "NU_DOCUMENTO",
                "TP_DOCUMENTO"
            };
        }

        #region FORM

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            if (context.Parameters != null && context.Parameters.Any(p => p.Id == "NU_DOCUMENTO") && context.Parameters.Any(p => p.Id == "TP_DOCUMENTO"))
            {
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    uow.BeginTransaction();

                    var TP_DOCUMENTO = context.Parameters.FirstOrDefault(p => p.Id == "TP_DOCUMENTO").Value;
                    var tpDoc = uow.DocumentoTipoRepository.GetTipoDocumento(TP_DOCUMENTO);

                    switch (tpDoc.TipoOperacion)
                    {
                        case TipoDocumentoOperacion.INGRESO:
                            this.InitializeIngreso(uow, form, context);
                            break;

                        case TipoDocumentoOperacion.EGRESO:
                            this.InitializeEgreso(uow, form, context);
                            break;

                        case TipoDocumentoOperacion.MODIFICACION:
                            this.InitializeActa(uow, form, context);
                            break;
                    }
                }
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            try
            {
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    uow.CreateTransactionNumber(this._identity.Application);
                    uow.BeginTransaction();

                    var tpDocumento = context.Parameters.FirstOrDefault(p => p.Id == "TP_DOCUMENTO").Value;
                    var tipoDocumento = uow.DocumentoTipoRepository.GetTipoDocumento(tpDocumento);

                    switch (tipoDocumento.TipoOperacion)
                    {
                        case TipoDocumentoOperacion.INGRESO:
                            if (this.UpdateIngreso(uow, tipoDocumento, form, context))
                            {
                                context.AddSuccessNotification("DOC260_Sec0_Error_Error01");
                            }
                            break;

                        case TipoDocumentoOperacion.EGRESO:
                            if (this.UpdateEgreso(uow, tipoDocumento, form, context))
                            {
                                context.AddSuccessNotification("DOC260_Sec0_Error_Error02");
                            }
                            break;

                        case TipoDocumentoOperacion.MODIFICACION:
                            if (this.UpdateActa(uow, tipoDocumento, form, context))
                            {
                                context.AddSuccessNotification("DOC260_Sec0_Error_Error03");
                            }
                            break;

                        default:
                            context.AddErrorNotification("DOC260_Sec0_Error_Error04");
                            break;
                    }

                    uow.Commit();
                }
            }
            catch (ValidationFailedException ex)
            {
                this._logger.Error(ex, ex.Message);
                throw ex;
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, ex.Message);
                throw new ValidationFailedException("DOC260_Sec0_Error_Error05", new string[] { ex.Message });
            }

            return form;
        }

        public override Form FormButtonAction(Form form, FormButtonActionContext context)
        {
            return base.FormButtonAction(form, context);
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new DOC260FormValidationModule(uow, this._identity), form, context);
        }

        #endregion FORM

        #region GRID

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton> {
                new GridButton("btnEditarEstado", "Editar", "fas fa-pencil-alt")
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var query = new Doc260Query();

                uow.HandleQuery(query);

                var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);
                grid.Rows = this._gridService.GetRows(query, grid.Columns, context, defaultSort, this.GridKeys);
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var dbQuery = new Doc260Query();

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new Doc260Query();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        #endregion GRID

        #region AUX

        public virtual void InitializeIngreso(IUnitOfWork uow, Form form, FormInitializeContext context)
        {
            string nuDocumento = context.Parameters.FirstOrDefault(p => p.Id == "NU_DOCUMENTO").Value;
            string tpDocumento = context.Parameters.FirstOrDefault(p => p.Id == "TP_DOCUMENTO").Value;
            var documento = uow.DocumentoRepository.GetIngreso(nuDocumento, tpDocumento);

            if (documento != null)
            {
                FormField nroDoc = form.GetField("nroDoc");
                var tpDoc = form.GetField("tpDocumento");

                //Obtener estados para cambio
                List<DocumentoAccion> acciones = documento.GetEstadosHabilitadosParaCambio(uow);

                FormField selectEstados = form.GetField("nuevoEstadoDocumento");
                selectEstados.Options = new List<SelectOption>();
                acciones.ForEach(accion =>
                {
                    selectEstados.Options.Add(new SelectOption(accion.Destino.Id, accion.Destino.Id + " - " + accion.Destino.Descripcion));
                });

                nroDoc.Value = nuDocumento;
                tpDoc.Value = uow.DocumentoTipoRepository.GetTipoDocumento(tpDocumento).DescripcionTipoDocumento;

                //Cargar select tipos dua
                FormField selectTiposDua = form.GetField("tpDua");
                selectTiposDua.Options = new List<SelectOption>();

                List<TipoDua> tiposDua = uow.TipoDuaRepository.GetTiposDua(tpDocumento);

                foreach (var tipoDua in tiposDua)
                {
                    selectTiposDua.Options.Add(new SelectOption(tipoDua.Id, tipoDua.Descripcion));
                }

                //Cargar select tipos referencia externa
                FormField selectTiposRefExterna = form.GetField("tpRefExterna");
                selectTiposRefExterna.Options = new List<SelectOption>();

                List<TipoReferenciaExterna> tiposRefExterna = uow.TipoReferenciaExternaRepository.GetTiposReferenciaExterna(tpDocumento);

                foreach (var tipoRefExterna in tiposRefExterna)
                {
                    selectTiposRefExterna.Options.Add(new SelectOption(tipoRefExterna.Id, tipoRefExterna.Descripcion));
                }

                //Cargar select agendas
                FormField selectAgendas = form.GetField("nroAgenda");
                selectAgendas.Options = new List<SelectOption>();

                List<Agenda> agendas = uow.AgendaRepository.GetAgendasDocumentables(documento.Empresa ?? -1);

                foreach (var agenda in agendas)
                {
                    selectAgendas.Options.Add(new SelectOption(agenda.Id.ToString(), agenda.Id.ToString()));
                }

                this.AgregarParametro(context, "mostrarForm", "true");
            }
        }

        public virtual void InitializeEgreso(IUnitOfWork uow, Form form, FormInitializeContext context)
        {
            string nuDocumento = context.Parameters.FirstOrDefault(p => p.Id == "NU_DOCUMENTO").Value;
            string tpDocumento = context.Parameters.FirstOrDefault(p => p.Id == "TP_DOCUMENTO").Value;
            var documento = uow.DocumentoRepository.GetEgreso(nuDocumento, tpDocumento);

            if (documento != null)
            {
                FormField nroDoc = form.GetField("nroDoc");
                var tpDoc = form.GetField("tpDocumento");

                //Obtener estados para cambio
                List<DocumentoAccion> acciones = documento.GetEstadosHabilitadosParaCambio(uow);

                FormField selectEstados = form.GetField("nuevoEstadoDocumento");
                selectEstados.Options = new List<SelectOption>();
                acciones.ForEach(accion =>
                {
                    selectEstados.Options.Add(new SelectOption(accion.Destino.Id, accion.Destino.Descripcion));
                });

                nroDoc.Value = nuDocumento;
                tpDoc.Value = uow.DocumentoTipoRepository.GetTipoDocumento(tpDocumento).DescripcionTipoDocumento;

                //Cargar select tipos dua
                FormField selectTiposDua = form.GetField("tpDua");
                selectTiposDua.Options = new List<SelectOption>();

                List<TipoDua> tiposDua = uow.TipoDuaRepository.GetTiposDua(tpDocumento);

                foreach (var tipoDua in tiposDua)
                {
                    selectTiposDua.Options.Add(new SelectOption(tipoDua.Id, tipoDua.Descripcion));
                }

                selectTiposDua.Value = tiposDua.FirstOrDefault()?.Id;

                //Cargar select tipos referencia externa
                FormField selectTiposRefExterna = form.GetField("tpRefExterna");
                selectTiposRefExterna.Options = new List<SelectOption>();

                List<TipoReferenciaExterna> tiposRefExterna = uow.TipoReferenciaExternaRepository.GetTiposReferenciaExterna(tpDocumento);

                foreach (var tipoRefExterna in tiposRefExterna)
                {
                    selectTiposRefExterna.Options.Add(new SelectOption(tipoRefExterna.Id, tipoRefExterna.Descripcion));
                }

                this.AgregarParametro(context, "mostrarForm", "true");
            }
        }

        public virtual void InitializeActa(IUnitOfWork uow, Form form, FormInitializeContext context)
        {
            var nuDocumento = context.Parameters.FirstOrDefault(p => p.Id == "NU_DOCUMENTO").Value;
            var tpDocumento = context.Parameters.FirstOrDefault(p => p.Id == "TP_DOCUMENTO").Value;
            var documento = uow.DocumentoRepository.GetActa(nuDocumento, tpDocumento);

            if (documento != null)
            {
                FormField nroDoc = form.GetField("nroDoc");
                var tpDoc = form.GetField("tpDocumento");

                //Obtener estados para cambio
                List<DocumentoAccion> acciones = documento.GetEstadosHabilitadosParaCambio(uow);

                FormField selectEstados = form.GetField("nuevoEstadoDocumento");
                selectEstados.Options = new List<SelectOption>();
                acciones.ForEach(accion =>
                {
                    selectEstados.Options.Add(new SelectOption(accion.Destino.Id, accion.Destino.Descripcion));
                });

                nroDoc.Value = nuDocumento;
                tpDoc.Value = uow.DocumentoTipoRepository.GetTipoDocumento(tpDocumento).DescripcionTipoDocumento;

                //Cargar select tipos dua
                FormField selectTiposDua = form.GetField("tpDua");
                selectTiposDua.Options = new List<SelectOption>();

                List<TipoDua> tiposDua = uow.TipoDuaRepository.GetTiposDua(tpDocumento);

                foreach (var tipoDua in tiposDua)
                {
                    selectTiposDua.Options.Add(new SelectOption(tipoDua.Id, tipoDua.Descripcion));
                }

                //Cargar select tipos referencia externa
                FormField selectTiposRefExterna = form.GetField("tpRefExterna");
                selectTiposRefExterna.Options = new List<SelectOption>();

                List<TipoReferenciaExterna> tiposRefExterna = uow.TipoReferenciaExternaRepository.GetTiposReferenciaExterna(tpDocumento);

                foreach (var tipoRefExterna in tiposRefExterna)
                {
                    selectTiposRefExterna.Options.Add(new SelectOption(tipoRefExterna.Id, tipoRefExterna.Descripcion));
                }

                this.AgregarParametro(context, "mostrarForm", "true");
            }
        }

        public virtual bool UpdateIngreso(IUnitOfWork uow, DocumentoTipo tipoDocumento, Form form, FormSubmitContext context)
        {
            bool retorno = false;
            string nuDocumento = context.Parameters.FirstOrDefault(p => p.Id == "NU_DOCUMENTO").Value;
            string tpDocumento = context.Parameters.FirstOrDefault(p => p.Id == "TP_DOCUMENTO").Value;
            var documento = uow.DocumentoRepository.GetIngreso(nuDocumento, tpDocumento);
            string errorMsg = "";

            if (documento != null)
            {
                var estadoDestino = form.GetField("nuevoEstadoDocumento").Value;
                var estadoOrigen = documento.Estado;
                var accion = uow.DocumentoRepository.GetAccion(documento.Tipo, documento.Estado, estadoDestino);

                documento.Estado = estadoDestino;

                switch (accion.Codigo)
                {
                    case AccionDocumento.Editar:
                        documento.Editar();
                        retorno = true;
                        break;

                    case AccionDocumento.ConfirmarEdicion:
                        documento.ConfirmarEdicion();
                        retorno = true;
                        break;

                    case AccionDocumento.Cancelar:
                        documento.Cancelar();
                        retorno = true;
                        break;

                    case AccionDocumento.EnviarDocumento:
                        documento.EnviarDocumento();
                        retorno = true;
                        break;

                    case AccionDocumento.AprobarDocumento:
                        if (!this.AprobarDocumento(uow, documento, form, out errorMsg))
                            context.AddErrorNotification(errorMsg);
                        else
                            retorno = true;
                        break;

                    case AccionDocumento.IniciarVerificacion:
                        documento.IniciarVerificacion();
                        retorno = true;
                        break;

                    case AccionDocumento.Finalizar:
                        if (!this.FinalizarDocumentoIngresoAduanero(documento, uow, estadoOrigen, out errorMsg))
                            context.AddErrorNotification(errorMsg);
                        else
                            retorno = true;
                        break;

                    default:
                        context.AddErrorNotification("DOC260_Sec0_Error_EstadoDocNoCorresponde");
                        break;
                }

                SetCommonData(uow, form, documento);

                if (retorno)
                {
                    uow.DocumentoRepository.UpdateIngreso(documento, uow.GetTransactionNumber());
                    uow.SaveChanges();
                }
            }

            return retorno;
        }

        public virtual bool UpdateEgreso(IUnitOfWork uow, DocumentoTipo tipoDocumento, Form form, FormSubmitContext context)
        {
            var retorno = false;
            var nuDocumento = context.Parameters.FirstOrDefault(p => p.Id == "NU_DOCUMENTO").Value;
            var tpDocumento = context.Parameters.FirstOrDefault(p => p.Id == "TP_DOCUMENTO").Value;
            var documento = uow.DocumentoRepository.GetEgreso(nuDocumento, tpDocumento);

            if ((tpDocumento == "SI" || tpDocumento == "SE") && uow.DocumentoRepository.GetAnyCuentaCorrienteDocumento(nuDocumento, tpDocumento))
            {
                throw new ValidationFailedException("DOC260_Sec0_Error_ExistenDocumentosSinGestionar");
            }

            if (documento != null)
            {
                var estadoDestino = form.GetField("nuevoEstadoDocumento").Value;
                var accion = uow.DocumentoRepository.GetAccion(documento.Tipo, documento.Estado, estadoDestino);

                documento.Estado = estadoDestino;

                switch (accion.Codigo)
                {
                    case AccionDocumento.Editar:
                        documento.Editar();
                        retorno = true;
                        break;

                    case AccionDocumento.ConfirmarEdicion:
                        documento.ConfirmarEdicion();
                        retorno = true;
                        break;

                    case AccionDocumento.Cancelar:
                        documento.Cancelar();
                        retorno = true;
                        break;

                    case AccionDocumento.GenerarLineas:
                        documento.GenerarLineas();
                        retorno = true;
                        break;

                    case AccionDocumento.EnviarDocumento:
                        documento.EnviarDocumento();
                        retorno = true;
                        break;

                    case AccionDocumento.AprobarDocumento:

                        var tpDoc = uow.DocumentoTipoRepository.GetTipoDocumento(documento.Tipo);
                        if (tpDoc.ManejaCamion)
                            this._documentoService.HabilitarCargaYCierreCamion(uow, documento);

                        if (!this.AprobarDocumento(uow, documento, form, out string errorMsg))
                            context.AddErrorNotification(errorMsg);
                        else
                            retorno = true;
                        break;

                    case AccionDocumento.IniciarVerificacion:

                        documento.IniciarVerificacion();
                        retorno = true;
                        break;

                    case AccionDocumento.Finalizar:

                        documento.Finalizar();
                        retorno = true;
                        break;

                    default:
                        context.AddErrorNotification("DOC260_Sec0_Error_EstadoDocNoCorrespondeEgreso");
                        break;
                }

                SetCommonData(uow, form, documento);

                if (retorno)
                {
                    uow.DocumentoRepository.UpdateEgreso(documento, uow.GetTransactionNumber());
                    uow.SaveChanges();
                }
            }

            return retorno;
        }

        protected virtual void SetCommonData(IUnitOfWork uow, Form form, IDocumento documento)
        {
            if (uow.DocumentoTipoRepository.RequiereReferenciaExterna(documento.Tipo, documento.Estado))
            {
                var nroRefExterna = form.GetField("nroRefExterna").Value;
                var tpRefExterna = form.GetField("tpRefExterna").Value;
                var fechRefExterna = string.IsNullOrEmpty(form.GetField("fechRefExterna").Value) ? DateTime.Now.ToString() : form.GetField("fechRefExterna").Value;

                documento.DocumentoReferenciaExterna = new DocumentoReferenciaExterna()
                {
                    Numero = nroRefExterna,
                    Tipo = tpRefExterna,
                    Fecha = DateTime.Parse(fechRefExterna, null, System.Globalization.DateTimeStyles.RoundtripKind)
                };
            }

            if (uow.DocumentoTipoRepository.RequiereFactura(documento.Tipo, documento.Estado))
            {
                documento.Factura = form.GetField("nroFactura").Value;
            }

            if (uow.DocumentoTipoRepository.RequiereAgenda(documento.Tipo, documento.Estado)
                && !uow.DocumentoTipoRepository.IsAutoAgendable(documento.Tipo))
            {
                var nroAgenda = int.Parse(form.GetField("nroAgenda").Value);
                SetDocumento(uow, documento, nroAgenda);
                documento.Agenda = nroAgenda;
            }
        }

        public virtual bool UpdateActa(IUnitOfWork uow, DocumentoTipo tipoDocumento, Form form, FormSubmitContext context)
        {
            var retorno = false;
            var errorMsg = "";
            var nuDocumento = context.Parameters.FirstOrDefault(p => p.Id == "NU_DOCUMENTO").Value;
            var tpDocumento = context.Parameters.FirstOrDefault(p => p.Id == "TP_DOCUMENTO").Value;
            var documento = uow.DocumentoRepository.GetActa(nuDocumento, tpDocumento);

            if (documento != null)
            {
                var estadoDestino = form.GetField("nuevoEstadoDocumento").Value;
                var accion = uow.DocumentoRepository.GetAccion(documento.Tipo, documento.Estado, estadoDestino);

                documento.Estado = estadoDestino;

                switch (accion.Codigo)
                {
                    case AccionDocumento.Editar:
                        documento.Editar();
                        retorno = true;
                        break;

                    case AccionDocumento.ConfirmarEdicion:
                        documento.ConfirmarEdicion();
                        retorno = true;
                        break;

                    case AccionDocumento.Cancelar:

                        if (!this.CancelarActaStock(uow, documento, form, out errorMsg))
                        {
                            context.AddErrorNotification(errorMsg);
                        }
                        else
                        {
                            documento.Cancelar();
                            retorno = true;
                        }
                        break;

                    case AccionDocumento.GenerarLineas:
                        documento.GenerarLineas();
                        retorno = true;
                        break;

                    case AccionDocumento.EnviarDocumento:
                        documento.EnviarDocumento();
                        retorno = true;
                        break;

                    case AccionDocumento.AprobarDocumento:
                        documento.AprobarDocumento(null, null, null);
                        retorno = true;
                        break;

                    case AccionDocumento.IniciarVerificacion:
                        documento.IniciarVerificacion();
                        retorno = true;
                        break;

                    case AccionDocumento.Finalizar:
                        documento.Finalizar();
                        retorno = true;
                        break;

                    default:
                        context.AddErrorNotification("DOC260_Sec0_Error_Error12");
                        break;
                }

                SetCommonData(uow, form, documento);

                if (retorno)
                {
                    uow.DocumentoRepository.UpdateActa(documento, uow.GetTransactionNumber());
                    uow.SaveChanges();
                }
            }

            return retorno;
        }

        public static void SetDocumento(IUnitOfWork uow, IDocumento documento, int nroAgenda)
        {
            var agenda = uow.AgendaRepository.GetAgenda(nroAgenda);
            var tpDUA = documento.DocumentoAduana?.Tipo ?? "";
            var dsDUA = uow.TipoDuaRepository.GetDescripcion(tpDUA) ?? "";
            var nroDUA = documento.DocumentoAduana?.Numero ?? "";

            agenda.Anexo1 = (documento.TipoAgrupador ?? "") + (documento.NumeroAgrupador ?? "");

            if (string.IsNullOrEmpty(agenda.Anexo1))
            {
                agenda.Anexo1 = string.Format("{0}{1}", nroDUA ?? "", dsDUA ?? "");
            }

            agenda.DUA = nroDUA;
            agenda.NumeroDocumento = $"{documento.Tipo}{documento.Numero}";
            agenda.NumeroTransaccion = uow.GetTransactionNumber();

            uow.AgendaRepository.UpdateAgenda(agenda);
        }

        public virtual bool AprobarDocumento(IUnitOfWork uow, IDocumento documento, Form form, out string errorMsg)
        {
            var resultado = false;

            errorMsg = "";

            var tipoDocumento = uow.DocumentoTipoRepository.GetTipoDocumento(documento.Tipo);
            var ingreso = documento is IDocumentoIngreso;

            DocumentoAduana dti = null;
            DocumentoAduana dua = null;
            Agenda agenda = null;

            if (uow.DocumentoTipoRepository.RequiereDTI(documento.Tipo, documento.Estado))
            {
                var nroDTI = form.GetField("nroDTI").Value;
                var fechaDTI = string.IsNullOrEmpty(form.GetField("fechDTI").Value) ? DateTime.Now.ToString() : form.GetField("fechDTI").Value;

                dti = new DocumentoAduana()
                {
                    Numero = nroDTI,
                    Tipo = "DTI",
                    Fecha = DateTime.Parse(fechaDTI, null, System.Globalization.DateTimeStyles.RoundtripKind)
                };
            }

            if (uow.DocumentoTipoRepository.RequiereDUA(documento.Tipo, documento.Estado))
            {
                var nroDua = form.GetField("nroDua").Value;
                var tpDua = form.GetField("tpDua").Value;
                var fechaVerificado = string.IsNullOrEmpty(form.GetField("fechVerificadoDua").Value) ? DateTime.Now.ToString() : form.GetField("fechVerificadoDua").Value;

                dua = new DocumentoAduana()
                {
                    Numero = nroDua,
                    Tipo = tpDua,
                    Fecha = DateTime.Parse(fechaVerificado, null, System.Globalization.DateTimeStyles.RoundtripKind)
                };

                if (ingreso && tipoDocumento.IngresoManual && tipoDocumento.AutoAgendable)
                {
                    var tipoDuaDesc = uow.TipoDuaRepository.GetDescripcion(tpDua);
                    agenda = this._documentoService.CrearAgenda(uow, (IDocumentoIngreso)documento, tipoDuaDesc, nroDua);
                }
            }

            if (ingreso && tipoDocumento.IngresoManual)
            {
                documento.AprobarDocumento(dti, dua, agenda?.Id);
                documento.Lineas = uow.DocumentoRepository.GetLineasIngreso(documento.Numero, documento.Tipo);
                resultado = true;
            }
            else
            {
                documento.AprobarDocumento(dti, dua, null);
                resultado = true;
            }

            if (ingreso && !tipoDocumento.AutoAgendable && documento.Agenda.HasValue)
            {
                SetDocumento(uow, documento, documento.Agenda.Value);
            }

            return resultado;
        }

        public virtual bool FinalizarDocumentoIngresoAduanero(IDocumentoIngreso documento, IUnitOfWork uow, string estadoOrigen, out string errorMsg)
        {
            bool resultado = false;
            errorMsg = "";

            var tpDocumento = uow.DocumentoTipoRepository.GetTipoDocumento(documento.Tipo);

            if (!tpDocumento.ManejaAgenda)
            {
                var estado = uow.DocumentoRepository.GetEstadoDestino(documento.Tipo, estadoOrigen, AccionDocumento.Finalizar);
                documento.Estado = estado;
                documento.Finalizar();
                resultado = true;
            }
            else
            {
                if (documento.Agenda == null)
                    throw new ValidationFailedException("General_Sec0_Error_AgendaNoExiste");

                var agenda = uow.AgendaRepository.GetAgenda(documento.Agenda.Value);
                if (agenda == null)
                    throw new ValidationFailedException("General_Sec0_Error_AgendaNoExiste");

                if (agenda.Estado == EstadoAgenda.Cerrada)
                {
                    IngresoDocumental ingreso = new IngresoDocumental(this._factoryService, this._parameterService, this._identity);
                    documento.Estado = estadoOrigen;
                    resultado = ingreso.BalancearDocumentoIngreso(uow, documento, agenda.Id, agenda.Detalles, this._identity.UserId, this._identity.Application, out errorMsg);
                }
                else
                {
                    var estado = uow.DocumentoRepository.GetEstadoDestino(documento.Tipo, estadoOrigen, AccionDocumento.FinalizarSinCierreAgenda);
                    if (string.IsNullOrEmpty(estado))
                        throw new ValidationFailedException("DOC260_Sec0_Error_FinalizacionSinCierreAgendaNoDefinida", new string[] { documento.Tipo, estadoOrigen });
                    else
                    {
                        documento.Estado = estado;
                        documento.Finalizar();
                        resultado = true;
                    }
                }
            }

            return resultado;
        }

        public virtual void AgregarParametro(FormInitializeContext context, string Id, string value)
        {
            ComponentParameter genericParam = new ComponentParameter()
            {
                Id = Id,
                Value = value
            };

            if (context.Parameters == null)
            {
                context.Parameters = new List<ComponentParameter>();
                context.Parameters.Add(genericParam);
            }
            else
            {
                if (context.Parameters.FirstOrDefault(p => p.Id == Id) != null)
                {
                    context.Parameters.FirstOrDefault(p => p.Id == Id).Value = genericParam.Value;
                }
                else
                {
                    context.Parameters.Add(genericParam);
                }
            }
        }

        public virtual bool CancelarActaStock(IUnitOfWork uow, IDocumentoActa documento, Form form, out string errorMsg)
        {
            bool resultado = false;
            errorMsg = "";

            try
            {
                // Busco ajustes relacionados desde el historial
                var historicos = uow.AjusteRepository.GetDocumentoAjustesStockHistoricoPorDocumento(documento.Tipo, documento.Numero);

                NivelacionAjusteDocumental nivelacion = new NivelacionAjusteDocumental();

                foreach (var historico in historicos)
                {
                    // Restauro los ajustes documentales importados
                    DocumentoAjusteStock ajuste;
                    ajuste = uow.AjusteRepository.GetAjustesDocumento(historico.NumeroAjuste);
                    if (ajuste == null)
                    {
                        ajuste = new DocumentoAjusteStock()
                        {
                            Aplicacion = historico.Aplicacion,
                            CantidadMovimiento = historico.CantidadMovimiento,
                            CodigoEmpresa = historico.CodigoEmpresa,
                            CodigoFuncionario = historico.CodigoFuncionario,
                            CodigoMotivoAjuste = historico.CodigoMotivoAjuste,
                            DescripcionMotivo = historico.DescripcionMotivo,
                            Endereco = historico.Endereco,
                            Faixa = historico.Faixa,
                            FechaActualizacion = historico.FechaActualizacion,
                            FechaCreacion = historico.FechaCreacion,
                            FechaMotivo = historico.FechaMotivo,
                            FuncionarioMotivo = historico.FuncionarioMotivo,
                            Identificador = historico.Identificador,
                            NumeroAjuste = historico.NumeroAjuste,
                            NumeroTransaccion = historico.NumeroTransaccion,
                            Predio = historico.Predio,
                            Producto = historico.Producto,
                        };
                        nivelacion.ajustesAgregar.Add(ajuste);
                    }
                    else
                    {
                        ajuste.CantidadMovimiento = ajuste.CantidadMovimiento + historico.CantidadMovimiento;
                        nivelacion.ajustesModificar.Add(ajuste);
                    }

                    //Elimino Ajuste documental historial 
                    nivelacion.ajustesHistoricosEliminar.Add(historico);
                }

                var ajusteStockDocumental = new AjusteStockDocumental();
                ajusteStockDocumental.ImpactarNivelacion(uow, nivelacion);

                //Restaurar saldo documentos en caso de ajustes negativos
                if (documento.OutDetail.Count > 0)
                {
                    List<DocumentoLinea> lineasIngresoAfectadas = new List<DocumentoLinea>();

                    foreach (var lineaEgresoActa in documento.OutDetail)
                    {
                        var lineaIngreso = lineasIngresoAfectadas.FirstOrDefault(s => s.Producto == lineaEgresoActa.Producto && s.Identificador == lineaEgresoActa.Identificador);

                        if (lineaIngreso == null)
                        {
                            lineaIngreso = uow.DocumentoRepository.GetLineaIngreso(lineaEgresoActa.DocumentoIngreso.Numero, lineaEgresoActa.DocumentoIngreso.Tipo, lineaEgresoActa.Producto, lineaEgresoActa.Identificador);
                            lineasIngresoAfectadas.Add(lineaIngreso);
                        }

                        lineaIngreso.CantidadDesafectada = lineaIngreso.CantidadDesafectada - lineaEgresoActa.CantidadDesafectada;
                    }

                    //Obtengo el documento asociado en las lineas de egreso del acta
                    IDocumento docuemntoAsociadoALineas = documento.OutDetail.FirstOrDefault().DocumentoIngreso;

                    //Actualizo las lineas de ingreso del documento asociado en las lineas de egreso (Mismo documento para todas las lineas de egreso del acta)
                    foreach (var lineaIngreso in lineasIngresoAfectadas)
                    {
                        uow.DocumentoRepository.UpdateDetail(docuemntoAsociadoALineas, lineaIngreso, uow.GetTransactionNumber());
                    }
                }

                resultado = true;
            }
            catch (Exception ex)
            {
                resultado = false;
                errorMsg = ex.Message;
            }

            return resultado;
        }

        #endregion AUX
    }
}