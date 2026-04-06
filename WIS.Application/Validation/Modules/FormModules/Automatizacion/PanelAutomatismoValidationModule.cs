using System.Collections.Generic;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Automatizacion
{
    public class PanelAutomatismoValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public PanelAutomatismoValidationModule(IUnitOfWork uow)
        {
            _uow = uow;
            this.Schema = new FormValidationSchema
            {
                ["automatismo"] = this.ValidateAutomatismo,
                ["tipoUbicacion"] = this.ValidateTipoUbicacion
            };
        }

        public virtual FormValidationGroup ValidateAutomatismo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                },
                OnSuccess = this.AutomatismoOnSuccess
            };
        }
        public virtual void AutomatismoOnSuccess(FormField field, Form form, List<ComponentParameter> componentParams)
        {
            var nuAutomatismo = string.Empty;

            if (!string.IsNullOrEmpty(field.Value))
            {
                var automatismo = _uow.AutomatismoRepository.GetAutomatismoByZona(field.Value);

                if (automatismo != null)
                {
                    nuAutomatismo = automatismo.Numero.ToString();
                }
            }

            componentParams.Add(new ComponentParameter() { Id = "AUT100_NU_AUTOMATISMO", Value = nuAutomatismo });
        }

        public virtual FormValidationGroup ValidateTipoUbicacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                }
            };
        }
    }
}
