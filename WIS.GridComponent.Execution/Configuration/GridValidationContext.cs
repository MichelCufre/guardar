using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;

namespace WIS.GridComponent.Execution.Configuration
{
    public class GridValidationContext : ComponentContext
    {
        public string GridId { get; set; }
        public GridRow Row { get; set; }
    }
}
