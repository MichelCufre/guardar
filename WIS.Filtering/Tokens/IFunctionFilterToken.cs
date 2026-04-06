using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Filtering.Tokens
{
    public interface IFunctionFilterToken : IFilterToken
    {
        bool IsIterable { get; }
    }
}
