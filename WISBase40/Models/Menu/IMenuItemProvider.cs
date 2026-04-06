using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Security.Models;

namespace WIS.WebApplication.Models.Menu
{
    public interface IMenuItemProvider
    {
        List<MenuItem> GetItems(Usuario user);
    }
}
