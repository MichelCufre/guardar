using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using WIS.Filtering.Expressions;

namespace WIS.Filtering.FilterExpressions
{
    public class FilterExpressionEqual : IFilterExpression
    {
        private readonly IExpressionService _expressionService;
        private IFilterExpression ExpressionLeft { get; set; }
        private IFilterExpression ExpressionRight { get; set; }

        public FilterExpressionEqual(IExpressionService expressionService, IFilterExpression expressionLeft, IFilterExpression expressionRight)
        {
            this._expressionService = expressionService;
            this.ExpressionLeft = expressionLeft;
            this.ExpressionRight = expressionRight;
        }

        public Expression Evaluate(FilterExpressionContext context)
        {
            return this.ResolveOperation(this.ExpressionLeft.Evaluate(context), this.ExpressionRight.Evaluate(context));
        }

        private Expression ResolveOperation(Expression expressionLeft, Expression expressionRight)
        {
            if (this.IsDateMemberExpression(expressionLeft))
                return this.ParseDateExpression(expressionLeft, expressionRight);

            if (this.IsNumericExpression(expressionLeft))
                return this.ParseNumericExpression(expressionLeft, expressionRight);

            return this.ParseExpression(expressionLeft, expressionRight);
        }

        private bool IsDateMemberExpression(Expression expression)
        {
            var type = expression.Type;

            return expression is MemberExpression && (type == typeof(DateTime) || type == typeof(DateTime?));
        }
        private bool IsNumericExpression(Expression expression)
        {
            Type type;

            if (expression is IExpressionPartialSearch partialSearch)
                type = Nullable.GetUnderlyingType(partialSearch.Content.Type);
            else
                type = Nullable.GetUnderlyingType(expression.Type);

            if (type == null)
                type = expression.Type;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        private Expression ParseDateExpression(Expression expressionLeft, Expression expressionRight)
        {
            if (expressionRight is ExpressionStartsWith dateStartsWith)
                return this._expressionService.DateTimeStartsWith(expressionLeft, dateStartsWith.Content);

            if (expressionRight is ExpressionEndsWith dateEndsWith)
                return this._expressionService.DateTimeEndsWith(expressionLeft, dateEndsWith.Content);

            if (expressionRight is ExpressionContains dateContains)
                return this._expressionService.DateTimeContains(expressionLeft, dateContains.Content);

            if (expressionRight is ExpressionStartsWithAndEndsWith dateStartsEndsWith)
                return Expression.MakeBinary(ExpressionType.And, this._expressionService.NumberStartsWith(expressionLeft, dateStartsEndsWith.Content), this._expressionService.NumberEndsWith(expressionLeft, dateStartsEndsWith.SecondaryContent));

            return this._expressionService.Equals(expressionLeft, expressionRight);
        }
        private Expression ParseNumericExpression(Expression expressionLeft, Expression expressionRight)
        {
            if (expressionRight is ExpressionStartsWith numberStartsWith)
                return this._expressionService.NumberStartsWith(expressionLeft, numberStartsWith.Content);

            if (expressionRight is ExpressionEndsWith numberEndsWith)
                return this._expressionService.NumberEndsWith(expressionLeft, numberEndsWith.Content);

            if (expressionRight is ExpressionContains numberContains)
                return this._expressionService.NumberContains(expressionLeft, numberContains.Content);

            if (expressionRight is ExpressionStartsWithAndEndsWith numberStartsEndsWith)
                return Expression.MakeBinary(ExpressionType.And, this._expressionService.NumberStartsWith(expressionLeft, numberStartsEndsWith.Content), this._expressionService.NumberEndsWith(expressionLeft, numberStartsEndsWith.SecondaryContent));

            return this._expressionService.Equals(expressionLeft, expressionRight);
        }
        private Expression ParseExpression(Expression expressionLeft, Expression expressionRight)
        {
            if (expressionRight is ExpressionStartsWith startsWith)
                return this._expressionService.StringStartsWith(expressionLeft, startsWith.Content);

            if (expressionRight is ExpressionEndsWith endsWith)
                return this._expressionService.StringEndsWith(expressionLeft, endsWith.Content);

            if (expressionRight is ExpressionContains contains)
                return this._expressionService.StringContains(expressionLeft, contains.Content);

            if (expressionRight is ExpressionStartsWithAndEndsWith startsEndsWith)
                return Expression.MakeBinary(ExpressionType.And, this._expressionService.NumberStartsWith(expressionLeft, startsEndsWith.Content), this._expressionService.NumberEndsWith(expressionLeft, startsEndsWith.SecondaryContent));

            return this._expressionService.Equals(expressionLeft, expressionRight);
        }
    }
}
