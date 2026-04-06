using System;

namespace WIS.Common.Attributes
{
    public class TextAttribute : Attribute
    {
        public string Value { get; set; }
        public TextAttribute(string value)
        {
            Value = value;
        }
    }
}
