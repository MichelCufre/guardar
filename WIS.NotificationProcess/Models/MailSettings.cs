using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.NotificationProcess.Models
{
    public class MailSettings
    {
        public const string Position = "MailSettings";
        public string SenderName { get; set; }
        public string SenderAddress { get; set; }
        public string SmtpUser { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpAddress { get; set; }
        public int? SmtpPort { get; set; }
        public string MutexId { get; set; }
        public int? MutexTimeout { get; set; }
        public bool EnableSsl { get; set; }
        public bool UseDefaultCredentials { get; set; }
    }
}
