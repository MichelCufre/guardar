using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Recepcion;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;
using WIS.Session;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.REC
{
    public class REC410ClasificacionDeRecepciones : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<REC410ClasificacionDeRecepciones> _logger;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IBarcodeService _barcodeService;
        protected readonly IGrupoService _grupoService;
        protected readonly IParameterService _parameterService;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IAlmacenamientoService _almacenamientoService;
        protected readonly ICodigoMultidatoService _codigoMultidatoService;
        protected readonly ISessionAccessor _session;

        public REC410ClasificacionDeRecepciones(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IFilterInterpreter filterInterpreter,
            ILogger<REC410ClasificacionDeRecepciones> logger,
            IFormValidationService formValidationService,
            IAlmacenamientoService almacenamientoService,
            IBarcodeService barcodeService,
            IGrupoService grupoService,
            IParameterService parameterService,
            ITrafficOfficerService concurrencyControl,
            ICodigoMultidatoService codigoMultidatoService,
            ISessionAccessor session)
        {
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._formValidationService = formValidationService;
            this._barcodeService = barcodeService;
            this._grupoService = grupoService;
            this._parameterService = parameterService;
            this._concurrencyControl = concurrencyControl;
            this._almacenamientoService = almacenamientoService;
            this._codigoMultidatoService = codigoMultidatoService;
            this._session = session;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            foreach (var field in form.Fields)
            {
                field.ReadOnly = true;
            }

            this.InicializarSelects(form);

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.ButtonId == "btnFinalizar")
            {
                FinalizarClasificacion(form, context, uow);

                form.GetField("etiqueta").Value = "";
                form.GetField("tipoEtiqueta").Value = "";
                form.GetField("lote").Value = "";
                form.GetField("codigoProducto").Value = "";
                form.GetField("descripcionProducto").Value = "";
                form.GetField("grupo").Value = "";
                form.GetField("cantidad").Value = "";
                form.GetField("vencimiento").Value = "";

                context.AddOrUpdateParameter("focusField", "estacion");
                context.AddOrUpdateParameter("Operacion", "ANTERIOR");
            }
            else
            {
                ConfirmarCampo(form, context, uow);
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var resultadovalidacion = this._formValidationService.Validate(new REC410ClasificacionDeRecepcionesFormValidationModule(uow, this._barcodeService, this._parameterService, this._identity, this._codigoMultidatoService), form, context);

            if (form.Fields.Any(x => x.Error != null))
            {
                context.AddParameter("Error", "true");
            }
            else
            {
                context.AddParameter("Error", "false");
            }

            return resultadovalidacion;
        }

        public override Form FormButtonAction(Form form, FormButtonActionContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var nuExterno = query.Parameters.Find(x => x.Id == "etiqueta").Value;
            var tpEtiqueta = query.Parameters.Find(x => x.Id == "tipoEtiqueta").Value;
            var etiqueta = uow.EtiquetaLoteRepository.GetEtiquetaLoteActiva(tpEtiqueta, nuExterno);

            if (uow.EtiquetaLoteRepository.AnyDetalleEtiquetaConStock(etiqueta.Numero))
                query.Parameters.Add(new ComponentParameter { Id = "ProductoSinClasificar", Value = "true" });
            else
                query.Parameters.Add(new ComponentParameter { Id = "ProductoSinClasificar", Value = "false" });

            return form;
        }


        #region Metodos Auxiliares

        public static void FinalizarClasificacion(Form form, FormSubmitContext context, IUnitOfWork uow)
        {
            uow.CreateTransactionNumber("Finalizar Clasificacion");
            uow.BeginTransaction();

            var transaccion = uow.GetTransactionNumber();
            var nuExterno = context.Parameters.Find(x => x.Id == "etiqueta").Value;
            var tpEtiqueta = context.Parameters.Find(x => x.Id == "tipoEtiqueta").Value;

            var etiqueta = uow.EtiquetaLoteRepository.GetEtiquetaLoteActiva(tpEtiqueta, nuExterno);

            etiqueta.Estado = SituacionDb.PalletSinProductos;
            etiqueta.FechaModificacion = DateTime.Now;
            etiqueta.NumeroTransaccion = transaccion;

            uow.EtiquetaLoteRepository.UpdateEtiquetaLote(etiqueta);

            uow.SaveChanges();
            uow.Commit();
        }

        public virtual void ConfirmarCampo(Form form, FormSubmitContext context, IUnitOfWork uow)
        {
            var focus = context.Parameters.Find(x => x.Id == "Focus").Value;
            var valor = form.GetField(focus).Value.ToUpper();
            var ais = _session.GetValue<Dictionary<string, object>>("REC410_AIS");

            if (context.ButtonId == "BorrarCampo")
            {
                form.GetField(focus).Value = "";

                if (focus == "codigoProducto")
                {
                    if (context.GetParameter("clasificarLoteNoEsperado") == "true")
                    {
                        form.GetField("lote").Value = "";
                        form.GetField("descripcionProducto").Value = "";
                        form.GetField("grupo").Value = "";

                        context.AddOrUpdateParameter("focusField", "codigoProducto");
                    }
                }
            }
            else
            {

                try
                {
                    switch (focus)
                    {
                        case "estacion":
                            ConfirmarEstacion(form, context, uow, valor);
                            break;
                        case "etiqueta":
                            ConfirmarEtiqueta(form, context, uow, valor);
                            break;
                        case "codigoProducto":
                            ConfirmarProducto(form, context, uow, valor);
                            break;
                        case "lote":
                            ConfirmarLote(form, context, uow, valor, ais);
                            break;
                        case "vencimiento":
                            ConfirmarVencimiento(form, context, uow, valor, ais);
                            break;
                        case "cantidad":
                            ConfirmarCantidad(form, context, uow, valor, ais);
                            break;
                        default:
                            // code block
                            break;
                    }
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, ex.Message);
                    context.AddErrorNotification("REC410_Sec0_Error_COL01_ConfirmarOperacion");
                }
            }
        }

        public virtual void ConfirmarCantidad(Form form, FormSubmitContext context, IUnitOfWork uow, string valor, Dictionary<string, object> ais = null)
        {
            var nroetiqueta = context.Parameters.Find(x => x.Id == "etiqueta").Value;
            var tipoEtiqueta = context.Parameters.Find(x => x.Id == "tipoEtiqueta").Value;

            var etiqueta = uow.EtiquetaLoteRepository.GetEtiquetaLoteActiva(tipoEtiqueta, nroetiqueta);

            var agenda = uow.AgendaRepository.GetAgenda(etiqueta.NumeroAgenda);
            var cdproducto = form.GetField("codigoProducto").Value.ToUpper();
            var lote = form.GetField("lote").Value.ToUpper();
            var producto = uow.ProductoRepository.GetProducto(agenda.IdEmpresa, cdproducto);

            if (string.IsNullOrEmpty(valor))
            {
                if (producto != null)
                {
                    if (ais == null)
                    {
                        if (!producto.IsFefo())
                        {
                            var manejoLote = producto.ManejoIdentificador != ManejoIdentificador.Producto;

                            form.GetField("vencimiento").Value = "";

                            if (!manejoLote)
                            {
                                form.GetField("lote").Value = "";
                                form.GetField("descripcionProducto").Value = "";
                                form.GetField("grupo").Value = "";
                                context.AddOrUpdateParameter("focusField", "codigoProducto");
                            }
                            else
                            {
                                context.AddOrUpdateParameter("focusField", "lote");
                            }
                        }

                        context.AddOrUpdateParameter("Operacion", "ANTERIOR");
                    }
                    else
                    {
                        form.GetField("vencimiento").Value = "";
                        form.GetField("lote").Value = "";
                        form.GetField("descripcionProducto").Value = "";
                        form.GetField("grupo").Value = "";
                        context.AddOrUpdateParameter("focusField", "codigoProducto");
                        context.Parameters.Add(new ComponentParameter() { Id = "Operacion", Value = "ANTERIOR" });
                    }
                }
            }
            else
            {
                var manejaSerie = producto.ManejoIdentificador == ManejoIdentificador.Serie ? "S" : "N";
                context.Parameters.RemoveAll(p => p.Id == "manejaSerie");
                context.Parameters.Add(new ComponentParameter() { Id = "manejaSerie", Value = manejaSerie });

                var detallesProductoLote = uow.EtiquetaLoteRepository.GetDetalles(etiqueta.Numero, producto.CodigoEmpresa, producto.Codigo, lote);
                var cantidadOriginal = detallesProductoLote.Select(d => d.Cantidad ?? 0).DefaultIfEmpty(0).Sum();
                var cantidadAuditada = decimal.Parse(valor, this._identity.GetFormatProvider());
                var clasificarDeMas = context.GetParameter("clasificarDeMas");
                var fstDetProdLote = detallesProductoLote.FirstOrDefault();

                if (cantidadOriginal != 0 && cantidadAuditada > cantidadOriginal && string.IsNullOrEmpty(clasificarDeMas))
                {
                    context.AddOrUpdateParameter("Operacion", "ConfirmarCantidadDeMas");
                }
                else
                {
                    context.AddOrUpdateParameter("Operacion", "AbrirPopup");
                }

                context.Parameters.Add(new ComponentParameter() { Id = "barraEtiqueta", Value = etiqueta.CodigoBarras });
                context.Parameters.Add(new ComponentParameter() { Id = "etiquetaLote", Value = etiqueta.Numero.ToString() });
                context.Parameters.Add(new ComponentParameter() { Id = "faixa", Value = (fstDetProdLote?.Faixa ?? 1).ToString(this._identity.GetFormatProvider()) });
            }
        }

        public virtual void ConfirmarVencimiento(Form form, FormSubmitContext context, IUnitOfWork uow, string valor, Dictionary<string, object> ais = null)
        {
            var nroetiqueta = context.Parameters.Find(x => x.Id == "etiqueta").Value;
            var tipoEtiqueta = context.Parameters.Find(x => x.Id == "tipoEtiqueta").Value;

            var etiqueta = uow.EtiquetaLoteRepository.GetEtiquetaLoteActiva(tipoEtiqueta, nroetiqueta);
            var agenda = uow.AgendaRepository.GetAgenda(etiqueta.NumeroAgenda);
            var cdproducto = form.GetField("codigoProducto").Value.ToUpper();

            var producto = uow.ProductoRepository.GetProducto(agenda.IdEmpresa, cdproducto);

            if (string.IsNullOrEmpty(valor))
            {
                form.GetField("vencimiento").Value = "";

                var manejoLote = producto.ManejoIdentificador != ManejoIdentificador.Producto;

                if (ais == null)
                {
                    if (!manejoLote)
                    {
                        form.GetField("lote").Value = "";
                        form.GetField("descripcionProducto").Value = "";
                        form.GetField("grupo").Value = "";
                        context.AddOrUpdateParameter("focusField", "codigoProducto");
                    }
                    context.AddOrUpdateParameter("Operacion", "ANTERIOR");
                }
                else
                {
                    form.GetField("vencimiento").Value = "";
                    form.GetField("lote").Value = "";
                    form.GetField("descripcionProducto").Value = "";
                    form.GetField("grupo").Value = "";
                    context.AddOrUpdateParameter("focusField", "codigoProducto");
                    context.Parameters.Add(new ComponentParameter() { Id = "Operacion", Value = "ANTERIOR" });
                }

            }
            else
            {
                context.AddOrUpdateParameter("focusField", "cantidad");

                var manejaSerie = producto.ManejoIdentificador == ManejoIdentificador.Serie;

                context.Parameters.RemoveAll(p => p.Id == "manejaSerie");
                context.Parameters.Add(new ComponentParameter() { Id = "manejaSerie", Value = (manejaSerie ? "S" : "N") });

                if (manejaSerie)
                {
                    form.GetField("cantidad").Value = "1";
                    ConfirmarCantidad(form, context, uow, "1", ais);

                    return;
                }

                context.AddOrUpdateParameter("Operacion", "SIGUIENTE");
            }
        }

        public virtual void ConfirmarLote(Form form, FormSubmitContext context, IUnitOfWork uow, string valor, Dictionary<string, object> ais = null)
        {
            if (string.IsNullOrEmpty(valor))
            {
                if (ais == null)
                {
                    form.GetField("lote").Value = "";
                    form.GetField("descripcionProducto").Value = "";
                    form.GetField("grupo").Value = "";
                    context.Parameters.Add(new ComponentParameter() { Id = "Operacion", Value = "ANTERIOR" });
                }
                else
                {
                    form.GetField("lote").Value = "";
                    form.GetField("descripcionProducto").Value = "";
                    form.GetField("grupo").Value = "";
                    context.AddOrUpdateParameter("focusField", "codigoProducto");
                    context.Parameters.Add(new ComponentParameter() { Id = "Operacion", Value = "ANTERIOR" });
                }
            }
            else
            {
                var nroetiqueta = context.Parameters.Find(x => x.Id == "etiqueta").Value;
                var tipoEtiqueta = context.Parameters.Find(x => x.Id == "tipoEtiqueta").Value;

                var etiqueta = uow.EtiquetaLoteRepository.GetEtiquetaLoteActiva(tipoEtiqueta, nroetiqueta);
                var agenda = uow.AgendaRepository.GetAgenda(etiqueta.NumeroAgenda);

                var codproducto = form.GetField("codigoProducto").Value;

                var lote = form.GetField("lote").Value.ToUpper();
                var detallesProducto = uow.EtiquetaLoteRepository.GetDetalles(etiqueta.Numero, agenda.IdEmpresa, codproducto);
                var detallesProductoLote = detallesProducto.Where(d => d.Identificador == lote).ToList();
                var clasificarLoteNoEsperado = context.GetParameter("clasificarLoteNoEsperado");
                var loteAnterior = context.GetParameter("loteAnterior")?.ToUpper();

                form.GetField("lote").Value = lote;

                if (detallesProducto.Count() == 0 // Lote no esperado. Evita tener que reconfirmar el lote no esperado si previamente se reconfirmo el producto no esperado
                    || detallesProductoLote.Count() > 0 // Si es lote esperado, no requiere reconfirmar
                    || (!string.IsNullOrEmpty(clasificarLoteNoEsperado) // Si producto esperado, lote no esperado y se permite clasificar lote no esperado, entonces la confirmacion del lote coincide con el primer lote ingresado
                        && !string.IsNullOrEmpty(loteAnterior)
                        && lote == loteAnterior))
                {
                    var empresa = int.Parse(context.GetParameter("cdEmpresa"));
                    var faixa = detallesProducto.FirstOrDefault()?.Faixa ?? 1;

                    context.Parameters.Add(new ComponentParameter() { Id = "faixa", Value = faixa.ToString(_identity.GetFormatProvider()) });

                    var producto = uow.ProductoRepository.GetProducto(empresa, codproducto);
                    var manejoVencimiento = producto.ManejaFechaVencimiento();

                    var manejaSerie = producto.ManejoIdentificador == ManejoIdentificador.Serie;

                    context.Parameters.RemoveAll(p => p.Id == "manejaSerie");
                    context.Parameters.Add(new ComponentParameter() { Id = "manejaSerie", Value = (manejaSerie ? "S" : "N") });

                    if (manejoVencimiento)
                    {
                        context.AddOrUpdateParameter("focusField", "vencimiento");

                        if (producto.IsFifo())
                        {
                            var vencimiento = (DateTime.Now).ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", _identity.GetFormatProvider());
                            form.GetField("vencimiento").Value = vencimiento;

                            ConfirmarVencimiento(form, context, uow, vencimiento, ais);

                            return;
                        }
                        else if (TryGetAIValue(ais, "vencimiento", out string vencimiento))
                        {
                            form.GetField("vencimiento").Value = vencimiento;

                            ConfirmarVencimiento(form, context, uow, vencimiento, ais);

                            return;
                        }
                    }
                    else
                    {
                        context.AddOrUpdateParameter("focusField", "cantidad");

                        if (manejaSerie)
                        {
                            form.GetField("cantidad").Value = "1";
                            ConfirmarCantidad(form, context, uow, "1", ais);

                            return;
                        }
                    }

                    context.AddOrUpdateParameter("Operacion", "SIGUIENTE");
                }
                else
                {
                    context.AddOrUpdateParameter("Operacion", "ConfirmarLoteNoEsperado");
                }
            }
        }

        public virtual void ConfirmarProducto(Form form, FormSubmitContext context, IUnitOfWork uow, string valor)
        {
            var nroetiqueta = context.Parameters.Find(x => x.Id == "etiqueta").Value;
            var tipoEtiqueta = context.Parameters.Find(x => x.Id == "tipoEtiqueta").Value;
            var etiqueta = uow.EtiquetaLoteRepository.GetEtiquetaLoteActiva(tipoEtiqueta, nroetiqueta);

            if (string.IsNullOrEmpty(valor))
            {
                this._concurrencyControl.RemoveLockByIdLock("T_ETIQUETA_LOTE", etiqueta.Numero.ToString(), _identity.UserId);
                form.GetField("tipoEtiqueta").Value = "";
                context.AddOrUpdateParameter("showBtnFinalizar", "true");
                context.AddOrUpdateParameter("Operacion", "ANTERIOR");
            }
            else
            {
                var agenda = uow.AgendaRepository.GetAgenda(etiqueta.NumeroAgenda);
                var ais = GetAIsProducto(uow, form, context, agenda.IdEmpresa);
                this._session.SetValue("REC410_AIS", ais);

                if (TryGetAIValue(ais, "codigoProducto", out string cdProducto))
                {
                    form.GetField("codigoProducto").Value = cdProducto;
                    valor = cdProducto;
                }

                if (!string.IsNullOrEmpty(form.GetField("descripcionProducto").Value))
                {
                    form.GetField("codigoProducto").Value = context.GetParameter("codigoBarrasProducto") ?? "";
                    form.GetField("descripcionProducto").Value = "";
                    form.GetField("grupo").Value = "";
                }

                var productoCodigoBarras = uow.ProductoCodigoBarraRepository.GetProductoCodigoBarra(valor, agenda.IdEmpresa);
                var producto = uow.ProductoRepository.GetProducto(productoCodigoBarras.IdEmpresa, productoCodigoBarras.IdProducto);

                if (producto != null)
                {
                    var manejaSerie = producto.ManejoIdentificador == ManejoIdentificador.Serie;

                    context.Parameters.RemoveAll(p => p.Id == "manejaSerie");
                    context.Parameters.Add(new ComponentParameter() { Id = "manejaSerie", Value = (manejaSerie ? "S" : "N") });

                    var detallesProducto = uow.EtiquetaLoteRepository.GetDetalles(etiqueta.Numero, producto.CodigoEmpresa, producto.Codigo);
                    var clasificarProductoNoEsperado = context.GetParameter("clasificarProductoNoEsperado");
                    var cbProductoAnterior = context.GetParameter("productoAnterior")?.ToUpper();
                    var cdProductoAnterior = (string)null;

                    if (!string.IsNullOrEmpty(cbProductoAnterior))
                    {
                        var productoAnteriorCodigoBarras = uow.ProductoCodigoBarraRepository.GetProductoCodigoBarra(cbProductoAnterior, agenda.IdEmpresa);
                        var productoAnterior = uow.ProductoRepository.GetProducto(productoCodigoBarras.IdEmpresa, productoAnteriorCodigoBarras.IdProducto);
                        cdProductoAnterior = productoAnterior.Codigo;
                    }

                    context.Parameters.RemoveAll(p => p.Id == "clasificarProductoNoEsperado");

                    if (detallesProducto.Count() > 0 || (!string.IsNullOrEmpty(clasificarProductoNoEsperado) && !string.IsNullOrEmpty(cdProductoAnterior) && cdProductoAnterior == producto.Codigo))
                    {
                        var grupo = this._grupoService.GetGrupo(producto);
                        form.GetField("codigoProducto").Value = producto.Codigo;
                        form.GetField("descripcionProducto").Value = producto.Descripcion;
                        form.GetField("grupo").Value = $"{grupo.Id} - {grupo.Descripcion}";

                        var manejoLote = producto.ManejoIdentificador != ManejoIdentificador.Producto;

                        if (!manejoLote)
                        {
                            var manejoVencimiento = producto.ManejaFechaVencimiento();

                            form.GetField("lote").Value = ManejoIdentificadorDb.IdentificadorProducto;
                            var faixa = detallesProducto.FirstOrDefault()?.Faixa ?? 1;
                            context.Parameters.Add(new ComponentParameter() { Id = "faixa", Value = faixa.ToString(_identity.GetFormatProvider()) });

                            if (manejoVencimiento)
                            {
                                context.AddOrUpdateParameter("focusField", "vencimiento");

                                if (producto.IsFifo())
                                {
                                    var vencimiento = (DateTime.Now).ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", _identity.GetFormatProvider());
                                    form.GetField("vencimiento").Value = vencimiento;

                                    ConfirmarVencimiento(form, context, uow, vencimiento, ais);

                                    return;
                                }
                                else if (TryGetAIValue(ais, "vencimiento", out string vencimiento))
                                {
                                    form.GetField("vencimiento").Value = vencimiento;

                                    ConfirmarVencimiento(form, context, uow, vencimiento, ais);

                                    return;
                                }
                            }
                            else
                            {
                                context.AddOrUpdateParameter("focusField", "cantidad");
                            }
                        }
                        else if (TryGetAIValue(ais, "lote", out string nuIdentificador))
                        {
                            form.GetField("lote").Value = nuIdentificador;

                            context.AddOrUpdateParameter("focusField", "lote");

                            ConfirmarLote(form, context, uow, nuIdentificador, ais);

                            return;
                        }

                        context.Parameters.Add(new ComponentParameter() { Id = "Operacion", Value = "SIGUIENTE" });
                    }
                    else
                    {
                        context.Parameters.Add(new ComponentParameter() { Id = "Operacion", Value = "ConfirmarProductoNoEsperado" });
                    }
                }
            }
        }

        protected virtual Dictionary<string, object> GetAIsProducto(IUnitOfWork uow, Form form, FormSubmitContext context, int empresa)
        {
            var resultado = _codigoMultidatoService.GetAIs(uow, "REC410", form.GetField("codigoProducto").Value, new Dictionary<string, string>
            {
                ["USERID"] = _identity.UserId.ToString(),
                ["NU_PREDIO"] = _identity.Predio,
                ["estacion"] = form.GetField("estacion").Value,
                ["reabastecer"] = form.GetField("reabastecer").Value,
                ["ignorarStock"] = form.GetField("ignorarStock").Value,
                ["etiqueta"] = form.GetField("etiqueta").Value,
                ["tipoEtiqueta"] = form.GetField("tipoEtiqueta").Value,
                ["CD_CAMPO"] = "codigoProducto",
            }, empresa).GetAwaiter().GetResult();

            return resultado?.AIs;
        }

        protected virtual bool TryGetAIValue(Dictionary<string, object> ais, string fieldId, out string fieldValue)
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

        public virtual void ConfirmarEtiqueta(Form form, FormSubmitContext context, IUnitOfWork uow, string valor)
        {
            if (string.IsNullOrEmpty(valor))
            {
                this._concurrencyControl.RemoveLockByIdLock("T_ESTACION_CLASIFICACION", form.GetField("estacion").Value, _identity.UserId);
                context.Parameters.Add(new ComponentParameter() { Id = "Operacion", Value = "ANTERIOR" });
            }
            else
            {
                var etiqueta = this._barcodeService.GetEtiquetaLote(valor);
                if (etiqueta != null)
                {
                    var cdEstacion = form.GetField("estacion").Value;

                    etiqueta = uow.EtiquetaLoteRepository.GetEtiquetaLoteEmpresaAsociadas(etiqueta.TipoEtiqueta, etiqueta.NumeroExterno);
                    if (etiqueta == null)
                    {
                        form.GetField("etiqueta").Value = "";
                        context.AddErrorNotification("REC410_msg_Error_NoTienesPermisosParaTrabajarConLaEmpresa");
                    }
                    else if (this._concurrencyControl.IsLocked("T_ETIQUETA_LOTE", etiqueta.Numero.ToString()))
                    {
                        context.AddErrorNotification("REC410_msg_Error_EtiquetaBloqueada");
                    }
                    else
                    {
                        var estacionEtiqueta = uow.MesaDeClasificacionRepository.GetEstacionDeClasificacion(etiqueta.IdUbicacion);
                        var reubicarEtiqueta = context.GetParameter("reubicarEtiqueta");

                        context.Parameters.RemoveAll(p => p.Id == "reubicarEtiqueta");

                        if (estacionEtiqueta != null && estacionEtiqueta.Codigo != int.Parse(cdEstacion) && string.IsNullOrEmpty(reubicarEtiqueta))
                        {
                            context.Parameters.Add(new ComponentParameter() { Id = "Operacion", Value = "ConfirmarReubicacionEtiqueta" });
                        }
                        else
                        {
                            var estacion = uow.MesaDeClasificacionRepository.GetEstacionDeClasificacion(int.Parse(cdEstacion));

                            if (estacion.Ubicacion != etiqueta.IdUbicacion)
                            {
                                uow.CreateTransactionNumber("Mover Stock a Mesa de Clasificacion");
                                uow.BeginTransaction();

                                try
                                {
                                    MovilizarStock(uow, estacion, etiqueta);

                                    uow.SaveChanges();
                                    uow.Commit();
                                }
                                catch (Exception ex)
                                {
                                    uow.Rollback();
                                    throw ex;
                                }
                            }

                            context.Parameters.RemoveAll(p => p.Id == "tipoEtiqueta");
                            context.Parameters.RemoveAll(p => p.Id == "barraEtiqueta");
                            context.Parameters.RemoveAll(p => p.Id == "etiquetaLote");
                            context.Parameters.RemoveAll(p => p.Id == "etiqueta");
                            context.Parameters.Add(new ComponentParameter() { Id = "tipoEtiqueta", Value = etiqueta.TipoEtiqueta });
                            context.Parameters.Add(new ComponentParameter() { Id = "barraEtiqueta", Value = etiqueta.CodigoBarras.ToString() });
                            context.Parameters.Add(new ComponentParameter() { Id = "etiquetaLote", Value = etiqueta.Numero.ToString() });
                            context.Parameters.Add(new ComponentParameter() { Id = "etiqueta", Value = etiqueta.NumeroExterno.ToString() });

                            Agenda agenda = uow.AgendaRepository.GetAgenda(etiqueta.NumeroAgenda);

                            context.Parameters.RemoveAll(p => p.Id == "cdEmpresa");
                            context.Parameters.Add(new ComponentParameter() { Id = "cdEmpresa", Value = agenda.IdEmpresa.ToString() });

                            form.GetField("etiqueta").Value = etiqueta.NumeroExterno;
                            form.GetField("tipoEtiqueta").Value = etiqueta.TipoEtiqueta;

                            this._concurrencyControl.AddLock("T_ETIQUETA_LOTE", etiqueta.Numero.ToString());

                            var recomponerSugerencia = RecomponerSugerenciaPendiente(form, context, uow, etiqueta);

                            context.Parameters.Add(new ComponentParameter() { Id = "showBtnFinalizar", Value = "true" });

                            if (!recomponerSugerencia)
                                context.Parameters.Add(new ComponentParameter() { Id = "Operacion", Value = "SIGUIENTE" });
                        }
                    }
                }
            }
        }

        public virtual bool RecomponerSugerenciaPendiente(Form form, FormSubmitContext context, IUnitOfWork uow, EtiquetaLote etiqueta)
        {
            var sugerencia = uow.AlmacenamientoRepository.GetSugerenciaAlmacenajePendiente(etiqueta.CodigoBarras);

            int empresa = -1;
            string cdProducto = "";
            string lote = "";
            decimal cantidad = 0;
            DateTime? vencimiento = null;
            bool reabastecer = false;
            bool ignorarStock = false;

            context.Parameters.RemoveAll(p => p.Id == "sugerencia");

            if (sugerencia != null)
            {
                if ((sugerencia.Detalles?.Count ?? 0) > 0)
                {
                    var detalle = sugerencia.Detalles.FirstOrDefault();
                    empresa = detalle.Empresa;
                    cdProducto = detalle.Producto;
                    lote = detalle.Lote;
                    vencimiento = detalle.Vencimiento;
                    cantidad = detalle.CantidadAuditada;
                }
                else
                {
                    sugerencia = null;
                }
            }
            else
            {
                sugerencia = uow.AlmacenamientoRepository.GetSugerenciaReabastecimientoPendiente(etiqueta.CodigoBarras);

                if (sugerencia != null)
                {
                    empresa = sugerencia.Empresa;
                    cdProducto = sugerencia.Producto;
                    lote = sugerencia.Lote;
                    vencimiento = sugerencia.Vencimiento;
                    cantidad = sugerencia.CantidadAuditada;
                    reabastecer = true;
                    ignorarStock = sugerencia.IgnorarStock;
                }
            }

            if (sugerencia != null)
            {
                var producto = uow.ProductoRepository.GetProducto(empresa, cdProducto);
                var grupo = this._grupoService.GetGrupo(producto);

                form.GetField("codigoProducto").Value = producto.Codigo;
                form.GetField("descripcionProducto").Value = producto.Descripcion;
                form.GetField("grupo").Value = $"{grupo.Id} - {grupo.Descripcion}";
                form.GetField("lote").Value = lote;
                form.GetField("vencimiento").Value = vencimiento?.ToString(_identity.GetFormatProvider());
                form.GetField("cantidad").Value = cantidad.ToString(_identity.GetFormatProvider());
                form.GetField("reabastecer").Value = reabastecer.ToString().ToLower();
                form.GetField("ignorarStock").Value = ignorarStock.ToString().ToLower();

                var manejaSerie = producto.ManejoIdentificador == ManejoIdentificador.Serie ? "S" : "N";
                context.Parameters.RemoveAll(p => p.Id == "manejaSerie");
                context.Parameters.Add(new ComponentParameter() { Id = "manejaSerie", Value = manejaSerie });

                context.Parameters.Add(new ComponentParameter() { Id = "sugerencia", Value = JsonSerializer.Serialize(sugerencia) });
                context.AddOrUpdateParameter("focusField", "cantidad");
                context.AddOrUpdateParameter("Operacion", "AbrirPopup");
                context.AddOrUpdateParameter("faixa", "1");

                return true;
            }

            return false;
        }

        public virtual void MovilizarStock(IUnitOfWork uow, EstacionDeClasificacion estacion, EtiquetaLote etiqueta)
        {

            uow.StockRepository.MovilizarStockEtiquetaLote(etiqueta.Numero, estacion.Ubicacion, uow.GetTransactionNumber());

            etiqueta.FechaModificacion = DateTime.Now;
            etiqueta.NumeroTransaccion = uow.GetTransactionNumber();
            etiqueta.IdUbicacion = estacion.Ubicacion;

            uow.EtiquetaLoteRepository.UpdateEtiquetaLote(etiqueta);

        }

        public virtual void ConfirmarEstacion(Form form, FormSubmitContext context, IUnitOfWork uow, string valor)
        {
            if (!string.IsNullOrEmpty(valor))
            {
                if (this._concurrencyControl.IsLocked("T_ESTACION_CLASIFICACION", valor))
                {
                    context.AddErrorNotification("REC410_msg_Error_EstacionBloqueada");
                }
                else
                {
                    var etiquetaExterna = context.GetParameter("etiqueta");
                    if (!string.IsNullOrEmpty(etiquetaExterna))
                    {
                        var tipoEtiqueta = context.GetParameter("tipoEtiqueta");
                        var etiqueta = uow.EtiquetaLoteRepository.GetEtiquetaLoteActiva(tipoEtiqueta, etiquetaExterna);

                        if (etiqueta != null)
                        {
                            this._concurrencyControl.RemoveLockByIdLock("T_ETIQUETA_LOTE", etiqueta.Numero.ToString(), _identity.UserId);
                        }
                    }

                    this._concurrencyControl.AddLock("T_ESTACION_CLASIFICACION", valor);
                    context.Parameters.Add(new ComponentParameter() { Id = "Operacion", Value = "SIGUIENTE" });
                }
            }
        }

        public virtual void InicializarSelects(Form form)
        {
            //Inicializar selects
            FormField selectPredio = form.GetField("estacion");

            selectPredio.Options = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                List<EstacionDeClasificacion> estaciones = uow.MesaDeClasificacionRepository.GetEstacionByNombreOrCodePartial(this._identity.UserId, this._identity.Predio);

                foreach (var estacion in estaciones)
                {
                    selectPredio.Options.Add(new SelectOption(estacion.Codigo.ToString(), $"{estacion.Codigo} - {estacion.Descripcion}"));
                }
            }

        }
        #endregion
    }
}
