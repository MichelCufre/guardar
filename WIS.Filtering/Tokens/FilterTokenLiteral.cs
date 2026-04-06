using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Filtering.Tokens
{
    public class FilterTokenLiteral : IFilterToken
    {
        public bool IsUnary { get; }
        public FilterTokenType TokenType { get; set; }
        public string Value { get; set; }

        public FilterTokenLiteral(string value)
        {
            this.IsUnary = false;
            this.Value = value;
            this.TokenType = FilterTokenType.LITERAL;
        }
    }
}
