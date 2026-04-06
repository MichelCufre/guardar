using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using WIS.Filtering.Expressions;

namespace WIS.Filtering.FilterExpressions
{
    public class FilterExpressionNegate : IFilterExpression
    {
        private readonly IExpressionService _expressionService;
        private IFilterExpression ExpressionLeft { get; set; }

        public FilterExpressionNegate(IExpressionService expressionService, IFilterExpression expression)
        {
            this._expressionService = expressionService;
            this.ExpressionLeft = expression;
        }

        public Expression Evaluate(FilterExpressionContext context)
        {
            var method = typeof(decimal).GetMethod("Parse", new[] { typeof(string) });

            Expression expLeft = Expression.Call(method, this.ExpressionLeft.Evaluate(context));

            return Expression.Negate(expLeft);
        }
    }
}
