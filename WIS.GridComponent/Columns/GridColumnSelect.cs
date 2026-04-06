using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common.Select;

namespace WIS.GridComponent.Columns
{
    public class GridColumnSelect : GridColumn
    {
        public List<SelectOption> Options { get; set; }

        public GridColumnSelect()
        {
            this.Type = GridColumnType.Select;
        }

        public GridColumnSelect(string id, List<SelectOption> options)
        {
            this.Id = id;
            this.Type = GridColumnType.Select;
            this.Options = options;
            this.Name = id;
        }

        public override void UpdateSpecificValues(IGridColumn column)
        {
            var castedColumn = (GridColumnSelect)column;

            this.Options = castedColumn.Options;
        }
    }
}
