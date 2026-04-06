using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Filtering.Tokens
{
    public class FilterTokenBetween : IFilterToken, IFunctionFilterToken
    {
        public bool IsIterable { get; }
        public bool IsUnary { get; }
        public FilterTokenType TokenType { get; set; }

        public FilterTokenBetween()
        {
            this.IsIterable = true;
            this.IsUnary = false;
            this.TokenType = FilterTokenType.BINARY_OPERATION_BETWEEN;
        }
    }
}
