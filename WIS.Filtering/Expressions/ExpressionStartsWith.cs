using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace WIS.Filtering.Expressions
{
    public class ExpressionStartsWith : Expression, IExpressionPartialSearch
    {
        public ConstantExpression Content { get; set; }

        public ExpressionStartsWith(ConstantExpression expression)
        {
            this.Content = expression;
        }
    }
}
