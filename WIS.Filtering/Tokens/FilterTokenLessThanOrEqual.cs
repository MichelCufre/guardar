using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Filtering.Tokens
{
    public class FilterTokenLessThanOrEqual : IFilterToken
    {
        public bool IsUnary { get; }
        public FilterTokenType TokenType { get; set; }

        public FilterTokenLessThanOrEqual()
        {
            this.IsUnary = false;
            this.TokenType = FilterTokenType.BINARY_LESS_THAN_OR_EQUAL;
        }
    }
}
