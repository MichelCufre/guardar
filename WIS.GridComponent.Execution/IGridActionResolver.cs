using System;
using System.Collections.Generic;
using System.Text;
using WIS.GridComponent.Execution.Serialization;

namespace WIS.GridComponent.Execution
{
    public interface IGridActionResolver
    {
        IGridWrapper InvokeAction(IGridWrapper wrapper, IGridController controller);
    }
}
