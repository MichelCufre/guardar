using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Components.Common.Redirection
{
    public class PageRedirection
    {
        public string Url { get; set; }
        public string Module { get; set; }
        public bool NewTab { get; set; }
        public List<ComponentParameter> Parameters { get; set; }
    }
}
