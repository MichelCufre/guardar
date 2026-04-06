namespace WIS.XmlData.WInterface.Models
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class StringEnumValueAttribute : Attribute
    {
        private string _value;

        public string Value
        {
            get
            {
                return this._value;
            }
        }

        public StringEnumValueAttribute(string value)
        {
            this._value = value;
        }
    }
}
