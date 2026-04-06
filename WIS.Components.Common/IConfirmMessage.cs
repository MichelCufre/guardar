using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Components.Common
{
    public interface IConfirmMessage
    {
        string Message { get; set; }
        string AcceptLabel { get; set; }
        string CancelLabel { get; set; }
        ButtonVariant AcceptVariant { get; set; }
        ButtonVariant CancelVariant { get; set; }
    }
}
