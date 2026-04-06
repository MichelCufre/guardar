using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Produccion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.Services.Interfaces;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Produccion
{
    class CodigoBarrasContenedorEnsambladoFormulaFormValidationModule : FormValidationModule
    {
        protected readonly IBarcodeService _barcodeService;
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;

        public CodigoBarrasContenedorEnsambladoFormulaFormValidationModule(IUnitOfWork uow, IIdentityService identity, IBarcodeService barcodeService)
        {
            this._uow = uow;
            this._identity = identity;
            this._barcodeService = barcodeService;

            this.Schema = new FormValidationSchema
            {
                ["NuContenedor"] = this.ValidateNuContenedor,
            };
        }

        public virtual FormValidationGroup ValidateNuContenedor(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value,16),
                   new CodigoBarrasContenedorEnsambladoFormulaValidationRule(this._uow, field.Value,_identity,_barcodeService),
                },
                OnFailure = ValidateNuContenedorOnFailure
            };
        }

        public virtual void ValidateNuContenedorOnFailure(FormField field, Form form, List<ComponentParameter> parameters)
        {
            field.Value = string.Empty;
        }
    }
}
