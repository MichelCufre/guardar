using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Recepcion;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class REG020FormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public REG020FormValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new FormValidationSchema
            {
                ["codigoFamilia"] = this.ValidateCodigoFamilia,
                ["descripcionFamilia"] = this.ValidateDescripcionFamilia
            };
        }

        public virtual FormValidationGroup ValidateCodigoFamilia(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new PositiveIntValidationRule(field.Value),
                   new CodigoFamiliaProductoRule(_uow, field.Value),
                },
            };
        }

        public virtual FormValidationGroup ValidateDescripcionFamilia(FormField field, Form form, List<ComponentParameter> parameters)
        {

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 100),
                },
            };
        }
    }
}
