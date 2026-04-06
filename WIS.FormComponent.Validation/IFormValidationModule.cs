using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.FormComponent.Validation
{
    public interface IFormValidationModule
    {
        FormValidator Validator { set; }
        void Validate(FormField field, Form form);
        void Validate(Form form);
    }
}
