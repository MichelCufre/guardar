using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using WIS.Filtering.Expressions;

namespace WIS.Filtering.FilterExpressions
{
    public class FilterExpressionLiteralGroupIterable : IFilterExpression
    {
        private readonly IExpressionService _expressionService;

        private int Index { get; set; }
        private List<string> Values { get; set; }

        public FilterExpressionLiteralGroupIterable(IExpressionService expressionService, List<string> values)
        {
            this.Values = values;
            this.Index = -1;
            this._expressionService = expressionService;
        }

        public Expression Evaluate(FilterExpressionContext context)
        {
            this.MoveNext();

            return Expression.Constant(this.Values[this.Index], typeof(string));
        }

        private void MoveNext()
        {
            if(this.Index < this.Values.Count - 1)
                this.Index++;
        }
    }
}
