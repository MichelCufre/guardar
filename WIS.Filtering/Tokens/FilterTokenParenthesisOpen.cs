using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Filtering.Tokens
{
    public class FilterTokenParenthesisOpen : IFilterToken
    {
        public bool IsUnary { get; }
        public FilterTokenType TokenType { get; set; }

        public FilterTokenParenthesisOpen()
        {
            this.IsUnary = false;
            this.TokenType = FilterTokenType.PARENTHESIS_OPEN;
        }
    }
}
