using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Filtering.Tokens
{
    public class FilterTokenGreaterThan : IFilterToken
    {
        public bool IsUnary { get; }
        public FilterTokenType TokenType { get; set; }

        public FilterTokenGreaterThan()
        {
            this.IsUnary = false;
            this.TokenType = FilterTokenType.BINARY_GREATER_THAN;
        }
    }
}
