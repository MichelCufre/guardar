using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;

namespace WIS.WebApplication.Models
{
    public class ModuleEndpointResolver : IModuleEndpointResolver
    {
        private readonly IModuleEndpointProvider _moduleProvider;

        public ModuleEndpointResolver(IModuleEndpointProvider moduleProvider)
        {
            this._moduleProvider = moduleProvider;
        }

        public string ResolveUrl(string module, string url, string token)
        {
            Dictionary<string, EndpointModule> modules = this._moduleProvider.GetModules();

            if (!string.IsNullOrEmpty(module) && modules.ContainsKey(module))
            {
                if (modules[module].RequiresRedirect)
                {
                    return new Uri(new Uri(modules[module].Address), $"/Redirect/RedirectToApp?token={WebUtility.UrlEncode(token)}&page={url}").AbsoluteUri;
                }

                return new Uri(new Uri(modules[module].Address), url.TrimStart('/')) + "?t=" + WebUtility.UrlEncode(token);
            }

            return url;
        }
    }
}
