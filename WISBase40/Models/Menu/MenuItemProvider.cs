using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using WIS.Configuration;
using WIS.Security.Models;

namespace WIS.WebApplication.Models.Menu
{
    public class MenuItemProvider : IMenuItemProvider
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IMenuItemParser _parser;
        private readonly ApplicationSettings _settings;

        public MenuItemProvider(IWebHostEnvironment environment, IMenuItemParser parser, IOptions<ApplicationSettings> options)
        {
            this._environment = environment;
            this._parser = parser;
            this._settings = options.Value;
        }
        
        public List<MenuItem> GetItems(Usuario user)
        {
            var xml = XDocument.Load(Path.Combine(this._environment.ContentRootPath, this._settings.MenuConfigPath));

            return this._parser.Parse(xml.Root.Elements(), user);
        }
    }
}
