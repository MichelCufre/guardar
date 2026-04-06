using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Recorridos;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class REG700RecorridoFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _securityService;
        protected readonly bool _isUpdate;
        protected readonly int? _nuRecorrido;

        public REG700RecorridoFormValidationModule(IUnitOfWork uow, IIdentityService securityService, int? nuRecorrido = null, bool isUpdate = false)
        {
            this._uow = uow;
            this._securityService = securityService;
            this._isUpdate = isUpdate;
            this._nuRecorrido = nuRecorrido;

            this.Schema = new FormValidationSchema
            {
                ["nombre"] = this.ValidateNombre,
                ["descripcion"] = this.ValidateDescripcion,
                ["predio"] = this.ValidateNumeroPredio,
                ["isEnabled"] = this.ValidateEsHabilitado
            };
        }

        public virtual FormValidationGroup ValidateNombre(FormField field, Form form, List<ComponentParameter> parameters)
        {

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 50),
                    new NombreRecorridoUnicoValidationRule(_uow, field.Value, _nuRecorrido),
                    new RecorridoRenombrableValidationRule(this._uow, field.Value, _nuRecorrido)
                },
            };
        }

        public virtual FormValidationGroup ValidateDescripcion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 200)
                },
            };
        }

        public virtual FormValidationGroup ValidateNumeroPredio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule>();

            if (!_isUpdate)
            {
                rules = [
                    new NonNullValidationRule(field.Value),
                    new PredioUsuarioExistenteValidationRule(this._uow, this._securityService.UserId ,field.Value)
                ];
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual FormValidationGroup ValidateEsHabilitado(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule>();

            if (_isUpdate)
            {
                var idRecorrido = int.Parse(form.GetField("idRecorrido").Value);

                rules = [
                    new RecorridoDeshabilitableValidationRule(this._uow, field.Value, idRecorrido)
                ];
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }
    }
}
