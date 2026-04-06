using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using WIS.Security.Models;

namespace WIS.WebApplication.Models.Menu
{
    public class MenuItemParser : IMenuItemParser
    {
        private readonly IModuleEndpointResolver _endpointResolver;

        public MenuItemParser(IModuleEndpointResolver resolver)
        {
            this._endpointResolver = resolver;
        }

        public List<MenuItem> Parse(IEnumerable<XElement> elements, Usuario user)
        {
            List<MenuItem> listaRetorno = new List<MenuItem>();

            foreach (var element in elements)
            {
                var newItem = new MenuItem
                {
                    Id = (string)element.Attribute("id"),
                    Label = (string)element.Attribute("label"),
                    Icon = (string)element.Attribute("icon"),
                    Module = (string)element.Attribute("module"),
                    Url = (string)element.Attribute("url")
                };

                if (user.Permisos.Any(d => d.UniqueName == newItem.Id))
                {
                    var resolvedUrl = this._endpointResolver.ResolveUrl(newItem.Module, newItem.Url, user.SessionTokenWeb);

                    if (newItem.Url == resolvedUrl)
                        newItem.IsLocal = true;

                    newItem.Url = resolvedUrl;

                    var submenu = element.Element("submenu");

                    if (submenu != null)
                    {
                        var submenuItems = submenu.Elements();

                        if (submenuItems != null && submenuItems.Any())
                            newItem.SubmenuItems = this.Parse(submenuItems, user);
                    }

                    listaRetorno.Add(newItem);
                }
            }

            return listaRetorno;
        }
    }
}
