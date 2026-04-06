using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class MantenimientoZonaUbicacionFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _securityService;

        public MantenimientoZonaUbicacionFormValidationModule(IUnitOfWork uow, IIdentityService securityService)
        {
            this._uow = uow;
            this._securityService = securityService;

            this.Schema = new FormValidationSchema
            {
                ["idZona"] = this.ValidateZonaUbicacion,
                ["descripcionZona"] = this.ValidatedDescripcionZona,
                ["tipoZona"] = this.ValidateTipoZona,
                ["idZonaPicking"] = this.ValidateZonaPicking,
                ["estacion"] = this.ValidateEstacion,
                ["estacionAlmacenaje"] = this.ValidateEstacionAlmacenaje,
                //["habilitada"] = this.ValidateZonaHabilitada,
            };
        }
                
        public virtual FormValidationGroup ValidateZonaUbicacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value,20),
                    new ZonaUbicacionExistenteValidationRule(_uow, field.Value.ToUpper())
                },
            };
        }
        
        public virtual FormValidationGroup ValidatedDescripcionZona(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 100)
                },
            };
        }
        
        public virtual FormValidationGroup ValidateTipoZona(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                    new TpZonaValidationRule(_uow,  field.Value),
                },
            };
        }
        
        public virtual FormValidationGroup ValidateZonaPicking(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule> {
                    new IdZonaUbicacionValidationRule(_uow, field.Value)
                },
            };
        }
        
        public virtual FormValidationGroup ValidateEstacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule> {
                    new StringMaxLengthValidationRule(field.Value, 20)

                },
            };
        }
        
        public virtual FormValidationGroup ValidateEstacionAlmacenaje(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule> {
                    new StringMaxLengthValidationRule(field.Value, 20)
                },
            };
        }
        
        public virtual FormValidationGroup ValidateZonaHabilitada(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return null;
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                },
            };
        }

    }
}
