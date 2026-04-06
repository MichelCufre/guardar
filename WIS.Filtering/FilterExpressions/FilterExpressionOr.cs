using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using WIS.Filtering.Expressions;

namespace WIS.Filtering.FilterExpressions
{
    public class FilterExpressionOr : IFilterExpression
    {
        private readonly IExpressionService _expressionService;
        private IFilterExpression ExpressionLeft { get; set; }
        private IFilterExpression ExpressionRight { get; set; }

        public FilterExpressionOr(IExpressionService expressionService, IFilterExpression expressionLeft, IFilterExpression expressionRight)
        {
            this._expressionService = expressionService;
            this.ExpressionLeft = expressionLeft;
            this.ExpressionRight = expressionRight;
        }

        public Expression Evaluate(FilterExpressionContext context)
        {
            return Expression.MakeBinary(ExpressionType.Or, this.ExpressionLeft.Evaluate(context), this.ExpressionRight.Evaluate(context));
        }
    }
}
