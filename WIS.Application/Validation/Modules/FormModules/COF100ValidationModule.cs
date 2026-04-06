using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class COF100ValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _usuarioLogueado;
        protected readonly string _aplicacion;
        public COF100ValidationModule(IUnitOfWork uow, int usuarioLogueado, string aplicacion)
        {
            this._uow = uow;
            this._usuarioLogueado = usuarioLogueado;
            this._aplicacion = aplicacion;

            this.Schema = new FormValidationSchema
            {
                ["estado"] = this.ValidateNuloyLargo,
                ["idTenant"] = this.ValidateNuloyLargo,
                ["apiTracking"] = this.ValidateNuloyLargo,
                ["apiWMS"] = this.ValidateNuloyLargo,
                ["apiUsers"] = this.ValidateNuloyLargo,
                ["accessTokenURL"] = this.ValidateNuloyLargo,
                ["grantType"] = this.ValidateNuloyLargo,
                ["clientId"] = this.ValidateNuloyLargo,
                ["clientSecret"] = this.ValidateNuloyLargo,
                ["scope"] = this.ValidateNuloyLargo,
                ["tpCont"] = this.ValidateNuloyLargo,
                ["tpContFicticio"] = this.ValidateNuloyLargo,
                ["cantDias"] = this.ValidateCantDias,
                ["agrupacionCD"] = this.ValidateNuloyLargo,
                ["fechaInicial"] = this.ValidateFechaInicial,
            };
        }

        public virtual FormValidationGroup ValidateNuloyLargo(FormField field, Form form, List<ComponentParameter> parameters)
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

        public virtual FormValidationGroup ValidateCantDias(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new NumeroEnteroValidationRule(field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateFechaInicial(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new DateTimeValidationRule(field.Value)
                }
            };
        }
    }
}
