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
using WIS.Domain.General;
using WIS.Domain.General.Configuracion;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Persistence.Database;
using WIS.Security;
using CondicionLiberacionDb = WIS.Domain.DataModel.Mappers.Constants.CondicionLiberacionDb;
using TipoExpedicion = WIS.Domain.DataModel.Mappers.Constants.TipoExpedicion;

namespace WIS.Application.Controllers.PRE
{
    public class PRE052CrearPreparacionLibre : AppController
    {
        protected readonly ISecurityService _security;
        protected readonly IIdentityService _identity;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IFormValidationService _formValidationService;
        protected readonly ITrackingService _trackingService;
        protected readonly IParameterService _parameterService;

        public PRE052CrearPreparacionLibre(
            ISecurityService security,
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IFormValidationService formValidationService,
            ITrackingService trackingService,
            IParameterService parameterService)
        {
            this._security = security;
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._formValidationService = formValidationService;
            this._trackingService = trackingService;
            _parameterService = parameterService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var configuracion = uow.PreparacionRepository.GetConfiguracionPreparacionManual();

            form.GetField("descripcion").Value = string.Empty;
            form.GetField("predio").Value = string.Empty;
            form.GetField("empresa").Value = string.Empty;
            form.GetField("cliente").Value = string.Empty;
            form.GetField("idReserva").Value = string.Empty;

            InicializarSelectTipoExpedicion(uow, form);
            InicializarSelectTipoPedido(uow, form);
            InicializarSelectPredio(uow, form);
            InicializarSelectTipoPreparacion(form);

            form.GetField("permitePickearVencido").Value = configuracion.PermitePickearVencido.ToString();
            form.GetField("permitePickearVencido").Disabled = !configuracion.PermitePickearVencidoHabilitado;

            form.GetField("permitePickearAveriado").Value = configuracion.PermitePickearAveriado.ToString();
            form.GetField("permitePickearAveriado").Disabled = !configuracion.PermitePickearAveriadoHabilitado;

            form.GetField("validarProductoProveedor").Value = configuracion.ValidarProductoProveedor.ToString();
            form.GetField("validarProductoProveedor").Disabled = !configuracion.ValidarProductoProveedorHabilitado;

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.BeginTransaction();
            try
            {
                uow.CreateTransactionNumber("PRE052 Crear Preparacion Libre");
                var transaccion = uow.GetTransactionNumber();

                var empresa = int.Parse(form.GetField("empresa").Value);
                var clienteId = form.GetField("cliente").Value;

                var preparacion = CrearPreparacion(form, uow);

                var nuPreparacion = uow.PreparacionRepository.AddPreparacion(preparacion);

                var cliente = uow.AgenteRepository.GetAgenteConRelaciones(empresa, clienteId);
                var pedido = CrearPedido(uow, form, cliente, nuPreparacion);

                uow.PedidoRepository.AddPedido(pedido);
                uow.SaveChanges();

                _trackingService.SincronizarPedido(uow, pedido, cliente, false);
                uow.PedidoRepository.UpdatePedido(pedido);
                uow.SaveChanges();

                if (pedido.NuCarga == null)
                {
                    var carga = new Carga
                    {
                        Descripcion = "Generada por la preparación manual: " + nuPreparacion,
                        Preparacion = nuPreparacion,
                        Ruta = (short)pedido.Ruta,
                        FechaAlta = DateTime.Now
                    };
                    uow.CargaRepository.AddCarga(carga);
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("PRE052_Sucess_msg_PrepLibre", new List<string> { nuPreparacion.ToString(), pedido.Id });
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

            return this._formValidationService.Validate(new CreatePrepManualLibreValidationModule(uow, this._identity, this._security), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "empresa": return this.SearchEmpresa(form, context);
                case "cliente": return this.SearchCliente(form, context);
            }

            return new List<SelectOption>();
        }

        #region Metodos Auxiliares

        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Empresa> empresas = uow.EmpresaRepository.GetByNombreOrCodePartial(context.SearchValue);

            foreach (var empresa in empresas)
            {
                opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchCliente(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            string empresa = form.GetField("empresa").Value;

            if (string.IsNullOrEmpty(empresa) || !int.TryParse(empresa, out int empresaId))
                return opciones;

            using var uow = this._uowFactory.GetUnitOfWork();

            var expedicionService = new ExpedicionConfiguracionService(uow, this._parameterService, new ParametroMapper());
            var configuracion = expedicionService.GetConfiguracionPedido();

            if (configuracion.PermitePedidosAProveedores)
            {
                List<Agente> clientes = uow.AgenteRepository.GetByDescripcionOrAgentePartial(context.SearchValue, empresaId);

                foreach (var cliente in clientes)
                {
                    opciones.Add(new SelectOption(cliente.CodigoInterno, $"{cliente.Empresa} - {cliente.Tipo} - {cliente.Codigo} - {cliente.Descripcion} "));
                }
            }
            else
            {
                List<Agente> clientes = uow.AgenteRepository.GetClienteByDescripcionOrAgentePartial(context.SearchValue, empresaId);

                foreach (var cliente in clientes)
                {
                    opciones.Add(new SelectOption(cliente.CodigoInterno, $"{cliente.Empresa} - {cliente.Codigo} - {cliente.Descripcion}"));
                }
            }

            return opciones;
        }

        public virtual void InicializarSelectTipoExpedicion(IUnitOfWork uow, Form form)
        {
            var opcionesConfiguraciones = new List<SelectOption>();
            var configuracionesExpedicion = uow.PedidoRepository.GetConfiguracionesExpedicion();

            foreach (var configExp in configuracionesExpedicion)
            {
                if (configExp.Tipo == TipoExpedicion.ReservasPrepManual)
                    continue;

                opcionesConfiguraciones.Add(new SelectOption(configExp.Tipo, configExp.Descripcion));
            }

            form.GetField("tipoExpedicion").Options = opcionesConfiguraciones;
            form.GetField("tipoExpedicion").Value = TipoExpedicion.Facturables;
        }

        public virtual void InicializarSelectTipoPedido(IUnitOfWork uow, Form form)
        {
            var opcionesTipos = new List<SelectOption>();
            var tipos = uow.PedidoRepository.GetTiposPedido(TipoExpedicion.Facturables);

            foreach (var tipo in tipos)
            {
                opcionesTipos.Add(new SelectOption(tipo.Key, tipo.Value));
            }

            form.GetField("tipoPedido").Options = opcionesTipos;
            form.GetField("tipoPedido").Value = TipoPedidoDb.Venta;
        }

        public virtual void InicializarSelectPredio(IUnitOfWork uow, Form form)
        {
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

        public virtual void InicializarSelectTipoPreparacion(Form form)
        {
            form.GetField("tipoPreparacion").Options = new List<SelectOption>()
            {
                new SelectOption(TipoPreparacionDb.Libre, "General_Sec0_lbl_DominioTipoPreparacionLibre")
            };

            form.GetField("tipoPreparacion").Value = TipoPreparacionDb.Libre;
            form.GetField("tipoPreparacion").Disabled = true;
        }

        public virtual Preparacion CrearPreparacion(Form form, IUnitOfWork uow)
        {
            return new Preparacion()
            {
                Descripcion = string.IsNullOrEmpty(form.GetField("descripcion").Value) ? "Preparación manual libre" : form.GetField("descripcion").Value,
                Empresa = int.Parse(form.GetField("empresa").Value),
                FechaInicio = DateTime.Now,
                Tipo = TipoPreparacionDb.Libre,
                Situacion = SituacionDb.PreparacionIniciada,
                Usuario = this._identity.UserId,
                CodigoContenedorValidado = "TPOPED",
                Predio = form.GetField("predio").Value,
                Transaccion = uow.GetTransactionNumber(),
                PermitePickVencido = bool.Parse(form.GetField("permitePickearVencido").Value),
                AceptaMercaderiaAveriada = bool.Parse(form.GetField("permitePickearAveriado").Value),
                ValidarProductoProveedor = bool.Parse(form.GetField("validarProductoProveedor").Value),
            };
        }

        public virtual Pedido CrearPedido(IUnitOfWork uow, Form form, Agente cliente, int nuPrep)
        {
            var configuracion = uow.PedidoRepository.GetConfiguracionExpedicion(form.GetField("tipoExpedicion").Value);
            return new Pedido
            {
                Id = uow.PedidoRepository.GetNextNuPedidoManual().ToString(), //Para poder sincronizar con tracking lo seteo aca.
                Tipo = form.GetField("tipoPedido").Value,
                Cliente = cliente.CodigoInterno,
                Empresa = cliente.Empresa,
                Ruta = cliente.Ruta.Id,
                FechaEntrega = DateTime.Now.AddDays(1),
                FechaLiberarDesde = DateTime.Now,
                Memo = "Pedido generado para preparación libre de tipo manual",
                Predio = form.GetField("predio").Value,
                FechaAlta = DateTime.Now,
                ConfiguracionExpedicion = configuracion,
                Estado = SituacionDb.PedidoAbierto,
                IsManual = true,
                Agrupacion = "P",
                NroPrepManual = nuPrep,
                NroIntzFacturacion = -1,
                Origen = "PRE052",
                CondicionLiberacion = CondicionLiberacionDb.SinCondicion,
                DireccionEntrega = cliente.Direccion,
                IsSincronizacionRealizada = false,
                NuCarga = null,
                Transaccion = uow.GetTransactionNumber(),
                Anexo4 = form.GetField("idReserva").Value,
            };
        }

        #endregion
    }
}
