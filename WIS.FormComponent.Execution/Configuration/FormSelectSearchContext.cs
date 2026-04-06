using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Components.Common;

namespace WIS.FormComponent.Execution.Configuration
{
    public class FormSelectSearchContext : ComponentContext
    {
        public string FieldId { get; set; }
        public int ResultLimit { get; set; }
        public string SearchValue { get; set; }

        public FormSelectSearchContext() : base()
        {
        }
    }
}
