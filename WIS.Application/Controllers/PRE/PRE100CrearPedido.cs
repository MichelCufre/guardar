using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Security;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.Expedicion;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;
using CondicionLiberacionDb = WIS.Domain.DataModel.Mappers.Constants.CondicionLiberacionDb;
using TipoExpedicion = WIS.Domain.DataModel.Mappers.Constants.TipoExpedicion;

namespace WIS.Application.Controllers.PRE
{
    public class PRE100CrearPedido : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;
        protected readonly ITrackingService _trackingService;
        protected readonly IParameterService _parameterService;

        public PRE100CrearPedido(IIdentityService identity, IUnitOfWorkFactory uowFactory, IFormValidationService formValidationService, ISecurityService security, ITrackingService trackingService, IParameterService parameterService)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._security = security;
            this._trackingService = trackingService;
            this._parameterService = parameterService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            var tipoExpedicionInicial = TipoExpedicion.Facturables;

            form.GetField("pedido").Value = string.Empty;
            form.GetField("predio").Value = string.Empty;
            form.GetField("empresa").Value = string.Empty;
            form.GetField("cliente").Value = string.Empty;
            form.GetField("ruta").Value = string.Empty;
            form.GetField("tipoExpedicion").Value = tipoExpedicionInicial;
            form.GetField("tipoPedido").Value = TipoPedidoDb.Venta;
            form.GetField("liberarDesde").Value = string.Empty;
            form.GetField("liberarHasta").Value = string.Empty;
            form.GetField("fechaEmision").Value = string.Empty;
            form.GetField("fechaEntrega").Value = string.Empty;
            form.GetField("memo").Value = string.Empty;
            form.GetField("direccionEntrega").Value = string.Empty;
            form.GetField("zona").Value = string.Empty;
            form.GetField("anexo").Value = string.Empty;
            form.GetField("idReserva").Value = string.Empty;
            form.GetField("ComparteContenedorPicking").Value = string.Empty;
            form.GetField("ComparteContenedorEntrega").Value = string.Empty;
            form.GetField("telofonoPrincipal").Value = string.Empty;
            form.GetField("telefonoSecundario").Value = string.Empty;
            form.GetField("latitud").Value = string.Empty;
            form.GetField("longitud").Value = string.Empty;

            using var uow = this._uowFactory.GetUnitOfWork();

            InicializarSelectTipoExpedicion(uow, form);
            InicializarSelectTipoPedido(uow, form, tipoExpedicionInicial);
            InicializarSelectPredio(uow, form);
            InicializarSelectZona(uow, form);

            Dictionary<string, bool> result = this._security.CheckPermissions(new List<string>
            {
                SecurityResources.PRE100CrearPedido_frm1_btn_Detalle,
                SecurityResources.PRE100CrearPedido_frm1_btn_DetalleLpn,
            });

            if (result[SecurityResources.PRE100CrearPedido_frm1_btn_Detalle])
                query.Parameters.Add(new ComponentParameter() { Id = "showBtnDetalles", Value = "true" });

            if (result[SecurityResources.PRE100CrearPedido_frm1_btn_DetalleLpn])
                query.Parameters.Add(new ComponentParameter() { Id = "showBtnDetallesLpn", Value = "true" });

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                uow.CreateTransactionNumber("PRE100 Alta Pedido");

                var fechaEmision = DateTimeExtension.ParseFromIso(form.GetField("fechaEmision").Value)?.Date;
                var fechaEntrega = DateTimeExtension.ParseFromIso(form.GetField("fechaEntrega").Value)?.Date;
                var liberarDesde = DateTimeExtension.ParseFromIso(form.GetField("liberarDesde").Value)?.Date;
                var liberarHasta = DateTimeExtension.ParseFromIso(form.GetField("liberarHasta").Value)?.Date;

                var pedido = new Pedido();
                var configuracion = uow.PedidoRepository.GetConfiguracionExpedicion(form.GetField("tipoExpedicion").Value);

                var ruta = form.GetField("ruta").Value;
                var empresa = form.GetField("empresa").Value;

                pedido.Id = form.GetField("pedido").Value;
                pedido.Tipo = form.GetField("tipoPedido").Value;
                pedido.Anexo = form.GetField("anexo").Value;
                pedido.Anexo4 = form.GetField("idReserva").Value;
                pedido.Cliente = form.GetField("cliente").Value;
                pedido.Empresa = Convert.ToInt32(empresa);
                pedido.FechaEmision = fechaEmision;
                pedido.FechaEntrega = fechaEntrega;
                pedido.FechaLiberarDesde = liberarDesde;
                pedido.FechaLiberarHasta = liberarHasta;
                pedido.Memo = form.GetField("memo").Value;
                pedido.Predio = form.GetField("predio").Value;
                pedido.DireccionEntrega = form.GetField("direccionEntrega").Value;
                pedido.Zona = form.GetField("zona").Value;
                pedido.FechaAlta = DateTime.Now;
                pedido.ConfiguracionExpedicion = configuracion;
                pedido.Estado = SituacionDb.PedidoAbierto;
                pedido.IsManual = true;
                pedido.CondicionLiberacion = CondicionLiberacionDb.SinCondicion;
                pedido.ComparteContenedorPicking = form.GetField("ComparteContenedorPicking").Value.ToUpper();
                pedido.ComparteContenedorEntrega = form.GetField("ComparteContenedorEntrega").Value.ToUpper();
                pedido.IsSincronizacionRealizada = false;
                pedido.NuCarga = null;
                pedido.Origen = "PRE100";
                pedido.Telefono = form.GetField("telofonoPrincipal").Value;
                pedido.TelefonoSecundario = form.GetField("telefonoSecundario").Value;
                pedido.Transaccion = uow.GetTransactionNumber();

                if (!string.IsNullOrEmpty(form.GetField("latitud").Value))
                    pedido.Latitud = decimal.Parse(form.GetField("latitud").Value, this._identity.GetFormatProvider());

                if (!string.IsNullOrEmpty(form.GetField("longitud").Value))
                    pedido.Longitud = decimal.Parse(form.GetField("longitud").Value, this._identity.GetFormatProvider());

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

                if (string.IsNullOrEmpty(pedido.Id))
                    pedido.Id = uow.PedidoRepository.GetNextNuPedidoManual().ToString();

                uow.PedidoRepository.AddPedido(pedido);
                uow.SaveChanges();

                _trackingService.SincronizarPedido(uow, pedido, agente, false);
                uow.PedidoRepository.UpdatePedido(pedido);
                uow.SaveChanges();

                uow.Commit();

                context.AddParameter("pedido", pedido.Id);
                context.AddParameter("cliente", pedido.Cliente);
                context.AddParameter("empresa", empresa);

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

            var expedicionService = new ExpedicionConfiguracionService(uow, this._parameterService, new ParametroMapper());
            var configuracion = expedicionService.GetConfiguracionPedido();
            return this._formValidationService.Validate(new CreatePedidoFormValidationModule(uow, this._identity, this._security, configuracion), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext query)
        {
            switch (query.FieldId)
            {
                case "empresa": return this.SearchEmpresa(form, query);
                case "cliente": return this.SearchCliente(form, query);
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
                selectPredio.Options.Add(new SelectOption(pred.Numero, $"{pred.Numero} - {pred.Descripcion}"));
            }

            if (predios.Count() == 1)
                selectPredio.Value = predios.FirstOrDefault().Numero;

            if (this._identity.Predio != GeneralDb.PredioSinDefinir)
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

        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var empresas = uow.EmpresaRepository.GetByNombreOrCodePartial(context.SearchValue);

            foreach (var empresa in empresas)
            {
                opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchCliente(Form form, FormSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            var empresa = form.GetField("empresa").Value;

            if (string.IsNullOrEmpty(empresa) || !int.TryParse(empresa, out int empresaId))
                return opciones;

            using var uow = this._uowFactory.GetUnitOfWork();

            var expedicionService = new ExpedicionConfiguracionService(uow, this._parameterService, new ParametroMapper());
            var configuracion = expedicionService.GetConfiguracionPedido();

            if (configuracion.PermitePedidosAProveedores)
            {
                var clientes = uow.AgenteRepository.GetByDescripcionOrAgentePartial(context.SearchValue, empresaId);

                foreach (var cliente in clientes)
                {
                    opciones.Add(new SelectOption(cliente.CodigoInterno, $"{cliente.Empresa} - {cliente.Tipo} - {cliente.Codigo} - {cliente.Descripcion} "));
                }
            }
            else
            {
                var clientes = uow.AgenteRepository.GetClienteByDescripcionOrAgentePartial(context.SearchValue, empresaId);

                foreach (var cliente in clientes)
                {
                    opciones.Add(new SelectOption(cliente.CodigoInterno, $"{cliente.Empresa} - {cliente.Codigo} - {cliente.Descripcion}"));
                }
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchRuta(Form form, FormSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            string predio = form.GetField("predio").Value;

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
