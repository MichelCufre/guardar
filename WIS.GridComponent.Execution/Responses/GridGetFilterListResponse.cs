using System;
using System.Collections.Generic;
using System.Text;
using WIS.GridComponent.Build;

namespace WIS.GridComponent.Execution.Responses
{
    public class GridGetFilterListResponse
    {
        public GridFilterData FilterData { get; set; }
        public string Username { get; set; }
    }
}
