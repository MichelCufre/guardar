using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WIS.GridComponent.Columns;

namespace WIS.GridComponent
{
    public class GridColumn : IGridColumn
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public GridColumnType Type { get; set; }
        public decimal Width { get; set; }
        public short Order { get; set; }
        public GridFixPosition Fixed { get; set; }
        public bool Insertable { get; set; }
        public bool Hidden { get; set; }
        public string CssClass { get; set; }
        public GridTextAlign LabelAlign { get; set; }
        public GridTextAlign TextAlign { get; set; }
        public string DefaultValue { get; set; }
        public bool AllowsFiltering { get; set; }
        public bool AllowsSorting { get; set; }
        public bool Translate { get; set; }

        [JsonIgnore]
        public bool IsNew { get; set; }

        public GridColumn()
        {
            this.Fixed = GridFixPosition.None;
            this.LabelAlign = GridTextAlign.Left;
            this.TextAlign = GridTextAlign.Left;
            this.Hidden = false;
            this.Insertable = false;
            this.Width = 100;
            this.IsNew = false;
            this.AllowsFiltering = true;
            this.AllowsSorting = true;
            this.Translate = false;
        }

        public virtual void UpdateSpecificValues(IGridColumn column)
        {
            return;
        }
    }
}