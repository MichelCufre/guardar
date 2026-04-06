using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;

namespace WIS.GridComponent.Execution.Configuration
{
    public class GridSelectSearchPayload
    {
        public GridRow Row { get; set; }
        public GridSelectSearchContext Query { get; set; }
    }
}
