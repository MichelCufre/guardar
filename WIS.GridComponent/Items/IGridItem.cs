using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.GridComponent.Items
{
    public interface IGridItem
    {
        string Id { get; set; }
        string Label { get; set; }
        string CssClass { get; set; }
        GridItemType ItemType { get; set; }
    }
}
