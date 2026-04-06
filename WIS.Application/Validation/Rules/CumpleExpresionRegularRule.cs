using System.Collections.Generic;
using System.Text.RegularExpressions;
using WIS.Validation;
using static iText.Kernel.Pdf.Colorspace.PdfSpecialCs;

namespace WIS.Application.Validation.Rules
{
    public class CumpleExpresionRegularRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly string _expresion;

        public CumpleExpresionRegularRule(string value, string expresion)
        {
            this._value = value;
            this._expresion = expresion;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            var regex = new Regex(_expresion, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            if (!regex.IsMatch(_value))
                errors.Add(new ValidationError("General_Sec0_Error_RegexInvalida"));

            return errors;
        }
    }
}
