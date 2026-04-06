using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Preparacion
{
    public class ConsultaAvancePedidosFormValidationModule : FormValidationModule
    {
        public ConsultaAvancePedidosFormValidationModule()
        {
            this.Schema = new FormValidationSchema
            {
                ["DsInventario"] = this.ValidateDsInventario
            };
        }

        public virtual FormValidationGroup ValidateDsInventario(FormField field, Form form, List<ComponentParameter> parameters)
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
    }
}
