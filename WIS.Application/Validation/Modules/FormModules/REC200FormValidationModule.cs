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
    public class REC200FormValidationModule : FormValidationModule
    {
        public REC200FormValidationModule()
        {
            this.Schema = new FormValidationSchema
            {
                ["tpSeleccion"] = this.ValidateTipoSeleccion
            };
        }

        public virtual FormValidationGroup ValidateTipoSeleccion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new CrossDockingSelectionTypeValidationRule(field.Value)
                }
            };
        }
    }
}
