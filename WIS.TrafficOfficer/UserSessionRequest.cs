using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.TrafficOfficer
{
    public class UserSessionRequest
    {
        public int? UserId { get; set; }
        public string UserLogin { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
    }
}
