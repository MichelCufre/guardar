using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Components.Common;
using WIS.Components.Common.Select;

namespace WIS.FormComponent
{
    public class FormField
    {
        public string Id { get; set; }
        public FormFieldType Type { get; set; }
        public bool Hidden { get; set; }
        public bool ReadOnly { get; set; }
        public bool Disabled { get; set; }
        public FormStatus Status { get; set; }
        public ComponentError Error { get; set; }
        public bool ForceCleanTouched { get; set; }
        public bool IsMultidataCodeReading { get; set; }
        public string Value { get; set; }
        public List<SelectOption> Options { get; set; }

        [JsonIgnore]
        public bool IsValidated { get; set; }

        public FormField()
        {
            this.Status = FormStatus.Ok;
            this.IsValidated = false;
            this.Options = new List<SelectOption>();
        }

        public bool IsValid()
        {
            return this.Status == FormStatus.Ok;
        }

        public string GetOptionLabel()
        {
            if (!string.IsNullOrEmpty(this.Value))
                return this.Options.FirstOrDefault(w => w.Value == this.Value)?.Label;

            return null;
        }

        public bool IsChecked()
        {
            return this.Value == "true";
        }

        public void SetError(string message, List<string> arguments = null)
        {
            this.Status = FormStatus.Error;
            this.Error = new ComponentError(message, arguments);
            this.IsValidated = true;
        }

        public void SetError(ComponentError error)
        {
            this.Status = FormStatus.Error;
            this.Error = error;
            this.IsValidated = true;
        }

        public void SetOk()
        {
            this.Status = FormStatus.Ok;
            this.Error = null;
            this.IsValidated = true;
        }

        public void SetChecked(bool value)
        {
            this.Value = value ? "true" : "false";
        }
    }
}
