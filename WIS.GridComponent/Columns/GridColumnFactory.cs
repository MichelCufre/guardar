using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.GridComponent.Columns
{
    public class GridColumnFactory
    {
        public IGridColumn Create(GridColumnType type)
        {
            switch (type)
            {
                case GridColumnType.Text:
                    return new GridColumnText();
                case GridColumnType.Button:
                    return new GridColumnButton();
                case GridColumnType.ItemList:
                    return new GridColumnItemList();
                case GridColumnType.Progress:
                    return new GridColumnProgress();
                case GridColumnType.Select:
                    return new GridColumnSelect();
                case GridColumnType.SelectAsync:
                    return new GridColumnSelectAsync();
                case GridColumnType.Toggle:
                    return new GridColumnToggle();
            }

            return new GridColumnText();
        }
    }
}
