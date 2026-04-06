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
    public class STO210FormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;
        public STO210FormValidationModule(IUnitOfWork uow,
             IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;

            this.Schema = new FormValidationSchema
            {
                ["ID_ENVASE"] = this.ValidateID_ENVASE,
                ["ND_TP_ENVASE"] = this.ValidateND_TP_ENVASE,
                ["CD_AGENTE"] = this.ValidateCD_AGENTE,
                ["ND_ESTADO_ENVASE"] = this.ValidateND_ESTADO_ENVASE,
                ["DS_OBSERVACIONES"] = this.ValidateDS_OBSERVACIONES,
                ["CD_EMPRESA"] = this.ValidateCD_EMPRESA,
                ["TP_AGENTE"] = this.ValidateTP_AGENTE,
            };
        }

        public virtual FormValidationGroup ValidateID_ENVASE(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 20),
                   new ExisteEnvaseValidationRule(this._uow,field.Value,form.GetField("ND_TP_ENVASE").Value),
                },
            };
        }
        public virtual FormValidationGroup ValidateTP_AGENTE(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 3),
                },
            };
        }

        public virtual FormValidationGroup ValidateCD_AGENTE(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 40),
                   new ExisteAgenteValidationRule(this._uow,field.Value,form.GetField("TP_AGENTE").Value,int.Parse(form.GetField("CD_EMPRESA").Value)),

                },
            };
        }
        public virtual FormValidationGroup ValidateND_TP_ENVASE(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 10),
                   new ExisteTipoEnvaseValidationRule(this._uow,field.Value),
                },
            };
        }
        public virtual FormValidationGroup ValidateND_ESTADO_ENVASE(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 10),

                },
            };
        }
        public virtual FormValidationGroup ValidateDS_OBSERVACIONES(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value)) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new StringMaxLengthValidationRule(field.Value, 200),

                },
            };
        }
        public virtual FormValidationGroup ValidateCD_EMPRESA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new NumberValidationRule<int>(field.Value,_culture),
                   new ExisteEmpresaValidationRule(this._uow,field.Value),
                },
            };
        }
    }
}
