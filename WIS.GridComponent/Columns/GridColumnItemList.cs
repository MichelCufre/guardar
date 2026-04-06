using System;
using System.Collections.Generic;
using System.Text;
using WIS.GridComponent.Items;

namespace WIS.GridComponent.Columns
{
    public class GridColumnItemList : GridColumn
    {
        public List<IGridItem> Items { get; set; }

        public GridColumnItemList() : base()
        {
            this.Type = GridColumnType.ItemList;
            this.Items = new List<IGridItem>();
            this.AllowsFiltering = false;
            this.AllowsSorting = false;
            this.Width = 41;
        }

        public GridColumnItemList(string id, List<IGridItem> items) : this()
        {
            this.Id = id;
            this.Items = items;
        }

        public override void UpdateSpecificValues(IGridColumn column)
        {
            var castedColumn = (GridColumnItemList)column;

            this.Items = castedColumn.Items;
        }
    }
}
