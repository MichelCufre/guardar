using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Recepcion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class CambioPasswordValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _user;
        protected readonly string _aplicacion;

        public CambioPasswordValidationModule(IUnitOfWork uow, int user, string aplicacion)
        {
            this._uow = uow;
            this._user = user;
            this._aplicacion = aplicacion;

            this.Schema = new FormValidationSchema
            {
                ["passwordNueva"] = this.ValidatePassword,
                ["rePasswordNueva"] = this.ValidateRePassword,
                ["passwordOld"] = this.ValidatePasswordOld
            };
        }

        public virtual FormValidationGroup ValidatePassword(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value)
                },
            };
        }
        
        public virtual FormValidationGroup ValidateRePassword(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value)
                },
            };
        }
        
        public virtual FormValidationGroup ValidatePasswordOld(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value)
                },
            };
        }
    }
}
