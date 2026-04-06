using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using WIS.Exceptions;
using WIS.Filtering.Expressions;

namespace WIS.Filtering.FilterExpressions
{
    public class FilterExpressionLiteral : IFilterExpression
    {
        private readonly IExpressionService _expressionService;

        private string Value { get; set; }

        public FilterExpressionLiteral(IExpressionService expressionService, string value)
        {
            this.Value = value;
            this._expressionService = expressionService;
        }

        public Expression Evaluate(FilterExpressionContext context)
        {
            var prefix = this.Value.Substring(0, 1);

            //Es una referencia a un campo de la base
            if (prefix == ":")
                return this.ProcessDatabaseLiteral(context);

            return this.ProcessLiteral();
        }

        private MemberExpression ProcessDatabaseLiteral(FilterExpressionContext context)
        {
            var columnName = this.Value.Substring(1).ToUpper();

            var columnParam = context.GetType("type");

            var propertyExp = Expression.Property(columnParam, columnName);

            return propertyExp;
        }
        private Expression ProcessLiteral()
        {
            //%cosa             - EndsWith
            //cosa%             - StartsWith
            //cosa%otracosa     - StartsWith and EndsWith
            //%cosa%            - Contains

            bool hasPrefix = this.Value.Substring(0, 1) == "%";
            bool hasSuffix = this.Value.Substring(this.Value.Length - 1, 1) == "%";

            var firstMatch = this.Value.IndexOf("%");

            if (firstMatch > -1 && firstMatch > 0 && firstMatch < this.Value.Length - 1)
            {
                if (this.Value.Split('%').Length > 2)
                    throw new InvalidFilterException("Filtro no puede contener mas de un comodin dentro del valor a buscar"); //TODO: Crear nuevo tipo de excepciones

                var startValue = this.Value.Substring(0, firstMatch);
                var endValue = this.Value.Substring(firstMatch + 1);

                return new ExpressionStartsWithAndEndsWith(Expression.Constant(startValue.Trim('%'), typeof(string)), Expression.Constant(endValue.Trim('%'), typeof(string)));
            }

            var expressionValue = Expression.Constant(this.Value.Trim('%'), typeof(string));

            if (hasPrefix && !hasSuffix)
                return new ExpressionEndsWith(expressionValue);

            if (!hasPrefix && hasSuffix)
                return new ExpressionStartsWith(expressionValue);

            if (hasPrefix && hasSuffix)
                return new ExpressionContains(expressionValue);

            return expressionValue;
        }
    }
}
