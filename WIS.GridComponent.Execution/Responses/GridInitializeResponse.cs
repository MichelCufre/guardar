using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;
using WIS.GridComponent.Build;

namespace WIS.GridComponent.Execution.Responses
{
    public class GridInitializeResponse : ComponentContext
    {
        public Grid Grid { get; set; }
        public GridFilterData FilterData { get; set; }
        public bool IsEditingEnabled { get; set; }
        public bool IsCommitEnabled { get; set; }
        public bool IsRollbackEnabled { get; set; }
        public bool IsAddEnabled { get; set; }
        public bool IsRemoveEnabled { get; set; }
        public bool IsCommitButtonUnavailable { get; set; }
        public List<GridColumnLink> Links { get; set; }

        public GridInitializeResponse()
        {
            this.Links = new List<GridColumnLink>();
        }
    }
}
