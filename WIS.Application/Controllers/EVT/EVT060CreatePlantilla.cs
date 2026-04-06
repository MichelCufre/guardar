using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Eventos;
using WIS.Domain.Services.Interfaces;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.EVT
{
    public class EVT060CreatePlantilla : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IParameterService _parameterService;

        public EVT060CreatePlantilla(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            ISecurityService security,
            IParameterService parameterService)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._security = security;
            this._parameterService = parameterService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {

            form.GetField("codigoPlantilla").Value = string.Empty;
            form.GetField("descripcionPlantilla").Value = string.Empty;
            form.GetField("codigoEvento").Value = string.Empty;
            form.GetField("tipoNotificacion").Value = string.Empty;
            form.GetField("asuntoNotificacion").Value = string.Empty;
            form.GetField("habilitarHtml").Value = "false";
            form.GetField("cuerpoNotificacion").Value = string.Empty;
            form.GetField("previewCuerpoNotificacion").Value = string.Empty;

            using var uow = this._uowFactory.GetUnitOfWork();

            this.InicializarSelects(uow, form);

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new CreateEventoTemplateFormValidationModule(uow, this._identity, this._security), form, context);
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber("Creación de plantillas de notificaciones");
            var nuTransaccion = uow.GetTransactionNumber();

            try
            {

                var tpNotificacion = TipoNotificacionHelper.GetTipoNotificacion(form.GetField("tipoNotificacion").Value);

                EventoTemplate plantilla = new EventoTemplate
                {
                    nuEvento = int.Parse(form.GetField("codigoEvento").Value),
                    CdEstilo = form.GetField("codigoPlantilla").Value,
                    dsEstilo = form.GetField("descripcionPlantilla").Value,
                    TipoNotificacion = tpNotificacion,
                    Asunto = form.GetField("asuntoNotificacion").Value,
                    IsHtml = bool.Parse(form.GetField("habilitarHtml").Value),
                    Cuerpo = form.GetField("cuerpoNotificacion").Value,
                    FechaAlta = DateTime.Now,
                    NumeroTransaccion = nuTransaccion
                };

                uow.EventoRepository.AddTemplate(plantilla);

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

        #region Metodos Auxiliares

        public virtual void InicializarSelects(IUnitOfWork uow, Form form)
        {
            FormField selectEvento = form.GetField("codigoEvento");
            selectEvento.Options = new List<SelectOption>();

            FormField selectTipoNotificacion = form.GetField("tipoNotificacion");
            selectTipoNotificacion.Options = new List<SelectOption>();

            uow.EventoRepository.GetEventos()?.ForEach(w =>
            {
                selectEvento.Options.Add(new SelectOption(w.Id.ToString(), w.Nombre));
            });

            uow.DominioRepository.GetDominios(CodigoDominioDb.EventoTipoNotificacion)?.ForEach(w =>
            {
                selectTipoNotificacion.Options.Add(new SelectOption(w.Valor, w.Descripcion));
            });
        }

        #endregion
    }
}
