using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Produccion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Produccion
{
    public class PRD100IngresoFormulaValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _securityService;

        public PRD100IngresoFormulaValidationModule(IUnitOfWork uow, IIdentityService securityService)
        {
            _uow = uow;
            _securityService = securityService;

            Schema = new FormValidationSchema
            {
                ["codigo"] = ValidateCodigo,
                ["empresa"] = ValidateEmpresa,
                ["nombre"] = ValidateNombre,
                ["descripcion"] = ValidateDescripcion
            };
        }

        public virtual FormValidationGroup ValidateCodigo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 10),
                    new FormulaExistsValidationRule(_uow, field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveIntValidationRule(field.Value),
                    new ExisteEmpresaValidationRule(_uow, field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateNombre(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 100)
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
                    new StringMaxLengthValidationRule(field.Value, 2000)
                }
            };
        }
    }
}
