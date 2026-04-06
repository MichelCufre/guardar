using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class STO740ProductosAtributosValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _usuario;
        protected readonly string _predio;

        public STO740ProductosAtributosValidationModule(IUnitOfWork uow, int usuarioLogueado, string predioLogueado)
        {
            this._uow = uow;
            this._usuario = usuarioLogueado;
            this._predio = predioLogueado;

            this.Schema = new FormValidationSchema
            {
                ["tipo"] = this.ValidateTipo,
            };
        }

        public virtual FormValidationGroup ValidateTipo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule>();

            if (!string.IsNullOrEmpty(field.Value))
            {
                rules.Add(new ExisteTipoLPNValidationRule(this._uow, field.Value));
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = ValidateTipoOnSuccess
            };
        }

        public virtual void ValidateTipoOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            parameters.RemoveAll(p => p.Id == "tipo");
            parameters.Add(new ComponentParameter("tipo", field.Value));
        }
    }
}
