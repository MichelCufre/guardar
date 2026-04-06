using System;
using System.Collections.Generic;
using WIS.GridComponent.Execution.Serialization;

namespace WIS.GridComponent.Execution
{
    public interface IGridCoordinator
    {
        Dictionary<GridAction, Func<IGridWrapper, IGridController, IGridWrapper>> Actions { get; }
        bool IsActionAvailable(GridAction pageAction);
    }
}
