using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace WIS.Filtering.Expressions
{
    public class ExpressionEndsWith : Expression, IExpressionPartialSearch
    {
        public ConstantExpression Content { get; set; }

        public ExpressionEndsWith(ConstantExpression expression)
        {
            this.Content = expression;
        }
    }
}
