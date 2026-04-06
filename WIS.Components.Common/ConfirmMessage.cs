using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Components.Common
{
    public class ConfirmMessage : IConfirmMessage
    {
        public string Message { get; set; }
        public string AcceptLabel { get; set; }
        public string CancelLabel { get; set; }
        public ButtonVariant AcceptVariant { get; set; }
        public ButtonVariant CancelVariant { get; set; }

        public ConfirmMessage()
        {
            this.AcceptVariant = ButtonVariant.Primary;
            this.CancelVariant = ButtonVariant.Secondary;
        }

        public ConfirmMessage(string message)
        {
            this.Message = message;
            this.AcceptVariant = ButtonVariant.Primary;
            this.CancelVariant = ButtonVariant.Secondary;
        }

        public ConfirmMessage(string message, ButtonVariant acceptVariant, ButtonVariant cancelVariant)
        {
            this.Message = message;
            this.AcceptVariant = acceptVariant;
            this.CancelVariant = cancelVariant;
        }
    }
}
