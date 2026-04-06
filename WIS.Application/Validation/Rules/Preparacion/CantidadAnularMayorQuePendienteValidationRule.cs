using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class CantidadAnularMayorQuePendienteValidationRule : IValidationRule
    {
        protected readonly decimal _qtAnulado;
        protected readonly decimal _qtPendiente;

        public CantidadAnularMayorQuePendienteValidationRule(string qtAnulada, string qtPendiente, IFormatProvider culture)
        {
            decimal.TryParse(qtAnulada, NumberStyles.Number, culture, out this._qtAnulado);
            decimal.TryParse(qtPendiente, NumberStyles.Number, culture, out this._qtPendiente);
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            if (_qtAnulado > _qtPendiente)
                errors.Add(new ValidationError("PRE110_Sec0_Error_Er001_QTMaximaAnularPendiente"));

            return errors;
        }
    }
}
