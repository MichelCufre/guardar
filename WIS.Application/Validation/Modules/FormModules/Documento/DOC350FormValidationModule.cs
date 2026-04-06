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
    public class DOC350FormValidationModule : FormValidationModule
    {
        protected readonly string _nuAgrupador;
        protected readonly string _tpAgrupador;
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly IFormatProvider _culture;

        public DOC350FormValidationModule(
            string nuAgrupador,
            string tpAgrupador,
            IUnitOfWork uow,
            IIdentityService identity)
        {
            this._nuAgrupador = nuAgrupador;
            this._tpAgrupador = tpAgrupador;
            this._uow = uow;
            this._identity = identity;
            this._culture = identity.GetFormatProvider();

            Schema = new FormValidationSchema
            {
                ["fechLlegada"] = this.ValidateDT_LLEGADA,
            };
        }

        public virtual FormValidationGroup ValidateDT_LLEGADA(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringIsoDateToDateTimeValidationRule(field.Value),
                    new FechaLlegadaAgrupadorValidationRule(this._tpAgrupador, this._nuAgrupador, this._uow, field.Value, this._culture)
                }
            };
        }
    }
}
