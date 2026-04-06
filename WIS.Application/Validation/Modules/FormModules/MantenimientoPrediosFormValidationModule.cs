using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General.Configuracion;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class MantenimientoPrediosFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly UbicacionConfiguracion _ubicacionConfiguracion;

        public MantenimientoPrediosFormValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;
            this._ubicacionConfiguracion = this._uow.UbicacionRepository.GetUbicacionConfiguracion();

            this.Schema = new FormValidationSchema
            {
                ["NU_PREDIO"] = this.ValidateNU_PREDIO,
                ["DS_PREDIO"] = this.ValidateDS_PREDIO
            };

        }

        public virtual FormValidationGroup ValidateNU_PREDIO(FormField field, Form form, List<ComponentParameter> parameters) //TODO: Cambiar nombres por valores adecuados
        {
            if (field.ReadOnly) return null;

            var rules = new List<IValidationRule>()
            {
                new NonNullValidationRule(field.Value),
                new StringMaxLengthValidationRule(field.Value,  this._ubicacionConfiguracion.PredioLargo),
            };

            var validateGroup = new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };

            if (_ubicacionConfiguracion.PredioNumerico)
            {
                rules.Add(new PositiveNumberMaxLengthValidationRule(field.Value, _ubicacionConfiguracion.PredioLargo));
            }
            else
            {
                field.Value = field.Value.ToUpper();
                rules.Add(new StringSoloLetrasValidationRule(field.Value.ToUpper()));
            }

            rules.Add(new IdPredioExistenteValidationRule(this._uow, field.Value));

            return validateGroup;

        }

        public virtual FormValidationGroup ValidateDS_PREDIO(FormField field, Form form, List<ComponentParameter> parameters)
        {

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 100),
                },
            };
        }
    }
}
