using WIS.Common.Attributes;

namespace WIS.Common.API.Attributes
{
    public class ApiDtoExampleAttribute : TextAttribute
    {
        public ApiDtoExampleAttribute(string value)
            : base(value)
        {
        }
    }
}
