using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class CreateGrupoFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public CreateGrupoFormValidationModule(IUnitOfWork uow)
        {
            _uow = uow;

            this.Schema = new FormValidationSchema
            {
                ["codigoGrupo"] = this.ValidateCodigoGrupo,
                ["descripcion"] = this.ValidateDescripcion,
                ["clase"] = this.ValidateClase,
            };
        }

        public virtual FormValidationGroup ValidateCodigoGrupo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 50),
                   new GrupoExistenteValidationRule(_uow, field.Value)
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
                   new StringMaxLengthValidationRule(field.Value, 100)
                }
            };
        }
        public virtual FormValidationGroup ValidateClase(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value, 2),
                        new ClaseNoExistenteValidationRule(_uow, field.Value),
                    },

            };
        }
    }
}
