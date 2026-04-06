using System;
using System.Collections.Generic;
using System.Text;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.FormComponent.Validation;

namespace WIS.Application.Validation
{
    public class FormValidationService : IFormValidationService
    {
        public Form Validate(IFormValidationModule module, Form form, FormValidationContext context)
        {
            var validator = new FormValidator(context.Parameters);

            module.Validator = validator;

            if (context.IsValidatingSingleField())
                return this.ValidateSingleField(module, form, context);

            return this.ValidateForm(module, form, context);
        }
        

        private Form ValidateSingleField(IFormValidationModule module, Form form, FormValidationContext context)
        {
            FormField field = form.GetField(context.FieldId);

            module.Validate(field, form);

            return form;
        }

        private Form ValidateForm(IFormValidationModule module, Form form, FormValidationContext context)
        {
            module.Validate(form);

            return form;
        }
    }
}
