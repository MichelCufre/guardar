using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Configuracion;
using WIS.Application.Validation.Rules.Impresion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General.Configuracion;
using WIS.Domain.Impresiones;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class MantenimientoTemplateEtiquetaValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public MantenimientoTemplateEtiquetaValidationModule(IUnitOfWork uow, IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;

            this.Schema = new FormValidationSchema
            {
                ["estilo"] = this.ValidateEstilo,
                ["lenguaje"] = this.ValidateLenguaje,
                ["cuerpo"] = this.ValidateCuerpo,
                ["altura"] = this.ValidateAltura,
                ["largura"] = this.ValidateLargura,
            };
        }

        public virtual FormValidationGroup ValidateEstilo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 15),
                    new NoExisteEstiloEtiquetaValidationRule(field.Value, this._uow)
                },
            };
        }
        public virtual FormValidationGroup ValidateLenguaje(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 10),
                    new ExisteLenguajeImpresionValidationRule(this._uow, field.Value)
                },
            };
        }
        public virtual FormValidationGroup ValidateCuerpo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 4000)
                },
            };
        }
        public virtual FormValidationGroup ValidateAltura(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new PositiveDecimalValidationRule(this._culture, field.Value)
                },
            };
        }
        public virtual FormValidationGroup ValidateLargura(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new PositiveDecimalValidationRule(this._culture, field.Value)
                },
            };
        }
    }
}
