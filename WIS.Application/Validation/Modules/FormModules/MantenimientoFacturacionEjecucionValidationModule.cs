using System;
using System.Collections.Generic;
using System.Globalization;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class MantenimientoFacturacionEjecucionValidationModule : FormValidationModule
    {
        public MantenimientoFacturacionEjecucionValidationModule()
        {
            this.Schema = new FormValidationSchema
            {
                ["fechaDesde"] = this.ValidateFechaDesde,
                ["fechaHasta"] = this.ValidateFechaHasta,
                ["horaDesde"] = this.ValidateHoraDesde,
                ["horaHasta"] = this.ValidateHoraHasta,
                ["horaFechaProgramacion"] = this.ValidateHoraFechaProgramacion,
                ["fechaProgramacion"] = this.ValidateFechaProgramacion,
                ["nombre"] = this.ValidateNombre,
            };
        }

        public virtual FormValidationGroup ValidateNombre(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 100)
                },
            };
        }

        public virtual FormValidationGroup ValidateFechaDesde(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (form.GetField("ingresarFechaDesde").Value == "false")
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new DateTimeValidationRule(field.Value)
                },
            };
        }

        public virtual FormValidationGroup ValidateFechaHasta(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var rules = new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new DateTimeValidationRule(field.Value)
                },
            };

            if (form.GetField("ingresarFechaDesde").Value == "true" && form.GetField("parcial").Value == "false")
                rules.Rules.Add(new DateTimeGreaterThanValidationRule(field.Value, form.GetField("fechaDesde").Value, "FAC001_frm1_error_FechaHastaMenorFechaDesde"));

            return rules;
        }

        public virtual FormValidationGroup ValidateFechaProgramacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var rules = new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new DateTimeValidationRule(field.Value),
                },
            };

            if (form.GetField("parcial").Value == "false")
            {
                DateTime tHasta = DateTime.ParseExact("23:59", "HH:mm", CultureInfo.InvariantCulture);
                DateTime fHasta = DateTime.Parse(form.GetField("fechaHasta").Value, CultureInfo.InvariantCulture);
                DateTime fechaHasta = new DateTime(fHasta.Year, fHasta.Month, fHasta.Day, tHasta.Hour, tHasta.Minute, 0);

                //Fecha programacion
                DateTime tProgramacion = DateTime.ParseExact("00:00", "HH:mm", CultureInfo.InvariantCulture);
                DateTime fProgramacion = DateTime.Parse(form.GetField("fechaProgramacion").Value, CultureInfo.InvariantCulture);
                DateTime fechaProgramacion = new DateTime(fProgramacion.Year, fProgramacion.Month, fProgramacion.Day, tProgramacion.Hour, tProgramacion.Minute, 0);

                rules.Rules.Add(new DateTimeGreaterThanCurrentDateAndTimeValidationRule(DateTimeExtension.ToIsoString(fechaProgramacion), validarHora: false));
                rules.Rules.Add(new DateAndTimeGreaterThanValidationRule(fechaHasta, fechaProgramacion, "FAC001_frm1_error_FechaProgramacionDebeSerMayorFechaHasta"));
            }

            return rules;
        }

        public virtual FormValidationGroup ValidateHoraDesde(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (form.GetField("parcial").Value == "false" || form.GetField("ingresarFechaDesde").Value == "false")
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new DateTimeValidationRule(field.Value)
                },
            };
        }
        public virtual FormValidationGroup ValidateHoraHasta(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (form.GetField("parcial").Value == "false")
                return null;

            var rules = new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new DateTimeValidationRule(field.Value)
                },
            };

            //Fecha hasta
            if (!((string.IsNullOrEmpty(form.GetField("fechaHasta").Value) || form.GetField("fechaHasta").Value == "__:__") ||
                (string.IsNullOrEmpty(form.GetField("horaHasta").Value) || form.GetField("horaHasta").Value == "__:__") ||
                (string.IsNullOrEmpty(form.GetField("horaDesde").Value) || form.GetField("horaDesde").Value == "__:__") ||
                (string.IsNullOrEmpty(form.GetField("fechaDesde").Value) || form.GetField("fechaDesde").Value == "__:__")
                ))
            {
                DateTime tHasta = DateTime.ParseExact(form.GetField("horaHasta").Value, "HH:mm", CultureInfo.InvariantCulture);
                DateTime fHasta = DateTime.Parse(form.GetField("fechaHasta").Value, CultureInfo.InvariantCulture);
                DateTime fechaHasta = new DateTime(fHasta.Year, fHasta.Month, fHasta.Day, tHasta.Hour, tHasta.Minute, 0);

                //Fecha desde
                DateTime tDesde = DateTime.ParseExact(form.GetField("horaDesde").Value, "HH:mm", CultureInfo.InvariantCulture);
                DateTime fDesde = DateTime.Parse(form.GetField("fechaDesde").Value, CultureInfo.InvariantCulture);
                DateTime fechaDesde = new DateTime(fDesde.Year, fDesde.Month, fDesde.Day, tDesde.Hour, tDesde.Minute, 0);

                rules.Rules.Add(new DateAndTimeGreaterThanValidationRule(fechaDesde, fechaHasta, "FAC001_frm1_error_FechaHastaMenorFechaDesde"));
            }

            return rules;
        }
        public virtual FormValidationGroup ValidateHoraFechaProgramacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (form.GetField("parcial").Value == "false")
                return null;

            var rules = new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new DateTimeValidationRule(field.Value),
                },
            };

            if (!((string.IsNullOrEmpty(form.GetField("horaHasta").Value) || form.GetField("horaHasta").Value == "__:__") ||
                (string.IsNullOrEmpty(form.GetField("fechaHasta").Value) || form.GetField("fechaHasta").Value == "__:__") ||
                (string.IsNullOrEmpty(form.GetField("horaFechaProgramacion").Value) || form.GetField("horaFechaProgramacion").Value == "__:__") ||
                (string.IsNullOrEmpty(form.GetField("fechaProgramacion").Value) || form.GetField("fechaProgramacion").Value == "__:__")
                ))
            {
                //Fecha hasta
                DateTime tHasta = DateTime.ParseExact(form.GetField("horaHasta").Value, "HH:mm", CultureInfo.InvariantCulture);
                DateTime fHasta = DateTime.Parse(form.GetField("fechaHasta").Value, CultureInfo.InvariantCulture);
                DateTime fechaHasta = new DateTime(fHasta.Year, fHasta.Month, fHasta.Day, tHasta.Hour, tHasta.Minute, 0);

                //Fecha programacion
                DateTime tProgramacion = DateTime.ParseExact(form.GetField("horaFechaProgramacion").Value, "HH:mm", CultureInfo.InvariantCulture);
                DateTime fProgramacion = DateTime.Parse(form.GetField("fechaProgramacion").Value, CultureInfo.InvariantCulture);
                DateTime fechaProgramacion = new DateTime(fProgramacion.Year, fProgramacion.Month, fProgramacion.Day, tProgramacion.Hour, tProgramacion.Minute, 0);

                rules.Rules.Add(new DateTimeGreaterThanCurrentDateAndTimeValidationRule(DateTimeExtension.ToIsoString(fechaProgramacion)));
                rules.Rules.Add(new DateAndTimeGreaterThanValidationRule(fechaHasta, fechaProgramacion, "FAC001_frm1_error_FechaProgramacionDebeSerMayorFechaHasta"));
            }

            return rules;
        }
    }
}
