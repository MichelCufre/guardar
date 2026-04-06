using System;
using System.Collections.Generic;
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
    public class STO395FormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public STO395FormValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new FormValidationSchema
            {
                ["CD_EMPRESA"] = this.ValidateEmpresa,
                ["CD_PRODUTO"] = this.ValidateProducto,
                ["DT_INICIO"] = this.ValidateFechaInicio,
                ["DT_FIN"] = this.ValidateFechaFin
            };
        }

        public virtual FormValidationGroup ValidateEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveIntValidationRule(field.Value),
                    new ExisteEmpresaValidationRule(this._uow, field.Value)
                }
            };
        }
        public virtual FormValidationGroup ValidateProducto(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string empresa = form.GetField("CD_EMPRESA").Value;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new ProductoExistsValidationRule(this._uow, empresa, field.Value)
                },
                Dependencies = { "CD_EMPRESA" }
            };
        }
        public virtual FormValidationGroup ValidateFechaInicio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string fechaFin = form.GetField("DT_FIN").Value;

            var validationGroup = new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>()
            };

            if (!string.IsNullOrEmpty(fechaFin))
                validationGroup.Rules.Add(new NonNullValidationRule(field.Value, "STO395_form1_Error_FechaInicioNull"));
            else
                return null;

            validationGroup.Rules.Add(new DateTimeValidationRule(field.Value));
            validationGroup.Rules.Add(new DateTimeLowerThanValidationRule(field.Value, fechaFin, "STO395_form1_Error_FechaInicioMenor"));

            return validationGroup;
        }
        public virtual FormValidationGroup ValidateFechaFin(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string fechaInicio = form.GetField("DT_INICIO").Value;

            var validationGroup = new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>()
            };

            if (!string.IsNullOrEmpty(fechaInicio))
                validationGroup.Rules.Add(new NonNullValidationRule(field.Value, "STO395_form1_Error_FechaFinNull"));
            else if (string.IsNullOrEmpty(field.Value))
                return null;

            validationGroup.Rules.Add(new DateTimeValidationRule(field.Value));
            validationGroup.Rules.Add(new DateTimeGreaterThanValidationRule(field.Value, fechaInicio, "STO395_form1_Error_FechaFinMayor"));
            validationGroup.Rules.Add(new DateTimeLowerThanValidationRule(field.Value, DateTime.Now.ToIsoString(), "STO395_form1_Error_FechaFinMenorDia"));

            return validationGroup;
        }
    }
}
