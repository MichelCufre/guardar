using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Filtering.Tokens
{
    public class FilterTokenAdd : IFilterToken
    {
        public FilterTokenType TokenType { get; set; }
        public bool IsUnary { get; }

        public FilterTokenAdd()
        {
            this.TokenType = FilterTokenType.BINARY_ADD;
            this.IsUnary = false;
        }
    }
}
