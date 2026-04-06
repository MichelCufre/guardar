using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;

namespace WIS.GridComponent.Execution.Responses
{
    public class GridFetchResponse : ComponentContext
    {
        public List<GridRow> Rows { get; set; }
        public List<string> Selection { get; set; }
    }
}
