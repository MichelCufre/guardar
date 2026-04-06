using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;
using WIS.Validation;

namespace WIS.FormComponent.Validation
{
    public class FormValidationGroup
    {
        public string FieldId { get; set; }
        public List<string> Dependencies { get; set; }
        public List<IValidationRule> Rules { get; set; }
        public bool BreakValidationChain { get; set; }
        public Action<FormField, Form, List<ComponentParameter>> OnSuccess { private get; set; }
        public Action<FormField, Form, List<ComponentParameter>> OnFailure { private get; set; }

        public FormValidationGroup()
        {
            this.Rules = new List<IValidationRule>();
            this.Dependencies = new List<string>();
            this.BreakValidationChain = false;
        }

        public ValidationErrorGroup Validate(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var errorGroup = new ValidationErrorGroup();

            foreach (var rule in this.Rules)
            {
                var result = rule.Validate();

                if (result.Count > 0)
                {
                    //Rompe la cadena de validación si una de las validaciones falla
                    errorGroup.AddErrors(result);
                    break;
                }
            }

            if (errorGroup.IsValid && this.OnSuccess != null)
                this.OnSuccess(field, form, parameters);

            if (!errorGroup.IsValid && this.OnFailure != null)
                this.OnFailure(field, form, parameters);

            return errorGroup;
        }
    }
}
