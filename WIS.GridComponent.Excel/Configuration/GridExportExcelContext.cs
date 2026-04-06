using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;
using WIS.Filtering;
using WIS.Sorting;

namespace WIS.GridComponent.Excel.Configuration
{
    public class GridExportExcelContext : ComponentContext
    {
        public string FileName { get; set; }
        public GridExportExcelType Type { get; set; }
        public string GridId { get; set; }
        public string ExplicitFilter { get; set; }
        public List<FilterCommand> Filters { get; set; }
        public List<SortCommand> Sorts { get; set; }
        public string UserLanguage { get; set; }

        public GridExportExcelContext() : base()
        {
            this.Filters = new List<FilterCommand>();
            this.Sorts = new List<SortCommand>();
        }
    }
}
