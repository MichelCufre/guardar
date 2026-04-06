using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class PRD230FormValidationModule : FormValidationModule
    {
        public PRD230FormValidationModule()
        {
            this.Schema = new FormValidationSchema
            {
                ["tpSalida"] = this.ValidateTP_SALIDA
            };
        }

        public virtual FormValidationGroup ValidateTP_SALIDA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value)
                }
            };
        }
    }
}
