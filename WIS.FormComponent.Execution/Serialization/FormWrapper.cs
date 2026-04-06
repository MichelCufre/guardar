using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using WIS.Serialization;
using WIS.Serialization.Binders;
using WIS.FormComponent.Execution.Configuration;
using WIS.Components.Common;
using WIS.Components.Common.Redirection;
using WIS.Components.Common.Select;
using WIS.FormComponent.Execution.Responses;

namespace WIS.FormComponent.Execution.Serialization
{
    public class FormWrapper : TransferWrapper, IFormWrapper
    {
        public FormAction Action { get; set; }
        public string FormId { get; set; }

        public FormWrapper() : base()
        {
        }
        public FormWrapper(IFormWrapper wrapper) : base(wrapper)
        {
            this.FormId = wrapper.FormId;
        }

        public override ISerializationBinder GetSerializationBinder()
        {
            //Esto se define por seguridad, no se permite pasar tipos no esperados
            return new CustomSerializationBinder(new List<Type> {
                typeof(Form),
                typeof(FormField),
                typeof(FormInitializePayload),
                typeof(FormButton),
                typeof(FormValidationPayload),
                typeof(FormSubmitPayload),
                typeof(FormButtonActionPayload),
                typeof(FormInitializeContext),
                typeof(FormValidationContext),
                typeof(FormSubmitContext),
                typeof(FormButtonActionContext),
                typeof(FormSelectSearchPayload),
                typeof(FormSelectSearchResponse),
                typeof(FormSelectSearchContext),

                typeof(ComponentParameter),
                typeof(ComponentContext),
                typeof(ApplicationNotification),
                typeof(SelectOption),
                typeof(ComponentError),
                typeof(ConfirmMessage),
                typeof(PageRedirection)
            });
        }
    }
}
