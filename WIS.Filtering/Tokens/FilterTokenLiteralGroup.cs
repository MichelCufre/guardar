using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Filtering.Tokens
{
    public class FilterTokenLiteralGroup : IFilterToken
    {
        public bool IsUnary { get; }
        public FilterTokenType TokenType { get; set; }
        public List<string> Values { get; set; }

        public FilterTokenLiteralGroup(List<string> values)
        {
            this.Values = values;
            this.IsUnary = false;
            this.TokenType = FilterTokenType.LITERAL;
        }
    }
}
