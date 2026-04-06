using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Recepcion
{
    public class REC275PanelDeEstrategiasFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public REC275PanelDeEstrategiasFormValidationModule(
            IUnitOfWork uow,
            IIdentityService identity)
        {
            this._uow = uow;
            this._culture = identity.GetFormatProvider();

            Schema = new FormValidationSchema
            {
                ["nombreEstrategia"] = this.ValidateNombreEstrategia,
            };
        }

        public virtual FormValidationGroup ValidateNombreEstrategia(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value,200)
                },
            };
        }
    }
}
