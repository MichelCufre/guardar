using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Components.Common.Notification;

namespace WIS.Components.Common
{
    public abstract class ComponentContext
    {
        public List<ComponentParameter> Parameters { get; set; }
        public List<ApplicationNotification> Notifications { get; set; }

        public ComponentContext()
        {
            this.Parameters = new List<ComponentParameter>();
            this.Notifications = new List<ApplicationNotification>();
        }

        public string GetParameter(string parameterId)
        {
            return this.Parameters.Where(d => d.Id == parameterId ).FirstOrDefault()?.Value;
        }
        public void AddParameter(string parameterId, string value)
        {
            this.Parameters.Add(new ComponentParameter(parameterId, value));
        }
        public void AddOrUpdateParameter(string parameterId, string value)
        {
            var parameter = this.Parameters.FirstOrDefault(d => d.Id == parameterId);

            if (parameter == null)
                this.Parameters.Add(new ComponentParameter(parameterId, value));
            else
                parameter.Value = value;

        }
        public bool HasParameter(string parameterId)
        {
            return this.Parameters.Any(d => d.Id == parameterId);
        }

        public void AddSuccessNotification(string message)
        {
            this.Notifications.Add(new ApplicationNotification(ApplicationNotificationType.Success, message));
        }
        public void AddSuccessNotification(string message, List<string> arguments)
        {
            this.Notifications.Add(new ApplicationNotification(ApplicationNotificationType.Success, message, arguments));
        }
        public void AddErrorNotification(string message)
        {
            this.Notifications.Add(new ApplicationNotification(ApplicationNotificationType.Error, message));
        }
        public void AddErrorNotification(string message, List<string> arguments)
        {
            this.Notifications.Add(new ApplicationNotification(ApplicationNotificationType.Error, message, arguments));
        }
        public void AddInfoNotification(string message)
        {
            this.Notifications.Add(new ApplicationNotification(ApplicationNotificationType.Info, message));
        }
        public void AddInfoNotification(string message, List<string> arguments)
        {
            this.Notifications.Add(new ApplicationNotification(ApplicationNotificationType.Info, message, arguments));
        }
        public void AddWarningNotification(string message)
        {
            this.Notifications.Add(new ApplicationNotification(ApplicationNotificationType.Warning, message));
        }
        public void AddWarningNotification(string message, List<string> arguments)
        {
            this.Notifications.Add(new ApplicationNotification(ApplicationNotificationType.Warning, message, arguments));
        }
    }
}
