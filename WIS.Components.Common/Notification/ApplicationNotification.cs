using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common.Notification;

namespace WIS.Components.Common
{
    public class ApplicationNotification
    {
        public ApplicationNotificationType Type { get; set; }
        public string Message { get; set; }
        public List<string> Arguments { get; set; }

        public ApplicationNotification()
        {
        }
        public ApplicationNotification(ApplicationNotificationType type, string message)
        {
            this.Type = type;
            this.Message = message;
        }
        public ApplicationNotification(ApplicationNotificationType type, string message, List<string> arguments)
        {
            this.Type = type;
            this.Message = message;
            this.Arguments = arguments;
        }
    }
}
