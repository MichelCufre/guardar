using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Preparacion
{
    public class CantidadAnularMayorQueAsociadoValidationRule : IValidationRule
    {
        protected readonly decimal _qtAnulado;
        protected readonly decimal _qtAsociadoLpn;
        protected readonly decimal _qtPendiente;

        public CantidadAnularMayorQueAsociadoValidationRule(string qtAnulado, string qtAsociado, string qtPendiente, IFormatProvider culture)
        {
            decimal.TryParse(qtAnulado, NumberStyles.Number, culture, out this._qtAnulado);
            decimal.TryParse(qtAsociado, NumberStyles.Number, culture, out this._qtAsociadoLpn);
            decimal.TryParse(qtPendiente, NumberStyles.Number, culture, out this._qtPendiente);

        }
        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            if (_qtAnulado > (_qtPendiente - _qtAsociadoLpn))
                errors.Add(new ValidationError("PRE110_Sec0_Error_Er001_QTMaximaAnularMayorAsociadoLpn", new List<string> { (_qtPendiente - _qtAsociadoLpn).ToString() }));

            return errors;
        }
    }
}
