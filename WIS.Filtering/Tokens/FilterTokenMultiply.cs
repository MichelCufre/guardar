using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Filtering.Tokens
{
    public class FilterTokenMultiply : IFilterToken
    {
        public bool IsUnary { get; }
        public FilterTokenType TokenType { get; set; }

        public FilterTokenMultiply()
        {
            this.IsUnary = false;
            this.TokenType = FilterTokenType.BINARY_MULTIPLY;
        }
    }
}
