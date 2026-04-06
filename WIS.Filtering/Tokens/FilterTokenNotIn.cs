namespace WIS.Filtering.Tokens
{
    public class FilterTokenNotIn : IFilterToken, IFunctionFilterToken
    {
        public bool IsIterable { get; }
        public bool IsUnary { get; }
        public FilterTokenType TokenType { get; set; }

        public FilterTokenNotIn()
        {
            this.IsIterable = false;
            this.IsUnary = false;
            this.TokenType = FilterTokenType.BINARY_OPERATION_NOT_IN;
        }
    }
}
