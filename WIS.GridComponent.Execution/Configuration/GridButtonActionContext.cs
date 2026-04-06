using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;
using WIS.Components.Common.Redirection;
using WIS.GridComponent.Execution.Serialization;

namespace WIS.GridComponent.Execution.Configuration
{
    public class GridButtonActionContext : ComponentContext
    {
        public string GridId { get; set; }
        public string ButtonId { get; set; }

        [JsonConverter(typeof(GridCellColumnConverter))]
        public GridColumn Column { get; set; }

        public GridRow Row { get; set; }
        public PageRedirection Redirection { get; set; }

        public GridButtonActionContext() : base()
        {
        }

        public void Redirect(string url, List<ComponentParameter> parameters = null)
        {
            this.Redirection = new PageRedirection
            {
                Url = url,
                Module = "",
                NewTab = false,
                Parameters = parameters
            };
        }
        public void Redirect(string url, bool newTab, List<ComponentParameter> parameters = null)
        {
            this.Redirection = new PageRedirection
            {
                Url = url,
                Module = "",
                NewTab = newTab,
                Parameters = parameters
            };
        }

    }
}
