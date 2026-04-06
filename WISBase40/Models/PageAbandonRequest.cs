using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WIS.WebApplication.Models
{
    public class PageAbandonRequest
    {
        public string Token { get; set; }
        public string Application { get; set; }
    }
}
