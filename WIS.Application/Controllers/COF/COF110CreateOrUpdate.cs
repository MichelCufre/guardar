using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Configuracion;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Integracion;
using WIS.Domain.Integracion.Enums;
using WIS.Domain.Security;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.COF
{
    public class COF110CreateOrUpdate : AppController
    {
        protected readonly ILogger<COF110CreateOrUpdate> _logger;

        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;

        public COF110CreateOrUpdate(IUnitOfWorkFactory uowFactory, IIdentityService identity, IFormValidationService formValidationService, ILogger<COF110CreateOrUpdate> logger)
        {
            _logger = logger;
            _uowFactory = uowFactory;
            _identity = identity;
            _formValidationService = formValidationService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                InicializarFormulario(uow, form, context);
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
            }

            return form;
        }
        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber(this._identity.Application);
            uow.BeginTransaction();
            try
            {
                var isUpdate = context.Parameters.FirstOrDefault(i => i.Id == "isUpdate")?.Value == "S";

                if (isUpdate)
                {
                    var nroIntegracion = context.GetParameter("nroIntegracion");

                    if (!int.TryParse(nroIntegracion, out int id))
                        throw new ValidationFailedException("COF110_Sec0_Error_NumeroIntegracionIncorrecto");

                    var integracion = uow.IntegracionServicioRepository.GetIntegrationById(id);

                    if (integracion == null)
                        throw new ValidationFailedException("COF110_Sec0_Error_ServicioIntegracionNoExisteIncorrecto");

                    UpdateIntegracion(uow, form, integracion);
                }
                else
                    InsertIntegracion(uow, form);

                context.AddSuccessNotification("General_Sec0_Success_SavedChanges");

                uow.SaveChanges();
                uow.Commit();
            }
            catch (ValidationFailedException ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                uow.Rollback();
                context.AddErrorNotification(ex.Message);
            }

            return form;
        }
        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new CreateIntegracionServicioFormValidationModule(uow, this._identity), form, context);
        }


        #region Metodos Auxiliares

        public virtual void InicializarFormulario(IUnitOfWork uow, Form form, FormInitializeContext context)
        {
            InicializarSelect(uow, form);

            var isUpdate = context.Parameters.FirstOrDefault(i => i.Id == "isUpdate")?.Value == "S";

            if (isUpdate)
            {
                var nroIntegracion = context.GetParameter("nroIntegracion");

                if (!int.TryParse(nroIntegracion, out int id))
                    throw new ValidationFailedException("COF110_Sec0_Error_NumeroIntegracionIncorrecto");

                var integracion = uow.IntegracionServicioRepository.GetIntegrationById(id);

                if (integracion == null)
                    throw new ValidationFailedException("COF110_Sec0_Error_ServicioIntegracionNoExisteIncorrecto");

                form.GetField("codigo").Value = integracion.Codigo;
                form.GetField("isEnabled").SetChecked(integracion.Habilitado);
                form.GetField("descripcion").Value = integracion.Descripcion;
                form.GetField("urlIntegracion").Value = integracion.UrlIntegracion;
                form.GetField("tipoComunicacion").Value = integracion.TipoComunicacion;
                form.GetField("user").Value = integracion.User;

                form.GetField("tipoAutorizacion").Value = integracion.TipoAutenticacion;
                if (integracion.TipoAutenticacion == TipoAutenticacionDb.OAUTH2)
                {
                    form.GetField("authServer").Value = integracion.UrlAuthServer;
                    form.GetField("scope").Value = integracion.Scope;
                    context.AddParameter("SHOW_OAUTH2_FIELDS", "S");
                }
            }
        }
        public virtual void InicializarSelect(IUnitOfWork uow, Form form)
        {
            var dominios = uow.DominioRepository.GetDominios(IntegracionServicioDb.TIPO_AUTOMATISMO_AUTH_DOMAIN);
            foreach (var d in dominios)
            {
                form.GetField("tipoAutorizacion").Options.Add(new SelectOption(d.Valor, d.Descripcion));
            }

            dominios = uow.DominioRepository.GetDominios(IntegracionServicioDb.TIPO_COMUNICACION_SERVICIOS_DOMAIN);
            foreach (var d in dominios)
            {
                form.GetField("tipoComunicacion").Options.Add(new SelectOption(d.Id, d.Descripcion));
            }
        }

        public virtual void UpdateIntegracion(IUnitOfWork uow, Form form, IntegracionServicio integracion)
        {
            CreateIntegracionServicio(form, integracion);

            integracion.FechaModificacion = DateTime.Now;
            integracion.Transaccion = uow.GetTransactionNumber();

            uow.IntegracionServicioRepository.Update(integracion);
        }

        public virtual void InsertIntegracion(IUnitOfWork uow, Form form)
        {
            var integracion = new IntegracionServicio();
            CreateIntegracionServicio(form, integracion);

            integracion.FechaRegistro = DateTime.Now;
            integracion.Transaccion = uow.GetTransactionNumber();

            uow.IntegracionServicioRepository.Add(integracion);
        }

        public virtual void CreateIntegracionServicio(Form form, IntegracionServicio integracionServicio)
        {
            if (ShouldInsertOrUpdatePassword(form))
            {
                var password = form.GetField("password").Value;

                string salt = string.Empty;
                int format = 0;

                integracionServicio.Secret = !string.IsNullOrEmpty(password) ? Encrypter.Encrypt(password, out salt, out format) : string.Empty;
                integracionServicio.SecretSalt = salt;
                integracionServicio.SecretFormat = format;
            }

            integracionServicio.TipoAutenticacion = form.GetField("tipoAutorizacion").Value;
            integracionServicio.User = form.GetField("user").Value;
            integracionServicio.Scope = form.GetField("scope").Value;
            integracionServicio.UrlAuthServer = form.GetField("authServer").Value;
            integracionServicio.Codigo = form.GetField("codigo").Value;
            integracionServicio.Habilitado = form.GetField("isEnabled").IsChecked();
            integracionServicio.Descripcion = form.GetField("descripcion").Value;
            integracionServicio.TipoComunicacion = form.GetField("tipoComunicacion").Value;
            integracionServicio.UrlIntegracion = form.GetField("urlIntegracion").Value;
        }

        public virtual bool ShouldInsertOrUpdatePassword(Form form)
        {
            var updatePasswordCheckIsDisabled = form.GetField("isUpdatePassword").Disabled;

            var updatePasswordIsChecked = form.GetField("isUpdatePassword").IsChecked();

            return (updatePasswordCheckIsDisabled || !updatePasswordCheckIsDisabled && updatePasswordIsChecked);
        }

        #endregion
    }
}
