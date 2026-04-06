using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class STO005FormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public STO005FormValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new FormValidationSchema
            {
                ["CD_EMPRESA"] = this.ValidateEmpresa,
                ["CD_PRODUTO"] = this.ValidateProducto
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
                    new ExisteEmpresaValidationRule(this._uow, field.Value)
                }
            };
        }
        public virtual FormValidationGroup ValidateProducto(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string empresa = form.GetField("CD_EMPRESA").Value;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new ProductoExistsValidationRule(this._uow, empresa, field.Value)
                },
                Dependencies = { "CD_EMPRESA" }
            };
        }
    }
}
