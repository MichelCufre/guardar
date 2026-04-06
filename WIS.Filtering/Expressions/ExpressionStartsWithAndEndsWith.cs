using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace WIS.Filtering.Expressions
{
    public class ExpressionStartsWithAndEndsWith : Expression, IExpressionPartialSearch
    {
        public ConstantExpression Content { get; set; }
        public ConstantExpression SecondaryContent { get; set; }

        public ExpressionStartsWithAndEndsWith(ConstantExpression expression)
        {
            this.Content = expression;
        }

        public ExpressionStartsWithAndEndsWith(ConstantExpression expression, ConstantExpression secondaryExpression)
        {
            this.Content = expression;
            this.SecondaryContent = secondaryExpression;
        }
    }
}
