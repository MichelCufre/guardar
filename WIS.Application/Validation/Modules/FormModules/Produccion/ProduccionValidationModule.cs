using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Produccion
{
    public class ProduccionValidationModule : FormValidationModule
    {
        public ProduccionValidationModule()
        {
            this.Schema = new FormValidationSchema
            {
                ["valor"] = this.ValidateValor,
                ["cantidadPasadas"] = this.ValidateCantidadPasadas
            };
        }

        public virtual FormValidationGroup ValidateValor(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var fieldPasadas = form.GetField("cantidadPasadas");

            if (fieldPasadas != null && !string.IsNullOrEmpty(fieldPasadas.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 1000)
                }
            };
        }
        public virtual FormValidationGroup ValidateCantidadPasadas(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new PositiveIntValidationRule(field.Value)
                }
            };
        }
    }
}
