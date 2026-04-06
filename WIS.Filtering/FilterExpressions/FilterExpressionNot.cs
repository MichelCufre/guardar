using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using WIS.Filtering.Expressions;

namespace WIS.Filtering.FilterExpressions
{
    public class FilterExpressionNot : IFilterExpression
    {
        private readonly IExpressionService _expressionService;
        private IFilterExpression ExpressionLeft { get; set; }

        public FilterExpressionNot(IExpressionService expressionService, IFilterExpression expression)
        {
            this._expressionService = expressionService;
            this.ExpressionLeft = expression;
        }

        public Expression Evaluate(FilterExpressionContext context)
        {
            return Expression.Not(this.ExpressionLeft.Evaluate(context));
        }
    }
}
