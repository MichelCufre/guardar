using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Configuracion
{
    public class CreateIntegracionServicioFormValidationModule : FormValidationModule
    {
        protected IUnitOfWork _uow;
        protected IIdentityService _identity;

        public CreateIntegracionServicioFormValidationModule(IUnitOfWork uow, IIdentityService identity)
        {
            this.Schema = new FormValidationSchema
            {
                ["codigo"] = this.ValidateCodigo,
                ["descripcion"] = this.ValidateDescripcion,
                ["urlIntegracion"] = this.ValidateUrlIntegracion,
                ["authServer"] = this.ValidateUrlAuthServer,
                ["scope"] = this.ValidateScope,
                ["user"] = this.ValidateUser,
            };

            this._uow = uow;
            this._identity = identity;
        }

        public virtual FormValidationGroup ValidateCodigo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 300),
                }
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
                    new StringMaxLengthValidationRule(field.Value, 300),
                }
            };
        }

        public virtual FormValidationGroup ValidateUrlIntegracion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 300),
                }
            };
        }

        public virtual FormValidationGroup ValidateUrlAuthServer(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 100),
                }
            };
        }

        public virtual FormValidationGroup ValidateScope(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 100),
                }
            };
        }

        public virtual FormValidationGroup ValidateUser(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 100),
                }
            };
        }
    }
}
