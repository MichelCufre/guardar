using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WIS.WebApplication.Models.Menu
{
    public class MenuItem
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public string Module { get; set; }
        public bool Visible { get; set; }
        public bool IsLocal { get; set; }
        public List<MenuItem> SubmenuItems { get; set; }

        public MenuItem()
        {
            this.Visible = true;
            this.IsLocal = false;
        }
    }
}
