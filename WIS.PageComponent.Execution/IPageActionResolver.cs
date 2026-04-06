using System;
using System.Collections.Generic;
using System.Text;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.PageComponent.Execution
{
    public interface IPageActionResolver
    {
        IPageWrapper InvokeAction(IPageWrapper wrapper, IPageController controller);
    }
}
