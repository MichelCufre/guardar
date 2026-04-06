using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.GridComponent
{
    public class GridColumnLinkMapping
    {
        public string Column { get; set; }
        public string MappedColumn { get; set; }

        public GridColumnLinkMapping(string column, string mappedColumn)
        {
            this.Column = column;
            this.MappedColumn = mappedColumn;
        }
    }
}