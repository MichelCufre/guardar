using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class REG310ReglaFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService identity;
        public REG310ReglaFormValidationModule(IUnitOfWork uow, IIdentityService identity)
        {
            this._uow = uow;
            this.identity = identity;

            Schema = new FormValidationSchema
            {
                ["descripcion"] = this.ValidateDescripcion,
                ["grupo"] = this.ValidateGrupo,
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
                    new StringMaxLengthValidationRule(field.Value,100)
                },
            };
        }

        public virtual FormValidationGroup ValidateGrupo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new GrupoNoExistenteValidationRule(this._uow, field.Value)
                },
            };
        }
    }
}
