using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Filtering.Tokens
{
    public class FilterTokenLiteralGroupIterable : IFilterToken
    {
        public bool IsUnary { get; }
        public FilterTokenType TokenType { get; set; }
        public List<string> Values { get; set; }

        public FilterTokenLiteralGroupIterable(List<string> values)
        {
            this.Values = values;
            this.IsUnary = false;
            this.TokenType = FilterTokenType.LITERAL;
        }
    }
}
