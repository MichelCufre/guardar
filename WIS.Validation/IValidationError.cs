using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Validation
{
    public interface IValidationError
    {
        string Message { get; set; }
        List<string> Arguments { get; set; }
    }
}
