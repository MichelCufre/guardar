using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class CreatePrepManualPedidoValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;

        public CreatePrepManualPedidoValidationModule(IUnitOfWork uow, IIdentityService identity, ISecurityService security)
        {
            this._uow = uow;
            this._identity = identity;
            this._security = security;

            this.Schema = new FormValidationSchema
            {
                ["descripcion"] = this.ValidateDesc,
                ["predio"] = this.ValidatePredio,
                ["empresa"] = this.ValidateEmpresa
            };
        }

        public virtual FormValidationGroup ValidateDesc(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string empresa = form.GetField("empresa").Value;

            if (string.IsNullOrEmpty(field.Value))
                return null;

            var rules = new List<IValidationRule>
            {
                //new NonNullValidationRule(field.Value),
                new StringMaxLengthValidationRule(field.Value, 60)
            };

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
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
                   new StringMaxLengthValidationRule(field.Value, 15),
                   new ExistePredioValidationRule(this._uow, this._identity.UserId, this._identity.Predio, field.Value)
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
                   new StringMaxLengthValidationRule(field.Value, 10),
                   new PositiveIntValidationRule(field.Value),
                   new ExisteEmpresaValidationRule(this._uow, field.Value),
                   new UserCanWorkWithEmpresaValidationRule(this._security, field.Value)
                }
            };
        }
    }
}
