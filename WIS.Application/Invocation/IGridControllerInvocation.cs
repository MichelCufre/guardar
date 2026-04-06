using System;
using System.Collections.Generic;
using System.Text;
using WIS.GridComponent.Execution;
using WIS.GridComponent.Execution.Serialization;

namespace WIS.Application.Invocation
{
    public interface IGridControllerInvocation
    {
        IGridWrapper Invoke(IGridWrapper data, IGridController controller);
    }
}
