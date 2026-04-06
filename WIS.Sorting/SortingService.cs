using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WIS.Sorting
{
    public class SortingService : ISortingService
    {
        private static readonly MethodInfo _orderByMethod = typeof(Queryable).GetMethods().Single(method => method.Name == "OrderBy" && method.GetParameters().Length == 2);
        private static readonly MethodInfo _orderByDescendingMethod = typeof(Queryable).GetMethods().Single(method => method.Name == "OrderByDescending" && method.GetParameters().Length == 2);
        private static readonly MethodInfo _thenByMethod = typeof(Queryable).GetMethods().Single(method => method.Name == "ThenBy" && method.GetParameters().Length == 2);
        private static readonly MethodInfo _thenByDescendingMethod = typeof(Queryable).GetMethods().Single(method => method.Name == "ThenByDescending" && method.GetParameters().Length == 2);

        public SortingService()
        {

        }

        public IQueryable<T> ApplySorting<T>(IQueryable<T> query, List<SortCommand> commands)
        {
            foreach (var command in commands)
            {
                query = this.ApplySortCommand<T>(query, command);
            }

            return query;
        }
        public IQueryable<T> ApplySorting<T>(IQueryable<T> query, SortCommand command)
        {
            return this.ApplySortCommand<T>(query, command);
        }

        private IQueryable<T> ApplySortCommand<T>(IQueryable<T> query, SortCommand sort)
        {
            if (sort.Direction == SortDirection.Ascending)
                query = this.ApplyOrderBy<T>(query, sort.ColumnId);

            if (sort.Direction == SortDirection.Descending)
                query = this.ApplyOrderByDescending<T>(query, sort.ColumnId);

            return query;
        }
        private IQueryable<T> ApplyOrderBy<T>(IQueryable<T> query, string columnId)
        {
            ParameterExpression paramExp = Expression.Parameter(typeof(T));
            Expression orderByProperty = Expression.Property(paramExp, columnId);
            LambdaExpression lambda = Expression.Lambda(orderByProperty, paramExp);

            MethodInfo genericMethod;

            if (query.Expression.Type == typeof(IOrderedQueryable<T>))
                genericMethod = _thenByMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);
            else
                genericMethod = _orderByMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);

            return (IQueryable<T>)genericMethod.Invoke(null, new object[] { query, lambda });
        }
        private IQueryable<T> ApplyOrderByDescending<T>(IQueryable<T> query, string columnId)
        {
            ParameterExpression paramExp = Expression.Parameter(typeof(T));
            Expression orderByProperty = Expression.Property(paramExp, columnId);
            LambdaExpression lambda = Expression.Lambda(orderByProperty, paramExp);

            MethodInfo genericMethod;

            if (query.Expression.Type == typeof(IOrderedQueryable<T>))
                genericMethod = _thenByDescendingMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);
            else
                genericMethod = _orderByDescendingMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);

            return (IQueryable<T>)genericMethod.Invoke(null, new object[] { query, lambda });
        }
    }
}
