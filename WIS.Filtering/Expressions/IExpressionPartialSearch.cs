using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace WIS.Filtering.Expressions
{
    public interface IExpressionPartialSearch
    {
        ConstantExpression Content { get; set; }
    }
}
