using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.FormComponent.Execution.Configuration
{
    public class FormSubmitPayload
    {
        public Form Form { get; set; }
        public FormSubmitContext Context { get; set; }
    }
}
