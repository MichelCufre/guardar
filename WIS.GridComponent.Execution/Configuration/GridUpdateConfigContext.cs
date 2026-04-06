using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;

namespace WIS.GridComponent.Execution.Configuration
{
    public class GridUpdateConfigContext : ComponentContext
    {
        public string GridId { get; set; }
        public List<GridColumn> Columns { get; set; }

        public GridUpdateConfigContext() : base()
        {
            this.Columns = new List<GridColumn>();
        }
    }
}
