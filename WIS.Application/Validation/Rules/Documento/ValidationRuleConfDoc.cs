using System.Collections.Generic;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Documento
{
    public class ValidationRuleConfDoc : IValidationRule
    {
        protected readonly string _doc;
        protected readonly string _configdoc;
        protected readonly string _message;
        public ValidationRuleConfDoc(string doc, string configdoc)
        {
            this._doc = doc;
            this._configdoc = configdoc;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (string.IsNullOrEmpty(this._configdoc))
                errors.Add(new ValidationError(this._message ?? "General_Sec0_Error_Error25"));
            else if (_configdoc != _doc)
            {
                errors.Add(new ValidationError(this._message ?? "General_Sec0_Error_Error85"));
            }
            return errors;
        }
    }
}
