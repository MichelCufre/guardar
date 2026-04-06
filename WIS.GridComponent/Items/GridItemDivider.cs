using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.GridComponent.Items
{
    public class GridItemDivider : GridItem, IGridItem
    {
        public GridItemDivider()
        {
            this.Id = string.Empty;
            this.Label = string.Empty;
            this.CssClass = string.Empty;
            this.ItemType = GridItemType.Divider;
        }
    }
}
