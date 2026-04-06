
using System;
using System.Collections.Generic;
using System.Text;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ParametroHastaValidationRule : IValidationRule
    {
        protected readonly string _valueDesde;
        protected readonly string _valueHatsta;


        public ParametroHastaValidationRule(string valueDede, string valueHasra)
        {
            this._valueDesde = valueDede;
            this._valueHatsta = valueHasra;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(this._valueDesde) || !string.IsNullOrEmpty(this._valueHatsta))
            {
                errors.Add(new ValidationError("General_Sec0_Error_ParametrosHasta"));
            }

            return errors;
        }

    }
}
