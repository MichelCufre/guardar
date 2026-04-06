using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Recepcion
{
    public class REC400CrearEstacionFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _idUsuario;

        public REC400CrearEstacionFormValidationModule(IUnitOfWork uow, int idUsuario)
        {
            this._uow = uow;
            this._idUsuario = idUsuario;

            this.Schema = new FormValidationSchema
            {
                ["predio"] = this.ValidatePredio,
                ["descripcion"] = this.ValidateDescripcion,
            };
        }

        public virtual FormValidationGroup ValidatePredio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PredioUsuarioExistenteValidationRule(this._uow, this._idUsuario, field.Value),
                },
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
                    new StringMaxLengthValidationRule(field.Value, 50),
                },
            };
        }
    }
}
