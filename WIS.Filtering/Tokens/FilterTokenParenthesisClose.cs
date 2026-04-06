using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Filtering.Tokens
{
    public class FilterTokenParenthesisClose : IFilterToken
    {
        public bool IsUnary { get; }
        public FilterTokenType TokenType { get; set; }

        public FilterTokenParenthesisClose()
        {
            this.IsUnary = false;
            this.TokenType = FilterTokenType.PARENTHESIS_CLOSE;
        }
    }
}
