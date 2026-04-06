using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;

namespace WIS.GridComponent.Execution.Configuration
{
    public class GridSelectSearchContext : ComponentContext
    {
        public string GridId { get; set; }
        public string ColumnId { get; set; }
        public int ResultLimit { get; set; }
        public string SearchValue { get; set; }
    }
}
