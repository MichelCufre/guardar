using System;
using System.Collections.Generic;
using System.Text;
using WIS.FormComponent.Execution.Serialization;

namespace WIS.FormComponent.Execution
{
    public interface IFormActionResolver
    {
        IFormWrapper InvokeAction(IFormWrapper wrapper, IFormController controller);
    }
}
