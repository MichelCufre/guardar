using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Components.Common;
using WIS.Components.Common.Select;

namespace WIS.FormComponent.Execution.Responses
{
    public class FormSelectSearchResponse : ComponentContext
    {
        public List<SelectOption> Options { get; set; }
        public bool MoreResultsAvailable { get; set; }

        public FormSelectSearchResponse() : base()
        {
        }
    }
}
