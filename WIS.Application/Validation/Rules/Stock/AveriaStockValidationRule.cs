using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class AveriaStockValidationRule : IValidationRule
    {
        protected readonly string _value;

        public AveriaStockValidationRule(string value)
        {
            this._value = value;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (this._value != "N" && this._value != "S")
                errors.Add(new ValidationError("General_Sec0_Error_InvalidValue"));

            return errors;
        }
    }
}
