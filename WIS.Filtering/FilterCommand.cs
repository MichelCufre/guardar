using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Filtering
{
    public class FilterCommand
    {
        public string ColumnId { get; set; }
        public string Value { get; set; }

        public FilterCommand()
        {

        }

        public FilterCommand(string columnId, string value)
        {
            this.ColumnId = columnId;
            this.Value = value;
        }
    }
}
