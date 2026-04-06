using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class MantenimientoServidoresImpresionValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public MantenimientoServidoresImpresionValidationModule(IUnitOfWork uow)
        {
            this.Schema = new FormValidationSchema
            {
                ["dsServidor"] = this.ValidateDsServidor,
                ["dominioServidor"] = this.ValidateDominioServidor,
                ["clientId"] = this.ValidateClientId,
            };

            this._uow = uow;
        }

        public virtual FormValidationGroup ValidateDsServidor(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 200),
                },
            };
        }

        public virtual FormValidationGroup ValidateDominioServidor(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 70),
                },
            };
        }

        public virtual FormValidationGroup ValidateClientId(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                },
            };
        }
    }
}
