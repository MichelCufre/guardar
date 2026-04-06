using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class NonNullValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly string _nombreCampo;

        public NonNullValidationRule(string value)
        {
            this._value = value;
        }

        public NonNullValidationRule(string value, string nombreCampo)
        {
            this._value = value;
            this._nombreCampo = nombreCampo;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (string.IsNullOrEmpty(this._value))
            {
                if (!string.IsNullOrEmpty(this._nombreCampo))
                    errors.Add(new ValidationError("General_Sec0_Error_Error76", new List<string>() { this._nombreCampo }));
                else
                    errors.Add(new ValidationError("General_Sec0_Error_Error25"));
            }

            return errors;
        }
    }
}
