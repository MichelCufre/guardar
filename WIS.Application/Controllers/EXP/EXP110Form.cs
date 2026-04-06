using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Expedicion;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Expedicion.EXP110EmpaquetadoPicking;
using WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Tracking.Models;
using WIS.Domain.Validation;
using WIS.Exceptions;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.EXP
{
    public class EXP110Form : AppController
    {
        protected readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;
        protected readonly ITaskQueueService _taskQueue;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IParameterService _parameterService;
        protected readonly IPrintingService _printingService;
        protected readonly ITrackingService _trackingService;
        protected readonly IBarcodeService _barcodeService;
        protected readonly ICodigoMultidatoService _codigoMultidatoService;
        protected readonly EmpaquetadoPickingLogic _logic;

        public EXP110Form(
            ISecurityService security,
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IFormValidationService formValidationService,
            ITaskQueueService taskQueue,
            ITrafficOfficerService concurrencyControl,
            IParameterService parameterService,
            IPrintingService printingService,
            ITrackingService trackingService,
            IBarcodeService barcodeService,
            ICodigoMultidatoService codigoMultidatoService)
        {
            this._security = security;
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._formValidationService = formValidationService;
            this._taskQueue = taskQueue;
            this._concurrencyControl = concurrencyControl;
            this._parameterService = parameterService;
            this._printingService = printingService;
            this._trackingService = trackingService;
            this._barcodeService = barcodeService;
            this._codigoMultidatoService = codigoMultidatoService;

            this._logic = new EmpaquetadoPickingLogic(printingService, trackingService, barcodeService, identity, Logger);
        }

        public override PageContext PageLoad(PageContext data)
        {
            data.Parameters.RemoveAll(p => p.Id == "PermiteCerrarEtiqueta");

            var permiteCerrar = this._security.IsUserAllowed("EXP110_frm1_btn_CerrarEtiqueta");

            data.Parameters.Add(new ComponentParameter("PermiteCerrarEtiqueta", permiteCerrar.ToString().ToLower()));

            return base.PageLoad(data);
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            if (this._identity.Predio == GeneralDb.PredioSinDefinir)
            {
                form.GetField("contenedorDestino").Disabled = true;

                return form;
            }

            form.Fields.ForEach(x => { x.Value = string.Empty; });

            form.GetField("contenedorDestino").ReadOnly = false;
            form.GetField("contenedorOrigen").ReadOnly = true;
            form.GetField("codigoBarraProducto").ReadOnly = true;

            form.GetField("modalidad").Options = new List<SelectOption>()
            {
                new SelectOption("Normal", MesaEmpaqueModalidadConstants.ModalidadNormal),
                new SelectOption("Unidad", MesaEmpaqueModalidadConstants.ModalidadUnidad)
            };

            form.GetField("modalidad").Value = "Normal";

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            switch (context.ButtonId)
            {
                case "contenedorDestino": ContenedorDestinoLogic(form, context); break;
                case "contenedorOrigen": ContenedorOrigenLogic(form, context); break;
                case "contenedorOrigenConfirmado": ContenedorOrigenConfirmadoLogic(form, context); break;
                case "codigoBarraProducto": CodigoBarraProductoLogic(form, context); break;
                case "BtnImprimirResumenEmpaquetado": ImprimirResumenEmpaquetado(form, context); break;
                case "BtnImprimirResumenEmpaquetadoBulto": ImprimirResumenEmpaquetadoBulto(form, context); break;
                case "BtnLimpiarCamposFormulario": LimpiarFormulario(form, context); break;
                case "BtnLimpiarTodoFormulario": LimpiarFormulario(form, context, true); break;
                case "BtnConfirmarEmpaquetarTodo": PreConfirmarEmpaquetarTodo(form, context); break;
                case "contenedorOrigenConfirmadoEmpaquetarTodo": EmpaquetarContenedorCompletamente(form, context); break;
                case "BtnPreConfirmarCerrarEtiqueta": PreConfirmarCerrarEtiqueta(form, context); break;
                case "BtnConfirmarCerrarEtiquetaEmpaque": ConfirmarCerrarEtiquetaEmpaque(form, context); break;

                default:
                    return form;
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ConfiguracionInicial confInicial = null;
            var contenedorDestinoDataJSON = context.GetParameter("CONT_DESTINO_DATA");
            var contenedorDestinoData = !string.IsNullOrEmpty(contenedorDestinoDataJSON) ? JsonConvert.DeserializeObject<ContenedorDestinoData>(contenedorDestinoDataJSON) : null;

            if (contenedorDestinoData == null && context.ButtonId == "contenedorOrigen" && !string.IsNullOrEmpty(form.GetField("contenedorOrigen").Value) && (form.GetField("contenedorDestino").Value != "(NUEVO)" && !string.IsNullOrEmpty(form.GetField("contenedorDestino").Value)))
            {
                int nuContenedorDestino = form.GetField("contenedorDestino").Value.ToNumber<int>();
                int nuPreparacionDestino = form.GetField("preparacionDestino").Value.ToNumber<int>();
                contenedorDestinoData = uow.EmpaquetadoPickingRepository.GetContenedorDestinoData(nuContenedorDestino, nuPreparacionDestino);
            }
            if (!string.IsNullOrEmpty(context.GetParameter("CONF_INICIAL")))
                confInicial = JsonConvert.DeserializeObject<ConfiguracionInicial>(context.GetParameter("CONF_INICIAL"));

            return this._formValidationService.Validate(new EXP110FormValidationModule(uow, this._identity, confInicial, contenedorDestinoData, context.IsSubmitting, context.ButtonId, _concurrencyControl, _parameterService, _printingService, _trackingService, _barcodeService, _codigoMultidatoService), form, context);
        }

        #region Metodos Auxiliares

        public virtual Form ContenedorDestinoLogic(Form form, FormSubmitContext context)
        {
            var contenedorDestino = form.GetField("contenedorDestino").Value;

            if (string.IsNullOrEmpty(contenedorDestino))
            {
                form.GetField("contenedorDestino").Value = "(NUEVO)";
                form.GetField("idExternoContenedorDestino").Value = string.Empty;

                context.AddOrUpdateParameter("CONTENEDOR_DESTINO_NUEVO", "S");
                context.AddOrUpdateParameter("VL_COMPARTE_CONTENEDOR_ENTREGA_DESTINO", null);
            }
            else
            {
                using var uow = this._uowFactory.GetUnitOfWork();

                var dataContenedorDestino = _logic.GetContenedorDestinoData(uow, contenedorDestino);

                var preparacion = uow.PreparacionRepository.GetPreparacionPorNumero(dataContenedorDestino.NumeroPreparacion);

                if (!uow.FuncionarioRepository.AnyFuncionarioPermisionByEmpresa(preparacion.Empresa.Value, _identity.UserId))
                    throw new ValidationFailedException("General_Sec0_Error_UsuarioSinPermisosParaEmpresa", new string[] { preparacion.Empresa.Value.ToString() });

                form.GetField("contenedorDestino").Value = dataContenedorDestino.NumeroContenedor.ToString();
                form.GetField("idExternoContenedorDestino").Value = dataContenedorDestino.IdExternoContenedor;
                form.GetField("preparacionDestino").Value = dataContenedorDestino.NumeroPreparacion.ToString();
                form.GetField("pesoEmpaque").Value = uow.ContenedorRepository.GetPesoTotalContenedor(dataContenedorDestino.NumeroPreparacion, dataContenedorDestino.NumeroContenedor).ToString();

                var cantPedidos = uow.EmpaquetadoPickingRepository.GetCantPedidosContenedor(dataContenedorDestino.NumeroContenedor, dataContenedorDestino.NumeroPreparacion, out int cantCLientes, out string codigoDescripcionCliente);

                if (cantCLientes > 1)
                {
                    form.GetField("numeroPedido").Value = "(VARIOS)";
                    form.GetField("codigoCliente").Value = "(VARIOS)";
                }
                else if (cantPedidos > 1)
                {
                    form.GetField("numeroPedido").Value = "(VARIOS)";
                    form.GetField("codigoCliente").Value = codigoDescripcionCliente;
                }
                else
                {
                    form.GetField("codigoCliente").Value = $"{dataContenedorDestino.CodigoCliente} - {dataContenedorDestino.DescripcionCliente}";
                    form.GetField("numeroPedido").Value = dataContenedorDestino.NumeroPedido;
                    form.GetField("descripcionEntrega").Value = dataContenedorDestino.Direccion;
                    form.GetField("tipoExpedicion").Value = dataContenedorDestino.TipoExpedicion;
                    form.GetField("codigoRuta").Value = dataContenedorDestino.CodigoRota;
                    form.GetField("fechaEntrega").Value = dataContenedorDestino.FechaEntrega.ToString();
                    form.GetField("descripcionAnexo4").Value = dataContenedorDestino.Anexo4;
                }

                context.AddOrUpdateParameter("CONTENEDOR_DESTINO_NUEVO", "N");
                context.AddOrUpdateParameter("VL_COMPARTE_CONTENEDOR_ENTREGA_DESTINO", dataContenedorDestino.CompartContenedorEntrega);
                context.AddOrUpdateParameter("CONT_DESTINO_DATA", JsonConvert.SerializeObject(dataContenedorDestino));
            }

            form.GetField("contenedorDestino").ReadOnly = true;
            form.GetField("contenedorOrigen").ReadOnly = false;
            form.GetField("codigoBarraProducto").ReadOnly = true;

            return form;
        }

        public virtual Form ContenedorOrigenLogic(Form form, FormSubmitContext context)
        {
            if (string.IsNullOrEmpty(form.GetField("contenedorOrigen").Value))
            {
                form.Fields.ForEach(x => { x.Value = ""; });

                form.GetField("contenedorDestino").ReadOnly = false;
                form.GetField("contenedorOrigen").ReadOnly = true;
                form.GetField("codigoBarraProducto").ReadOnly = true;

                if (form.GetField("contenedorDestino").Value != "(NUEVO)")
                    DesbloquearContenedor(form, context, contenedorDestino: true);

                context.AddOrUpdateParameter("AUX_CONT_ORIGEN_NU_CONTENEDOR", null);
                context.AddOrUpdateParameter("AUX_CONT_ORIGEN_NU_PREPARACION", null);
                context.AddOrUpdateParameter("AUX_CONT_ORIGEN_ID_EXTERNO_CONTENEDOR", null);
                context.AddOrUpdateParameter("CONT_DESTINO_DATA", null);
                context.AddOrUpdateParameter("CONT_ORIGEN_DATA", null);
            }
            else if (form.GetField("contenedorDestino").Value == "(NUEVO)")
            {
                using var uow = this._uowFactory.GetUnitOfWork();

                if (_logic.PedidoIniciadosEnContenedores(uow, form.GetField("contenedorOrigen").Value, out string idsExternoContenedores))
                {
                    context.AddOrUpdateParameter("CONFIRMATION_MSG_ARG", idsExternoContenedores);
                    context.AddOrUpdateParameter("CONFIRMATION_MSG", "S");

                    return form;
                }
                else
                    context.AddOrUpdateParameter("CONFIRMATION_MSG", "N");
            }
            else
                context.AddOrUpdateParameter("CONFIRMATION_MSG", "N");

            return form;
        }

        public virtual Form PreConfirmarEmpaquetarTodo(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (_logic.ContenedorTieneUnPedido(uow, form.GetField("contenedorOrigen").Value, out int contenedorOrigenNumero, out string contenedorOrigenUbicacion, out int cdEmpresa, out string cdCliente, out int nuPreparacion, out string nuPedido))
            {
                context.AddOrUpdateParameter("CONFIRMATION_MSG_EMPAQUETARTODO", "S");
            }
            else
            {
                context.AddErrorNotification("EXP110_form1_Msg_ContenedorNoValidoEmpaquetarTodo");
            }

            return form;
        }

        public virtual Form EmpaquetarContenedorCompletamente(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            (bool Notificar, int? Camion) interfazFacturacion = (false, null);

            uow.CreateTransactionNumber("EXP110EmpaquetarContenedorCompletamente");
            uow.BeginTransaction();

            try
            {
                var confInicial = context.GetParameter("CONF_INICIAL");
                string cdCliente;
                string nuPedido;
                int cdEmpresa;
                bool isContenedorOrigenVacio = false;

                Contenedor contenedorDestino = null;

                var datosTrackingOrigen = new EgresoContenedorTracking();
                var datosTrackingDestino = new EgresoContenedorTracking();

                if (_logic.ContenedorTieneUnPedido(uow, form.GetField("contenedorOrigen").Value, out int contenedorOrigenNumero, out string contenedorOrigenUbicacion, out cdEmpresa, out cdCliente, out int nuPreparacion, out nuPedido))
                {
                    var configuracionInicial = JsonConvert.DeserializeObject<ConfiguracionInicial>(confInicial);
                    var contenedorOrigen = uow.ContenedorRepository.GetContenedor(nuPreparacion, contenedorOrigenNumero);
                    var data = uow.EmpaquetadoPickingRepository.GetDatosClientePedidoOriginal(contenedorOrigenNumero, nuPreparacion);

                    var camionContenedorOrigen = uow.ContenedorRepository.GetCamionAsignado(contenedorOrigenNumero, nuPreparacion);

                    datosTrackingOrigen.Egreso = camionContenedorOrigen;
                    datosTrackingOrigen.Contenedor = contenedorOrigen;

                    contenedorDestino = GetOrCreateContenedorDestino(uow, form, context, nuPreparacion, configuracionInicial, contenedorOrigen, data);

                    _logic.EmpaquetarTodosProductosContenedor(uow, contenedorOrigenNumero, contenedorDestino.Numero, nuPreparacion, contenedorDestino.NumeroPreparacion, cdCliente, cdEmpresa, nuPedido, contenedorOrigenUbicacion, contenedorDestino.Ubicacion, out isContenedorOrigenVacio);

                    uow.SaveChanges();
                    datosTrackingOrigen.Baja = isContenedorOrigenVacio;

                    var camionContenedorDestino = uow.ContenedorRepository.GetCamionAsignado(contenedorDestino.Numero, contenedorDestino.NumeroPreparacion);

                    datosTrackingDestino.Egreso = camionContenedorDestino;
                    datosTrackingDestino.Contenedor = contenedorDestino;
                    datosTrackingDestino.Baja = false;

                    var isEnderecoEquipoManual = uow.EquipoRepository.GetEquipoManualByEndereco(contenedorOrigenUbicacion, out Equipo equipoContenedor);
                    if (isEnderecoEquipoManual && !uow.EmpaquetadoPickingRepository.AnyContenedoresEquipo(contenedorOrigenUbicacion))
                    {
                        uow.EquipoRepository.ModificarEquipo(equipoContenedor, _identity.UserId, true);
                    }

                    _logic.GenerarEgresoYFacturacion(uow, form, context, contenedorDestino, contenedorOrigenNumero, nuPreparacion, cdCliente, 
                                                    cdEmpresa, nuPedido, isContenedorOrigenVacio, "EmpaquetarContenedorCompletamente",
                                                    camionContenedorOrigen, camionContenedorDestino, out interfazFacturacion);

                    if (form.GetField("contenedorDestino").Value == "(NUEVO)")
                    {
                        form.GetField("codigoCliente").Value = $"{data.CodigoCliente} - {data.DescripcionCliente}";
                        form.GetField("numeroPedido").Value = data.NumeroPedido;
                        form.GetField("descripcionEntrega").Value = data.Direccion;
                        form.GetField("tipoExpedicion").Value = data.TipoExpedicion;
                        form.GetField("tipoPedido").Value = data.TipoPedido;
                        form.GetField("codigoRuta").Value = GetRuta(uow, data.CodigoRuta.ToString());
                        form.GetField("fechaEntrega").Value = data?.FechaEntrega == null ? "" : data?.FechaEntrega.ToString();
                        form.GetField("descripcionAnexo4").Value = data.Anexo4;
                        form.GetField("preparacionDestino").Value = contenedorDestino.NumeroPreparacion.ToString();
                        form.GetField("contenedorDestino").Value = contenedorDestino.Numero.ToString();
                        form.GetField("idExternoContenedorDestino").Value = contenedorDestino.IdExterno;

                        context.AddOrUpdateParameter("AUX_PREPARACION_NUEVO", contenedorDestino.NumeroPreparacion.ToString());
                        context.AddOrUpdateParameter("AUX_CONTENEDOR_DESTINO", JsonConvert.SerializeObject(contenedorDestino));
                    }

                    form.GetField("pesoEmpaque").Value = uow.ContenedorRepository.GetPesoTotalContenedor(contenedorDestino.NumeroPreparacion, contenedorDestino.Numero).ToString();
                    form.GetField("contenedorOrigen").Value = string.Empty;
                    form.GetField("idExternoContenedorOrigen").Value = string.Empty;

                    context.AddSuccessNotification("EXP110_form1_Msg_ContenedorEmpaquetadoCompletamente");
                }
                else
                    context.AddErrorNotification("EXP110_form1_Msg_ContenedorNoValidoEmpaquetarTodo");

                form.GetField("contenedorOrigen").ReadOnly = false;
                form.GetField("codigoBarraProducto").ReadOnly = true;

                context.AddParameter("EMPAQUETARTODO_COMPLETADO", "S");

                uow.SaveChanges();
                uow.Commit();

                try
                {                    
                    _logic.ImprimirEtiqueta(uow, _identity.UserId, confInicial, null, contenedorDestino, cdCliente, cdEmpresa, nuPedido);

                    context.AddSuccessNotification("EXP110SelecProd_form_Msg_SeEnvioEtiquetaImpresora", new List<string> { contenedorDestino.TipoContenedor, contenedorDestino.IdExterno });
                    context.AddOrUpdateParameter("AUX_CONT_IMPRESION", contenedorDestino.Numero.ToString());
                }
                catch (Exception ex)
                {
                    context.AddErrorNotification("EXP110SelecProd_form_Msg_FalloEnvioEtiquetaImpresora", new List<string> { contenedorDestino.TipoContenedor, contenedorDestino.IdExterno });
                    context.AddOrUpdateParameter("AUX_CONT_IMPRESION", contenedorDestino.Numero.ToString());
                }

                _trackingService.RegularizarEgresosMesaEmpaque(uow, datosTrackingOrigen, datosTrackingDestino);
                uow.SaveChanges();

                if (interfazFacturacion.Notificar && _taskQueue.IsEnabled())
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.Facturacion, interfazFacturacion.Camion.ToString());
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }
            return form;
        }

        public virtual Contenedor GetOrCreateContenedorDestino(IUnitOfWork uow, Form form, FormSubmitContext context, int nuPreparacion, ConfiguracionInicial configuracionInicial, Contenedor contenedorOrigen, DatosClientePedidoOriginal data)
        {
            Contenedor contenedorDestino;

            if (form.GetField("contenedorDestino").Value == "(NUEVO)")
            {
                var tipoContenedor = BarcodeDb.TIPO_CONTENEDOR_W;
                string idExterno;
                string codigoBarras;

                do
                {
                    idExterno = uow.ContenedorRepository.GetUltimaSecuenciaTipoContenedor(tipoContenedor).ToString();
                    codigoBarras = _barcodeService.GenerateBarcode(idExterno, tipoContenedor);
                }
                while (uow.ContenedorRepository.ExisteContenedorActivoByCodigoBarras(codigoBarras));

                contenedorDestino = new Contenedor
                {
                    NumeroPreparacion = nuPreparacion,
                    Numero = uow.ContenedorRepository.GetNextNuContenedor(),
                    TipoContenedor = tipoContenedor,
                    Estado = EstadoContenedor.EnPreparacion,
                    Ubicacion = configuracionInicial.Ubicacion,
                    CodigoSubClase = contenedorOrigen.CodigoSubClase,
                    FechaAgregado = DateTime.Now,
                    IdContenedorEmpaque = "S",
                    CantidadBulto = uow.ContenedorRepository.GetQtBulto(nuPreparacion, data.CompartContenedorEntrega, data.CodigoCliente, data.NumeroPedido, data.Direccion),
                    NumeroTransaccion = uow.GetTransactionNumber(),
                    IdExterno = idExterno,
                    CodigoBarras = codigoBarras,
                };

                uow.ContenedorRepository.AddContenedor(contenedorDestino);
            }
            else
            {
                var contDestinoData = context.GetParameter("CONT_DESTINO_DATA");
                if (contDestinoData == null)
                {
                    var contDestino = form.GetField("contenedorDestino").Value.ToNumber<int>();
                    var prepDestino = form.GetField("preparacionDestino").Value.ToNumber<int>();
                    var contDestinoDataParam = uow.EmpaquetadoPickingRepository.GetContenedorDestinoData(contDestino, prepDestino);

                    context.AddOrUpdateParameter("CONT_DESTINO_DATA", JsonConvert.SerializeObject(contDestinoDataParam));
                    contDestinoData = context.GetParameter("CONT_DESTINO_DATA");
                }

                contenedorDestino = _logic.GetContenedorEmpaquetado(uow, contDestinoData);
            }

            return contenedorDestino;
        }

        public virtual Form ContenedorOrigenConfirmadoLogic(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            context.AddOrUpdateParameter("CONFIRMATION_MSG", null);

            var nuevoContenedor = true;
            var contDestino = -1;
            var prepDestino = -1;

            var data = _logic.GetDatosPedidoOriginal(uow, form.GetField("contenedorOrigen").Value, out Contenedor contenedorOrigen);
            if (contenedorOrigen == null)
            {
                form.GetField("contenedorOrigen").Value = "";
                context.AddErrorNotification("EXP110ImpresionBulto_form1_Error_SituacionContenedorOrigen");
                return form;
            }

            var contDestinoData = context.GetParameter("CONT_DESTINO_DATA");

            if (!string.IsNullOrEmpty(contDestinoData) && form.GetField("contenedorDestino").Value != "(NUEVO)")
            {
                nuevoContenedor = false;
                contDestino = form.GetField("contenedorDestino").Value.ToNumber<int>();
                prepDestino = form.GetField("preparacionDestino").Value.ToNumber<int>();

                if (contenedorOrigen.NumeroPreparacion != prepDestino)
                    _logic.ValidarMultiPreparacion(uow, contenedorOrigen, contDestino, prepDestino);
            }

            var preparacion = uow.PreparacionRepository.GetPreparacionPorNumero(contenedorOrigen.NumeroPreparacion);
            if (!uow.FuncionarioRepository.AnyFuncionarioPermisionByEmpresa(preparacion.Empresa.Value, _identity.UserId))
                throw new ValidationFailedException("General_Sec0_Error_UsuarioSinPermisosParaEmpresa", new string[] { preparacion.Empresa.Value.ToString() });

            form.GetField("contenedorOrigen").Value = contenedorOrigen.Numero.ToString();
            form.GetField("idExternoContenedorOrigen").Value = contenedorOrigen.IdExterno;

            if (data != null)
            {
                form.GetField("codigoCliente").Value = $"{data.CodigoCliente} - {data.DescripcionCliente}";
                form.GetField("numeroPedido").Value = data.NumeroPedido;
                form.GetField("descripcionEntrega").Value = data.Direccion;
                form.GetField("tipoExpedicion").Value = data.TipoExpedicion;
                form.GetField("tipoPedido").Value = data.TipoPedido;
                form.GetField("codigoRuta").Value = GetRuta(uow, data.CodigoRuta.ToString());
                form.GetField("fechaEntrega").Value = data?.FechaEntrega == null ? "" : data?.FechaEntrega.ToString();
                form.GetField("descripcionAnexo4").Value = data.Anexo4;

                context.AddOrUpdateParameter("CONT_ORIGEN_DATA", JsonConvert.SerializeObject(data));
            }
            else
            {
                var cantPedidos = uow.EmpaquetadoPickingRepository.GetCantPedidosContenedor(contenedorOrigen.Numero, contenedorOrigen.NumeroPreparacion, out int cantCLientes, out string codigoDescripcionCliente);

                if (cantCLientes > 1)
                {
                    form.GetField("numeroPedido").Value = "(VARIOS)";
                    form.GetField("codigoCliente").Value = "(VARIOS)";
                }
                else if (cantPedidos > 1)
                {
                    form.GetField("numeroPedido").Value = "(VARIOS)";
                    form.GetField("codigoCliente").Value = codigoDescripcionCliente;
                }
            }

            context.AddOrUpdateParameter("AUX_CONT_ORIGEN_NU_CONTENEDOR", contenedorOrigen.Numero.ToString());
            context.AddOrUpdateParameter("AUX_CONT_ORIGEN_ID_EXTERNO_CONTENEDOR", contenedorOrigen.IdExterno);
            context.AddOrUpdateParameter("AUX_CONT_ORIGEN_NU_PREPARACION", contenedorOrigen.NumeroPreparacion.ToString());

            context.AddOrUpdateParameter("GO_CODIGO_BARRA_PROD", "S");

            form.GetField("contenedorOrigen").ReadOnly = true;
            form.GetField("codigoBarraProducto").ReadOnly = false;

            if (!nuevoContenedor)
            {
                var contDestinoDataParam = uow.EmpaquetadoPickingRepository.GetContenedorDestinoData(contDestino, prepDestino);
                context.AddOrUpdateParameter("CONT_DESTINO_DATA", JsonConvert.SerializeObject(contDestinoDataParam));
                form.GetField("pesoEmpaque").Value = uow.ContenedorRepository.GetPesoTotalContenedor(prepDestino, contDestino).ToString();
            }

            return form;
        }

        public virtual Form CodigoBarraProductoLogic(Form form, FormSubmitContext context)
        {
            if (string.IsNullOrEmpty(form.GetField("codigoBarraProducto").Value))
            {
                form.Fields.ForEach(x =>
                {
                    if (x.Id != "contenedorDestino" && x.Id != "idExternoContenedorDestino")
                        x.Value = string.Empty;
                });

                form.GetField("contenedorOrigen").ReadOnly = false;
                form.GetField("codigoBarraProducto").ReadOnly = true;

                DesbloquearContenedor(form, context);

                context.AddOrUpdateParameter("AUX_CONT_ORIGEN_NU_CONTENEDOR", null);
                context.AddOrUpdateParameter("AUX_CONT_ORIGEN_NU_PREPARACION", null);
                context.AddOrUpdateParameter("AUX_CONT_ORIGEN_ID_EXTERNO_CONTENEDOR", null);
            }
            else
            {
                using var uow = this._uowFactory.GetUnitOfWork();

                DatosClientePedidoOriginal contenedorOrigen = null;

                var contenedorOrigenJSON = context.GetParameter("CONT_ORIGEN_DATA");
                if (!string.IsNullOrEmpty(contenedorOrigenJSON))
                    contenedorOrigen = JsonConvert.DeserializeObject<DatosClientePedidoOriginal>(contenedorOrigenJSON);

                var nuContenedorOrigen = context.GetParameter("AUX_CONT_ORIGEN_NU_CONTENEDOR").ToNumber<int>();
                var nuPreparacionOrigen = context.GetParameter("AUX_CONT_ORIGEN_NU_PREPARACION").ToNumber<int>();
                var preparacionDestino = uow.PreparacionRepository.GetPreparacionPorNumero(nuPreparacionOrigen);

                var empresa = contenedorOrigen != null ? contenedorOrigen.Empresa : preparacionDestino.Empresa ?? -1;

                var codigoBarras = form.GetField("codigoBarraProducto").Value = form.GetField("codigoBarraProducto").Value.ToUpper();

                var ais = GetAIsProducto(uow, form, context, empresa, nuPreparacionOrigen);

                if (TryGetAIValue(ais, "codigoBarraProducto", out string cdProducto))
                {
                    codigoBarras = cdProducto;
                }

                var codigoBarrasProducto = uow.ProductoCodigoBarraRepository.GetProductoCodigoBarra(codigoBarras, empresa);

                var producto = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(empresa, codigoBarrasProducto.IdProducto);

                form.GetField("descripcionProducto").Value = producto.Descripcion;
                context.AddOrUpdateParameter("AUX_PROD_LEIDO", JsonConvert.SerializeObject(producto));

                if (form.GetField("contenedorDestino").Value != "(NUEVO)")
                {
                    var nuContenedorDestino = form.GetField("contenedorDestino").Value.ToNumber<int>();
                    var nuPreparacionDestino = form.GetField("preparacionDestino").Value.ToNumber<int>();

                    var pedidoDestino = uow.PedidoRepository.GetPedidosByPreparacionAndContenedor(nuPreparacionDestino, nuContenedorDestino).FirstOrDefault();

                    var compatibilidadPedidosContenedorOrigen = uow.EmpaquetadoPickingRepository.GetCompatibilidadContenedores(nuContenedorOrigen, nuPreparacionOrigen);

                    if (!compatibilidadPedidosContenedorOrigen.Any(c => c.CompartContenedorEntrega == pedidoDestino?.ComparteContenedorEntrega))
                    {
                        throw new ValidationFailedException("EXP110_form1_Error_PedidoNoPermiteCompartirContenedor");
                    }

                    var contenedorDestino = uow.ContenedorRepository.GetContenedor(nuPreparacionDestino, nuContenedorDestino);
                    context.AddOrUpdateParameter("AUX_CONTENEDOR_DESTINO_JSON", JsonConvert.SerializeObject(contenedorDestino));

                    var contDestinoDataParam = uow.EmpaquetadoPickingRepository.GetContenedorDestinoData(contenedorDestino.Numero, contenedorDestino.NumeroPreparacion);
                    context.AddOrUpdateParameter("CONT_DESTINO_DATA", JsonConvert.SerializeObject(contDestinoDataParam));

                }
            }

            return form;
        }

        public virtual Dictionary<string, object> GetAIsProducto(IUnitOfWork uow, Form form, FormSubmitContext context, int empresa, int nuPreparacion)
        {
            var fechaEntrega = form.GetField("fechaEntrega").Value;
            var pesoEmpaque = form.GetField("pesoEmpaque").Value;

            if (!string.IsNullOrEmpty(fechaEntrega))
            {
                fechaEntrega = DateTime.Parse(fechaEntrega, _identity.GetFormatProvider()).ToString(CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(pesoEmpaque))
            {
                pesoEmpaque = decimal.Parse(pesoEmpaque, _identity.GetFormatProvider()).ToString(CultureInfo.InvariantCulture);
            }

            var resultado = _codigoMultidatoService.GetAIs(uow, "EXP110", form.GetField("codigoBarraProducto").Value, new Dictionary<string, string>
            {
                ["USERID"] = _identity.UserId.ToString(),
                ["NU_PREDIO"] = _identity.Predio,
                ["NU_PREPARACION"] = nuPreparacion.ToString(),
                ["contenedorOrigen"] = form.GetField("contenedorOrigen").Value,
                ["contenedorDestino"] = form.GetField("contenedorDestino").Value,
                ["pesoEmpaque"] = pesoEmpaque,
                ["descripcionEntrega"] = form.GetField("descripcionEntrega").Value,
                ["descripcionAnexo4"] = form.GetField("descripcionAnexo4").Value,
                ["fechaEntrega"] = fechaEntrega,
                ["numeroPedido"] = form.GetField("numeroPedido").Value,
                ["codigoCliente"] = form.GetField("codigoCliente").Value,
                ["codigoRuta"] = form.GetField("codigoRuta").Value,
                ["tipoPedido"] = form.GetField("tipoPedido").Value,
                ["tipoExpedicion"] = form.GetField("tipoExpedicion").Value,
                ["CD_CAMPO"] = "codigoBarraProducto",
            }, empresa).GetAwaiter().GetResult();

            return resultado?.AIs;
        }

        public virtual bool TryGetAIValue(Dictionary<string, object> ais, string fieldId, out string fieldValue)
        {
            if (ais != null && ais.ContainsKey(fieldId))
            {
                var value = ais[fieldId];

                if (value is DateTime)
                {
                    fieldValue = ((DateTime)ais[fieldId]).ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", _identity.GetFormatProvider());
                }
                else
                {
                    fieldValue = ais[fieldId].ToString();
                }

                return true;
            }

            fieldValue = null;

            return false;
        }

        public virtual Form ImprimirResumenEmpaquetadoBulto(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string confInicial = context.GetParameter("CONF_INICIAL");
            string contenedorBultoJSON = context.GetParameter("AUX_DATOS_CONT_BULTO");

            if (!string.IsNullOrEmpty(contenedorBultoJSON))
            {
                DatosContenedorBulto datosContenedorBulto = JsonConvert.DeserializeObject<DatosContenedorBulto>(contenedorBultoJSON);
                Contenedor contenedor = uow.ContenedorRepository.GetContenedor(datosContenedorBulto.NumeroPreparacion, datosContenedorBulto.NumeroContenedor);

                _logic.IsPedidoCompletoImprimirResumen(uow, _identity.UserId, confInicial, contenedor, datosContenedorBulto.CodigoCliente, datosContenedorBulto.Empresa, datosContenedorBulto.NumeroPedido);
                context.AddSuccessNotification("EXP110_form1_Msg_SeEnvioAImprimirResumen");
            }

            return form;
        }

        public virtual Form ImprimirResumenEmpaquetado(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var pedProdContSelect = context.GetParameter("AUX_ROW_SELECTED_PEDPRODCONT");
            var pedProdLoteSelect = context.GetParameter("AUX_ROW_SELECTED_PEDPRODLOTE");
            var contenedorDestinoJson = context.GetParameter("AUX_CONTENEDOR_DESTINO");
            Contenedor contenedorDestino = JsonConvert.DeserializeObject<Contenedor>(contenedorDestinoJson);
            string dataClientePedido = context.GetParameter("CONT_ORIGEN_DATA");
            DatosClientePedidoOriginal data = JsonConvert.DeserializeObject<DatosClientePedidoOriginal>(dataClientePedido);
            string nuPedido = pedProdContSelect != null ? pedProdContSelect.Split('$')[3] : data.NumeroPedido;
            string cdCliente = pedProdContSelect != null ? pedProdContSelect.Split('$')[4] : data.CodigoCliente;
            string confInicial = context.GetParameter("CONF_INICIAL");

            if (_logic.IsPedidoCompleto(uow, data.Empresa, cdCliente, nuPedido, out Error _))
            {
                _logic.IsPedidoCompletoImprimirResumen(uow, _identity.UserId, confInicial, contenedorDestino, cdCliente, data.Empresa, nuPedido);
                context.AddSuccessNotification("EXP110_form1_Msg_SeEnvioAImprimirResumen");

                form.Fields.ForEach(x =>
                {
                    if (x.Id != "contenedorDestino" && x.Id != "idExternoContenedorDestino")
                        x.Value = string.Empty;
                });

                form.GetField("contenedorOrigen").ReadOnly = false;
                form.GetField("codigoBarraProducto").ReadOnly = true;

                context.AddOrUpdateParameter("AUX_CONT_ORIGEN_NU_CONTENEDOR", null);
                context.AddOrUpdateParameter("AUX_CONT_ORIGEN_NU_PREPARACION", null);
                context.AddOrUpdateParameter("AUX_CONT_ORIGEN_ID_EXTERNO_CONTENEDOR", null);
            }

            return form;
        }

        public virtual Form LimpiarFormulario(Form form, FormSubmitContext context, bool cleanAll = false)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (cleanAll)
            {
                form.Fields.ForEach(x =>
                {
                    if (x.Id != "modalidad")
                        x.Value = string.Empty;
                });

                form.GetField("contenedorDestino").ReadOnly = false;
                form.GetField("contenedorOrigen").ReadOnly = true;
                form.GetField("codigoBarraProducto").ReadOnly = true;

                DesbloquearContenedor(form, context, todos: true);

                context.AddOrUpdateParameter("AUX_CONT_ORIGEN_NU_CONTENEDOR", null);
                context.AddOrUpdateParameter("AUX_CONT_ORIGEN_NU_PREPARACION", null);
                context.AddOrUpdateParameter("AUX_CONT_ORIGEN_ID_EXTERNO_CONTENEDOR", null);

            }
            else
            {
                form.Fields.ForEach(x =>
                {
                    if (x.Id != "contenedorDestino" && x.Id != "idExternoContenedorDestino" && x.Id != "preparacionDestino" && x.Id != "modalidad")
                        x.Value = string.Empty;
                });

                form.GetField("contenedorOrigen").ReadOnly = false;
                form.GetField("codigoBarraProducto").ReadOnly = true;

                DesbloquearContenedor(form, context);

                context.AddOrUpdateParameter("AUX_CONT_ORIGEN_NU_CONTENEDOR", null);
                context.AddOrUpdateParameter("AUX_CONT_ORIGEN_NU_PREPARACION", null);
                context.AddOrUpdateParameter("AUX_CONT_ORIGEN_ID_EXTERNO_CONTENEDOR", null);

                ContenedorDestinoData contenedorDestino = !string.IsNullOrEmpty(context.GetParameter("CONT_DESTINO_DATA")) ?
                                                     JsonConvert.DeserializeObject<ContenedorDestinoData>(context.GetParameter("CONT_DESTINO_DATA")) :
                                                     null;

                if (contenedorDestino != null)
                {
                    form.GetField("pesoEmpaque").Value = uow.ContenedorRepository.GetPesoTotalContenedor(contenedorDestino.NumeroPreparacion, contenedorDestino.NumeroContenedor).ToString();

                    var cantPedidos = uow.EmpaquetadoPickingRepository.GetCantPedidosContenedor(contenedorDestino.NumeroContenedor, contenedorDestino.NumeroPreparacion, out int cantCLientes, out string codigoDescripcionCliente);

                    if (cantCLientes > 1)
                    {
                        form.GetField("numeroPedido").Value = "(VARIOS)";
                        form.GetField("codigoCliente").Value = "(VARIOS)";
                    }
                    else if (cantPedidos > 1)
                    {
                        form.GetField("numeroPedido").Value = "(VARIOS)";
                        form.GetField("codigoCliente").Value = codigoDescripcionCliente;
                    }
                    else
                    {
                        form.GetField("codigoCliente").Value = $"{contenedorDestino.CodigoCliente} - {contenedorDestino.DescripcionCliente}";
                        form.GetField("numeroPedido").Value = contenedorDestino.NumeroPedido;
                        form.GetField("descripcionEntrega").Value = contenedorDestino.Direccion;
                        form.GetField("tipoExpedicion").Value = contenedorDestino.TipoExpedicion;
                        form.GetField("codigoRuta").Value = contenedorDestino.CodigoRota;
                        form.GetField("fechaEntrega").Value = contenedorDestino.FechaEntrega.ToString();
                        form.GetField("descripcionAnexo4").Value = contenedorDestino.Anexo4;
                    }
                }
            }

            return form;
        }

        public virtual string GetRuta(IUnitOfWork uow, string codRuta)
        {
            string codigoRutaResult = codRuta;
            if (short.TryParse(codigoRutaResult, out short codigoRotaShort))
            {
                Ruta ruta = uow.RutaRepository.GetRuta(codigoRotaShort);
                if (ruta != null)
                {
                    codigoRutaResult = codRuta + "-" + ruta.Descripcion;
                }
            }

            return codigoRutaResult;
        }

        public virtual void DesbloquearContenedor(Form form, FormSubmitContext context, bool todos = false, bool contenedorDestino = false)
        {
            if (todos || contenedorDestino)
            {
                var idLock = string.Empty;
                var paramContenedor = context.GetParameter("CONT_DESTINO_DATA");
                var dataContenedor = !string.IsNullOrEmpty(paramContenedor) ? JsonConvert.DeserializeObject<ContenedorDestinoData>(paramContenedor) : null;

                if (dataContenedor != null)
                    idLock = $"{dataContenedor.NumeroPreparacion}#{dataContenedor.NumeroContenedor}";

                if (!string.IsNullOrEmpty(idLock))
                    this._concurrencyControl.RemoveLockByIdLock("T_CONTENEDOR", idLock, _identity.UserId);
            }

            if (todos || !contenedorDestino)
            {
                var idLock = string.Empty;
                string nuContenedor = context.GetParameter("AUX_CONT_ORIGEN_NU_CONTENEDOR");
                string nuPreparacion = context.GetParameter("AUX_CONT_ORIGEN_NU_PREPARACION");

                if (!string.IsNullOrEmpty(nuPreparacion) && !string.IsNullOrEmpty(nuContenedor))
                    idLock = $"{nuPreparacion}#{nuContenedor}";

                if (!string.IsNullOrEmpty(idLock))
                    this._concurrencyControl.RemoveLockByIdLock("T_CONTENEDOR", idLock, _identity.UserId);
            }
        }

        public virtual Form PreConfirmarCerrarEtiqueta(Form form, FormSubmitContext context)
        {
            var contDestino = form.GetField("contenedorDestino").Value;

            if (!string.IsNullOrEmpty(contDestino) && contDestino != "(NUEVO)")
                context.AddOrUpdateParameter("CONFIRMATION_MSG_CIERRE_ETIQUETA", "S");
            else
                context.AddErrorNotification("EXP110_form1_Error_ContenedorDestinoNulo");

            return form;
        }

        public virtual Form ConfirmarCerrarEtiquetaEmpaque(Form form, FormSubmitContext context)
        {

            context.AddParameter("CIERRE_ETIQUETA_COMPLETADO", "S");
            return form;
        }

        #endregion
    }
}