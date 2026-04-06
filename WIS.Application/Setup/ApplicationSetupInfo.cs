using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Application.Setup
{
    public class ApplicationSetupInfo
    {
        public string Application { get; set; }
        public int User { get; set; }
        public string Predio { get; set; }
        public string Token { get; set; }
        public Dictionary<string, object> Session { get; set; }
    }
}
