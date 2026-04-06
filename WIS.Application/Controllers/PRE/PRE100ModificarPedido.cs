using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.Expedicion;
using WIS.Domain.Services.Interfaces;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.PRE
{
    public class PRE100ModificarPedido : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;
        protected readonly ITrackingService _trackingService;
        protected readonly IParameterService _parameterService;

        public PRE100ModificarPedido(IIdentityService identity, IUnitOfWorkFactory uowFactory, IFormValidationService formValidationService, ISecurityService security, ITrackingService trackingService, IParameterService parameterService)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._security = security;
            this._trackingService = trackingService;
            this._parameterService = parameterService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            var pedidoId = context.GetParameter("pedido");
            var clienteId = context.GetParameter("cliente");
            var empresaId = int.Parse(context.GetParameter("empresa"));

            using var uow = this._uowFactory.GetUnitOfWork();

            var pedido = uow.PedidoRepository.GetPedido(empresaId, clienteId, pedidoId);
            var empresa = uow.EmpresaRepository.GetEmpresa(empresaId);
            var cliente = uow.AgenteRepository.GetAgente(empresaId, clienteId);

            context.AddParameter("empresaNombre", empresa.Nombre);

            context.AddParameter("agenteDescripcion", cliente.Descripcion);
            context.AddParameter("agenteCodigo", cliente.Codigo);
            context.AddParameter("agenteTipo", cliente.Tipo);

            var ruta = Convert.ToString(pedido.Ruta);

            form.GetField("predio").Value = pedido.Predio;
            form.GetField("ruta").Value = ruta;
            form.GetField("tipoExpedicion").Value = pedido.ConfiguracionExpedicion.Tipo;

            form.GetField("tipoPedido").Value = pedido.Tipo;
            form.GetField("liberarDesde").Value = pedido.FechaLiberarDesde.ToIsoString();
            form.GetField("liberarHasta").Value = pedido.FechaLiberarHasta.ToIsoString();
            form.GetField("fechaEmision").Value = pedido.FechaEmision.ToIsoString();
            form.GetField("fechaEntrega").Value = pedido.FechaEntrega.ToIsoString();
            form.GetField("memo").Value = pedido.Memo;
            form.GetField("direccionEntrega").Value = pedido.DireccionEntrega;
            form.GetField("zona").Value = pedido.Zona;
            form.GetField("anexo").Value = pedido.Anexo;
            form.GetField("idReserva").Value = pedido.Anexo4;
            form.GetField("ComparteContenedorPicking").Value = pedido.ComparteContenedorPicking;
            form.GetField("ComparteContenedorEntrega").Value = pedido.ComparteContenedorEntrega;
            form.GetField("telofonoPrincipal").Value = pedido.Telefono;
            form.GetField("telefonoSecundario").Value = pedido.TelefonoSecundario;

            if (pedido.Latitud != null)
            {
                var latitud = pedido.Latitud ?? 0m;
                form.GetField("latitud").Value = latitud.ToString(this._identity.GetFormatProvider());
            }

            if (pedido.Longitud != null)
            {
                var longitud = pedido.Longitud ?? 0m;
                form.GetField("longitud").Value = longitud.ToString(this._identity.GetFormatProvider());
            }

            this.InicializarSelectTipoExpedicion(uow, form);
            this.InicializarSelectTipoPedido(uow, form, pedido.ConfiguracionExpedicion.Tipo);

            this.InicializarSelectRuta(form, ruta);
            this.InicializarSelectPredio(uow, form);

            this.InicializarSelectZona(uow, form);

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber(this._identity.Application);
            uow.BeginTransaction();

            try
            {
                var pedidoId = context.GetParameter("pedido");
                var cliente = context.GetParameter("cliente");
                var empresa = int.Parse(context.GetParameter("empresa"));
                var sincTracking = false;

                uow.CreateTransactionNumber("PRE100 Modificacion Pedido");

                var fechaEmision = DateTimeExtension.ParseFromIso(form.GetField("fechaEmision").Value)?.Date;
                var fechaEntrega = DateTimeExtension.ParseFromIso(form.GetField("fechaEntrega").Value)?.Date;
                var liberarDesde = DateTimeExtension.ParseFromIso(form.GetField("liberarDesde").Value)?.Date;
                var liberarHasta = DateTimeExtension.ParseFromIso(form.GetField("liberarHasta").Value)?.Date;

                var pedido = uow.PedidoRepository.GetPedido(empresa, cliente, pedidoId);
                var configuracion = uow.PedidoRepository.GetConfiguracionExpedicion(form.GetField("tipoExpedicion").Value);
                var ruta = form.GetField("ruta").Value;

                if (pedido.Actividad == EstadoPedidoDb.Vencido && pedido.FechaLiberarHasta.HasValue && liberarHasta.HasValue && liberarHasta.Value.Date != pedido.FechaLiberarHasta.Value.Date)
                {
                    pedido.Actividad = EstadoPedidoDb.Activo;
                }

                pedido.Anexo = form.GetField("anexo").Value;
                pedido.Anexo4 = form.GetField("idReserva").Value;
                pedido.FechaEmision = fechaEmision;
                pedido.FechaEntrega = fechaEntrega;
                pedido.FechaLiberarDesde = liberarDesde;
                pedido.FechaLiberarHasta = liberarHasta;
                pedido.Memo = form.GetField("memo").Value;
                pedido.Predio = form.GetField("predio").Value;
                pedido.FechaModificacion = DateTime.Now;
                pedido.ConfiguracionExpedicion = configuracion;
                pedido.Tipo = form.GetField("tipoPedido").Value;
                pedido.ComparteContenedorPicking = form.GetField("ComparteContenedorPicking").Value.ToUpper();
                pedido.ComparteContenedorEntrega = form.GetField("ComparteContenedorEntrega").Value.ToUpper();
                pedido.Telefono = form.GetField("telofonoPrincipal").Value;
                pedido.TelefonoSecundario = form.GetField("telefonoSecundario").Value;
                pedido.Transaccion = uow.GetTransactionNumber();

                if (!string.IsNullOrEmpty(form.GetField("latitud").Value))
                    pedido.Latitud = decimal.Parse(form.GetField("latitud").Value, this._identity.GetFormatProvider());
                else
                    pedido.Latitud = null;

                if (!string.IsNullOrEmpty(form.GetField("longitud").Value))
                    pedido.Longitud = decimal.Parse(form.GetField("longitud").Value, this._identity.GetFormatProvider());
                else
                    pedido.Longitud = null;

                if (!string.IsNullOrEmpty(form.GetField("zona").Value))
                    pedido.Zona = form.GetField("zona").Value;
                else
                    pedido.Zona = null;

                var direcEntrega = form.GetField("direccionEntrega").Value;
                if (direcEntrega != pedido.DireccionEntrega || !pedido.IsSincronizacionRealizada)
                    sincTracking = true;

                pedido.DireccionEntrega = form.GetField("direccionEntrega").Value;

                var agente = uow.AgenteRepository.GetAgenteConRelaciones(pedido.Empresa, pedido.Cliente);

                if (!string.IsNullOrEmpty(ruta))
                    pedido.Ruta = string.IsNullOrEmpty(ruta) ? null : Convert.ToInt32(ruta);
                else
                {
                    var fieldRuta = form.GetField("ruta");

                    if (agente.RutasPorDefecto.Any(d => d.Predio == pedido.Predio))
                    {
                        var rutaPredio = agente.RutasPorDefecto.FirstOrDefault(d => d.Predio == pedido.Predio);

                        var rutaDefault = uow.RutaRepository.GetRuta(rutaPredio.Ruta);
                        if (rutaDefault != null)
                            pedido.Ruta = rutaDefault.Id;
                    }
                    else
                    {
                        var rutaDefault = uow.RutaRepository.GetRuta(agente.Ruta.Id);
                        if (rutaDefault != null)
                            pedido.Ruta = rutaDefault.Id;
                    }
                }

                if (sincTracking)
                    _trackingService.SincronizarPedido(uow, pedido, agente, false);

                uow.PedidoRepository.UpdatePedido(pedido);

                uow.SaveChanges();
                uow.Commit();
                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }
            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var expedicionService = new ExpedicionConfiguracionService(uow, this._parameterService, new ParametroMapper());
            var configuracion = expedicionService.GetConfiguracionPedido();
            return this._formValidationService.Validate(new CreatePedidoFormValidationModule(uow, this._identity, this._security, configuracion), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext query)
        {
            switch (query.FieldId)
            {
                case "ruta": return this.SearchRuta(form, query);
            }

            return new List<SelectOption>();
        }

        #region Auxs

        public virtual void InicializarSelectTipoExpedicion(IUnitOfWork uow, Form form)
        {
            var configuraciones = uow.PedidoRepository.GetConfiguracionesExpedicion();

            var listaConfiguraciones = new List<SelectOption>();

            foreach (var configuracion in configuraciones)
            {
                listaConfiguraciones.Add(new SelectOption(configuracion.Tipo, configuracion.Descripcion));
            }

            form.GetField("tipoExpedicion").Options = listaConfiguraciones;
        }

        public virtual void InicializarSelectTipoPedido(IUnitOfWork uow, Form form, string tipoExpedicionInicial)
        {
            var listaTipos = new List<SelectOption>();

            var tipos = uow.PedidoRepository.GetTiposPedido(tipoExpedicionInicial);

            foreach (var tipo in tipos)
            {
                listaTipos.Add(new SelectOption(tipo.Key, tipo.Value));
            }

            form.GetField("tipoPedido").Options = listaTipos;
        }

        public virtual void InicializarSelectRuta(Form form, string ruta)
        {
            form.GetField("ruta").Options = this.SearchRuta(form, new FormSelectSearchContext()
            {
                SearchValue = ruta
            });
        }

        public virtual void InicializarSelectPredio(IUnitOfWork uow, Form form)
        {
            // Predios
            var selectPredio = form.GetField("predio");
            selectPredio.Options = new List<SelectOption>();

            var dbQuery = new GetPrediosUsuarioQuery();
            uow.HandleQuery(dbQuery);

            var predios = dbQuery.GetPrediosUsuario(_identity.UserId).OrderBy(x => x.Numero);
            foreach (var pred in predios)
            {
                selectPredio.Options.Add(new SelectOption(pred.Numero, $"{pred.Numero} - {pred.Descripcion}")); ;
            }

            if (predios.Count() == 1)
                selectPredio.Value = predios.FirstOrDefault().Numero;

            if (!this._identity.Predio.Equals(GeneralDb.PredioSinDefinir))
                selectPredio.Value = this._identity.Predio;
        }

        public virtual void InicializarSelectZona(IUnitOfWork uow, Form form)
        {
            var listaZonas = new List<SelectOption>();

            var zonas = uow.ZonaRepository.GetZonas();

            foreach (var zona in zonas)
            {
                listaZonas.Add(new SelectOption(zona.CdZona, $"{zona.CdZona} - {zona.NmZona}"));
            }

            form.GetField("zona").Options = listaZonas;
        }

        public virtual List<SelectOption> SearchRuta(Form form, FormSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            var predio = form.GetField("predio").Value;

            if (string.IsNullOrEmpty(predio))
                return opciones;

            using var uow = this._uowFactory.GetUnitOfWork();

            var rutas = uow.RutaRepository.GetByDescripcionOrCodePartial(context.SearchValue, predio);

            foreach (var ruta in rutas)
            {
                var descRuta = $"{ruta.Id} - {ruta.Descripcion}";
                descRuta += $" - {(ruta.Onda == null ? "SIN ONDA" : ruta.Onda?.Descripcion)}";

                if (!string.IsNullOrEmpty(ruta.Zona))
                    descRuta += $" - {ruta.Zona}";

                opciones.Add(new SelectOption(ruta.Id.ToString(), descRuta));
            }

            return opciones;
        }

        #endregion
    }
}
