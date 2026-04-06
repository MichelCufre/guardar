using System;
using System.Collections.Generic;
using System.Text;
using WIS.Filtering;
using WIS.Sorting;

namespace WIS.GridComponent.Build
{
    public class GridFilterData
    {
        public long Id { get; set; }
        public string GridId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? Date { get; set; }
        public List<FilterCommand> Filters { get; set; }
        public List<SortCommand> Sorts { get; set; }
        public string ExplicitFilter { get; set; }
        public bool IsGlobal { get; set; }
        public bool IsDefault { get; set; }

        public GridFilterData()
        {
            this.Filters = new List<FilterCommand>();
            this.Sorts = new List<SortCommand>();
        }
    }
}
