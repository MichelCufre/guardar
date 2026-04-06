using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class CreateTipoVehiculoValidationModule : FormValidationModule
    {
        protected readonly IFormatProvider _culture;

        public CreateTipoVehiculoValidationModule(IFormatProvider culture)
        {
            this._culture = culture;

            this.Schema = new FormValidationSchema
            {
                ["descripcion"] = this.ValidateDescripcion,
                ["volumen"] = this.ValidateVolumen,
                ["peso"] = this.ValidatePeso,
                ["pallets"] = this.ValidatePallets,
                ["porcentajeOcupacionVolumen"] = this.ValidatePorcentajeOcupacionVolumen,
                ["porcentajeOcupacionPeso"] = this.ValidatePorcentajeOcupacionPeso,
                ["porcentajeOcupacionPallet"] = this.ValidatePorcentajeOcupacionPallet,
                ["refrigerado"] = this.ValidateRefrigerado,
                ["cargaLateral"] = this.ValidateCargaLateral,
                ["admiteZorra"] = this.ValidateAdmiteZorra,
                ["soloCabina"] = this.ValidateSoloCabina
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
                   new StringMaxLengthValidationRule(field.Value, 100)
                }
            };
        }
        public virtual FormValidationGroup ValidateVolumen(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new DecimalLengthWithPresicionValidationRule(field.Value, 15, 3, this._culture)
                }
            };
        }
        public virtual FormValidationGroup ValidatePeso(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new DecimalLengthWithPresicionValidationRule(field.Value, 15, 3, this._culture)
                }
            };
        }
        public virtual FormValidationGroup ValidatePallets(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new DecimalLengthWithPresicionValidationRule(field.Value, 15, 3, this._culture)
                }
            };
        }
        public virtual FormValidationGroup ValidatePorcentajeOcupacionVolumen(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 3),
                   new PositiveIntValidationRule(field.Value),
                   new IntLowerThanValidationRule(field.Value, 100)
                }
            };
        }
        public virtual FormValidationGroup ValidatePorcentajeOcupacionPeso(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 3),
                   new PositiveIntValidationRule(field.Value),
                   new IntLowerThanValidationRule(field.Value, 100)
                }
            };
        }
        public virtual FormValidationGroup ValidatePorcentajeOcupacionPallet(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 3),
                   new PositiveIntValidationRule(field.Value),
                   new IntLowerThanValidationRule(field.Value, 100)
                }
            };
        }
        public virtual FormValidationGroup ValidateRefrigerado(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new BooleanStringValidationRule(field.Value)
                }
            };
        }
        public virtual FormValidationGroup ValidateCargaLateral(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new BooleanStringValidationRule(field.Value)
                }
            };
        }
        public virtual FormValidationGroup ValidateAdmiteZorra(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new BooleanStringValidationRule(field.Value)
                }
            };
        }
        public virtual FormValidationGroup ValidateSoloCabina(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new BooleanStringValidationRule(field.Value)
                }
            };
        }
    }
}