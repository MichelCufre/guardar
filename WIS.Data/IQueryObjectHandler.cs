using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Data
{
    public interface IQueryObjectHandler<ContextDataType>
    {
        void HandleQuery<T>(IQueryObject<T, ContextDataType> query);
    }
}
