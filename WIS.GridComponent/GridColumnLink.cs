using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.GridComponent
{
    public class GridColumnLink
    {
        public string Column { get; set; }
        public string Url { get; set; }
        public List<GridColumnLinkMapping> PropertyMapping { get; set; }

        public GridColumnLink(string column, string url, List<GridColumnLinkMapping> mapping)
        {
            this.Column = column;
            this.Url = url;
            this.PropertyMapping = mapping;
        }
    }
}
