using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using WIS.Security.Models;

namespace WIS.WebApplication.Models.Menu
{
    public interface IMenuItemParser
    {
        List<MenuItem> Parse(IEnumerable<XElement> elements, Usuario user);
    }
}
