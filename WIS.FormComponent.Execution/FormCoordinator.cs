using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Components.Common.Notification;
using WIS.Components.Common.Select;
using WIS.Exceptions;
using WIS.FormComponent.Execution.Configuration;
using WIS.FormComponent.Execution.Responses;
using WIS.FormComponent.Execution.Serialization;

namespace WIS.FormComponent.Execution
{
    public class FormCoordinator : IFormCoordinator
    {
        private readonly IFormConfigProvider _configProvider;

        public Dictionary<FormAction, Func<IFormWrapper, IFormController, IFormWrapper>> Actions { get; }

        public FormCoordinator(IFormConfigProvider configProvider)
        {
            this._configProvider = configProvider;

            this.Actions = new Dictionary<FormAction, Func<IFormWrapper, IFormController, IFormWrapper>>
            {
                [FormAction.ButtonAction] = this.ButtonAction,
                [FormAction.Initialize] = this.Initialize,
                [FormAction.SelectSearch] = this.SelectSearch,
                [FormAction.Submit] = this.Submit,
                [FormAction.ValidateField] = this.ValidateField
            };
        }

        public bool IsActionAvailable(FormAction action)
        {
            return this.Actions.ContainsKey(action);
        }

        private IFormWrapper Initialize(IFormWrapper wrapper, IFormController controller)
        {
            var data = wrapper.GetData<FormInitializePayload>();

            IFormWrapper response = new FormWrapper(wrapper);

            data.Form = controller.FormInitialize(data.Form, data.Context);

            response.SetData(data);

            return response;
        }
        private IFormWrapper ValidateField(IFormWrapper wrapper, IFormController controller)
        {
            var data = wrapper.GetData<FormValidationPayload>();

            IFormWrapper response = new FormWrapper(wrapper);

            data.Form = controller.FormValidateForm(data.Form, data.Context);

            response.SetData(data);

            return response;
        }
        private IFormWrapper ButtonAction(IFormWrapper wrapper, IFormController controller)
        {
            var data = wrapper.GetData<FormButtonActionPayload>();

            IFormWrapper response = new FormWrapper(wrapper);

            data.Form = controller.FormButtonAction(data.Form, data.Context);

            response.SetData(data);

            return response;
        }
        private IFormWrapper Submit(IFormWrapper wrapper, IFormController controller)
        {
            var data = wrapper.GetData<FormSubmitPayload>();

            IFormWrapper response = new FormWrapper(wrapper);

            try
            {
                var validationContext = new FormValidationContext
                {
                    FieldId = null,
                    Parameters = data.Context.Parameters,
                    IsSubmitting = true,
                    ButtonId = data.Context.ButtonId
                };

                data.Form = controller.FormValidateForm(data.Form, validationContext);

                if (data.Form.Fields.Any(d => !d.IsValid()))
                    throw new ValidationFailedException("General_Sec0_Error_Error07");

                data.Form = controller.FormSubmit(data.Form, data.Context);

                if (data.Context.Notifications.Any(d => d.Type == ApplicationNotificationType.Error))
                {
                    var msg = data.Context.Notifications.FirstOrDefault().Message ?? "General_Sec0_Error_Error07";
                    var args = data.Context.Notifications.FirstOrDefault()?.Arguments?.ToArray() ?? new string[] { };

                    throw new ValidationFailedException(msg, args);
                }

                response.SetData(data);
            }
            catch (ValidationFailedException ex)
            {
                //FIX 17/06/21 - Para el caso de grillas que son formulario y el submit es por formulario, valida solamente si hay algun error de campos
                //El problema se da porque no se muestra el mensaje de ValidationFailedException cuando no son errores de validacion
                if (data.Form.Fields.Any(d => !d.IsValid()) || ex.setData)
                    response.SetData(data);

                response.SetError(ex.Message, ex.StrArguments);
            }

            return response;
        }
        private IFormWrapper SelectSearch(IFormWrapper wrapper, IFormController controller)
        {
            var data = wrapper.GetData<FormSelectSearchPayload>();

            IFormWrapper response = new FormWrapper(wrapper);

            var result = new SelectResult(this._configProvider.GetSelectResultLimit());

            data.Context.ResultLimit = result.GetUsableResultLimit();

            result.SetOptions(controller.FormSelectSearch(data.Form, data.Context));

            var responseData = new FormSelectSearchResponse
            {
                Options = result.Options,
                MoreResultsAvailable = result.MoreResultsAvailable,
                Parameters = data.Context.Parameters,
                Notifications = data.Context.Notifications
            };

            response.SetData(responseData);

            return response;
        }
    }
}
