using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;
using WIS.Filtering;

namespace WIS.GridComponent.Execution.Configuration
{
    public class GridNotifySelectionContext : ComponentContext
    {
        public string GridId { get; set; }
        public string RowId { get; set; }
        public GridSelection Selection { get; set; }
        public List<FilterCommand> Filters { get; set; }

        public GridNotifySelectionContext() : base()
        {
            this.Filters = new List<FilterCommand>();
        }
    }
}
