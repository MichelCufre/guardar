using Microsoft.Extensions.Options;
using System.Collections.Generic;
using WIS.Configuration;

namespace WIS.WebApplication.Models
{
    public class ModuleEndpointProvider : IModuleEndpointProvider
    {
        private readonly ModuleUrl _modules;

        public ModuleEndpointProvider(IOptions<ModuleUrl> options)
        {
            this._modules = options.Value;
        }

        public Dictionary<string, EndpointModule> GetModules()
        {
            return new Dictionary<string, EndpointModule>
            {
            };
        }
    }
}
