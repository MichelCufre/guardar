using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WIS.WebApplication.Models
{
    public interface IModuleEndpointResolver
    {
        string ResolveUrl(string module, string url, string token);
    }
}
