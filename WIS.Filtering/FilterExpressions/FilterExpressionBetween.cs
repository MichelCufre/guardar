using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using WIS.Filtering.Expressions;

namespace WIS.Filtering.FilterExpressions
{
    public class FilterExpressionBetween : IFilterExpression
    {
        private readonly IExpressionService _expressionService;
        private IFilterExpression ExpressionLeft { get; set; }
        private IFilterExpression ExpressionRight { get; set; }

        public FilterExpressionBetween(IExpressionService expressionService, IFilterExpression expressionLeft, IFilterExpression expressionRight)
        {
            this._expressionService = expressionService;
            this.ExpressionLeft = expressionLeft;
            this.ExpressionRight = expressionRight;
        }

        public Expression Evaluate(FilterExpressionContext context)
        {
            return Expression.MakeBinary(ExpressionType.And, this._expressionService.GreaterThanOrEqual(this.ExpressionLeft.Evaluate(context), this.ExpressionRight.Evaluate(context)), this._expressionService.LessThanOrEqual(this.ExpressionLeft.Evaluate(context), this.ExpressionRight.Evaluate(context)));
        }
    }
}
