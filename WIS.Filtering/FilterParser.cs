using System;
using System.Collections.Generic;
using System.Text;
using WIS.Filtering.Expressions;
using WIS.Filtering.FilterExpressions;
using WIS.Filtering.Tokens;

namespace WIS.Filtering
{
    public class FilterParser : IFilterParser
    {
        private readonly IExpressionService _expressionService;
        private readonly IFilterTokenizer _tokenizer;

        public FilterParser(IExpressionService expressionService, IFilterTokenizer tokenizer)
        {
            this._tokenizer = tokenizer;
            this._expressionService = expressionService;
        }

        public IFilterExpression Parse(string value)
        {
            this._tokenizer.Tokenize(value);

            if (!this._tokenizer.IsValid())
                throw new InvalidOperationException("General_Sec0_Error_Error06");

            var filterList = this._tokenizer.GetPolishNotation().GetEnumerator();

            return this.ParseRecursive(ref filterList);
        }

        public IFilterExpression ParseRecursive(ref List<IFilterToken>.Enumerator tokenEnumerator)
        {
            IFilterExpression operandLeft;
            IFilterExpression operandRight;

            tokenEnumerator.MoveNext();

            switch (tokenEnumerator.Current.TokenType)
            {
                case FilterTokenType.LITERAL:                    
                    return this.ParseLiteral(tokenEnumerator.Current);
                case FilterTokenType.UNARY_NOT:
                    operandLeft = this.ParseRecursive(ref tokenEnumerator);
                    return new FilterExpressionNot(this._expressionService, operandLeft);
                case FilterTokenType.BINARY_AND:
                    operandLeft = this.ParseRecursive(ref tokenEnumerator);
                    operandRight = this.ParseRecursive(ref tokenEnumerator);
                    return new FilterExpressionAnd(this._expressionService, operandLeft, operandRight);
                case FilterTokenType.BINARY_OR:
                    operandLeft = this.ParseRecursive(ref tokenEnumerator);
                    operandRight = this.ParseRecursive(ref tokenEnumerator);
                    return new FilterExpressionOr(this._expressionService, operandLeft, operandRight);
                case FilterTokenType.BINARY_EQUAL:
                    operandLeft = this.ParseRecursive(ref tokenEnumerator);
                    operandRight = this.ParseRecursive(ref tokenEnumerator);
                    return new FilterExpressionEqual(this._expressionService, operandLeft, operandRight);
                case FilterTokenType.BINARY_GREATER_THAN:
                    operandLeft = this.ParseRecursive(ref tokenEnumerator);
                    operandRight = this.ParseRecursive(ref tokenEnumerator);
                    return new FilterExpressionGreaterThan(this._expressionService, operandLeft, operandRight);
                case FilterTokenType.BINARY_GREATER_THAN_OR_EQUAL:
                    operandLeft = this.ParseRecursive(ref tokenEnumerator);
                    operandRight = this.ParseRecursive(ref tokenEnumerator);
                    return new FilterExpressionGreaterThanOrEqual(this._expressionService, operandLeft, operandRight);
                case FilterTokenType.BINARY_LESS_THAN:
                    operandLeft = this.ParseRecursive(ref tokenEnumerator);
                    operandRight = this.ParseRecursive(ref tokenEnumerator);
                    return new FilterExpressionLessThan(this._expressionService, operandLeft, operandRight);
                case FilterTokenType.BINARY_LESS_THAN_OR_EQUAL:
                    operandLeft = this.ParseRecursive(ref tokenEnumerator);
                    operandRight = this.ParseRecursive(ref tokenEnumerator);
                    return new FilterExpressionLessThanOrEqual(this._expressionService, operandLeft, operandRight);
                case FilterTokenType.BINARY_ADD:
                    operandLeft = this.ParseRecursive(ref tokenEnumerator);
                    operandRight = this.ParseRecursive(ref tokenEnumerator);
                    return new FilterExpressionAdd(this._expressionService, operandLeft, operandRight);
                case FilterTokenType.BINARY_SUBTRACT:
                    operandLeft = this.ParseRecursive(ref tokenEnumerator);
                    operandRight = this.ParseRecursive(ref tokenEnumerator);
                    return new FilterExpressionSubtract(this._expressionService, operandLeft, operandRight);
                case FilterTokenType.BINARY_MULTIPLY:
                    operandLeft = this.ParseRecursive(ref tokenEnumerator);
                    operandRight = this.ParseRecursive(ref tokenEnumerator);
                    return new FilterExpressionMultiply(this._expressionService, operandLeft, operandRight);
                case FilterTokenType.BINARY_DIVIDE:
                    operandLeft = this.ParseRecursive(ref tokenEnumerator);
                    operandRight = this.ParseRecursive(ref tokenEnumerator);
                    return new FilterExpressionDivide(this._expressionService, operandLeft, operandRight);
                case FilterTokenType.BINARY_OPERATION_IN:
                    operandLeft = this.ParseRecursive(ref tokenEnumerator);
                    operandRight = this.ParseRecursive(ref tokenEnumerator);
                    return new FilterExpressionIn(this._expressionService, operandLeft, operandRight);
                case FilterTokenType.BINARY_OPERATION_NOT_IN:
                    operandLeft = this.ParseRecursive(ref tokenEnumerator);
                    operandRight = this.ParseRecursive(ref tokenEnumerator);
                    return new FilterExpressionNotIn(this._expressionService, operandLeft, operandRight);
                case FilterTokenType.BINARY_OPERATION_BETWEEN:
                    operandLeft = this.ParseRecursive(ref tokenEnumerator);
                    operandRight = this.ParseRecursive(ref tokenEnumerator);
                    return new FilterExpressionBetween(this._expressionService, operandLeft, operandRight);
                case FilterTokenType.UNARY_NEGATE:
                    operandLeft = this.ParseRecursive(ref tokenEnumerator);
                    return new FilterExpressionNegate(this._expressionService, operandLeft);
            }

            return null;
        }

        public IFilterExpression ParseLiteral(IFilterToken token)
        {
            if (token is FilterTokenLiteral tokenLiteral)
                return new FilterExpressionLiteral(this._expressionService, tokenLiteral.Value);

            if (token is FilterTokenLiteralNull)
                return new FilterExpressionLiteralNull(this._expressionService);

            if (token is FilterTokenLiteralGroup tokenGroup)
                return new FilterExpressionLiteralGroup(this._expressionService, tokenGroup.Values);

            if (token is FilterTokenLiteralGroupIterable tokenGroupIterable)
                return new FilterExpressionLiteralGroupIterable(this._expressionService, tokenGroupIterable.Values);

            return null;
        }
    }
}
