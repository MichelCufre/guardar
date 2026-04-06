using System.Collections.Generic;
using System.Linq;
using WIS.Components.Common;
using WIS.Validation;

namespace WIS.FormComponent.Validation
{
    public class FormValidator
    {
        private readonly List<ComponentParameter> _parameters;

        public FormValidationSchema Schema { get; set; }
        public List<FormValidationGroup> Groups { get; set; }

        public FormValidator(List<ComponentParameter> parameters)
        {
            this._parameters = parameters;
            this.Schema = new FormValidationSchema();
            this.Groups = new List<FormValidationGroup>();
        }

        public void Validate(FormField field, Form form)
        {
            if (this.Schema.Count == 0)
                return;

            this.SetValidationRules(field, form);

            List<FormField> validatedFields = new List<FormField>();

            foreach (var validation in this.Groups)
            {
                FormField fieldToValidate = field.Id != validation.FieldId ? form.GetField(validation.FieldId) : field;

                validatedFields.Add(fieldToValidate);

                ValidationErrorGroup result = validation.Validate(fieldToValidate, form, this._parameters);

                bool dependencyFailed = false;

                if (validation.Dependencies.Count > 0)
                {
                    foreach (var dependency in validation.Dependencies)
                    {
                        if (validatedFields.Any(d => d.Id == dependency && !d.IsValid()))
                        {
                            dependencyFailed = true;
                            fieldToValidate.ForceCleanTouched = true;
                            break;
                        }
                    }
                }

                if (!dependencyFailed)
                {
                    if (result.IsValid)
                    {
                        fieldToValidate.SetOk();
                    }
                    else
                    {
                        var value = result.GetMessage();

                        fieldToValidate.SetError(value.Message, value.Arguments);

                        if (validation.BreakValidationChain)
                            break;
                    }
                }
            }
        }
        public void Validate(Form form)
        {
            if (this.Schema.Count == 0)
                return;

            this.SetValidationRules(form);

            foreach (var validation in this.Groups)
            {
                FormField field = form.GetField(validation.FieldId);

                ValidationErrorGroup result = validation.Validate(field, form, this._parameters);

                if (result.IsValid)
                {
                    field.SetOk();
                }
                else
                {
                    var value = result.GetMessage();

                    field.SetError(value.Message, value.Arguments);

                    if (validation.BreakValidationChain)
                        break;
                }
            }
        }

        private void SetValidationRules(FormField field, Form form)
        {
            if (field != null && this.Schema.ContainsKey(field.Id))
            {
                var result = this.Schema[field.Id](field, form, this._parameters);

                if (result == null)
                    return;

                result.FieldId = field.Id;

                foreach (var dependency in result.Dependencies)
                {
                    var subField = form.GetField(dependency);

                    this.SetValidationRules(subField, form);
                }

                this.Groups.Add(result);
            }
        }

        private void SetValidationRules(Form form)
        {
            foreach (var field in form.Fields)
            {
                if (this.Groups.Any(d => d.FieldId == field.Id))
                    continue;

                this.SetValidationRules(field, form);
            }
        }
    }
}
