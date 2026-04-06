using Microsoft.EntityFrameworkCore;
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
using WIS.Domain.DataModel.Queries.Documento;
using WIS.Domain.Documento;
using WIS.Domain.Documento.Constants;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
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
    public class DOC320 : AppController
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
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected List<string> GridKeys { get; }

        public DOC320(ISessionAccessor session,
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFormValidationService formValidationService,
            IGridValidationService gridValidationService,
            IFilterInterpreter filterInterpreter,
            IFactoryService factoryService,
            IDocumentoService documentoService)
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
            this._documentoService = documentoService;

            this.GridKeys = new List<string>
            {
                "NU_AGRUPADOR", "TP_AGRUPADOR"
            };
        }

        #region GRID

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem> {
                new GridItemHeader("DOC080_Sec0_lbl_Acciones"),
                new GridButton("btnDetalles", "DOC320_grid1_btn_Detalles", "fas fa-list"),
                new GridItemDivider(),
                new GridButton("btnEditar", "DOC320_grid1_btn_Editar", "fas fa-pencil-alt"),
                new GridButton("btnEnvioMasivo", "DOC320_grid1_btn_EnvioMasivo", "fas fa-paper-plane"),
                new GridItemDivider(),
                new GridButton("btnInfo", "DOC320_grid1_btn_VerInfo", "fas fa-info"),
                new GridButton("btnCancelarEnvio", "DOC320_grid1_btn_CancelarEnvio", "far fa-file-pdf")
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var dbQuery = new DocumentoAgrupadorDOC320Query();

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

                this.FormatGrid(grid);
            }

            return grid;
        }

        public virtual void FormatGrid(Grid grid)
        {
            foreach (var row in grid.Rows)
            {
                switch (row.GetCell("ID_ESTADO").Value)
                {
                    case EstadoDocumentoAgrupador.EDICION:
                        row.DisabledButtons.Add("btnEnvioMasivo");
                        row.DisabledButtons.Add("btnCancelarEnvio");
                        row.DisabledButtons.Add("btnInfo");
                        break;

                    case EstadoDocumentoAgrupador.CONFIRMADO:
                        row.DisabledButtons.Add("btnEditar");
                        row.DisabledButtons.Add("btnDetalles");
                        row.DisabledButtons.Add("btnCancelarEnvio");
                        break;

                    case EstadoDocumentoAgrupador.ENVIADO:
                        row.DisabledButtons.Add("btnEnvioMasivo");
                        row.DisabledButtons.Add("btnEditar");
                        row.DisabledButtons.Add("btnDetalles");
                        if (row.GetCell("TP_AGRUPADOR").Value != "RTM")
                        {
                            row.DisabledButtons.Add("btnCancelarEnvio");
                        }
                        break;
                    case EstadoDocumentoAgrupador.CANCELADO:
                        row.DisabledButtons.Add("btnEnvioMasivo");
                        row.DisabledButtons.Add("btnEditar");
                        row.DisabledButtons.Add("btnDetalles");
                        row.DisabledButtons.Add("btnCancelarEnvio");
                        break;
                    default:
                        row.DisabledButtons.Add("btnEnvioMasivo");
                        row.DisabledButtons.Add("btnEditar");
                        row.DisabledButtons.Add("btnDetalles");
                        row.DisabledButtons.Add("btnCancelarEnvio");
                        row.DisabledButtons.Add("btnInfo");
                        break;
                }
            }
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            try
            {
                switch (context.ButtonId)
                {
                    case "btnInfo":
                        context.Redirect("/documento/DOC350", new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "nuAgrupador", Value = context.Row.GetCell("NU_AGRUPADOR").Value },
                            new ComponentParameter(){ Id = "tpAgrupador", Value = context.Row.GetCell("TP_AGRUPADOR").Value },
                        });
                        break;
                    case "btnDetalles":
                        context.Redirect("/documento/DOC330", new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "nuAgrupador", Value = context.Row.GetCell("NU_AGRUPADOR").Value },
                            new ComponentParameter(){ Id = "tpAgrupador", Value = context.Row.GetCell("TP_AGRUPADOR").Value },
                        });
                        break;

                    case "btnEditar":
                        context.Redirect("/documento/DOC320", new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "editar", Value = true.ToString() },
                            new ComponentParameter(){ Id = "nuAgrupador", Value = context.Row.GetCell("NU_AGRUPADOR").Value },
                            new ComponentParameter(){ Id = "tpAgrupador", Value = context.Row.GetCell("TP_AGRUPADOR").Value },
                        });
                        context.AddParameter("editar", "true");
                        break;

                    case "btnEnvioMasivo":
                        try
                        {
                            if (this._session.GetValue<bool>("DOC320_ENVIO_MASIVO" + context.Row.GetCell("NU_AGRUPADOR").Value))
                            {
                                context.AddErrorNotification("Negado");
                            }
                            else
                            {
                                this._session.SetValue("DOC320_ENVIO_MASIVO" + context.Row.GetCell("NU_AGRUPADOR").Value, true);

                                using (var uow = this._uowFactory.GetUnitOfWork())
                                {
                                    uow.CreateTransactionNumber("DOC320 EnvioMasivo");

                                    var tipo = uow.DocumentoRepository.GetDocumentoAgrupadorTipo(context.Row.GetCell("TP_AGRUPADOR").Value);

                                    switch (tipo.TipoOperacion)
                                    {
                                        case TipoOperacionAgrupador.INGRESO:
                                            this.AceptarDocumentoIngresoAduanero(uow, context.Row.GetCell("NU_AGRUPADOR").Value, context.Row.GetCell("TP_AGRUPADOR").Value);
                                            context.AddSuccessNotification("DOC320_Sec0_Not_SuccesEnvioAgrupador");
                                            break;
                                        case TipoOperacionAgrupador.EGRESO:
                                            this.FinalizarDocumentoEgresoAduanero(uow, context.Row.GetCell("NU_AGRUPADOR").Value, context.Row.GetCell("TP_AGRUPADOR").Value);
                                            context.AddSuccessNotification("DOC320_Sec0_Not_SuccesEnvioAgrupador");
                                            break;
                                    }
                                }
                            }
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            this._logger.Error(ex, ex.Message);
                            context.AddErrorNotification("DOC320_Sec0_Not_ErrorEnvioAgrupadorConcurrencia");
                        }
                        catch (Exception ex)
                        {
                            this._logger.Error(ex, ex.Message);
                            context.AddErrorNotification("DOC320_Sec0_Not_ErrorEnvioAgrupador", new List<string> { ex.Message });
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, ex.Message);
                context.AddErrorNotification(ex.Message);
            }
            finally
            {
                this._session.SetValue("DOC320_ENVIO_MASIVO" + context.Row.GetCell("NU_AGRUPADOR").Value, false);
            }

            return context;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var dbQuery = new DocumentoAgrupadorDOC320Query();

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("DT_ADDROW", SortDirection.Descending);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new DocumentoAgrupadorDOC320Query();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        #endregion GRID

        #region FORM

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            if (form.Id == "DOC320_form_1")
            {
                this.InitForm1(form, context);
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            switch (form.Id)
            {
                case "DOC320_form_1":
                    this.FormSubmit1(form, context);
                    break;
                case "DOC320_form_2":
                    this.FormSubmit2(form, context);
                    break;
            }

            return form;
        }

        public override Form FormButtonAction(Form form, FormButtonActionContext context)
        {
            switch (context.ButtonId)
            {
                case "BtnConfirmarContinuar":
                    FormValidationContext valContext = new FormValidationContext() { Parameters = context.Parameters };
                    form = this.FormValidateForm(form, valContext);

                    if (form.IsValid())
                        this.FormSubmit(form, new FormSubmitContext());
                    else
                        context.AddErrorNotification("General_Sec0_Error_ValidarFormulario");

                    break;

                case "hideFormButton":
                    context.Redirect("/documento/DOC320", new List<ComponentParameter>() { });
                    break;
            }
            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new DOC320FormValidationModule(form, uow, this._identity), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "cdEmpresa":
                    return this.SearchEmpresa(form, context);
                case "cdTransportadora":
                    return this.SearchTransportadora(form, context);
                case "tipoVehiculo":
                    return this.SearchTipoVehiculo(form, context);
                default:
                    return new List<SelectOption>();
            }
        }

        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                List<Empresa> empresas = uow.EmpresaRepository.GetEmpresasUsuarioDocumentalesByNombreOrCodePartial(context.SearchValue, this._identity.UserId);

                foreach (var empresa in empresas)
                {
                    if (string.IsNullOrEmpty(empresa.NumeroFiscal))
                        opciones.Add(new SelectOption(empresa.Id.ToString(), string.Format("{0} - {1}", empresa.Id, empresa.Nombre)));
                    else
                        opciones.Add(new SelectOption(empresa.Id.ToString(), string.Format("{0} - {1} - {2}", empresa.Id, empresa.Nombre, empresa.NumeroFiscal)));
                }
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchTransportadora(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                List<Transportista> transportistas = uow.TransportistaRepository.GetByDescripcionOrCodePartial(context.SearchValue);

                foreach (var transportista in transportistas)
                {
                    opciones.Add(new SelectOption(transportista.Id.ToString(), transportista.Descripcion));
                }
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchTipoVehiculo(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var tiposVehiculos = uow.TipoVehiculoRepository.GetTipoByDescripcionOrCodePartial(context.SearchValue);

                foreach (var tipoVehiculo in tiposVehiculos)
                {
                    opciones.Add(new SelectOption(tipoVehiculo.Id.ToString(), tipoVehiculo.Tipo));
                }
            }

            return opciones;
        }

        public virtual void CrearAgrupador(Form form, FormSubmitContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                uow.CreateTransactionNumber("DOC320 CrearAgrupador");

                var nuTransaccion = uow.GetTransactionNumber();
                var agrupador = this.CrearDocumentoAgrupador(form, uow, context);

                uow.DocumentoRepository.AddAgrupador(agrupador, nuTransaccion);
                uow.SaveChanges();

                if (context.ButtonId == "BtnConfirmarContinuar")
                {
                    context.Redirect("/documento/DOC330", new List<ComponentParameter>()
                    {
                        new ComponentParameter(){ Id = "nuAgrupador", Value = agrupador.Numero },
                        new ComponentParameter(){ Id = "tpAgrupador", Value = agrupador.Tipo.TipoAgrupador },
                    });
                }

                context.AddSuccessNotification("DOC320_Sec0_Not_Succes", new List<string> { agrupador.Numero, agrupador.Tipo.Descripcion });
            }
        }

        public virtual IDocumentoAgrupador CrearDocumentoAgrupador(Form form, IUnitOfWork uow, FormSubmitContext context)
        {
            //Obtener tipo de agrupador
            var tipoAgrupador = uow.DocumentoRepository.GetDocumentoAgrupadorTipo(form.GetField("tpAgrupador").Value);
            IDocumentoAgrupador agrupador = this._factoryService.CreateDocumentoAgrupador(tipoAgrupador.TipoAgrupador);
            DateTime fechaSalida;
            var culture = this._identity.GetFormatProvider();

            if (DateTime.TryParse(form.GetField("fechaSalida").Value, this._identity.GetFormatProvider(), DateTimeStyles.None, out fechaSalida))
                agrupador.FechaSalida = fechaSalida;

            agrupador.FechaAlta = DateTime.Now;
            agrupador.Tipo = tipoAgrupador;

            bool.TryParse(context.GetParameter("editar"), out bool editMode);

            if (editMode)
                agrupador.Numero = form.GetField("nroAgrupador").Value;
            else
                agrupador.Numero = agrupador.ObtenerNumeroAgrupador(uow);

            agrupador.NumeroLacre = form.GetField("nroLacre").Value;
            agrupador.Cantidad = int.Parse(form.GetField("qtVolumen").Value);
            agrupador.Peso = decimal.Parse(form.GetField("qtPeso").Value, culture);
            agrupador.ValorTotal = decimal.Parse(form.GetField("vlTotal").Value, culture);
            agrupador.Estado = form.GetField("idEstado").Value;
            agrupador.Transportadora = uow.TransportistaRepository.GetTransportista(int.Parse(form.GetField("cdTransportadora").Value));
            agrupador.TipoVehiculo = uow.TipoVehiculoRepository.GetTipo(int.Parse(form.GetField("tipoVehiculo").Value));
            agrupador.PesoLiquido = decimal.Parse(form.GetField("qtPesoLiquido").Value, culture);
            agrupador.Motorista = form.GetField("motorista").Value;
            agrupador.Placa = form.GetField("placa").Value;

            if (int.TryParse(form.GetField("cdEmpresa").Value, out int cdEmpresa))
            {
                agrupador.Empresa = cdEmpresa;
            }

            agrupador.Anexo1 = form.GetField("anexo1").Value;
            agrupador.Anexo2 = form.GetField("anexo2").Value;
            agrupador.Anexo3 = form.GetField("anexo3").Value;
            agrupador.Anexo4 = form.GetField("anexo4").Value;

            if (tipoAgrupador.ManejaPredio)
                agrupador.Predio = form.GetField("nuPredio").Value;

            return agrupador;
        }

        public virtual void InitForm1(Form form, FormInitializeContext context)
        {
            //Inicializar select tipo
            FormField selectTipoAgrupador = form.GetField("tpAgrupador");
            FormField selectPredios = form.GetField("nuPredio");
            FormField fieldNuAgrupado = form.GetField("nroAgrupador");
            FormField fieldEstado = form.GetField("idEstado");

            selectTipoAgrupador.Options = new List<SelectOption>();
            selectPredios.Options = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var tiposAgrupador = uow.DocumentoRepository.GetDocumentoAgrupadorTipos();

                foreach (var tipo in tiposAgrupador)
                {
                    selectTipoAgrupador.Options.Add(new SelectOption(tipo.TipoAgrupador, tipo.Descripcion));
                }

                var predios = uow.PredioRepository.GetPrediosUsuario(_identity.UserId);

                foreach (var predio in predios)
                {
                    selectPredios.Options.Add(new SelectOption(predio.Numero, predio.Descripcion));
                }

                bool.TryParse(context.GetParameter("editar"), out bool editMode);

                if (editMode)
                {
                    string nuAgrupador = context.GetParameter("nuAgrupador");
                    string tpAgrupador = context.GetParameter("tpAgrupador");

                    IDocumentoAgrupador docAgrupador = uow.DocumentoRepository.GetAgrupador(nuAgrupador, tpAgrupador);

                    selectTipoAgrupador.Value = docAgrupador.Tipo.TipoAgrupador;
                    selectTipoAgrupador.ReadOnly = true;

                    fieldEstado.Value = docAgrupador.Estado;

                    fieldNuAgrupado.Value = docAgrupador.Numero;
                    fieldNuAgrupado.ReadOnly = true;

                    form.GetField("nroLacre").Value = docAgrupador.NumeroLacre;
                    form.GetField("fechaSalida").Value = docAgrupador.FechaSalida.ToIsoString();
                    form.GetField("qtVolumen").Value = docAgrupador.Cantidad.ToString();
                    form.GetField("qtPeso").Value = docAgrupador.Peso.ToString();
                    form.GetField("vlTotal").Value = docAgrupador.ValorTotal.ToString();
                    form.GetField("qtPesoLiquido").Value = docAgrupador.PesoLiquido.ToString();
                    form.GetField("motorista").Value = docAgrupador.Motorista;
                    form.GetField("placa").Value = docAgrupador.Placa;
                    form.GetField("anexo1").Value = docAgrupador.Anexo1;
                    form.GetField("anexo2").Value = docAgrupador.Anexo2;
                    form.GetField("anexo3").Value = docAgrupador.Anexo3;
                    form.GetField("anexo4").Value = docAgrupador.Anexo4;

                    if (docAgrupador.Transportadora != null)
                    {
                        form.GetField("cdTransportadora").Options = new List<SelectOption> { new SelectOption(docAgrupador.Transportadora.Id.ToString(), docAgrupador.Transportadora.Descripcion) };
                        form.GetField("cdTransportadora").Value = docAgrupador.Transportadora.Id.ToString();
                    }

                    if (docAgrupador.TipoVehiculo != null)
                    {
                        form.GetField("tipoVehiculo").Options = new List<SelectOption> { new SelectOption(docAgrupador.TipoVehiculo.Id.ToString(), docAgrupador.TipoVehiculo.Tipo) };
                        form.GetField("tipoVehiculo").Value = docAgrupador.TipoVehiculo.Id.ToString();
                    }

                    if (docAgrupador.Empresa != null)
                    {
                        string nmEmpresa = uow.EmpresaRepository.GetNombre(docAgrupador.Empresa ?? -1);
                        form.GetField("cdEmpresa").Options = new List<SelectOption> { new SelectOption(docAgrupador.Empresa.ToString(), nmEmpresa) };
                        form.GetField("cdEmpresa").Value = docAgrupador.Empresa.ToString();
                    }

                    if (docAgrupador.Tipo.ManejaPredio)
                    {
                        context.AddParameter("ManejaPredio", "true");
                        selectPredios.Value = docAgrupador.Predio;
                    }
                    else
                    {
                        context.AddParameter("ManejaPredio", "false");
                    }
                }
                else
                {
                    foreach (var f in form.Fields)
                        f.ReadOnly = false;

                    fieldNuAgrupado.ReadOnly = true;
                    fieldEstado.Value = EstadoDocumentoAgrupador.EDICION;
                }
            }
        }

        public virtual void FormSubmit1(Form form, FormSubmitContext context)
        {
            try
            {
                bool.TryParse(context.GetParameter("editar"), out bool editMode);

                if (editMode)
                {
                    using (var uow = this._uowFactory.GetUnitOfWork())
                    {
                        uow.CreateTransactionNumber("DOC320 Actualizar Agrupador");

                        var nuTransaccion = uow.GetTransactionNumber();
                        var agrupador = this.CrearDocumentoAgrupador(form, uow, context);

                        uow.DocumentoRepository.UpdateAgrupador(agrupador, nuTransaccion);
                        uow.SaveChanges();

                        if (context.ButtonId == "BtnConfirmarContinuar")
                        {
                            context.Redirect("/documento/DOC330", new List<ComponentParameter>()
                            {
                                new ComponentParameter(){ Id = "nuAgrupador", Value = agrupador.Numero },
                                new ComponentParameter(){ Id = "tpAgrupador", Value = agrupador.Tipo.TipoAgrupador },
                            });
                        }

                        context.AddSuccessNotification("DOC320_Sec0_Not_SuccesEd", new List<string> { agrupador.Numero, agrupador.Tipo.Descripcion });
                    }
                }
                else
                {
                    this.CrearAgrupador(form, context);
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
                    context.Redirect("/documento/DOC320", new List<ComponentParameter>() { });
                if (!context.Notifications.Any(c => c.Type == ApplicationNotificationType.Error))
                    context.AddParameter("success", "true");
            }

        }

        public virtual void FormSubmit2(Form form, FormSubmitContext context)
        {
            var nuAgrupador = context.Parameters.FirstOrDefault(p => p.Id == "NU_AGRUPADOR").Value;
            var tpAgrupador = context.Parameters.FirstOrDefault(p => p.Id == "TP_AGRUPADOR").Value;
            var motivoCancelacion = form.Fields.FirstOrDefault(f => f.Id == "motivoCancelacion").Value;

            try
            {
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    uow.CreateTransactionNumber(this._identity.Application);

                    var nuTransaccion = uow.GetTransactionNumber();
                    var agrupador = uow.DocumentoRepository.GetAgrupadorWithDetail(nuAgrupador, tpAgrupador);

                    this._documentoService.ValidarCancelacionIngreso(uow, agrupador);

                    var acciones = uow.DocumentoRepository.GetEstadosAgrupacion(AccionDocumento.Desagrupar);

                    foreach (var doc in agrupador.LineasIngresoAgrupadas)
                    {
                        var estado = acciones.First(a => a.Origen.Id == doc.Estado && a.TipoDocumento == doc.Tipo)?.Destino?.Id;

                        if (string.IsNullOrEmpty(estado))
                            throw new ValidationFailedException("DOC320_Sec0_Not_DesagrupacionNoDefinida", new string[] { doc.Tipo, doc.Estado });

                        doc.Estado = estado;
                        doc.Desagrupar();
                        uow.DocumentoRepository.UpdateIngreso(doc, nuTransaccion);
                    }

                    agrupador.CancelarAgrupador();
                    agrupador.Motivo = motivoCancelacion;
                    uow.DocumentoRepository.UpdateAgrupador(agrupador, nuTransaccion);

                    uow.SaveChanges();
                    context.AddSuccessNotification("DOC320_Sec0_Not_CancelarEnvioOK");
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
                throw new ValidationFailedException("DOC320_Sec0_Not_ErrorCancelarEnvio", new string[] { ex.Message });
            }
        }

        #endregion FORM

        public virtual void AceptarDocumentoIngresoAduanero(IUnitOfWork uow, string nuAgrupador, string tpAgrupador)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var agrupador = uow.DocumentoRepository.GetAgrupadorWithDetail(nuAgrupador, tpAgrupador);
            var agendas = this._documentoService.CrearAgenda(uow, agrupador);
            var acciones = uow.DocumentoRepository.GetEstados(AccionDocumento.FinalizarSinCierreAgenda);

            foreach (var doc in agrupador.LineasIngresoAgrupadas)
            {
                var estado = acciones.FirstOrDefault(a => a.TipoDocumento == doc.Tipo && a.Origen.Id == doc.Estado)?.Destino?.Id;

                if (string.IsNullOrEmpty(estado))
                    throw new ValidationFailedException("DOC320_Sec0_Error_FinalizacionSinCierreAgendaNoDefinida", new string[] { doc.Tipo, doc.Estado });

                var agenda = agendas.FirstOrDefault(a => a.NumeroDocumento == $"{doc.Tipo}{doc.Numero}"
                    && a.TipoDocumento == TipoDocumentoAgendaDb.DocumentoAduanero);

                doc.Estado = estado;
                doc.AprobarDocumento(null, null, agenda.Id);
                doc.Finalizar();

                uow.DocumentoRepository.UpdateIngreso(doc, nuTransaccion);
            }

            agrupador.EnviarAgrupador();

            uow.DocumentoRepository.UpdateAgrupador(agrupador, nuTransaccion);
            uow.SaveChanges();
        }

        public virtual void FinalizarDocumentoEgresoAduanero(IUnitOfWork uow, string nuAgrupador, string tpAgrupador)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var agrupador = uow.DocumentoRepository.GetAgrupadorWithDetail(nuAgrupador, tpAgrupador);

            agrupador.EnviarAgrupador();
            uow.DocumentoRepository.UpdateAgrupador(agrupador, nuTransaccion);

            uow.SaveChanges();
        }
    }
}



