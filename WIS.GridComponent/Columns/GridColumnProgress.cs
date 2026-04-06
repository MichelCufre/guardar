using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.GridComponent.Columns
{
    public class GridColumnProgress : GridColumn
    {
        public GridColumnProgress()
        {
            this.Type = GridColumnType.Progress;
        }

        public GridColumnProgress(string id)
        {
            this.Id = id;
            this.Type = GridColumnType.Progress;
        }
    }
}
