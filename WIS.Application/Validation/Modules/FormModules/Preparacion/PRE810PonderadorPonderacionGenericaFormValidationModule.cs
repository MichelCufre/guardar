using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Preparacion
{
    public class PRE810PonderadorPonderacionGenericaFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public PRE810PonderadorPonderacionGenericaFormValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new FormValidationSchema
            {
                ["valor"] = this.ValidateValor,
                ["valorHasta"] = this.ValidatevalorHasta,
                ["operador"] = this.ValidateOperador,
                ["ponderacion"] = this.ValidateNumber,
            };
        }
        protected FormValidationGroup ValidateValor(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value)
                },
            };
        }
        protected FormValidationGroup ValidatevalorHasta(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (form.GetField("operador").Value != ColasTrabajoDb.OperacionEntre)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value)
                },
            };
        }
        protected FormValidationGroup ValidateNumber(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                    new PositiveIntValidationRule(field.Value)
                },
            };
        }

        protected FormValidationGroup ValidateOperador(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                },
                OnSuccess = ValidateOperadorOnSuccess
            };
        }
        public virtual void ValidateOperadorOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            parameters.Add(new ComponentParameter("operacionSelect", field.Value));

        }
    }
}
