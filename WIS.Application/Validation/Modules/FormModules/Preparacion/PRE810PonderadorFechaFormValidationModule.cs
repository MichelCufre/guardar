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
    public class PRE810PonderadorFechaFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public PRE810PonderadorFechaFormValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new FormValidationSchema
            {
                ["dias"] = this.ValidateDias,
                ["horas"] = this.ValidateNumber,
                ["diasHasta"] = this.ValidateDiasHasta,
                ["horasHasta"] = this.ValidateHoraHasta,
                ["operador"] = this.ValidateOperador,
                ["ponderacion"] = this.ValidateNumber,
            };
        }
        public virtual FormValidationGroup ValidateDias(FormField field, Form form, List<ComponentParameter> parameters)
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
        public virtual FormValidationGroup ValidateDiasHasta(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (form.GetField("operador").Value != ColasTrabajoDb.OperacionEntre)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule> {
                     new NonNullValidationRule(field.Value),
                    new PositiveIntValidationRule(field.Value)
                },
            };
        }
        public virtual FormValidationGroup ValidateHoraHasta(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (form.GetField("operador").Value != ColasTrabajoDb.OperacionEntre)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                    new PositiveIntValidationRule(field.Value)
                },
            };
        }

        public virtual FormValidationGroup ValidateNumber(FormField field, Form form, List<ComponentParameter> parameters)
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

        public virtual FormValidationGroup ValidateOperador(FormField field, Form form, List<ComponentParameter> parameters)
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
