using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Preparacion
{
    public class PreferenciaFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _formatProvider;

        public PreferenciaFormValidationModule(IUnitOfWork uow, IFormatProvider culture)
        {
            this._uow = uow;
            this._formatProvider = culture;

            this.Schema = new FormValidationSchema
            {
                ["dsPreferencia"] = this.ValidateDescripcionPreferencia,
                ["predio"] = this.ValidatePredio,
                ["bloqueMin"] = this.ValidateBloqueMinimo,
                ["bloqueMax"] = this.ValidateBloqueMaximo,
                ["calleMin"] = this.ValidateCalleMinima,
                ["calleMax"] = this.ValidateCalleMaxima,
                ["columnaMin"] = this.ValidateColumnaMinima,
                ["columnaMax"] = this.ValidateColumnaMaxima,
                ["alturaMin"] = this.ValidateAlturaMinima,
                ["alturaMax"] = this.ValidateAlturaMaxima,
                ["pesoMax"] = this.ValidatePesoMaximo,
                ["volumenMax"] = this.ValidateVolumenMaximo,
                ["clientesSimultaneos"] = this.ValidateClientesSimultaneos,
                ["pedidosSimultaneos"] = this.ValidatePedidosSimultaneos,
                ["maxPickeos"] = this.ValidatePickeosMaximos,
                ["maxUnidades"] = this.ValidateUnidadesMaximas,
                ["habilitarControlAcceso"] = this.ValidateControlAcceso,
            };
        }

        public virtual FormValidationGroup ValidateDescripcionPreferencia(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    //new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 200),
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
                    //new NonNullValidationRule(field.Value),
                }
            };
        }
        public virtual FormValidationGroup ValidateBloqueMinimo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    //new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 10),
                },
            };
        }
        public virtual FormValidationGroup ValidateBloqueMaximo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    //new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 10),
                },
            };
        }
        public virtual FormValidationGroup ValidateCalleMinima(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    //new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 10),
                },
            };
        }
        public virtual FormValidationGroup ValidateCalleMaxima(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    //new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 10),
                },
            };
        }
        public virtual FormValidationGroup ValidateColumnaMinima(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    //new NonNullValidationRule(field.Value),
                    new PositiveNumberMaxLengthValidationRule(field.Value,10),
                },
            };
        }
        public virtual FormValidationGroup ValidateColumnaMaxima(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    //new NonNullValidationRule(field.Value),
                    new PositiveNumberMaxLengthValidationRule(field.Value,10),
                },
            };
        }
        public virtual FormValidationGroup ValidateAlturaMinima(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    //new NonNullValidationRule(field.Value),
                    new PositiveNumberMaxLengthValidationRule(field.Value,10),
                },
            };
        }
        public virtual FormValidationGroup ValidateAlturaMaxima(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    //new NonNullValidationRule(field.Value),
                    new PositiveNumberMaxLengthValidationRule(field.Value,10),
                },
            };
        }
        public virtual FormValidationGroup ValidatePesoMaximo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    //new NonNullValidationRule(field.Value),
                    new PositiveDecimalValidationRule(_formatProvider, field.Value),
                    new DecimalLengthWithPresicionValidationRule(field.Value, 12, 3,_formatProvider)
                },
            };
        }
        public virtual FormValidationGroup ValidateVolumenMaximo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    //new NonNullValidationRule(field.Value),
                    new PositiveDecimalValidationRule(_formatProvider,field.Value),
                    new DecimalLengthWithPresicionValidationRule(field.Value, 14, 4, _formatProvider)
                },
            };
        }
        public virtual FormValidationGroup ValidateClientesSimultaneos(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    //new NonNullValidationRule(field.Value),
                    new PositiveNumberMaxLengthValidationRule(field.Value,10),
                },
            };
        }
        public virtual FormValidationGroup ValidatePedidosSimultaneos(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    //new NonNullValidationRule(field.Value),
                    new PositiveNumberMaxLengthValidationRule(field.Value,10),
                },
            };
        }
        public virtual FormValidationGroup ValidatePickeosMaximos(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    //new NonNullValidationRule(field.Value),
                    new PositiveNumberMaxLengthValidationRule(field.Value,10),
                },
            };
        }
        public virtual FormValidationGroup ValidateUnidadesMaximas(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    //new NonNullValidationRule(field.Value),
                    new PositiveNumberMaxLengthValidationRule(field.Value,10),
                },
            };
        }

        public virtual FormValidationGroup ValidateControlAcceso(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new BooleanStringValidationRule(field.Value)
                },
                OnSuccess = this.ValidateControlAccesoOnSuccess
            };
        }

        public virtual void ValidateControlAccesoOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var controlAccesoHabilitado = field.IsChecked();

            form.GetField("bloqueMin").ReadOnly = controlAccesoHabilitado;
            form.GetField("bloqueMax").ReadOnly = controlAccesoHabilitado;
            form.GetField("calleMin").ReadOnly = controlAccesoHabilitado;
            form.GetField("calleMax").ReadOnly = controlAccesoHabilitado;
            form.GetField("columnaMin").ReadOnly = controlAccesoHabilitado;
            form.GetField("columnaMax").ReadOnly = controlAccesoHabilitado;
            form.GetField("alturaMin").ReadOnly = controlAccesoHabilitado;
            form.GetField("alturaMax").ReadOnly = controlAccesoHabilitado;

            form.GetField("habilitarPedCompleto").Disabled = !controlAccesoHabilitado;
        }
    }
}
