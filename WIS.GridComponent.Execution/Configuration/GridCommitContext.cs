using System;
using System.Collections.Generic;
using System.Text;
using WIS.GridComponent.Build.Configuration;

namespace WIS.GridComponent.Execution.Configuration
{
    public class GridCommitContext
    {
        public string GridId { get; set; }
        public GridFetchContext Query { get; set; }
        public List<GridRow> Rows { get; set; }
    }
}
