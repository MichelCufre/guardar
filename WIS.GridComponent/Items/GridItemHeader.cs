using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.GridComponent.Items
{
    public class GridItemHeader : GridItem, IGridItem
    {
        public GridItemHeader(string label)
        {
            this.Id = string.Empty;
            this.Label = label;
            this.CssClass = string.Empty;
            this.ItemType = GridItemType.Header;
        }
    }
}
