using System;
using System.Linq.Expressions;

namespace WIS.Filtering.Expressions
{
	public interface IExpressionLiteralConverter
    {
        Expression ConvertType(ConstantExpression expression, Type type);
        Expression ConvertTypeGroup(ConstantExpression expression, Type type);
        Expression ConvertNumberType(MemberExpression expression, Type type);
        Expression ConvertDateType(MemberExpression expression, Type type);
        Expression ConvertDatePartialKeyword(ConstantExpression expression);
    }
}
