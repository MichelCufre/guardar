using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class PRE250ReglasValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _idUsuario;
        protected readonly IFormatProvider _formatProvider;

        public PRE250ReglasValidationModule(IUnitOfWork uow, int idUsuario, IFormatProvider provider)
        {
            this._uow = uow;
            this._idUsuario = idUsuario;
            this._formatProvider = provider;

            this.Schema = new FormValidationSchema
            {
                ["descripcion"] = this.ValidateDescripcion,
                ["nuOrden"] = this.ValidateNuOrden,
                ["dtInicio"] = this.ValidateDtInicio,
                ["dtFin"] = this.ValidateDtFin,
                ["tpFrecuencia"] = this.ValidateTpFrecuencia,
                ["nuFrecuencia"] = this.ValidateNuFrecuencia,
                ["horaInicio"] = this.ValidateHoraInicio,
                ["horaFin"] = this.ValidateHoraFin,
                ["dias"] = this.ValidateDias,
                //["activa"] = this.ValidateActiva,
            };
        }

        public virtual FormValidationGroup ValidateDescripcion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            //if (field.ReadOnly) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 200),
                },
            };
        }
        public virtual FormValidationGroup ValidateNuOrden(FormField field, Form form, List<ComponentParameter> parameters)
        {
            //if (field.ReadOnly) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 5),
                    new PositiveShortValidationRule(field.Value),
                },
            };
        }
        public virtual FormValidationGroup ValidateDtInicio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            List<IValidationRule> rules = new List<IValidationRule>()
            {
                new NonNullValidationRule(field.Value),
                new DateTimeValidationRule(field.Value),
               // new DateTimeGreaterThanValidationRule(field.Value, DateTimeExtension.ToIsoString(DateTime.Now.Date), "PRE250_Sec0_Error_DtInicioMenorFechaActual")
            };

            if (!string.IsNullOrEmpty(form.GetField("dtFin").Value))
            {
                rules.Add(new DateTimeGreaterThanValidationRule(form.GetField("dtFin").Value, field.Value, "PRE250_Sec0_Error_DtFinMenorDtInicio"));
            }

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = rules
            };
        }
        public virtual FormValidationGroup ValidateDtFin(FormField field, Form form, List<ComponentParameter> parameters)
        {
            List<IValidationRule> rules = new List<IValidationRule>()
            {
                new NonNullValidationRule(field.Value),
                new DateTimeValidationRule(field.Value),
            };

            if (!parameters.Any(x => x.Id == "nuRegla"))            
                rules.Add(new DateTimeGreaterThanValidationRule(field.Value, DateTimeExtension.ToIsoString(DateTime.Now.Date), "PRE250_Sec0_Error_DtInicioMenorFechaActual"));

            if (!string.IsNullOrEmpty(form.GetField("dtFin").Value))
                rules.Add(new DateTimeGreaterThanValidationRule(form.GetField("dtFin").Value, field.Value, "PRE250_Sec0_Error_DtFinMenorDtInicio"));
            
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = rules
            };
        }
        public virtual FormValidationGroup ValidateHoraInicio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var tpFrecuencia = form.GetField("tpFrecuencia");

            var rules = new List<IValidationRule>();

            if (tpFrecuencia != null && tpFrecuencia.Value != "D")
            {
                rules.Add(new NonNullValidationRule(field.Value));
                rules.Add(new TimeSpanValidationRule(field.Value, this._formatProvider));
            }

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = rules,
            };
        }
        public virtual FormValidationGroup ValidateHoraFin(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var tpFrecuencia = form.GetField("tpFrecuencia");

            var rules = new List<IValidationRule>();

            if (tpFrecuencia != null && tpFrecuencia.Value != "D")
            {
                rules.Add(new NonNullValidationRule(field.Value));
                rules.Add(new TimeSpanValidationRule(field.Value, this._formatProvider));
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Dependencies = { "horaInicio" },
                Rules = rules
            };
        }
        public virtual FormValidationGroup ValidateNuFrecuencia(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var tpFrecuencia = form.GetField("tpFrecuencia");

            var rules = new List<IValidationRule>
            {
                new PositiveNumberMaxLengthValidationRule(field.Value,8),
            };

            if (tpFrecuencia != null && tpFrecuencia.Value != "D")
            {
                rules.Add(new NonNullValidationRule(field.Value));
                field.Disabled = false;
            }
            else
                field.Disabled = true;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = rules
            };
        }
        public virtual FormValidationGroup ValidateTpFrecuencia(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                },
                OnSuccess = this.ValidateTpFrecuencia_OnSuccess,
            };
        }

        public virtual void ValidateTpFrecuencia_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var horaFin = form.GetField("horaFin");
            var horaInicio = form.GetField("horaInicio");
            var nuFrecuencia = form.GetField("nuFrecuencia");

            if (field.Value == "D")
            {
                horaFin.Value = null;
                horaInicio.Value = null;
                nuFrecuencia.Value = null;

                horaFin.Disabled = true;
                horaInicio.Disabled = true;
                nuFrecuencia.Disabled = false;
            }
            else
            {
                horaFin.Disabled = false;
                horaInicio.Disabled = false;
                nuFrecuencia.Disabled = false;
            }
        }
        public virtual FormValidationGroup ValidateDias(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                },
            };
        }
    }
}
