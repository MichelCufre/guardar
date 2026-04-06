using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Documento;
using WIS.Components.Common;
using WIS.Components.Common.Notification;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.DataModel.Queries.Documento;
using WIS.Domain.Documento;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Extension;
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
    public class DOC080 : AppController
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
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected List<string> GridKeys { get; }

        public DOC080(ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFormValidationService formValidationService,
            IGridValidationService gridValidationService,
            IFilterInterpreter filterInterpreter,
            IFactoryService factoryService)
        {
            this._session = session;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._formValidationService = formValidationService;
            this._gridValidationService = gridValidationService;
            this._filterInterpreter = filterInterpreter;
            this._factoryService = factoryService;

            this.GridKeys = new List<string>
            {
                "NU_DOCUMENTO",
                "TP_DOCUMENTO"
            };
        }

        #region FORM

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            context.Parameters.Add(new ComponentParameter() { Id = "formatoNumDoc", Value = "" });

            bool.TryParse(context.GetParameter("editar"), out bool editMode);

            //Inicializar selects
            this.InicializarSelects(form, editMode);

            if (editMode)
            {
                this.InicializarModoEdit(form, context);
            }
            else
            {
                foreach (var f in form.Fields)
                {
                    f.Value = "";
                    f.ReadOnly = false;
                }

                form.GetField("vlArbitraje").Value = "1";
                form.GetField("fechProgramado").Value = DateTime.Now.ToString("o", this._identity.GetFormatProvider());

                this.InicializarCamposInsert(form);
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            try
            {
                bool.TryParse(context.GetParameter("editar"), out bool editMode);

                if (editMode)
                {
                    var documento = this.EditarDocumentoObject(form);
                    if (documento != null)
                    {
                        using (var uow = this._uowFactory.GetUnitOfWork())
                        {
                            if (documento.CanEdit(uow))
                            {
                                uow.CreateTransactionNumber(this._identity.Application);

                                var nuTransaccion = uow.GetTransactionNumber();

                                uow.DocumentoRepository.UpdateIngreso(documento, nuTransaccion);
                                uow.SaveChanges();

                                if (context.ButtonId == "BtnConfirmarContinuar")
                                {
                                    context.Redirect("/documento/DOC081", new List<ComponentParameter>()
                                    {
                                        new ComponentParameter(){ Id = "nuDocumento", Value = documento.Numero },
                                        new ComponentParameter(){ Id = "tpDocumento", Value = documento.Tipo },
                                        new ComponentParameter(){ Id = "cdEmpresa", Value = documento.Empresa.ToString() },
                                    });
                                }

                                context.AddSuccessNotification("DOC080_Sec0_Error_Error01");
                            }
                            else
                            {
                                context.AddErrorNotification("DOC080_Sec0_Error_DocumentoNoEditable");
                            }
                        }
                    }
                    else
                    {
                        context.AddErrorNotification("DOC080_Sec0_Error_Error02");
                    }
                }
                else
                {
                    this.CrearDocumento(form, context);
                }
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, ex.Message);
                context.AddErrorNotification("DOC080_Sec0_Error_Error03", new List<string> { ex.Message });
            }
            finally
            {
                if (context.Redirection == null)
                    context.Redirect("/documento/DOC080", new List<ComponentParameter>() { });
                if (!context.Notifications.Any(a => a.Type == ApplicationNotificationType.Error))
                    context.AddParameter("OK", "true");
            }
            return form;
        }

        public override Form FormButtonAction(Form form, FormButtonActionContext context)
        {
            if (context.ButtonId == "BtnConfirmarContinuar")
            {
                bool.TryParse(context.GetParameter("editar"), out bool editMode);

                if (editMode)
                {
                    var documento = this.EditarDocumentoObject(form);
                    if (documento != null)
                    {
                        using (var uow = this._uowFactory.GetUnitOfWork())
                        {
                            if (documento.CanEdit(uow))
                            {
                                uow.CreateTransactionNumber(this._identity.Application);

                                var nuTransaccion = uow.GetTransactionNumber();

                                uow.DocumentoRepository.UpdateIngreso(documento, nuTransaccion);
                                uow.SaveChanges();

                                context.AddSuccessNotification("DOC080_Sec0_Error_Error01");
                                context.Redirect("/documento/DOC081", new List<ComponentParameter>()
                                {
                                    new ComponentParameter(){ Id = "nuDocumento", Value = documento.Numero },
                                    new ComponentParameter(){ Id = "tpDocumento", Value = documento.Tipo },
                                    new ComponentParameter(){ Id = "cdEmpresa", Value = documento.Empresa.ToString() },
                                });
                            }
                            else
                            {
                                context.AddErrorNotification("DOC080_Sec0_Error_DocumentoNoEditable");
                            }
                        }
                    }
                    else
                    {
                        context.AddErrorNotification("DOC080_Sec0_Error_Error02");
                    }

                    context.Redirect("/documento/DOC080", new List<ComponentParameter>() { });
                }
                else
                {
                    FormValidationContext valContext = new FormValidationContext() { Parameters = context.Parameters };
                    form = this.FormValidateForm(form, valContext);

                    if (form.IsValid())
                        this.CrearDocumento(form, context);
                    else
                        context.AddErrorNotification("General_Sec0_Error_ValidarFormulario");
                }
            }
            else if (context.ButtonId == "hideFormButton")
            {
                context.Redirect("/documento/DOC080", new List<ComponentParameter>() { });
            }

            return form;
        }
        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            bool.TryParse(context.GetParameter("editar"), out bool editMode);

            return this._formValidationService.Validate(new DOC080FormValidationModule(editMode, uow, this._identity), form, context);
        }
        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "cdEmpresa":
                    return this.SearchEmpresa(form, context);

                case "cdCliente":
                    return this.SearchCliente(form, context);

                case "cdDespachante":
                    return this.SearchDespachante(form, context);

                case "cdUnidadMedida":
                    return this.SearchUnidadMedida(form, context);

                default:
                    return new List<SelectOption>();
            }
        }

        #region METODOS

        public virtual void InicializarSelects(Form form, bool editMode)
        {
            //Inicializar selects
            FormField selectIngreso = form.GetField("tpIngreso");
            FormField selectVia = form.GetField("cdVia");
            FormField selectTransportadora = form.GetField("cdTransportadora");
            FormField selectMoneda = form.GetField("cdMoneda");
            FormField selectAlmacenajeSeguro = form.GetField("tpAlmacSeguro");
            FormField selectRegimenAduanero = form.GetField("cdRegimenAduana");
            FormField nroDocNoGenerado = form.GetField("nroDocNoGenerado");
            FormField nroDoc = form.GetField("nroDoc");

            selectIngreso.Options = new List<SelectOption>();
            selectVia.Options = new List<SelectOption>();
            selectTransportadora.Options = new List<SelectOption>();
            selectMoneda.Options = new List<SelectOption>();
            selectAlmacenajeSeguro.Options = new List<SelectOption>();
            selectRegimenAduanero.Options = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var documentosIngreso = new List<DocumentoTipo>();

                if (editMode)
                    documentosIngreso = uow.DocumentoTipoRepository.GetDocumentosIngresoHabilitados();
                else
                    documentosIngreso = uow.DocumentoTipoRepository.GetDocumentosIngresoManualHabilitados();

                foreach (var documentoHabilitado in documentosIngreso)
                {
                    selectIngreso.Options.Add(new SelectOption(documentoHabilitado.TipoDocumento, documentoHabilitado.DescripcionTipoDocumento));
                }

                nroDocNoGenerado.Value = string.Empty;
                nroDoc.Value = string.Empty;

                var vias = uow.ViaRepository.GetVias();
                foreach (var via in vias)
                {
                    selectVia.Options.Add(new SelectOption(via.Id, via.Descripcion));
                }

                if (editMode)
                    selectVia.Options.Add(new SelectOption("-", "-"));

                var transportistas = uow.TransportistaRepository.GetTransportistas();
                foreach (var transportista in transportistas)
                {
                    selectTransportadora.Options.Add(new SelectOption(transportista.Id.ToString(), $"{transportista.Id} - {transportista.Descripcion}"));
                }

                var monedas = uow.MonedaRepository.GetMonedas();
                foreach (var moneda in monedas)
                {
                    selectMoneda.Options.Add(new SelectOption(moneda.Codigo, moneda.Descripcion));
                }

                var tiposAlmacenajeSeguro = uow.TipoAlmacenajeSeguroRepository.GetTiposDeAlmacenajeYSeguro();
                foreach (var almacenajeSeguro in tiposAlmacenajeSeguro)
                {
                    selectAlmacenajeSeguro.Options.Add(new SelectOption(almacenajeSeguro.Tipo.ToString(), almacenajeSeguro.Descripcion));
                }

                var regimenesAduaneros = uow.RegimenAduaneroRepository.GetRegimenesAduaneros();
                foreach (var regimenAduanero in regimenesAduaneros)
                {
                    selectRegimenAduanero.Options.Add(new SelectOption(regimenAduanero.CodigoRegimen.ToString(), regimenAduanero.Descripcion));
                }

                var fieldEmpresa = form.GetField("cdEmpresa");
                fieldEmpresa.Options = new List<SelectOption>();
                fieldEmpresa.Value = "";

                var fieldProveedor = form.GetField("cdCliente");
                fieldProveedor.Options = new List<SelectOption>();
                fieldProveedor.Value = "";

                var fieldDespachante = form.GetField("cdDespachante");
                fieldDespachante.Options = new List<SelectOption>();
                fieldDespachante.Value = "";

                var fieldUnidadMedida = form.GetField("cdUnidadMedida");
                fieldUnidadMedida.Options = new List<SelectOption>();
                fieldUnidadMedida.Value = "";

                InicializarSelectPredio(uow, form);
            }
        }
        public virtual void InicializarCamposInsert(Form form)
        {
            form.GetField("nroDoc").ReadOnly = true;
            form.GetField("descEmpresa").ReadOnly = true;
            form.GetField("fechAlta").ReadOnly = true;
            form.GetField("descProveedor").ReadOnly = true;
            form.GetField("tp_dua").ReadOnly = true;
            form.GetField("nroDua").ReadOnly = true;
            form.GetField("fechDua").ReadOnly = true;
            form.GetField("fechMod").ReadOnly = true;
            form.GetField("descDespachante").ReadOnly = true;
            form.GetField("fechEnviado").ReadOnly = true;
            form.GetField("descUniMedida").ReadOnly = true;
            form.GetField("descAlmacSeguro").ReadOnly = true;
            form.GetField("descVia").ReadOnly = true;
            form.GetField("descTransportadora").ReadOnly = true;
            form.GetField("descMoneda").ReadOnly = true;
            form.GetField("nuDocTransporte").ReadOnly = true;
            form.GetField("nuAgenda").ReadOnly = true;
            form.GetField("cdFuncionario").ReadOnly = true;
            form.GetField("totalFob").ReadOnly = true;
            form.GetField("totalCif").ReadOnly = true;
            form.GetField("totalCifLineas").ReadOnly = true;
        }

        public virtual void InicializarModoEdit(Form form, FormInitializeContext context)
        {
            string nuDocumento = context.GetParameter("nuDocumento");
            string tpDocumento = context.GetParameter("tpDocumento");

            foreach (var f in form.Fields)
            {
                f.ReadOnly = true;
            }

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var doc = uow.DocumentoRepository.GetIngreso(nuDocumento, tpDocumento);

                if (doc != null)
                {
                    #region carga de datos para edicion

                    form.GetField("nroDoc").Value = doc.Numero;
                    form.GetField("tpIngreso").Value = doc.Tipo;
                    form.GetField("cdMoneda").Value = doc.Moneda;
                    form.GetField("cdVia").Value = string.IsNullOrEmpty(doc.Via) ? "-" : doc.Via;

                    if (doc.Transportista != null)
                        form.GetField("cdTransportadora").Value = doc.Transportista.ToString();

                    var fieldEmpresa = form.GetField("cdEmpresa");

                    Empresa empresa = uow.EmpresaRepository.GetEmpresa(doc.Empresa ?? -1);

                    if (empresa != null)
                    {
                        var empresaOption = new SelectOption(empresa.Id.ToString(), empresa.Nombre);
                        fieldEmpresa.Options = new List<SelectOption>();
                        fieldEmpresa.Options.Add(empresaOption);
                        fieldEmpresa.Value = empresa.Id.ToString();

                        var fieldProovedor = form.GetField("cdCliente");
                        var fieldUnidadMedida = form.GetField("cdUnidadMedida");
                        var fieldDespachante = form.GetField("cdDespachante");

                        fieldProovedor.Options = new List<SelectOption>();
                        fieldUnidadMedida.Options = new List<SelectOption>();
                        fieldDespachante.Options = new List<SelectOption>();

                        if (doc.Cliente != null)
                        {
                            Agente cliente = uow.AgenteRepository.GetAgente(empresa.Id, doc.Cliente);

                            if (cliente != null)
                            {
                                fieldProovedor.Options.Add(new SelectOption(cliente.CodigoInterno, cliente.Descripcion));
                                fieldProovedor.Value = cliente.CodigoInterno;
                            }

                            fieldProovedor.ReadOnly = false;
                        }
                        else
                        {
                            fieldProovedor.Options.Add(new SelectOption("-", "-"));
                            fieldProovedor.Value = "-";
                        }

                        if (doc.UnidadMedida != null)
                        {
                            UnidadMedida unidadMedida = uow.UnidadMedidaRepository.GetUnidadMedida(doc.UnidadMedida);

                            if (unidadMedida != null)
                            {
                                fieldUnidadMedida.Options.Add(new SelectOption(unidadMedida.Id, unidadMedida.Descripcion));
                                fieldUnidadMedida.Value = doc.UnidadMedida;
                                fieldUnidadMedida.ReadOnly = false;
                            }
                        }

                        if (doc.Despachante != null)
                        {
                            Despachante despachante = uow.DespachanteRepository.GetDespachante(doc.Despachante ?? -1);

                            if (despachante != null)
                            {
                                fieldDespachante.Options.Add(new SelectOption(despachante.Id.ToString(), despachante.Nombre));
                                fieldDespachante.Value = doc.Despachante.ToString();
                                fieldDespachante.ReadOnly = false;
                            }
                        }
                    }

                    form.GetField("fechAlta").Value = doc.FechaAlta.ToIsoString();
                    form.GetField("fechMod").Value = doc.FechaModificacion.ToIsoString();
                    form.GetField("fechEnviado").Value = doc.FechaEnviado.ToIsoString();

                    var export = form.GetField("nroExport");
                    export.Value = doc.NumeroExportacion;
                    export.ReadOnly = false;

                    var import = form.GetField("nroImport");
                    import.Value = doc.NumeroImportacion;
                    import.ReadOnly = false;

                    var programado = form.GetField("fechProgramado");
                    programado.Value = doc.FechaProgramado.ToString("yyyy/MM/dd");
                    programado.ReadOnly = false;

                    var factura = form.GetField("nroFactura");
                    factura.ReadOnly = false;
                    factura.Value = doc.Factura;

                    var conocimineto = form.GetField("nroConocimiento");
                    conocimineto.Value = doc.Conocimiento;
                    conocimineto.ReadOnly = false;

                    decimal auxVol = doc.Volumen ?? 0;
                    var volumen = form.GetField("qtVolumen");
                    volumen.Value = doc.Volumen == null ? "" : auxVol.ToString("0.000", this._identity.GetFormatProvider());
                    volumen.ReadOnly = false;

                    var bultos = form.GetField("qtBultos");
                    bultos.Value = doc.CantidadBulto.ToString();
                    bultos.ReadOnly = false;

                    decimal auxPeso = doc.Peso ?? 0;
                    var peso = form.GetField("qtPeso");
                    peso.Value = doc.Peso == null ? "" : auxPeso.ToString("0.000", this._identity.GetFormatProvider());
                    peso.ReadOnly = false;

                    var arbitraje = form.GetField("vlArbitraje");
                    arbitraje.Value = doc.ValorArbitraje.ToString();
                    arbitraje.ReadOnly = false;

                    var cont20 = form.GetField("qtContenedor20");
                    cont20.Value = doc.CantidadContenedor20.ToString();
                    cont20.ReadOnly = false;

                    var cont40 = form.GetField("qtContenedor40");
                    cont40.Value = doc.CantidadContenedor40.ToString();
                    cont40.ReadOnly = false;

                    var tpAlmSeguro = form.GetField("tpAlmacSeguro");
                    tpAlmSeguro.Value = doc.TipoAlmacenajeYSeguro.ToString();
                    tpAlmSeguro.ReadOnly = false;

                    var descDoc = form.GetField("descDocumento");
                    descDoc.Value = doc.Descripcion;
                    descDoc.ReadOnly = false;

                    var anexo1 = form.GetField("descAnexo1");
                    anexo1.Value = doc.Anexo1;
                    anexo1.ReadOnly = false;

                    var anexo2 = form.GetField("descAnexo2");
                    anexo2.Value = doc.Anexo2;
                    anexo2.ReadOnly = false;

                    var anexo3 = form.GetField("descAnexo3");
                    anexo3.Value = doc.Anexo3;
                    anexo3.ReadOnly = false;

                    var anexo4 = form.GetField("descAnexo4");
                    anexo4.Value = doc.Anexo4;
                    anexo4.ReadOnly = false;

                    var anexo5 = form.GetField("descAnexo5");
                    anexo5.Value = doc.Anexo5;
                    anexo5.ReadOnly = false;

                    decimal auxSeguro = doc.ValorSeguro ?? 0;
                    var seguro = form.GetField("vlSeguro");
                    seguro.Value = doc.ValorSeguro == null ? "" : auxSeguro.ToString("0.000", this._identity.GetFormatProvider());
                    seguro.ReadOnly = false;

                    decimal auxFlete = doc.ValorFlete ?? 0;
                    var flete = form.GetField("vlFlete");
                    flete.Value = doc.ValorFlete == null ? "" : auxFlete.ToString("0.000", this._identity.GetFormatProvider());
                    flete.ReadOnly = false;

                    form.GetField("predio").ReadOnly = false;
                    form.GetField("predio").Value = doc.Predio;

                    decimal auxOtrGastos = doc.ValorOtrosGastos ?? 0;
                    var otrosGastos = form.GetField("vlOtrosGastos");
                    otrosGastos.Value = doc.ValorOtrosGastos == null ? "" : auxOtrGastos.ToString("0.000", this._identity.GetFormatProvider());
                    otrosGastos.ReadOnly = false;

                    form.GetField("nuDocTransporte").Value = doc.DocumentoTransporte;
                    form.GetField("nuAgenda").Value = doc.Agenda.ToString();
                    form.GetField("cdFuncionario").Value = doc.Usuario.ToString();

                    var icms = form.GetField("icms");
                    icms.Value = doc.ICMS.ToString();
                    icms.ReadOnly = false;

                    var ii = form.GetField("ii");
                    ii.Value = doc.II.ToString();
                    ii.ReadOnly = false;

                    var ip = form.GetField("ipi");
                    ip.Value = doc.IPI.ToString();
                    ip.ReadOnly = false;

                    var iisuspenso = form.GetField("iisuspenso");
                    iisuspenso.Value = doc.IISUSPENSO.ToString();
                    iisuspenso.ReadOnly = false;

                    var ipisuspenso = form.GetField("ipisuspenso");
                    ipisuspenso.Value = doc.IPISUSPENSO.ToString();
                    ipisuspenso.ReadOnly = false;

                    var pisconfins = form.GetField("pisconfins");
                    pisconfins.Value = doc.PISCONFINS.ToString();
                    pisconfins.ReadOnly = false;

                    var regimenAduana = form.GetField("cdRegimenAduana");
                    regimenAduana.Value = doc.RegimenAduana.ToString();
                    regimenAduana.ReadOnly = false;

                    var query = new DocumentoDetalleQuery(nuDocumento, tpDocumento);

                    uow.HandleQuery(query);

                    if (doc.Validado)
                    {
                        ValoresTotalesDetalleDocumento valores = query.GetValoresTotales(doc);

                        form.GetField("totalFob").Value = valores.FOB.ToString();
                        form.GetField("totalCif").Value = valores.CIF.ToString();
                        form.GetField("totalCifLineas").Value = valores.Lineas.ToString();
                    }
                    else
                    {
                        form.GetField("totalFob").Value = "0";
                        form.GetField("totalCif").Value = "0";
                        form.GetField("totalCifLineas").Value = "0";
                    }

                    #endregion carga de datos para edicion
                }
            }
        }
        public virtual void InicializarSelectPredio(IUnitOfWork uow, Form form)
        {
            // Predios
            FormField selectPredio = form.GetField("predio");
            selectPredio.Options = new List<SelectOption>();
            var dbQuery = new GetPrediosUsuarioQuery();
            uow.HandleQuery(dbQuery);

            var predios = dbQuery.GetPrediosUsuario(_identity.UserId).OrderBy(x => x.Numero);
            foreach (var pred in predios)
            {
                selectPredio.Options.Add(new SelectOption(pred.Numero, $"{pred.Numero} - {pred.Descripcion}"));
            }

            if (predios.Count() == 1)
                selectPredio.Value = predios.FirstOrDefault().Numero;

            if (!this._identity.Predio.Equals(GeneralDb.PredioSinDefinir))
                selectPredio.Value = this._identity.Predio;
        }


        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                List<Empresa> empresas = uow.EmpresaRepository.GetEmpresasUsuarioDocumentalesByNombreOrCodePartial(context.SearchValue, this._identity.UserId);

                foreach (var empresa in empresas)
                {
                    opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
                }
            }

            return opciones;
        }
        public virtual List<SelectOption> SearchCliente(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (int.TryParse(form.GetField("cdEmpresa").Value, out int cdEmpresa))
                {
                    List<Agente> agentes = uow.AgenteRepository.GetAgenteByNombrePartial(cdEmpresa, AgenteTipo.Proveedor, context.SearchValue);

                    foreach (var agente in agentes)
                    {
                        opciones.Add(new SelectOption(agente.CodigoInterno, $"{agente.Tipo} - {agente.Codigo} - {agente.Descripcion}"));
                    }
                }
                else
                {
                    form.GetField("cdEmpresa").SetError("General_Sec0_Error_Error25");
                }
            }

            return opciones;
        }
        public virtual List<SelectOption> SearchDespachante(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                List<Despachante> despachantes = uow.DespachanteRepository.GetDespachanteByNombrePartial(context.SearchValue);

                foreach (var despachante in despachantes)
                {
                    opciones.Add(new SelectOption(despachante.Id.ToString(), $"{despachante.Id} - {despachante.Nombre}"));
                }
            }

            return opciones;
        }
        public virtual List<SelectOption> SearchUnidadMedida(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                List<UnidadMedida> unidadesMedida = uow.UnidadMedidaRepository.GetByNombreOrCodePartial(context.SearchValue);

                foreach (var unidadMedida in unidadesMedida)
                {
                    opciones.Add(new SelectOption(unidadMedida.Id, unidadMedida.Descripcion));
                }
            }

            return opciones;
        }

        public virtual void CrearDocumento(Form form, FormSubmitContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                uow.CreateTransactionNumber("DOC080 CrearDocumento");

                var nuTransaccion = uow.GetTransactionNumber();
                var documento = this.CrearDocumentoObject(form, uow);

                uow.DocumentoRepository.AddIngreso(documento, nuTransaccion);
                uow.SaveChanges();

                context.AddSuccessNotification("DOC080_Sec0_Error_Error04", new List<string> { documento.Numero, documento.Tipo });

                if (context.ButtonId == "BtnConfirmarContinuar")
                {
                    context.Redirect("/documento/DOC081", new List<ComponentParameter>()
                    {
                        new ComponentParameter(){ Id = "nuDocumento", Value = documento.Numero },
                        new ComponentParameter(){ Id = "tpDocumento", Value = documento.Tipo },
                        new ComponentParameter(){ Id = "cdEmpresa", Value = documento.Empresa.ToString() },
                    });
                }
            }
        }
        public virtual void CrearDocumento(Form form, FormButtonActionContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                uow.CreateTransactionNumber("DOC080 CrearDocumento");

                var nuTransaccion = uow.GetTransactionNumber();
                var documento = this.CrearDocumentoObject(form, uow);

                uow.DocumentoRepository.AddIngreso(documento, nuTransaccion);
                uow.SaveChanges();

                context.AddSuccessNotification("DOC080_Sec0_Error_Error04", new List<string> { documento.Numero, documento.Tipo });
                context.Redirect("/documento/DOC081", new List<ComponentParameter>()
                {
                    new ComponentParameter(){ Id = "nuDocumento", Value = documento.Numero },
                    new ComponentParameter(){ Id = "tpDocumento", Value = documento.Tipo },
                    new ComponentParameter(){ Id = "cdEmpresa", Value = documento.Empresa.ToString() },
                });
            }
        }

        public virtual IDocumentoIngreso CrearDocumentoObject(Form form, IUnitOfWork uow)
        {
            var tpIngreso = form.GetField("tpIngreso").Value;
            var tipoDocumento = uow.DocumentoTipoRepository.GetTipoDocumento(tpIngreso);
            var documento = this._factoryService.CreateDocumentoIngreso(tpIngreso);
            var estadoInicial = uow.DocumentoTipoRepository.GetEstadoInicial(tpIngreso);
            var culture = this._identity.GetFormatProvider();

            if (documento != null)
            {
                documento.Numero = "";
                documento.Tipo = tpIngreso;
                documento.Descripcion = form.GetField("descDocumento").Value;
                documento.Usuario = this._identity.UserId;
                documento.DocumentoAduana = new DocumentoAduana();
                documento.Moneda = form.GetField("cdMoneda").Value;

                if ((form.GetField("cdMoneda").Value != "1" && !string.IsNullOrEmpty(form.GetField("cdMoneda").Value)))
                {
                    if (decimal.TryParse(form.GetField("vlArbitraje").Value, NumberStyles.Number, culture, out decimal arbitraje))
                        documento.ValorArbitraje = arbitraje;
                }
                else
                {
                    documento.ValorArbitraje = 1;
                }

                documento.GeneraAgenda = tipoDocumento.AutoAgendable;
                documento.Situacion = SituacionDb.Activo;
                documento.FechaAlta = DateTime.Now;
                documento.Empresa = int.Parse(form.GetField("cdEmpresa").Value);
                documento.Via = form.GetField("cdVia").Value;
                documento.Factura = form.GetField("nroFactura").Value;
                documento.Conocimiento = form.GetField("nroConocimiento").Value;

                if (decimal.TryParse(form.GetField("qtBultos").Value, NumberStyles.Number, culture, out decimal qtBultos))
                    documento.CantidadBulto = qtBultos;

                if (int.TryParse(form.GetField("cdTransportadora").Value, out int transportista))
                    documento.Transportista = transportista;

                documento.Estado = estadoInicial;

                DateTime fechaProgramado;
                if (DateTime.TryParse(form.GetField("fechProgramado").Value, this._identity.GetFormatProvider(), DateTimeStyles.None, out fechaProgramado))
                    documento.FechaProgramado = fechaProgramado;

                documento.UnidadMedida = form.GetField("cdUnidadMedida").Value;
                documento.NumeroImportacion = form.GetField("nroImport").Value;
                documento.NumeroExportacion = form.GetField("nroExport").Value;

                if (short.TryParse(form.GetField("cdDespachante").Value, out short cdDespachante))
                    documento.Despachante = cdDespachante;

                if (decimal.TryParse(form.GetField("vlSeguro").Value, NumberStyles.Number, culture, out decimal ValorSeguro))
                    documento.ValorSeguro = ValorSeguro;
                else
                    documento.ValorSeguro = 0;

                if (decimal.TryParse(form.GetField("qtVolumen").Value, NumberStyles.Number, culture, out decimal qtVolumen))
                    documento.Volumen = qtVolumen;

                if (decimal.TryParse(form.GetField("qtPeso").Value, NumberStyles.Number, culture, out decimal qtPeso))
                    documento.Peso = qtPeso;

                if (short.TryParse(form.GetField("tpAlmacSeguro").Value, out short tpAlmacSeguro))
                    documento.TipoAlmacenajeYSeguro = tpAlmacSeguro;

                documento.Predio = form.GetField("predio").Value;

                if (short.TryParse(form.GetField("qtContenedor20").Value, out short qtContenedor20))
                    documento.CantidadContenedor20 = qtContenedor20;

                if (short.TryParse(form.GetField("qtContenedor40").Value, out short qtContenedor40))
                    documento.CantidadContenedor40 = qtContenedor40;

                if (decimal.TryParse(form.GetField("vlFlete").Value, NumberStyles.Number, culture, out decimal vlFlete))
                    documento.ValorFlete = vlFlete;
                else
                    documento.ValorFlete = 0;

                documento.IdManual = "S";
                documento.AgendarAutomaticamente = tipoDocumento.AutoAgendable;

                if (decimal.TryParse(form.GetField("vlOtrosGastos").Value, NumberStyles.Number, culture, out decimal vlOtrosGastos))
                    documento.ValorOtrosGastos = vlOtrosGastos;
                else
                    documento.ValorOtrosGastos = 0;

                documento.DocumentoTransporte = form.GetField("nuDocTransporte").Value;
                documento.Anexo1 = form.GetField("descAnexo1").Value;
                documento.Anexo2 = form.GetField("descAnexo2").Value;
                documento.Anexo3 = form.GetField("descAnexo3").Value;
                documento.Anexo4 = form.GetField("descAnexo4").Value;
                documento.Anexo5 = form.GetField("descAnexo5").Value;
                documento.Cliente = form.GetField("cdCliente").Value;

                if (decimal.TryParse(form.GetField("icms").Value, NumberStyles.Number, culture, out decimal icms))
                    documento.ICMS = icms;

                if (decimal.TryParse(form.GetField("ii").Value, NumberStyles.Number, culture, out decimal ii))
                    documento.II = ii;

                if (decimal.TryParse(form.GetField("ipi").Value, NumberStyles.Number, culture, out decimal ipi))
                    documento.IPI = ipi;

                if (decimal.TryParse(form.GetField("iisuspenso").Value, NumberStyles.Number, culture, out decimal iisuspenso))
                    documento.IISUSPENSO = iisuspenso;

                if (decimal.TryParse(form.GetField("ipisuspenso").Value, NumberStyles.Number, culture, out decimal ipisuspenso))
                    documento.IPISUSPENSO = ipisuspenso;

                if (decimal.TryParse(form.GetField("pisconfins").Value, NumberStyles.Number, culture, out decimal pisconfins))
                    documento.PISCONFINS = pisconfins;

                if (int.TryParse(form.GetField("cdRegimenAduana").Value, NumberStyles.Number, culture, out int cdRegimenAduana))
                    documento.RegimenAduana = cdRegimenAduana;

                if (uow.DocumentoTipoRepository.DocumentoNumeracionAutogenerada(tpIngreso))
                    documento.Numero = documento.GetNumeroDocumento(uow);
                else
                    documento.Numero = form.GetField("nroDocNoGenerado").Value;
            }

            return documento;
        }
        public virtual IDocumentoIngreso EditarDocumentoObject(Form form)
        {
            var tpIngreso = form.GetField("tpIngreso").Value;

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var tipoDocumento = uow.DocumentoTipoRepository.GetTipoDocumento(tpIngreso);
                var documento = uow.DocumentoRepository.GetIngreso(form.GetField("nroDoc").Value, tpIngreso);
                var culture = this._identity.GetFormatProvider();

                if (documento != null)
                {
                    documento.Descripcion = form.GetField("descDocumento").Value;
                    documento.Usuario = this._identity.UserId;
                    documento.Moneda = form.GetField("cdMoneda").Value;

                    if (decimal.TryParse(form.GetField("vlArbitraje").Value, NumberStyles.Number, culture, out decimal arbitraje))
                        documento.ValorArbitraje = arbitraje;

                    documento.Factura = form.GetField("nroFactura").Value;
                    documento.Conocimiento = form.GetField("nroConocimiento").Value;

                    if (decimal.TryParse(form.GetField("qtBultos").Value, NumberStyles.Number, culture, out decimal qtBultos))
                        documento.CantidadBulto = qtBultos;

                    if (int.TryParse(form.GetField("cdTransportadora").Value, out int transportista))
                        documento.Transportista = transportista;

                    documento.UnidadMedida = form.GetField("cdUnidadMedida").Value;
                    documento.NumeroImportacion = form.GetField("nroImport").Value;
                    documento.NumeroExportacion = form.GetField("nroExport").Value;

                    if (short.TryParse(form.GetField("cdDespachante").Value, out short cdDespachante))
                        documento.Despachante = cdDespachante;

                    if (decimal.TryParse(form.GetField("vlSeguro").Value, NumberStyles.Number, culture, out decimal ValorSeguro))
                        documento.ValorSeguro = ValorSeguro;

                    if (decimal.TryParse(form.GetField("qtVolumen").Value, NumberStyles.Number, culture, out decimal qtVolumen))
                        documento.Volumen = qtVolumen;

                    if (decimal.TryParse(form.GetField("qtPeso").Value, NumberStyles.Number, culture, out decimal qtPeso))
                        documento.Peso = qtPeso;

                    if (short.TryParse(form.GetField("tpAlmacSeguro").Value, out short tpAlmacSeguro))
                        documento.TipoAlmacenajeYSeguro = tpAlmacSeguro;

                    documento.Predio = form.GetField("predio").Value;

                    if (short.TryParse(form.GetField("qtContenedor20").Value, out short qtContenedor20))
                        documento.CantidadContenedor20 = qtContenedor20;

                    if (short.TryParse(form.GetField("qtContenedor40").Value, out short qtContenedor40))
                        documento.CantidadContenedor40 = qtContenedor40;

                    if (decimal.TryParse(form.GetField("vlFlete").Value, NumberStyles.Number, culture, out decimal vlFlete))
                        documento.ValorFlete = vlFlete;

                    documento.IdManual = "S";
                    documento.AgendarAutomaticamente = tipoDocumento.AutoAgendable;

                    if (decimal.TryParse(form.GetField("vlOtrosGastos").Value, NumberStyles.Number, culture, out decimal vlOtrosGastos))
                        documento.ValorOtrosGastos = vlOtrosGastos;

                    documento.DocumentoTransporte = form.GetField("nuDocTransporte").Value;
                    documento.Anexo1 = form.GetField("descAnexo1").Value;
                    documento.Anexo2 = form.GetField("descAnexo2").Value;
                    documento.Anexo3 = form.GetField("descAnexo3").Value;
                    documento.Anexo4 = form.GetField("descAnexo4").Value;
                    documento.Anexo5 = form.GetField("descAnexo5").Value;
                    documento.Cliente = form.GetField("cdCliente").Value;

                    DateTime programado;
                    if (DateTime.TryParse(form.GetField("fechProgramado").Value, this._identity.GetFormatProvider(), DateTimeStyles.None, out programado))
                        documento.FechaProgramado = programado;

                    documento.FechaModificacion = DateTime.Now;

                    if (decimal.TryParse(form.GetField("icms").Value, NumberStyles.Number, culture, out decimal icms))
                        documento.ICMS = icms;

                    if (decimal.TryParse(form.GetField("ii").Value, NumberStyles.Number, culture, out decimal ii))
                        documento.II = ii;

                    if (decimal.TryParse(form.GetField("ipi").Value, NumberStyles.Number, culture, out decimal ipi))
                        documento.IPI = ipi;

                    if (decimal.TryParse(form.GetField("iisuspenso").Value, NumberStyles.Number, culture, out decimal iisuspenso))
                        documento.IISUSPENSO = iisuspenso;

                    if (decimal.TryParse(form.GetField("ipisuspenso").Value, NumberStyles.Number, culture, out decimal ipisuspenso))
                        documento.IPISUSPENSO = ipisuspenso;

                    if (decimal.TryParse(form.GetField("pisconfins").Value, NumberStyles.Number, culture, out decimal pisconfins))
                        documento.PISCONFINS = pisconfins;

                    if (int.TryParse(form.GetField("cdRegimenAduana").Value, NumberStyles.Number, culture, out int cdRegimenAduana))
                        documento.RegimenAduana = cdRegimenAduana;

                    return documento;
                }
            }
            return null;
        }

        #endregion METODOS

        #endregion FORM

        #region GRID

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem> {
                new GridItemHeader("DOC080_Sec0_lbl_Acciones"),
                new GridButton("btnDetalles", "General_Sec0_btn_Detalles", "fas fa-list"),
                new GridButton("btnSaldo", "DOC080_Sec0_btn_Saldos", "fas fa-clipboard-list"),
                new GridButton("btnDocumentos", "General_Sec0_btn_Documentos", "fas fa-paperclip"),
                new GridItemDivider(),
                new GridButton("btnEditar", "General_Sec0_btn_Editar", "fas fa-pencil-alt")
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var query = new OrdenesDeIngresoQuery();

                uow.HandleQuery(query);

                var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

                grid.Rows = this._gridService.GetRows(query, grid.Columns, context, defaultSort, this.GridKeys);

                var docsEditables = uow.DocumentoTipoRepository.GetDocumentosHabilitadosParaEdicion();
                var docsSimularCC = uow.DocumentoTipoRepository.GetDocumentosHabilitadosParaSimularCC();

                grid.Rows.ForEach(row =>
                {
                    var tipoDoc = row.GetCell("TP_DOCUMENTO").Value;
                    var estado = row.GetCell("ID_ESTADO").Value;

                    if (!docsSimularCC.ContainsKey(tipoDoc) || !docsSimularCC[tipoDoc].Contains(estado))
                        row.DisabledButtons.Add("btnSimularCuentaCorriente");

                    if (!docsEditables.ContainsKey(tipoDoc) || !docsEditables[tipoDoc].Contains(estado))
                        row.DisabledButtons.Add("btnEditar");
                });
            }

            return grid;
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext data)
        {
            if (data.ButtonId == "btnEditar")
            {
                data.Parameters.Add(new ComponentParameter
                {
                    Id = "editar",
                    Value = "true"
                });

                data.Redirect("/documento/DOC080", new List<ComponentParameter>()
                {
                    new ComponentParameter(){ Id = "editar", Value = true.ToString().ToLower() },
                    new ComponentParameter(){ Id = "nuDocumento", Value = data.Row.GetCell("NU_DOCUMENTO").Value },
                    new ComponentParameter(){ Id = "tpDocumento", Value = data.Row.GetCell("TP_DOCUMENTO").Value },
                    new ComponentParameter(){ Id = "cdEmpresa", Value = data.Row.GetCell("CD_EMPRESA").Value },
                });
            }
            else if (data.ButtonId == "btnSaldo")
            {
                data.Redirect("/documento/DOC020", new List<ComponentParameter>()
                {
                    new ComponentParameter(){ Id = "nuDocumento", Value = data.Row.GetCell("NU_DOCUMENTO").Value },
                    new ComponentParameter(){ Id = "tpDocumento", Value = data.Row.GetCell("TP_DOCUMENTO").Value },
                    new ComponentParameter(){ Id = "cdEmpresa", Value = data.Row.GetCell("CD_EMPRESA").Value },
                });
            }
            else if (data.ButtonId == "btnDetalles")
            {
                data.Redirect("/documento/DOC081", new List<ComponentParameter>()
                {
                    new ComponentParameter(){ Id = "nuDocumento", Value = data.Row.GetCell("NU_DOCUMENTO").Value },
                    new ComponentParameter(){ Id = "tpDocumento", Value = data.Row.GetCell("TP_DOCUMENTO").Value },
                    new ComponentParameter(){ Id = "cdEmpresa", Value = data.Row.GetCell("CD_EMPRESA").Value },
                });
            }
            else if (data.ButtonId == "btnDocumentos")
            {
                data.Parameters.RemoveAll(p => p.Id == "codigoEntidad");

                var tpDocumento = data.Row.GetCell("TP_DOCUMENTO").Value;
                var nuDocumento = data.Row.GetCell("NU_DOCUMENTO").Value;

                data.AddParameter("codigoEntidad", $"{tpDocumento}${nuDocumento}");
            }

            return data;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var dbQuery = new OrdenesDeIngresoQuery();

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new OrdenesDeIngresoQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        #endregion GRID
    }
}