using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Filtering.Tokens
{
    public class FilterTokenNegate : IFilterToken
    {
        public bool IsUnary { get; }
        public FilterTokenType TokenType { get; set; }

        public FilterTokenNegate()
        {
            this.IsUnary = true;
            this.TokenType = FilterTokenType.UNARY_NEGATE;
        }
    }
}
