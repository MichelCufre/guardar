using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class CreateEquipoFormValidationModule : FormValidationModule
    {
        protected readonly int _userId;
        protected readonly string _userPredio;
        protected readonly IUnitOfWork _uow;

        public CreateEquipoFormValidationModule(IUnitOfWork uow, int userId, string userPredio)
        {
            this._uow = uow;
            this._userId = userId;
            this._userPredio = userPredio;

            this.Schema = new FormValidationSchema
            {
                ["predio"] = this.ValidatePredio,
                ["descripcion"] = this.ValidateDescripcion,
                ["tipo"] = this.ValidateTipoHerramienta,
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
                   new StringMaxLengthValidationRule(field.Value, 40)
                }
            };
        }
        public virtual FormValidationGroup ValidateTipoHerramienta(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 4),
                   new ExisteHerramientaValidationRule(this._uow, field.Value)
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
                   new StringMaxLengthValidationRule(field.Value, 10),
                   new ExistePredioValidationRule(this._uow, this._userId, this._userPredio, field.Value)
                }
            };
        }
    }
}
