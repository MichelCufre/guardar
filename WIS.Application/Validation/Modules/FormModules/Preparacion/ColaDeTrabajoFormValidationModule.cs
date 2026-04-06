using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Preparacion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Preparacion
{
    public class ColaDeTrabajoFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public ColaDeTrabajoFormValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new FormValidationSchema
            {
                ["nuColaTrabajo"] = this.ValidateNuColaTrabajo,
                ["nuPredio"] = this.ValidatePredio,
                ["descripcion"] = this.ValidateDescripcion,
            };
        }


        public virtual FormValidationGroup ValidateNuColaTrabajo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                    new ExisteColaDeTrabajoValidationRule(_uow, field.Value),
                    new PositiveIntValidationRule(field.Value)
                },
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
                }
            };
        }

        public virtual FormValidationGroup ValidateDescripcion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 200),
                },
            };
        }
    }
}
