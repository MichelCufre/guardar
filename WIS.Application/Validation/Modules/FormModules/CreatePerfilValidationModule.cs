using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Recepcion;
using WIS.Components.Common;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class CreatePerfilValidationModule : FormValidationModule
    {
        public CreatePerfilValidationModule()
        {
            this.Schema = new FormValidationSchema
            {
                ["descripcion"] = this.ValidateDescripcion
            };
        }

        public virtual FormValidationGroup ValidateDescripcion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value,200)
                }
            };
        }
    }
}
