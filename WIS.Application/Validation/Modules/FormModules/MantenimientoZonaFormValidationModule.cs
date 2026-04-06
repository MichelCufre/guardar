using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class MantenimientoZonaFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public MantenimientoZonaFormValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new FormValidationSchema
            {
                ["CD_ZONA"] = this.ValidateCodigoZona,
                ["NM_ZONA"] = this.ValidateNombreZona,
                ["DS_ZONA"] = this.ValidateDescripcionZona,
                ["CD_LOCALIDAD"] = this.ValidateLocalidad,
            };
        }

        public virtual FormValidationGroup ValidateCodigoZona(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value,20),
                    new ZonaExistenteValidationRule(_uow, field.Value.ToUpper())
                },
            };
        }

        public virtual FormValidationGroup ValidateNombreZona(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value,100)
                },
            };
        }

        public virtual FormValidationGroup ValidateDescripcionZona(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value,200)
                },
            };
        }

        public virtual FormValidationGroup ValidateLocalidad(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new ExisteDominioValidationRule(field.Value,this._uow,CodigoDominioDb.TipoLocalidad),
                },
            };
        }
    }
}
