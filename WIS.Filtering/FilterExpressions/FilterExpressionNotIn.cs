using System.Linq.Expressions;
using WIS.Filtering.Expressions;

namespace WIS.Filtering.FilterExpressions
{
    public class FilterExpressionNotIn : IFilterExpression
    {
        private readonly IExpressionService _expressionService;
        private IFilterExpression ExpressionLeft { get; set; }
        private IFilterExpression ExpressionRight { get; set; }

        public FilterExpressionNotIn(IExpressionService expressionService, IFilterExpression expressionLeft, IFilterExpression expressionRight)
        {
            this._expressionService = expressionService;
            this.ExpressionLeft = expressionLeft;
            this.ExpressionRight = expressionRight;
        }

        public Expression Evaluate(FilterExpressionContext context)
        {
            return Expression.Not(this._expressionService.In(this.ExpressionLeft.Evaluate(context), this.ExpressionRight.Evaluate(context)));
        }
    }
}
