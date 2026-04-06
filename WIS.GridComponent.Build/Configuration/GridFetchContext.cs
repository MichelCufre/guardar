using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;
using WIS.Filtering;
using WIS.Sorting;

namespace WIS.GridComponent.Build.Configuration
{
    public class GridFetchContext : ComponentContext
    {
        public string ExplicitFilter { get; set; }
        public int RowsToFetch { get; set; }
        public int RowsToSkip { get; set; }
        public List<FilterCommand> Filters { get; set; }
        public List<SortCommand> Sorts { get; set; }
        public bool IsGridInitialize { get; set; }

        public GridFetchContext()
        {
            this.IsGridInitialize = false;
            this.Filters = new List<FilterCommand>();
            this.Sorts = new List<SortCommand>();
        }
    }
}