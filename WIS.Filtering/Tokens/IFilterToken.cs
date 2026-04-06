using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Filtering.Tokens
{
    public interface IFilterToken
    {
        FilterTokenType TokenType { get; set; }
        bool IsUnary { get; }
    }
}
