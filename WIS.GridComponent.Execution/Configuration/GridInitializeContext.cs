using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;
using WIS.GridComponent.Build.Configuration;

namespace WIS.GridComponent.Execution.Configuration
{
    public class GridInitializeContext : ComponentContext
    {
        public GridFetchContext FetchContext { get; set; }

        public bool IsEditingEnabled { get; set; }
        public bool IsCommitEnabled { get; set; }
        public bool IsRollbackEnabled { get; set; }
        public bool IsAddEnabled { get; set; }
        public bool IsRemoveEnabled { get; set; }
        public bool IsCommitButtonUnavailable { get; set; }
        public List<GridColumnLink> Links { get; set; }

        public GridInitializeContext()
        {
            this.IsEditingEnabled = false;
            this.IsCommitEnabled = true;
            this.IsRollbackEnabled = true;
            this.IsAddEnabled = true;
            this.IsRemoveEnabled = true;
            this.IsCommitButtonUnavailable = false;
            this.Links = new List<GridColumnLink>();
        }

        public void AddLink(string column, string url, List<GridColumnLinkMapping> mapping)
        {
            this.Links.Add(new GridColumnLink(column, url, mapping));
        }
        public void AddLink(string column, string url)
        {
            this.Links.Add(new GridColumnLink(column, url, new List<GridColumnLinkMapping>
            {
                new GridColumnLinkMapping(column, column)
            }));
        }
    }
}
