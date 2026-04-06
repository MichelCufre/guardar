using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Components.Common;
using WIS.Components.Common.Redirection;

namespace WIS.FormComponent.Execution.Configuration
{
    public class FormButtonActionContext : ComponentContext
    {
        public string ButtonId { get; set; }
        public PageRedirection Redirection { get; set; }
        public bool ResetForm { get; set; }

        public FormButtonActionContext() : base()
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
    }
}
