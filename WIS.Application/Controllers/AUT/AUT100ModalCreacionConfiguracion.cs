using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Security;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Automatizacion;
using WIS.Components.Common.Select;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.Automatismo.Logic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Automatismo;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.PRE
{
    public class AUT100ModalCreacionConfiguracion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly ILogger<AUT100ModalCreacionConfiguracion> _logger;
        protected readonly IFormValidationService _formValidationService;
        protected readonly AutomatismoMapper _automatismoMapper;
        protected readonly IAutomatismoFactory _automatismoFactory;

        public AUT100ModalCreacionConfiguracion(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            ISecurityService security,
            ILogger<AUT100ModalCreacionConfiguracion> logger,
            IAutomatismoFactory automatismoFactory)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._security = security;
            this._logger = logger;
            this._automatismoMapper = new AutomatismoMapper(automatismoFactory);
            this._automatismoFactory = automatismoFactory;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            bool isUpdate = context.Parameters.FirstOrDefault(i => i.Id == "IS_UPDATE")?.Value == "S";

            if (isUpdate) return InitializeUpdateForm(form, context);

            else return InitializeInsertForm(form);
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                uow.BeginTransaction();

                var isUpdate = context.Parameters.FirstOrDefault(i => i.Id == "IS_UPDATE")?.Value == "S";

                if (isUpdate)
                    UpdateAutomatismo(uow, form, context);
                else
                    Insert(uow, form, context);

                uow.SaveChanges();
                uow.Commit();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "AUT100ModalCreacionConfiguracion - FormSubmit");

                uow.Rollback();

                context.AddParameter("AUT100_POSICIONES_CREADAS", "N");
                context.AddParameter("AUT100_INTERFACES_CREADAS", "N");
                context.AddParameter("AUT100_CARACTERISTICAS_CREADAS", "N");

                context.AddErrorNotification(e.Message);
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new CreateAutomatismoForm1ValidationModule(uow, this._identity), form, context);
        }

        public override Form FormButtonAction(Form form, FormButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                uow.BeginTransaction();
                uow.CreateTransactionNumber($"Reestablecer características por defecto");

                var nuAutomatismo = context.GetParameter("AUT100_NU_AUTOMATISMO");

                if (string.IsNullOrEmpty(nuAutomatismo) || !int.TryParse(nuAutomatismo, out int numeroAutomatismo))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                AutomatismoLogic logic = new AutomatismoLogic(uow, _automatismoFactory);

                var automatismo = uow.AutomatismoRepository.GetAutomatismoById(numeroAutomatismo);

                automatismo.Caracteristicas = uow.AutomatismoCaracteristicaRepository.GetCaracteristicasByNumeroAutomatismo(numeroAutomatismo);

                logic.RestablecerCaracteristicasPorDefecto(automatismo);

                uow.SaveChanges();
                uow.Commit();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "AUT100ModalCreacionConfiguracion - FormButtonAction");

                uow.Rollback();

                context.AddErrorNotification(e.Message);
            }

            return form;
        }

        #region Metodos Auxiliares

        public virtual Form InitializeInsertForm(Form form)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var numeroAutomatismo = form.GetField("numeroAutomatismo");
            numeroAutomatismo.Value = "";
            numeroAutomatismo.ReadOnly = true;

            form.GetField("codigoExterno").Value = string.Empty;
            form.GetField("descripcion").Value = string.Empty;

            form.GetField("isEnabled").SetChecked(true);

            var zonaUbicacion = form.GetField("zonaUbicacion");
            zonaUbicacion.Value = "";
            zonaUbicacion.ReadOnly = true;

            form.GetField("qtPicking").Value = string.Empty;
            form.GetField("qtEntrada").Value = string.Empty;
            form.GetField("qtAjuste").Value = string.Empty;
            form.GetField("qtRechazo").Value = string.Empty;
            form.GetField("qtTransito").Value = string.Empty;
            form.GetField("qtSalida").Value = string.Empty;

            this.InicializarSelectTipo(uow, form);

            this.InicializarSelectPredio(uow, form);

            return form;
        }

        public virtual Form InitializeUpdateForm(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (string.IsNullOrEmpty(form.GetField("numeroAutomatismo").Value))
                throw new Exception("AUT100Modal_Sec0_Error_AutomatismoNoSeleccionado");

            var nuAutomatismo = int.Parse(form.GetField("numeroAutomatismo").Value);

            form.GetField("numeroAutomatismo").Disabled = true;

            IAutomatismo automatismo = uow.AutomatismoRepository.GetAutomatismoById(nuAutomatismo);

            var codigoExterno = form.GetField("codigoExterno");
            codigoExterno.Value = automatismo.Codigo;

            var descripcion = form.GetField("descripcion");
            descripcion.Value = automatismo.Descripcion;

            var zonaUbicacion = form.GetField("zonaUbicacion");
            zonaUbicacion.Value = automatismo.ZonaUbicacion;
            zonaUbicacion.ReadOnly = true;

            form.GetField("isEnabled").SetChecked(automatismo.IsEnabled);

            this.InicializarSelectTipo(uow, form);

            var tipoAutomatismo = form.GetField("tipo");
            tipoAutomatismo.Value = automatismo.Tipo;
            tipoAutomatismo.ReadOnly = true;

            var predio = form.GetField("predio");

            InicializarSelectPredio(uow, form);

            predio.Value = automatismo.Predio;
            predio.ReadOnly = true;

            if (uow.AutomatismoPosicionRepository.AnyPosicionAutomatismo(nuAutomatismo))
                context.AddParameter("AUT100_POSICIONES_CREADAS", "S");

            if (uow.AutomatismoInterfazRepository.AutomatismoHasAnyInterfaz(nuAutomatismo))
                context.AddParameter("AUT100_INTERFACES_CREADAS", "S");

            if (uow.AutomatismoCaracteristicaRepository.AutomatismoHasAnyCaracteristica(nuAutomatismo))
                context.AddParameter("AUT100_CARACTERISTICAS_CREADAS", "S");

            context.AddParameter("SHOW_GRID_PUESTOS", "S");
            context.AddParameter("NU_AUTOMATISMO", nuAutomatismo.ToString());

            return form;
        }

        protected virtual void InicializarSelectTipo(IUnitOfWork uow, Form form)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            var dominios = uow.DominioRepository.GetDominios(AutomatismoDb.TIPO_AUTOMATISMO_DOMAIN);

            foreach (var dominio in dominios)
            {
                if (this._security.IsUserAllowed(SecurityResources.AUT100_Access_Allow_TipoAutomatismo + dominio.Valor))
                    opciones.Add(new SelectOption(dominio.Valor, dominio.Descripcion));
            }

            if (opciones.Count == 1)
            {
                form.GetField("tipo").Value = opciones.FirstOrDefault().Value;

                if (form.GetField("tipo").Value == AutomatismoTipo.AutoStore)
                {
                    form.GetField("qtPicking").Value = "1";
                    form.GetField("qtPicking").ReadOnly = true;
                }
                else
                {
                    form.GetField("qtPicking").ReadOnly = false;
                }
            }
            form.GetField("tipo").Options = opciones;
        }

        protected virtual void InicializarSelectPredio(IUnitOfWork uow, Form form)
        {
            FormField selectPredio = form.GetField("predio");
            selectPredio.Options = new List<SelectOption>();
            var dbQuery = new GetPrediosUsuarioQuery();
            uow.HandleQuery(dbQuery);

            var predios = dbQuery.GetPrediosUsuario(_identity.UserId).OrderBy(x => x.Numero);

            foreach (var pred in predios)
                selectPredio.Options.Add(new SelectOption(pred.Numero, $"{pred.Numero} - { pred.Descripcion}"));

            if (predios.Count() == 1)
                selectPredio.Value = predios.FirstOrDefault().Numero;

            if (!this._identity.Predio.Equals(GeneralDb.PredioSinDefinir))
                selectPredio.Value = this._identity.Predio;
        }

        public virtual void Insert(IUnitOfWork uow, Form form, FormSubmitContext context)
        {
            var logic = new AutomatismoLogic(uow, _automatismoFactory);

            uow.CreateTransactionNumber($"Creación Automatismo - {_identity.Application}");

            var automatismo = logic.GetAutomatismo(form.GetField("codigoExterno").Value,
                                                   form.GetField("descripcion").Value,
                                                   form.GetField("tipo").Value,
                                                   form.GetField("predio").Value,
                                                   form.GetField("isEnabled").IsChecked());

            logic.CrearZonaUbicacion(automatismo);

            uow.SaveChanges();

            uow.AutomatismoRepository.Add(automatismo);

            var cantidadUbicaciones = GetCantidadUbicaciones(form);

            if (logic.InsertarCaracteristicasPorDefecto(automatismo))
                context.AddParameter("AUT100_CARACTERISTICAS_CREADAS", "S");

            automatismo.GenerarUbicaciones(cantidadUbicaciones);

            uow.SaveChanges();

            logic.CrearUbicaciones(automatismo);

            if (automatismo.Posiciones.Count > 0)
                context.AddParameter("AUT100_POSICIONES_CREADAS", "S");

            if (logic.CreateAutomatismoInterfaz(automatismo))
                context.AddParameter("AUT100_INTERFACES_CREADAS", "S");

            uow.SaveChanges();

            context.AddParameter("AUT100_NU_AUTOMATISMO", automatismo.Numero.ToString());

            if (string.IsNullOrEmpty(form.GetField("codigoExterno").Value))
                form.GetField("codigoExterno").Value = automatismo.Numero.ToString();

            form.GetField("numeroAutomatismo").Value = automatismo.Numero.ToString();
            form.GetField("zonaUbicacion").Value = automatismo.ZonaUbicacion;

            context.AddSuccessNotification("AUT100Modal_Sec0_Success_AutomatismoCreado");
        }

        public virtual void UpdateAutomatismo(IUnitOfWork uow, Form form, FormSubmitContext context)
        {
            uow.CreateTransactionNumber("AUT100 - UpdateAutomatismo");

            var numeroAutomatismo = int.Parse(form.GetField("numeroAutomatismo").Value);

            IAutomatismo automatismo = uow.AutomatismoRepository.GetAutomatismoById(numeroAutomatismo);

            automatismo.Codigo = form.GetField("codigoExterno").Value;
            automatismo.Descripcion = form.GetField("descripcion").Value;
            automatismo.Tipo = form.GetField("tipo").Value;
            automatismo.IsEnabled = form.GetField("isEnabled").IsChecked();
            automatismo.Transaccion = uow.GetTransactionNumber();

            uow.AutomatismoRepository.Update(automatismo);

            uow.SaveChanges();

            var logic = new AutomatismoLogic(uow, _automatismoFactory);

            logic.UpdateZonaUbicacion(automatismo);

            if (context.Parameters.Any(i => i.Id == "UPDATE_UBICACIONES" && i.Value == "true"))
                UpdateUbicaciones(uow, form, context, automatismo);

            context.AddSuccessNotification("AUT100Modal_Sec0_Success_AutomatismoModificado");

            uow.SaveChanges();
        }

        public virtual void UpdateUbicaciones(IUnitOfWork uow, Form form, FormSubmitContext context, IAutomatismo automatismo)
        {
            var cantidadUbicaciones = GetCantidadUbicaciones(form);

            var logic = new AutomatismoLogic(uow, _automatismoFactory);

            automatismo.GenerarUbicaciones(cantidadUbicaciones);

            logic.CrearUbicaciones(automatismo);

            context.AddParameter("AUT100_POSICIONES_CREADAS", "S");

        }

        public virtual AutomatismoCantidades GetCantidadUbicaciones(Form form)
        {
            return new AutomatismoCantidades(
                    int.TryParse(form.GetField("qtPicking").Value, out int qtPicking) ? qtPicking : 0,
                    int.TryParse(form.GetField("qtEntrada").Value, out int qtEntrada) ? qtEntrada : 0,
                    int.TryParse(form.GetField("qtAjuste").Value, out int qtAjuste) ? qtAjuste : 0,
                    int.TryParse(form.GetField("qtRechazo").Value, out int qtRechazo) ? qtRechazo : 0,
                    int.TryParse(form.GetField("qtTransito").Value, out int qtTransito) ? qtTransito : 0,
                    int.TryParse(form.GetField("qtSalida").Value, out int qtSalida) ? qtSalida : 0);
        }

        #endregion
    }
}