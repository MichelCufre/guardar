using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class DecimalLengthWithPresicionValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly int _largo;
        protected readonly int _presicion;
        protected readonly IFormatProvider _proveedor;
        protected readonly bool _aceptaDecimales;
        protected readonly string _separador;

        public DecimalLengthWithPresicionValidationRule(string value, int largo, int precision, IFormatProvider proveedorDeFormato, bool aceptaDecimales = true, string separador = null)
        {
            this._value = value;
            this._largo = largo - precision;
            this._presicion = precision;
            this._proveedor = proveedorDeFormato;
            this._aceptaDecimales = aceptaDecimales;
            this._separador = separador;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            var separador = _separador ?? ((CultureInfo)_proveedor).NumberFormat.NumberDecimalSeparator; //TODO: Cambiar dependencia de IFormatProvider a proveedor de formato de numero para evitar downcasting

            string pattern = @"^-?[0-9]{0," + this._largo.ToString() + "}([" + separador + "][0-9]{0," + this._presicion.ToString() + "})?$"; // DECIMAL

            if (string.IsNullOrEmpty(this._value))
                return errors;

            if (!decimal.TryParse(this._value, NumberStyles.Any, _proveedor, out decimal outValue))
                errors.Add(new ValidationError("General_Sec0_Error_FormatoIncorrecto", new List<string> { separador }));
            else
            {
                if (!Regex.IsMatch(this._value, pattern))
                {
                    errors.Add(new ValidationError("General_Sec0_Error_Error39", new List<string> { this._largo.ToString(), this._presicion.ToString(), separador }));
                }
            }

            if (!_aceptaDecimales && outValue != Math.Truncate(outValue))
                errors.Add(new ValidationError("General_Sec0_Error_NoAceptaDecimales"));

            return errors;
        }
    }
}
