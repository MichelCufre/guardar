using WIS.FormComponent.Execution;
using WIS.FormComponent.Execution.Serialization;

namespace WIS.Application.Invocation
{
    public interface IFormControllerInvocation
    {
        IFormWrapper Invoke(IFormWrapper data, IFormController controller);
    }
}