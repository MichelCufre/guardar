using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.GridComponent.Items;

namespace WIS.GridComponent.Columns
{
    public class GridColumnButton : GridColumn
    {
        public List<GridButton> Buttons { get; set; }

        public GridColumnButton() : base()
        {
            this.Type = GridColumnType.Button;
            this.AllowsFiltering = false;
            this.AllowsSorting = false;
            this.Width = 41;
        }

        public GridColumnButton(string id, List<GridButton> buttons) : this()
        {
            this.Id = id;
            this.Buttons = buttons;
            this.Width = 41 * buttons?.Count() ?? 1;
        }

        public override void UpdateSpecificValues(IGridColumn column)
        {
            var castedColumn = (GridColumnButton)column;

            this.Buttons = castedColumn.Buttons;
        }
    }
}
