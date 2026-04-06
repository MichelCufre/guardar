using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WIS.AuthenticationService.Model
{
    public class AppSettings
    {
        public const string Position = "AppSettings";
        public string ServiceUrl { get; set; }
        public string Secret { get; set; }
    }
}
