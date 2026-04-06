using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;

namespace WIS.GridComponent.Execution.Responses
{
    public class GridFetchStatsResponse : ComponentContext
    {
        public GridStats Stats { get; set; }
    }
}
