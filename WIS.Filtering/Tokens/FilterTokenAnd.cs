using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Filtering.Tokens
{
    public class FilterTokenAnd : IFilterToken
    {
        public FilterTokenType TokenType { get; set; }
        public bool IsUnary { get; }

        public FilterTokenAnd()
        {
            this.IsUnary = false;
            this.TokenType = FilterTokenType.BINARY_AND;
        }
    }
}
