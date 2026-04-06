using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;

namespace WIS.FormComponent.Validation
{
    public class FormValidationSchema : Dictionary<string, Func<FormField, Form, List<ComponentParameter>, FormValidationGroup>>
    {
    }
}
