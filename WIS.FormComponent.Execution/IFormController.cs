using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Components.Common.Select;
using WIS.FormComponent.Execution.Configuration;

namespace WIS.FormComponent.Execution
{
    public interface IFormController
    {
        Form FormInitialize(Form form, FormInitializeContext context);
        Form FormValidateForm(Form form, FormValidationContext context);
        Form FormButtonAction(Form form, FormButtonActionContext context);
        Form FormSubmit(Form form, FormSubmitContext context);
        List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context);
    }
}
