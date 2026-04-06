using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Porteria
{
    public class RegistroEntradaDatosGeneralesFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public RegistroEntradaDatosGeneralesFormValidationModule(
            IUnitOfWork uow,
            IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;
            this.Schema = new FormValidationSchema
            {
                ["CD_POTERIA_MOTIVO"] = this.ValidateCD_POTERIA_MOTIVO,
                ["CD_EMPRESA"] = this.ValidateCD_EMPRESA,
                ["CD_CLIENTE"] = this.ValidateCD_CLIENTE,
                ["DS_NOTA"] = this.ValidateDS_NOTA,
                ["NU_CONTAINER"] = this.ValidateNU_CONTAINER,
            };
        }

        public virtual FormValidationGroup ValidateCD_POTERIA_MOTIVO(FormField field, Form form, List<ComponentParameter> parameters)
        {

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 20),
                    new ExisteDominioValidationRule(field.Value, this._uow)
                },
            };
        }
        public virtual FormValidationGroup ValidateCD_EMPRESA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value)) 
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NumberValidationRule<int?>(field.Value,_culture),
                   new ExisteEmpresaValidationRule(this._uow, field.Value),
                },
            };
        }
        public virtual FormValidationGroup ValidateCD_CLIENTE(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value)) 
                return null;

            string CD_EMPRESA = form.GetField("CD_EMPRESA").Value;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Dependencies = { "CD_EMPRESA" },
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(CD_EMPRESA, "Debe ingresar empresa"),
                    new StringMaxLengthValidationRule(field.Value, 10),
                    new ExisteClienteProveedorEmpresaValidationRule(this._uow,field.Value,CD_EMPRESA),
                },
            };
        }
        public virtual FormValidationGroup ValidateDS_NOTA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new StringMaxLengthValidationRule(field.Value, 400)
                },
            };
        }
        public virtual FormValidationGroup ValidateNU_CONTAINER(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NumberValidationRule<int>(field.Value,_culture)
                },
            };
        }
    }
}
