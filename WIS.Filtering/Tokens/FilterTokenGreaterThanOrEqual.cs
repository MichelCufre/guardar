using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Filtering.Tokens
{
    public class FilterTokenGreaterThanOrEqual : IFilterToken
    {
        public bool IsUnary { get; }
        public FilterTokenType TokenType { get; set; }

        public FilterTokenGreaterThanOrEqual()
        {
            this.IsUnary = false;
            this.TokenType = FilterTokenType.BINARY_GREATER_THAN_OR_EQUAL;
        }
    }
}
