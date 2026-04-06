using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Sorting
{
    public class SortCommand
    {
        public string ColumnId { get; set; }
        public SortDirection Direction { get; set; }

        public SortCommand()
        {

        }

        public SortCommand(string columnId, SortDirection direction)
        {
            this.ColumnId = columnId;
            this.Direction = direction;
        }
    }
}
