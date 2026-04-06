using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;

namespace WIS.FormComponent
{
    public class FormButton
    {
        public string Id { get; set; }
        public bool Hidden { get; set; }
        public bool Disabled { get; set; }
        public string Label { get; set; }
        public FormButtonVariant Variant { get; set; }
        public FormButtonType ButtonType { get; set; }
        [JsonConverter(typeof(ConfirmMessageConverter))]
        public IConfirmMessage ConfirmMessage { get; set; }

        public FormButton()
        {
            this.Variant = FormButtonVariant.Primary;
            this.ButtonType = FormButtonType.Unknown;
        }
    }
}
