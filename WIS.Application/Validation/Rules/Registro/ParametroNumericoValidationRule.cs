using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ParametroNumericoValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly int _largo;
        protected readonly int? _precision;
        protected readonly IFormatProvider _proveedor;
        protected readonly bool _permitirNegativo;
        protected readonly bool _mayorACero;

        public ParametroNumericoValidationRule(string value, int largo, IFormatProvider proveedor, int? precision, bool permitirNegativo = false, bool mayorACero = false)
        {
            this._value = value;
            this._largo = largo - (precision ?? 0);
            this._precision = precision;
            this._proveedor = proveedor;
            this._permitirNegativo = permitirNegativo;
            this._mayorACero = mayorACero;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            var separador = ",";

            if (string.IsNullOrEmpty(this._value))
                return errors;

            if (_precision != null && _proveedor != null)
            {
                if (_proveedor != null)
                    separador = ((CultureInfo)_proveedor).NumberFormat.NumberDecimalSeparator;

                string pattern = @"^(-)?[0-9]{0," + this._largo.ToString() + "}(\\" + separador + "[0-9]{0," + this._precision.ToString() + "})?$"; // DECIMAL

                if (!decimal.TryParse(this._value, NumberStyles.AllowDecimalPoint, _proveedor, out decimal outValue))
                    errors.Add(new ValidationError("General_Sec0_Error_FormatoIncorrecto", new List<string> { separador }));
                else
                {
                    if (!Regex.IsMatch(this._value, pattern))
                        errors.Add(new ValidationError("General_Sec0_Error_Error89", new List<string> { this._largo.ToString(), this._precision.ToString() }));

                    if (!_permitirNegativo && outValue < 0)
                        errors.Add(new ValidationError("General_Sec0_Error_Error26"));
                    else if (_mayorACero && outValue <= 0)
                        errors.Add(new ValidationError("General_Sec0_Error_MayorACero"));
                }
            }
            else
            {
                if (!double.TryParse(_value, out double parsedValue))
                    errors.Add(new ValidationError("General_Sec0_Error_Error14"));
                else if (_value.Length > _largo)
                    errors.Add(new ValidationError("General_Sec0_Error_LargoMaxExcedidoConEsperado", new List<string> { _largo.ToString() }));
                else if (!_permitirNegativo && parsedValue < 0)
                    errors.Add(new ValidationError("General_Sec0_Error_Error26"));
                else if (_mayorACero && parsedValue <= 0)
                    errors.Add(new ValidationError("General_Sec0_Error_MayorACero"));
            }
            return errors;
        }
    }
}
