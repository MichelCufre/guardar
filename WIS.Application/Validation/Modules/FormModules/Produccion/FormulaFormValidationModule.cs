using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Produccion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Produccion
{
    public class FormulaFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;
        protected readonly bool _isUpdate;

        public FormulaFormValidationModule(IUnitOfWork uow, IFormatProvider culture, bool isUpdate = false)
        {
            this._uow = uow;
            this._culture = culture;
            this._isUpdate = isUpdate;

            if (_isUpdate)
            {
                this.Schema = new FormValidationSchema
                {
                    ["nombre"] = this.ValidateNombre,
                    ["descripcion"] = this.ValidateDescripcion,
                    ["pasadas"] = this.ValidatePasadas
                };
            }
            else
            {
                this.Schema = new FormValidationSchema
                {
                    ["codigo"] = this.ValidateCodigo,
                    ["empresa"] = this.ValidateEmpresa,
                    ["nombre"] = this.ValidateNombre,
                    ["descripcion"] = this.ValidateDescripcion,
                    ["pasadas"] = this.ValidatePasadas
                };

            }
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

        public virtual FormValidationGroup ValidatePasadas(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveIntValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 10)
                }
            };
        }

    }
}
