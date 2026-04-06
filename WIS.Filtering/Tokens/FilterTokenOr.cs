using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Filtering.Tokens
{
    public class FilterTokenOr : IFilterToken
    {
        public bool IsUnary { get; }
        public FilterTokenType TokenType { get; set; }

        public FilterTokenOr()
        {
            this.IsUnary = false;
            this.TokenType = FilterTokenType.BINARY_OR;
        }
    }
}
