using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using WIS.Filtering.Expressions;

namespace WIS.Filtering.FilterExpressions
{
    public class FilterExpressionLiteralGroup : IFilterExpression
    {
        private readonly IExpressionService _expressionService;

        private List<string> Values { get; set; }

        public FilterExpressionLiteralGroup(IExpressionService expressionService, List<string> values)
        {
            this.Values = values;
            this._expressionService = expressionService;
        }

        public Expression Evaluate(FilterExpressionContext context)
        {
            return Expression.Constant(this.Values, typeof(List<string>));
        }

        public IEnumerable<Expression> EvaluateNextIndividual()
        {
            foreach (var value in this.Values)
            {
                yield return Expression.Constant(value, typeof(string));
            }
        }
    }
}
