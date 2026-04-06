using System;

namespace WIS.Domain.General.Filters
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SwaggerIgnoreAttribute : Attribute
    {
    }
}
