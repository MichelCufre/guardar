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
    public class PRD111FormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _securityService;

        public PRD111FormValidationModule(IUnitOfWork uow, IIdentityService securityService)
        {
            _uow = uow;
            _securityService = securityService;

            Schema = new FormValidationSchema
            {
                ["descripcion"] = this.ValidateDescripcion,
                ["tipoLinea"] = this.ValidateTipoLinea,
                ["predio"] = this.ValidatePredio
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
        public virtual FormValidationGroup ValidateTipoLinea(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new TipoLineaValidationRule(this._uow,field.Value)
                }
            };
        }
        public virtual FormValidationGroup ValidatePredio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new ExistePredioValidationRule(this._uow,this._securityService.UserId,this._securityService.Predio,field.Value)
                }
            };
        }
    }
}
