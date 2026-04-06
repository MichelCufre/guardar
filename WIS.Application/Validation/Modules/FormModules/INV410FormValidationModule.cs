using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.GridComponent;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class INV410FormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _securityService;

        public INV410FormValidationModule(IUnitOfWork uow, IIdentityService securityService)
        {
            this._uow = uow;
            this._securityService = securityService;

            this.Schema = new FormValidationSchema
            {
                ["descInventario"] = this.ValidateDsInventario,
                ["empresa"] = this.ValidateEmpresa,
                ["tipoCierreConteo"] = this.ValidateTipoCierreConteo,
                ["predio"] = this.ValidatePredio,
                ["excluirSueltos"] = this.ValidateExcluirSueltos,
                ["excluirLpns"] = this.ValidateExcluirExcluirLpns,
                ["generarPrimerConteo"] = this.ValidateGenerarPrimerConteo,
            };
        }

        public virtual FormValidationGroup ValidateDsInventario(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 199)
                }
            };
        }

        public virtual FormValidationGroup ValidateEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 10),
                    new ExisteEmpresaValidationRule(this._uow, field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateTipoCierreConteo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 20),
                    new ExisteEstadoCierreConteo(this._uow, field.Value)
                },
                OnSuccess = this.OnSuccessValidateTipoCierreConteo
            };
        }
        public virtual void OnSuccessValidateTipoCierreConteo(FormField field, Form form, List<ComponentParameter> parameters)
        {
        }

        public virtual FormValidationGroup ValidatePredio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new ExistePredioValidationRule(this._uow, this._securityService.UserId,this._securityService.Predio ,field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateExcluirSueltos(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            if (form.GetField("excluirSueltos").IsChecked() && form.GetField("excluirLpns").IsChecked())
                form.GetField("excluirLpns").SetChecked(false);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                }
            };
        }

        public virtual FormValidationGroup ValidateExcluirExcluirLpns(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            if (form.GetField("excluirLpns").IsChecked() && form.GetField("excluirSueltos").IsChecked())
                form.GetField("excluirSueltos").SetChecked(false);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                }
            };
        }

        public virtual FormValidationGroup ValidateGenerarPrimerConteo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var tipoCierreConteo = form.GetField("tipoCierreConteo").Value;

            if (string.IsNullOrEmpty(tipoCierreConteo))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Dependencies = { "tipoCierreConteo" },
                Rules = new List<IValidationRule>
                {
                    new GenerarPrimerConteoValidationRule(field.Value, tipoCierreConteo)
                }
            };
        }
    }
}
