using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Components.Common.Select
{
    public class SelectOption
    {
        public string Label { get; set; }
        public string Value { get; set; }

        public SelectOption()
        {
        }
        public SelectOption(string value, string label)
        {
            this.Label = label;
            this.Value = value;
        }
    }
}
