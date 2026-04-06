using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Recepcion
{
    public class IvaBaseValidationRule : IValidationRule
    {
        protected readonly IFormatProvider _formato;
        protected readonly string _totalDigitado;
        protected readonly string _ivaBase;
        protected readonly string _ivaMinimo;

        public IvaBaseValidationRule(IFormatProvider proveedorDeFormato, string totalDigitado, string ivaMinimo, string ivaBase)
        {
            this._formato = proveedorDeFormato;
            this._totalDigitado = totalDigitado;
            this._ivaBase = ivaBase;
            this._ivaMinimo = ivaMinimo;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            decimal.TryParse(_totalDigitado, NumberStyles.Number, _formato, out decimal totalDigitado);
            decimal.TryParse(_ivaBase, NumberStyles.Number, _formato, out decimal ivaBase);
            decimal.TryParse(_ivaMinimo, NumberStyles.Number, _formato, out decimal ivaMinimo);

            if (!(ivaBase <= (totalDigitado * (decimal)0.19)))
            {
                errors.Add(new ValidationError("REC500_Sec0_Error_IvaBaseMayorAlPermitido"));
            }
            else if (!((ivaMinimo + ivaBase) <= totalDigitado * (decimal)0.19))
            {
                errors.Add(new ValidationError("REC500_Sec0_Error_SumaImpuestosMayorAlPermitido"));
            }

            return errors;
        }
    }
}
