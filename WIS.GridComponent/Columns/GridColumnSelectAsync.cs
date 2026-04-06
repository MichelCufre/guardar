using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common.Select;

namespace WIS.GridComponent.Columns
{
    public class GridColumnSelectAsync : GridColumn
    {
        public List<SelectOption> Options { get; set; }

        public GridColumnSelectAsync()
        {
            this.Type = GridColumnType.SelectAsync;
        }

        public GridColumnSelectAsync(string id, List<SelectOption> options)
        {
            this.Id = id;
            this.Type = GridColumnType.SelectAsync;
            this.Options = options;
        }

        public override void UpdateSpecificValues(IGridColumn column)
        {
            var castedColumn = (GridColumnSelectAsync)column;

            this.Options = castedColumn.Options;
        }
    }
}
