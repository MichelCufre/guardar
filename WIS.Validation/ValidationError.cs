using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Validation
{
    public class ValidationError : IValidationError
    {
        public string Message { get; set; }
        public List<string> Arguments { get; set; }

        public ValidationError(string message)
        {
            this.Message = message;
        }

        public ValidationError(string message, List<string> arguments)
        {
            this.Message = message;
            this.Arguments = arguments;
        }
    }
}
