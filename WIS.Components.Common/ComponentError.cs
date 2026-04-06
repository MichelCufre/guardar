using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Components.Common
{
    public class ComponentError
    {
        public string Message { get; set; }
        public List<string> Arguments { get; set; }

        public ComponentError(string message, List<string> arguments)
        {
            this.Message = message;
            this.Arguments = arguments;
        }
    }
}
