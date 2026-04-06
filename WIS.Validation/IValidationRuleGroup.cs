using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Validation
{
    public interface IValidationRuleGroup
    {
        bool BreakValidationChain { get; set; }

        IValidationErrorGroup Validate();
    }
}
