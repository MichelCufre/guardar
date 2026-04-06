using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General.API.Dtos
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SwaggerRequestType : Attribute
    {
        public Type Type { get; }

        public SwaggerRequestType(Type requestType)
        {
            Type = requestType;
        }
    }
}
