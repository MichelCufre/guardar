using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WIS.GridComponent.Columns;

namespace WIS.GridComponent
{
    public interface IGridColumn
    {
        string Id { get; set; }
        string Name { get; set; }
        GridColumnType Type { get; set; }
        decimal Width { get; set; }
        short Order { get; set; }
        GridFixPosition Fixed { get; set; }
        bool Insertable { get; set; }
        bool Hidden { get; set; }
        string CssClass { get; set; }
        GridTextAlign LabelAlign { get; set; }
        GridTextAlign TextAlign { get; set; }
        string DefaultValue { get; set; }
        bool AllowsFiltering { get; set; }
        bool Translate { get; set; }

        [JsonIgnore]
        bool IsNew { get; set; }

        void UpdateSpecificValues(IGridColumn column);
    }
}
