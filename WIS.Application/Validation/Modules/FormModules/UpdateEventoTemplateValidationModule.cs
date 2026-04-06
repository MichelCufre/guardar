using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Evento;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class UpdateEventoTemplateFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;

        public UpdateEventoTemplateFormValidationModule(IUnitOfWork uow, IIdentityService identity, ISecurityService security)
        {
            this.Schema = new FormValidationSchema
            {
                ["codigoPlantilla"] = this.ValidateCodigoPlantilla,
                ["descripcionPlantila"] = this.ValidateDescripcionPlantilla,
                ["codigoEvento"] = this.ValidateNumeroEvento,
                ["tipoNotificacion"] = this.ValidateTipoNotificacion,
                ["asuntoNotificacion"] = this.ValidateAsuntoNotificacion,
                ["habilitarHtml"] = this.ValidateHabilitarHtml
            };

            this._uow = uow;
            this._identity = identity;
            this._security = security;
        }

        public virtual FormValidationGroup ValidateCodigoPlantilla(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var nuEvento = form.GetField("codigoEvento").Value;
            var tpNotificacion = form.GetField("tipoNotificacion").Value;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 15),
                   new NoExisteEventoTemplateValidationRule(this._uow, nuEvento, tpNotificacion, field.Value)
                },
                Dependencies = { "codigoEvento", "tipoNotificacion" }
            };
        }

        public virtual FormValidationGroup ValidateDescripcionPlantilla(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new StringMaxLengthValidationRule(field.Value, 200)
                }
            };
        }

        public virtual FormValidationGroup ValidateNumeroEvento(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new ExisteEventoValidationRule(this._uow, field.Value),
                   new StringMaxLengthValidationRule(field.Value, 10),
                   new PositiveIntValidationRule(field.Value),
                }
            };
        }

        public virtual FormValidationGroup ValidateTipoNotificacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 60)
                }
            };
        }

        public virtual FormValidationGroup ValidateAsuntoNotificacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new StringMaxLengthValidationRule(field.Value, 70)
                }
            };
        }

        public virtual FormValidationGroup ValidateHabilitarHtml(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new BooleanStringValidationRule(field.Value)
                }
            };
        }
    }
}
