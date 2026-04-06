using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;
using WIS.Filtering;

namespace WIS.GridComponent.Execution.Configuration
{
    public class GridMenuItemActionContext : ComponentContext
    {
        public string GridId { get; set; }
        public string ButtonId { get; set; }

        public GridSelection Selection { get; set; }
        public List<FilterCommand> Filters { get; set; }

        public string Redirect { get; set; }

        public GridMenuItemActionContext() : base()
        {
            this.Filters = new List<FilterCommand>();
        }
    }
}
