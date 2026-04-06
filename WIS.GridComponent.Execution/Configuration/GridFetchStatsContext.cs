using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;
using WIS.Filtering;
using WIS.Sorting;

namespace WIS.GridComponent.Execution.Configuration
{
    public class GridFetchStatsContext : ComponentContext
    {
        public string ExplicitFilter { get; set; }
        public List<FilterCommand> Filters { get; set; }
        public List<SortCommand> Sorts { get; set; }

        public GridFetchStatsContext()
        {
            this.Filters = new List<FilterCommand>();
            this.Sorts = new List<SortCommand>();
        }
    }
}
