using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Automatizacion;
using WIS.Components.Common;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Automatizacion
{
    public class CreateAutomatismoForm1ValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;

        public CreateAutomatismoForm1ValidationModule(IUnitOfWork uow, IIdentityService identity)
        {
            this.Schema = new FormValidationSchema
            {
                ["codigoExterno"] = this.ValidateCodigoExterno,
                ["descripcion"] = this.ValidateDescripcion,
                ["tipo"] = this.ValidateTipo,
                ["predio"] = this.ValidatePredio,
                ["qtPicking"] = this.ValidateCantidadEnUbicacion,
                ["qtEntrada"] = this.ValidateCantidadEnUbicacion,
                ["qtAjuste"] = this.ValidateCantidadEnUbicacion,
                ["qtRechazo"] = this.ValidateCantidadEnUbicacion,
                ["qtTransito"] = this.ValidateCantidadEnUbicacion,
                ["qtSalida"] = this.ValidateCantidadEnUbicacion,
            };

            this._uow = uow;
            this._identity = identity;
        }

        public virtual FormValidationGroup ValidateCodigoExterno(FormField field, Form form, List<ComponentParameter> parameters)
        {
            int? nroAutomatismo = null;
            if (int.TryParse(form.GetField("numeroAutomatismo").Value, out int parsedValue))
                nroAutomatismo = parsedValue;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 100),
                    new CodigoExternoAutomatismoValidationRule(this._uow, field.Value?.ToUpper(), nroAutomatismo)
                }
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
                    new StringMaxLengthValidationRule(field.Value, 400),
                }
            };
        }

        public virtual FormValidationGroup ValidateTipo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            int? nroAutomatismo = null;
            if (int.TryParse(form.GetField("numeroAutomatismo").Value, out int parsedValue))
                nroAutomatismo = parsedValue;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 40),
                    new TipoAutomatismoValidationRule(this._uow, field.Value, nroAutomatismo)
                },
                OnSuccess = this.ValidateTipo_OnSucess,
                OnFailure = this.ValidateTipo_OnFailure
            };
        }
        public virtual void ValidateTipo_OnSucess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var fieldCantPicking = form.GetField("qtPicking");
            if (fieldCantPicking != null)
            {
                if (field.Value == AutomatismoTipo.AutoStore)
                {
                    fieldCantPicking.Value = "1";
                    fieldCantPicking.ReadOnly = true;
                }
                else
                {
                    if (!parameters.Any(s => s.Id == "isSubmit"))
                        fieldCantPicking.Value = string.Empty;

                    fieldCantPicking.ReadOnly = false;
                }
            }

        }
        public virtual void ValidateTipo_OnFailure(FormField field, Form form, List<ComponentParameter> parameters)
        {
        }

        public virtual FormValidationGroup ValidatePredio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 10),
                    new ExistePredioValidationRule(this._uow, this._identity.UserId, this._identity.Predio, field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateCantidadEnUbicacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new PositiveIntValidationRule(field.Value)
                }
            };
        }
    }
}
