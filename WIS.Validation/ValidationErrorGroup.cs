using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Validation
{
    public class ValidationErrorGroup : IValidationErrorGroup
    {
        public bool IsValid { get; private set; }
        public List<IValidationError> Errors { get; private set; }

        public ValidationErrorGroup()
        {
            this.IsValid = true;
            this.Errors = new List<IValidationError>();
        }

        public void AddErrors(List<IValidationError> errors)
        {
            this.IsValid = false;
            this.Errors.AddRange(errors);
        }

        public ValidationError GetMessage()
        {
            var sb = new StringBuilder();
            var arguments = new List<string>();

            bool first = true;

            foreach (var error in this.Errors)
            {
                if (error.Arguments != null)
                    arguments.AddRange(error.Arguments);

                if (!first)
                    sb.Append(" ");
                else
                    first = false;

                sb.Append(error.Message);
            }

            return new ValidationError(sb.ToString(), arguments);
        }
    }
}
