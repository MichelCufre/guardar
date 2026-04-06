using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Components.Common;
using WIS.Components.Common.Select;

namespace WIS.GridComponent.Execution.Responses
{
    public class GridSelectSearchResponse : ComponentContext
    {
        public List<SelectOption> Options { get; set; }
        public bool MoreResultsAvailable { get; set; }
    }
}
