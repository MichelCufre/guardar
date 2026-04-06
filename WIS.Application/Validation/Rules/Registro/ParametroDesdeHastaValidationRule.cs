using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ParametroDesdeHastaValidationRule : IValidationRule
    {
        protected readonly string _valueDesde;
        protected readonly string _valueHasta;
        protected readonly IFormatProvider _proveedor;
        protected readonly bool _validar;

        public ParametroDesdeHastaValidationRule(string valueDesde, string valueHasta, IFormatProvider proveedor, bool validar = true)
        {
            this._valueDesde = valueDesde;
            this._valueHasta = valueHasta;
            this._proveedor = proveedor;
            _validar = validar;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(this._valueDesde) && string.IsNullOrEmpty(this._valueHasta))
                errors.Add(new ValidationError("General_Sec0_Error_ParametrosDesde"));
            else if (!string.IsNullOrEmpty(this._valueHasta) && string.IsNullOrEmpty(this._valueDesde))
                errors.Add(new ValidationError("General_Sec0_Error_ParametrosHasta"));

            if (errors.Count > 0)
                return errors;

            if (_validar)
            {
                var valueDesde = decimal.Parse(this._valueDesde, NumberStyles.AllowDecimalPoint, _proveedor);
                var valueHasta = decimal.Parse(this._valueHasta, NumberStyles.AllowDecimalPoint, _proveedor);

                if (valueDesde > valueHasta)
                    errors.Add(new ValidationError("General_Sec0_Error_NumeroColumnaDesdeMenorHasta", new List<string>() { _valueHasta.ToString(), _valueDesde.ToString() }));
            }


            return errors;
        }

    }
}
