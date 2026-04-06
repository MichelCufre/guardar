using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class REG080FormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _security;

        public REG080FormValidationModule(IUnitOfWork uow, IIdentityService security)
        {
            this._uow = uow;
            this._security = security;

            this.Schema = new FormValidationSchema
            {
                ["CD_PORTA"] = this.ValidatePuerta,
                ["DS_PORTA"] = this.ValidateDescPuerta,
                ["CD_SITUACAO"] = this.ValidateSituacion,
                ["NU_PREDIO"] = this.ValidatePredio,
                ["CD_EMPRESA"] = this.ValidateEmpresa,
                ["ID_BLOQUE"] = this.ValidateBloque,
                ["TP_PUERTA"] = this.ValidateTipoPuerta
            };
        }

        public virtual FormValidationGroup ValidatePuerta(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value) || field.ReadOnly) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveShortNumberMaxLengthValidationRule(field.Value, 3),
                    new PuertaEmbarqueInexistenteValidationRule(this._uow, field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateDescPuerta(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value)) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 30),
                }
            };
        }

        public virtual FormValidationGroup ValidateSituacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 3)
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
                    new ExistePredioValidationRule(this._uow, this._security.UserId, this._security.Predio, field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value)) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 10),
                    new PositiveIntValidationRule(field.Value),
                    new EmpresaExistenteValidationRule(this._uow, field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateBloque(FormField field, Form form, List<ComponentParameter> parameters)
        {

            var config = _uow.UbicacionRepository.GetUbicacionConfiguracion();

            List<IValidationRule> reglas = new List<IValidationRule>();

            reglas.Add(new NonNullValidationRule(field.Value));
            reglas.Add(new StringMaxLengthValidationRule(field.Value, config.BloqueLargo));

            if (config.BloqueNumerico)
            {
                reglas.Add(new PositiveLongValidationRule(field.Value));
            }
            else
            {
                reglas.Add(new StringSoloLetrasValidationRule(field.Value));
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = reglas
            };
        }

        public virtual FormValidationGroup ValidateTipoPuerta(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 10)
                }
            };
        }
    }
}
