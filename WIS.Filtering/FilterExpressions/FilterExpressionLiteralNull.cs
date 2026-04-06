using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using WIS.Filtering.Expressions;

namespace WIS.Filtering.FilterExpressions
{
    public class FilterExpressionLiteralNull : IFilterExpression
    {
        private readonly IExpressionService _expressionService;

        public FilterExpressionLiteralNull(IExpressionService expressionService)
        {
            this._expressionService = expressionService;
        }

        public Expression Evaluate(FilterExpressionContext context)
        {
            return Expression.Constant(null, typeof(string));
        }
    }
}
