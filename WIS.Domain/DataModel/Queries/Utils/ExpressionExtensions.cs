using System;
using System.Linq.Expressions;

namespace WIS.Domain.DataModel.Queries.Utils
{
	public static class ExpressionExtensions
	{
		public static Expression <Func <T, bool>> And <T> (
			this Expression <Func <T, bool>> expression,
			Expression <Func <T, bool>> addedExpression)
		{
			var parameter = Expression.Parameter (typeof (T));

			var body = Expression.AndAlso (
				Expression.Invoke (expression, parameter),
				Expression.Invoke (addedExpression, parameter));

			return Expression.Lambda <Func <T, bool>> (body, parameter);
		}
	}
}
