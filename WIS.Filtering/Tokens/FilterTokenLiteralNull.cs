using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Filtering.Tokens
{
    public class FilterTokenLiteralNull : IFilterToken
    {
        public bool IsUnary { get; }
        public FilterTokenType TokenType { get; set; }

        public FilterTokenLiteralNull()
        {
            this.IsUnary = false;
            this.TokenType = FilterTokenType.LITERAL;
        }
    }
}
