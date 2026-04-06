using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Documento;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Documento
{
    public class DOC081FormValidationModule : FormValidationModule
    {
        protected readonly string _nroDocumento;
        protected readonly string _tipoDocumento;
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public DOC081FormValidationModule(
            string nroDocumento,
            string tipoDocumento,
            IUnitOfWork uow,
            IIdentityService identity)
        {
            this._nroDocumento = nroDocumento;
            this._tipoDocumento = tipoDocumento;
            this._uow = uow;
            this._culture = identity.GetFormatProvider();

            Schema = new FormValidationSchema
            {
                ["QT_VALIDAR_LINEAS"] = this.ValidateQT_VALIDAR_LINEAS,
                ["QT_VALIDAR_VALOR"] = this.ValidateQT_VALIDAR_VALOR
            };
        }

        public virtual FormValidationGroup ValidateQT_VALIDAR_LINEAS(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new DecimalLengthWithPresicionValidationRule(field.Value, 100,50, this._culture),
                    new DecimalCultureSeparatorValidationRule(this._culture, field.Value),
                    new PositiveDecimalValidationRule(this._culture, field.Value),
                    new CantidadTotalLineasDocumentoIngresoValidationRule(this._uow, this._nroDocumento, this._tipoDocumento, field.Value, this._culture)
                },
                BreakValidationChain = true
            };
        }

        public virtual FormValidationGroup ValidateQT_VALIDAR_VALOR(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new DecimalLengthWithPresicionValidationRule(field.Value, 100,50,this._culture),
                    new DecimalCultureSeparatorValidationRule(this._culture, field.Value),
                    new PositiveDecimalValidationRule(this._culture, field.Value),
                    new ValorTotalDocumentoIngresoValidationRule(this._uow, this._nroDocumento, this._tipoDocumento, field.Value, this._culture)
                },
                BreakValidationChain = true
            };
        }
    }
}
