using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class EVT020CrearContactoValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public EVT020CrearContactoValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new FormValidationSchema
            {
                ["NM_CONTACTO"] = this.ValidateNombreContacto,
                ["DS_CONTACTO"] = this.ValidateDescricionContacto,
                ["NU_TELEFONO"] = this.ValidateTelefono,
                ["DS_EMAIL"] = this.ValidateEmail,
                ["CD_EMPRESA"] = this.ValidateEmpresa,
                ["CD_CLIENTE"] = this.ValidateAgente,
            };
        }

        public virtual FormValidationGroup ValidateAgente(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var empresa = form.GetField("CD_EMPRESA")?.Value;

            var validationGrup = new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 10),
                },
            };

            if (!string.IsNullOrEmpty(field.Value) && !string.IsNullOrEmpty(empresa))
                validationGrup.Rules.Add(new ExisteClienteValidationRule(this._uow, field.Value, empresa));

            return validationGrup;
        }

        public virtual FormValidationGroup ValidateEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var validationGrup = new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 10),
                    new ExisteEmpresaValidationRule(this._uow, field.Value)
                },
                OnSuccess = this.ValidateEmpresaOnSuccess
            };

            return validationGrup;
        }

        public virtual void ValidateEmpresaOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!parameters.Any(s => s.Id == "isSubmit"))
            {
                form.GetField("CD_CLIENTE").Value = string.Empty;
                form.GetField("CD_CLIENTE").Options.Clear();
            }
        }

        public virtual FormValidationGroup ValidateEmail(FormField field, Form form, List<ComponentParameter> parameters)
        {
            int? empresa = null;
            if (int.TryParse(form.GetField("CD_EMPRESA").Value, out int parsedValue))
                empresa = parsedValue;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 100),
                    new EmailValidationRule(_uow, field.Value, validacionUsuario: false, validarMailEnUso: false, empresa: empresa)
                }
            };
        }

        public virtual FormValidationGroup ValidateTelefono(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 30),
                }
            };
        }

        public virtual FormValidationGroup ValidateDescricionContacto(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 200)
                }
            };
        }

        public virtual FormValidationGroup ValidateNombreContacto(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 100)
                }
            };
        }
    }
}
