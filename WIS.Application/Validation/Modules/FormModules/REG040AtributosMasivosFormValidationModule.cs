using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class REG040AtributosMasivosFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public REG040AtributosMasivosFormValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new FormValidationSchema
            {
                ["tpUbicacion"] = this.ValidateIdUbicacionTipo,
                ["rotatividad"] = this.ValidateIdRotatividad,
                ["zona"] = this.ValidateidZonaUbicacion,
                ["controlAcceso"] = this.ValidateControlAcceso,
            };
        }


        public virtual FormValidationGroup ValidateIdUbicacionTipo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule> {
                    //new NonNullValidationRule(field.Value),
                    new PositiveShortNumberMaxLengthValidationRule(field.Value,2),
                    new IdUbicacionTipoNoExistenteValidationRule(_uow, field.Value),
                },
            };
        }

        public virtual FormValidationGroup ValidateIdRotatividad(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule> {
                    //new NonNullValidationRule(field.Value),
                    new PositiveShortNumberMaxLengthValidationRule(field.Value,2),
                    new IdProductoRotatividadNoExistenteValidationRule(_uow,  field.Value),
                },
            };
        }

        public virtual FormValidationGroup ValidateidZonaUbicacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule> {
                    //new NonNullValidationRule(field.Value),
                    new IdZonaUbicacionValidationRule(_uow,  field.Value),
                },
            };
        }

        public virtual FormValidationGroup ValidateControlAcceso(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;
            
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule> {
                    new ExisteControlAccesoValidationRule(_uow, field.Value),
                },
            };
        }

    }
}
