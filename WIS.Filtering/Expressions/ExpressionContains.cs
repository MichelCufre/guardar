using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace WIS.Filtering.Expressions
{
    public class ExpressionContains : Expression, IExpressionPartialSearch
    {
        public ConstantExpression Content { get; set; }

        public ExpressionContains(ConstantExpression expression)
        {
            this.Content = expression;
        }
    }
}
