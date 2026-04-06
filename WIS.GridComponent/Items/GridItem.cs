using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.GridComponent.Items
{
    public abstract class GridItem : IGridItem
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string CssClass { get; set; }
        public GridItemType ItemType { get; set; }
    }
}
