using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.FormComponent.Execution.Configuration
{
    public class FormInitializePayload
    {
        public Form Form { get; set; }
        public FormInitializeContext Context { get; set; }

        public FormInitializePayload()
        {
            this.Form = new Form();
            this.Context = new FormInitializeContext();
        }
    }
}
