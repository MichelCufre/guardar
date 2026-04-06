using System;

namespace WIS.Components.Common
{
    public class ComponentParameter
    {
        public string Id { get; set; }
        public string Value { get; set; }

        public ComponentParameter()
        {

        }
        public ComponentParameter(string id, string value)
        {
            this.Id = id;
            this.Value = value;
        }
    }
}
