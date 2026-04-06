using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.TrafficOfficer.Configuration
{
    public class TrafficOfficerSettings
    {
        public const string Position = "TrafficOfficerSettings";

        public string Endpoint { get; set; }
        public string SystemName { get; set; }
    }
}
