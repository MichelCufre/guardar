using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using WIS.Extension;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class DateTimeImportExcelValidationRule : IValidationRule
    {
        protected readonly string _valueDateString;
        protected readonly IFormatProvider _proveedor;

        public DateTimeImportExcelValidationRule(IFormatProvider proveedorDeFormato, string valueDateString)
        {
            this._valueDateString = valueDateString;
            this._proveedor = proveedorDeFormato;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!DateTime.TryParse(this._valueDateString, _proveedor, DateTimeStyles.None, out DateTime fecha))
                errors.Add(new ValidationError("General_Sec0_Error_InvalidDate"));

            return errors;
        }
    }
}
