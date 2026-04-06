using System;
using System.Collections.Generic;
using System.Text;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.PageComponent.Execution
{
    public interface IPageCoordinator
    {
        Dictionary<PageAction, Func<IPageWrapper, IPageController, IPageWrapper>> Actions { get; }
        bool IsActionAvailable(PageAction pageAction);
    }
}
