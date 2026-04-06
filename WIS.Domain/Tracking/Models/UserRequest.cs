using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Tracking.Models
{
    public class UserRequest
    {
        public int UserId { get; set; }
        public string LoginName { get; set; }
        public string DomainName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Tenant { get; set; }
    }
}
