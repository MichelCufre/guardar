using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Porteria
{
    public class RegistroEntradaAsociarContactosFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public RegistroEntradaAsociarContactosFormValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new FormValidationSchema
            {
                ["NU_DOCUMENTO"] = this.ValidateNU_DOCUMENTO,
                ["NU_DOCUMENTO_SEARCH"] = this.ValidateNU_DOCUMENTO,
                ["NM_PERSONA"] = this.ValidateNM_PERSONA,
                ["AP_PERSONA"] = this.ValidateAP_PERSONA,
                ["NU_CELULAR"] = this.ValidateNU_CELULAR,
            };
        }

        public virtual FormValidationGroup ValidateNU_DOCUMENTO(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new StringMaxLengthValidationRule(field.Value, 20)
                },
            };
        }
        public virtual FormValidationGroup ValidateNM_PERSONA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new StringMaxLengthValidationRule(field.Value, 100)
                },
            };
        }
        public virtual FormValidationGroup ValidateAP_PERSONA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new StringMaxLengthValidationRule(field.Value, 100)
                },
            };
        }
        public virtual FormValidationGroup ValidateNU_CELULAR(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new StringMaxLengthValidationRule(field.Value, 15),

                },
            };
        }                
    }
}
