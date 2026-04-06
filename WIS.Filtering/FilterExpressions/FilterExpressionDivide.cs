using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using WIS.Filtering.Expressions;

namespace WIS.Filtering.FilterExpressions
{
    public class FilterExpressionDivide : IFilterExpression
    {
        private readonly IExpressionService _expressionService;
        private IFilterExpression ExpressionLeft { get; set; }
        private IFilterExpression ExpressionRight { get; set; }

        public FilterExpressionDivide(IExpressionService expressionService, IFilterExpression expressionLeft, IFilterExpression expressionRight)
        {
            this._expressionService = expressionService;
            this.ExpressionLeft = expressionLeft;
            this.ExpressionRight = expressionRight;
        }

        public Expression Evaluate(FilterExpressionContext context)
        {
            var method = typeof(decimal).GetMethod("Parse", new[] { typeof(string) });

            Expression expLeft = Expression.Call(method, this.ExpressionLeft.Evaluate(context));
            Expression expRight = Expression.Call(method, this.ExpressionRight.Evaluate(context));

            return Expression.MakeBinary(ExpressionType.Divide, expLeft, expRight);
        }
    }
}
