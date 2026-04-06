using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Validation
{
    public interface IValidationRule
    {
        List<IValidationError> Validate();
    }
}
