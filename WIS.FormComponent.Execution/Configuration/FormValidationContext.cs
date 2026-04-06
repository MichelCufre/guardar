using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Components.Common;

namespace WIS.FormComponent.Execution.Configuration
{
    public class FormValidationContext : ComponentContext
    {
        public string FieldId { get; set; }
        public string Redirect { get; set; }
        public bool IsSubmitting { get; set; }
        public string ButtonId { get; set; } //TODO

        public FormValidationContext() : base()
        {
        }

        public bool IsValidatingSingleField()
        {
            return this.FieldId != null;
        }
    }
}
