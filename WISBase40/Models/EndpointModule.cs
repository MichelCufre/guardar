using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WIS.WebApplication.Models
{
    public class EndpointModule
    {
        public string Address { get; set; }
        public bool RequiresRedirect { get; set; }

        public EndpointModule(string address, bool requiresRedirect)
        {
            this.Address = address;
            this.RequiresRedirect = requiresRedirect;
        }
    }
}
