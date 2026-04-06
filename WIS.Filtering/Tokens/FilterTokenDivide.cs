using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Filtering.Tokens
{
    public class FilterTokenDivide : IFilterToken
    {
        public bool IsUnary { get; }
        public FilterTokenType TokenType { get; set; }

        public FilterTokenDivide()
        {
            this.IsUnary = false;
            this.TokenType = FilterTokenType.BINARY_DIVIDE;
        }
    }
}
