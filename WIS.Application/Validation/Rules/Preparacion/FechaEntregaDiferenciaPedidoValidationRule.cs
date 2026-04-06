using System;
using System.Collections.Generic;
using System.Text;
using WIS.Extension;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Preparacion
{
    public class FechaEntregaDiferenciaPedidoValidationRule : IValidationRule
    {
        protected readonly string _valueDateString;
        protected readonly string _valueDateEmision;
        protected readonly string _message;

        public FechaEntregaDiferenciaPedidoValidationRule(string valueDateString, string valueDateEmision, string message = null)
        {
            this._valueDateString = valueDateString;
            this._valueDateEmision = valueDateEmision;
            this._message = message;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (DateTimeExtension.TryParseFromIso(this._valueDateEmision, out DateTime fechaEmision))
            {
                DateTime? date = DateTimeExtension.ParseFromIso(this._valueDateString);

                if (date < fechaEmision.AddDays(1))
                    errors.Add(new ValidationError(this._message ?? "WPRE100_Sec0_Error_Er009_FechaEntregaMenor48hs"));
            }

            return errors;
        }
    }
}
