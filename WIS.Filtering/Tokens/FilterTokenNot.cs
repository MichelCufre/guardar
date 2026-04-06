using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Filtering.Tokens
{
    public class FilterTokenNot : IFilterToken
    {
        public bool IsUnary { get; }
        public FilterTokenType TokenType { get; set; }

        public FilterTokenNot()
        {
            this.IsUnary = true;
            this.TokenType = FilterTokenType.UNARY_NOT;
        }
    }
}
