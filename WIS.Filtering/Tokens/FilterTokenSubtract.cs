using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Filtering.Tokens
{
    public class FilterTokenSubtract : IFilterToken
    {
        public bool IsUnary { get; }
        public FilterTokenType TokenType { get; set; }

        public FilterTokenSubtract()
        {
            this.IsUnary = false;
            this.TokenType = FilterTokenType.BINARY_SUBTRACT;
        }
    }
}
