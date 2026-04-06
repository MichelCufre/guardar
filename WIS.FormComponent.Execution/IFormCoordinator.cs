using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.FormComponent.Execution.Serialization;

namespace WIS.FormComponent.Execution
{
    public interface IFormCoordinator
    {
        Dictionary<FormAction, Func<IFormWrapper, IFormController, IFormWrapper>> Actions { get; }
        bool IsActionAvailable(FormAction pageAction);
    }
}
