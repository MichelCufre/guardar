using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace WIS.Filtering.Expressions
{
	public class ExpressionService : IExpressionService
    {
        private readonly IExpressionLiteralConverter _converter;

        public ExpressionService(IExpressionLiteralConverter converter)
        {
            this._converter = converter;
        }

        public Expression StringContains(Expression paramExpLeft, Expression paramExpRight)
        {
            MethodInfo methodContains = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });

            MethodCallExpression paramExpLeftLower = Expression.Call(paramExpLeft, "ToLower", null);
            MethodCallExpression paramExpRightLower = Expression.Call(paramExpRight, "ToLower", null);

            return Expression.Call(paramExpLeftLower, methodContains, paramExpRightLower);
        }
        public Expression StringStartsWith(Expression paramExpLeft, Expression paramExpRight)
        {
            MethodInfo methodStartsWith = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });

            MethodCallExpression paramExpLeftLower = Expression.Call(paramExpLeft, "ToLower", null);
            MethodCallExpression paramExpRightLower = Expression.Call(paramExpRight, "ToLower", null);

            return Expression.Call(paramExpLeftLower, methodStartsWith, paramExpRightLower);
        }
        public Expression StringEndsWith(Expression paramExpLeft, Expression paramExpRight)
        {
            MethodInfo methodEndsWith = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });

            MethodCallExpression paramExpLeftLower = Expression.Call(paramExpLeft, "ToLower", null);
            MethodCallExpression paramExpRightLower = Expression.Call(paramExpRight, "ToLower", null);

            return Expression.Call(paramExpLeftLower, methodEndsWith, paramExpRightLower);
        }

        public Expression DateTimeContains(Expression paramExpLeft, Expression paramExpRight)
        {
            this.ResolveDateType(paramExpLeft, paramExpRight, out Expression convertedLeft, out Expression convertedRight);

            return this.StringContains(convertedLeft, convertedRight);
        }
        public Expression DateTimeStartsWith(Expression paramExpLeft, Expression paramExpRight)
        {
            ResolveDateType(paramExpLeft, paramExpRight, out Expression convertedLeft, out Expression convertedRight);

            return this.StringStartsWith(convertedLeft, convertedRight);
        }
        public Expression DateTimeEndsWith(Expression paramExpLeft, Expression paramExpRight)
        {
            ResolveDateType(paramExpLeft, paramExpRight, out Expression convertedLeft, out Expression convertedRight);

            return this.StringEndsWith(convertedLeft, convertedRight);
        }

        public Expression NumberContains(Expression paramExpLeft, Expression paramExpRight)
        {
            ResolveNumberType(paramExpLeft, paramExpRight, out Expression convertedLeft, out Expression convertedRight);

            return this.StringContains(convertedLeft, convertedRight);
        }
        public Expression NumberStartsWith(Expression paramExpLeft, Expression paramExpRight)
        {
            ResolveNumberType(paramExpLeft, paramExpRight, out Expression convertedLeft, out Expression convertedRight);

            return this.StringStartsWith(convertedLeft, convertedRight);
        }
        public Expression NumberEndsWith(Expression paramExpLeft, Expression paramExpRight)
        {
            ResolveNumberType(paramExpLeft, paramExpRight, out Expression convertedLeft, out Expression convertedRight);

            return this.StringEndsWith(convertedLeft, convertedRight);
        }

        public Expression Equals(Expression paramExpLeft, Expression paramExpRight)
        {
            this.ResolveType(paramExpLeft, paramExpRight, out Expression convertedLeft, out Expression convertedRight);

            if (convertedLeft.Type == typeof(string))
            {
                convertedLeft = Expression.Call(paramExpLeft, "ToLower", null);
                string paramExpRightAux = paramExpRight.ToString();
                if (paramExpRightAux != "null" && paramExpRightAux != "NULL")
                    convertedRight = Expression.Call(paramExpRight, "ToLower", null);
            }

            return Expression.MakeBinary(ExpressionType.Equal, convertedLeft, convertedRight);
        }
        public Expression GreaterThan(Expression paramExpLeft, Expression paramExpRight)
        {
            this.ResolveType(paramExpLeft, paramExpRight, out Expression convertedLeft, out Expression convertedRight);

            if (convertedLeft.Type == typeof(string))
            {
                var loweredLeft = Expression.Call(convertedLeft, "ToLower", null);
                var loweredRight = Expression.Call(convertedRight, "ToLower", null);

                var methodInfo = paramExpLeft.Type.GetMethod("CompareTo", new Type[] { typeof(string) });

                return Expression.MakeBinary(ExpressionType.GreaterThan, Expression.Call(loweredLeft, methodInfo, loweredRight), Expression.Constant((int)0));
            }

            return Expression.MakeBinary(ExpressionType.GreaterThan, convertedLeft, convertedRight);
        }
        public Expression GreaterThanOrEqual(Expression paramExpLeft, Expression paramExpRight)
        {
            this.ResolveType(paramExpLeft, paramExpRight, out Expression convertedLeft, out Expression convertedRight);

            if (convertedLeft.Type == typeof(string))
            {
                var loweredLeft = Expression.Call(convertedLeft, "ToLower", null);
                var loweredRight = Expression.Call(convertedRight, "ToLower", null);

                var methodInfo = paramExpLeft.Type.GetMethod("CompareTo", new Type[] { typeof(string) });

                return Expression.MakeBinary(ExpressionType.GreaterThanOrEqual, Expression.Call(loweredLeft, methodInfo, loweredRight), Expression.Constant((int)0));
            }

            return Expression.MakeBinary(ExpressionType.GreaterThanOrEqual, convertedLeft, convertedRight);
        }
        public Expression LessThan(Expression paramExpLeft, Expression paramExpRight)
        {
            this.ResolveType(paramExpLeft, paramExpRight, out Expression convertedLeft, out Expression convertedRight);

            if (convertedLeft.Type == typeof(string))
            {
                var loweredLeft = Expression.Call(convertedLeft, "ToLower", null);
                var loweredRight = Expression.Call(convertedRight, "ToLower", null);

                var methodInfo = paramExpLeft.Type.GetMethod("CompareTo", new Type[] { typeof(string) });

                return Expression.MakeBinary(ExpressionType.LessThan, Expression.Call(loweredLeft, methodInfo, loweredRight), Expression.Constant((int)0));
            }

            return Expression.MakeBinary(ExpressionType.LessThan, convertedLeft, convertedRight);
        }
        public Expression LessThanOrEqual(Expression paramExpLeft, Expression paramExpRight)
        {
            this.ResolveType(paramExpLeft, paramExpRight, out Expression convertedLeft, out Expression convertedRight);

            if (convertedLeft.Type == typeof(string))
            {
                var loweredLeft = Expression.Call(convertedLeft, "ToLower", null);
                var loweredRight = Expression.Call(convertedRight, "ToLower", null);

                var methodInfo = paramExpLeft.Type.GetMethod("CompareTo", new Type[] { typeof(string) });

                return Expression.MakeBinary(ExpressionType.LessThanOrEqual, Expression.Call(loweredLeft, methodInfo, loweredRight), Expression.Constant((int)0));
            }

            return Expression.MakeBinary(ExpressionType.LessThanOrEqual, convertedLeft, convertedRight);
        }

        public Expression In(Expression paramExpLeft, Expression paramExpRight)
        {
            this.ResolveType(paramExpLeft, paramExpRight, out Expression convertedLeft, out Expression convertedRight);

            var methodInfo = convertedRight.Type.GetMethod("Contains", new Type[] { convertedLeft.Type });

            if (convertedLeft.Type == typeof(string))
                convertedLeft = Expression.Call(paramExpLeft, "ToLower", null);

            return Expression.Call(convertedRight, methodInfo, convertedLeft);
        }

        private void ResolveDateType(Expression expressionLeft, Expression expressionRight, out Expression convertedLeft, out Expression convertedRight)
        {
            convertedLeft = expressionLeft;
            convertedRight = expressionRight;

            if (expressionLeft is MemberExpression mExpL && expressionRight is ConstantExpression cExpR)
            {
                convertedLeft = this._converter.ConvertDateType(mExpL, cExpR.Type);
                convertedRight = this._converter.ConvertDatePartialKeyword(cExpR);
            }
            else if (expressionRight is MemberExpression mExpR && expressionLeft is ConstantExpression cExpL)
            {
                convertedRight = this._converter.ConvertDateType(mExpR, cExpL.Type);
                convertedLeft = this._converter.ConvertDatePartialKeyword(cExpL);
            }
        }
        private void ResolveNumberType(Expression expressionLeft, Expression expressionRight, out Expression convertedLeft, out Expression convertedRight)
        {
            convertedLeft = expressionLeft;
            convertedRight = expressionRight;

            if (expressionLeft is MemberExpression mExpL && expressionRight is ConstantExpression cExpR)
                convertedLeft = this._converter.ConvertNumberType(mExpL, cExpR.Type);

            if (expressionRight is MemberExpression mExpR && expressionLeft is ConstantExpression cExpL)
                convertedRight = this._converter.ConvertNumberType(mExpR, cExpL.Type);
        }
        private void ResolveType(Expression expressionLeft, Expression expressionRight, out Expression convertedLeft, out Expression convertedRight)
        {
            convertedLeft = expressionLeft;
            convertedRight = expressionRight;

            if (expressionLeft is MemberExpression mExpL && expressionRight is ConstantExpression cExpR)
            {
                if (cExpR.Type.IsGenericType && cExpR.Type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    convertedRight = this._converter.ConvertTypeGroup(cExpR, mExpL.Type);
                }
                else
                {
                    convertedRight = this._converter.ConvertType(cExpR, mExpL.Type);
                }
            }

            if (expressionRight is MemberExpression mExpR && expressionLeft is ConstantExpression cExpL)
            {
                if (cExpL.Type.IsGenericType && cExpL.Type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    convertedLeft = this._converter.ConvertTypeGroup(cExpL, mExpR.Type);
                }
                else
                {
                    convertedLeft = this._converter.ConvertType(cExpL, mExpR.Type);
                }
            }
        }
    }
}
