using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Validation
{
    public interface IValidationErrorGroup
    {
        List<IValidationError> Errors { get; }
        bool IsValid { get; }

        void AddErrors(List<IValidationError> error);

        ValidationError GetMessage();
    }
}
