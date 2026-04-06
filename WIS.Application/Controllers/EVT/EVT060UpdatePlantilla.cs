using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Eventos;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.EVT
{
    public class EVT060UpdatePlantilla : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IParameterService _parameterService;

        public EVT060UpdatePlantilla(IIdentityService identity,
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
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(context.GetParameter("nuEvento"), out int nuEvento))
                throw new ValidationFailedException("EVT060UpdatePlantilla_Sec0_Error_EventoNoValido");

            var tpNotificacion = context.GetParameter("tpNotificacion");
            var cdPlantilla = context.GetParameter("cdPlantilla");

            if (!string.IsNullOrEmpty(tpNotificacion) && !string.IsNullOrEmpty(cdPlantilla))
            {
                EventoTemplate plantilla = uow.EventoRepository.GetEventoTemplate(nuEvento, tpNotificacion, cdPlantilla);

                if (plantilla == null)
                    throw new ValidationFailedException("EVT060UpdatePlantilla_Frm1_Error_PlantillaNoExiste");

                this.InicializarCamposUpdate(uow, form, plantilla);
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new UpdateEventoTemplateFormValidationModule(uow, this._identity, this._security), form, context);
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("Modificación de plantillas de notificaciones");
            var nuTransaccion = uow.GetTransactionNumber();

            try
            {
                if (!int.TryParse(context.GetParameter("nuEvento"), out int nuEvento))
                    throw new ValidationFailedException("EVT060UpdatePlantilla_Sec0_Error_EventoNoValido");

                var tpNotificacion = context.GetParameter("tpNotificacion");
                var cdPlantilla = context.GetParameter("cdPlantilla");

                EventoTemplate plantilla = uow.EventoRepository.GetEventoTemplate(nuEvento, tpNotificacion, cdPlantilla);

                if (plantilla == null)
                    throw new ValidationFailedException("EVT060UpdatePlantilla_Frm1_Error_PlantillaNoExiste");


                plantilla.dsEstilo = form.GetField("descripcionPlantilla").Value;
                plantilla.Asunto = form.GetField("asuntoNotificacion").Value;
                plantilla.IsHtml = bool.Parse(form.GetField("habilitarHtml").Value);
                plantilla.Cuerpo = form.GetField("cuerpoNotificacion").Value;
                plantilla.FechaModificacion = DateTime.Now;
                plantilla.NumeroTransaccion = nuTransaccion;

                uow.EventoRepository.UpdateTemplate(plantilla);

                uow.SaveChanges();
                uow.Commit();
                context.AddSuccessNotification("EVT060UpdatePlantilla_Frm1_Success_Edicion");
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }
            return form;
        }

        #region Metodos Auxiliares

        public virtual void InicializarCamposUpdate(IUnitOfWork uow, Form form, EventoTemplate plantilla)
        {

            form.GetField("codigoPlantilla").ReadOnly = true;
            form.GetField("codigoEvento").ReadOnly = true;
            form.GetField("tipoNotificacion").ReadOnly = true;

            form.GetField("codigoPlantilla").Value = plantilla.CdEstilo;
            form.GetField("descripcionPlantilla").Value = plantilla.dsEstilo;
            form.GetField("asuntoNotificacion").Value = plantilla.Asunto;
            form.GetField("cuerpoNotificacion").Value = plantilla.Cuerpo;
            form.GetField("previewCuerpoNotificacion").Value = plantilla.Cuerpo;
            form.GetField("habilitarHtml").Value = plantilla.IsHtml.ToString();

            FormField selectEvento = form.GetField("codigoEvento");
            selectEvento.Options = new List<SelectOption>();

            uow.EventoRepository.GetEventos()?.ForEach(w =>
            {
                selectEvento.Options.Add(new SelectOption(w.Id.ToString(), w.Nombre));
            });
            selectEvento.Value = plantilla.nuEvento.ToString();

            FormField selectTipoNotificacion = form.GetField("tipoNotificacion");
            selectTipoNotificacion.Options = new List<SelectOption>();

            uow.DominioRepository.GetDominios(CodigoDominioDb.EventoTipoNotificacion)?.ForEach(w =>
            {
                selectTipoNotificacion.Options.Add(new SelectOption(w.Valor, w.Descripcion));
            });
            selectTipoNotificacion.Value = plantilla.TipoNotificacion.ToString();
        }
        
        #endregion

    }
}
