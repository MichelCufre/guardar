using WIS.PageComponent.Execution;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.Application.Invocation
{
    public interface IPageControllerInvocation
    {
        IPageWrapper Invoke(IPageWrapper data, IPageController controller);
    }
}