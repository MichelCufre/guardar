using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace WIS.Filtering.FilterExpressions
{
    public interface IFilterExpression
    {
        Expression Evaluate(FilterExpressionContext context);
    }
}
