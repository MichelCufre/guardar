using System;
using System.Linq;
using System.Linq.Expressions;
using WIS.Data.Middleware;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Middlewares
{
    public class PredioQueryMiddleware : IQueryMiddleware
    {
        protected readonly WISDB _context;
        protected readonly int _userId;
        protected readonly string _predio;
        protected readonly string _predioNullMapValue;

        public PredioQueryMiddleware(WISDB context, int userId, string predio, string predioNullMapValue = null)
        {
            this._context = context;
            this._userId = userId;
            this._predio = predio;
            this._predioNullMapValue = predioNullMapValue;
        }

        public IQueryable<T> ApplyFilter<T>(IQueryable<T> query)
        {
            if (this._predio != GeneralDb.PredioSinPredio && !string.IsNullOrEmpty(this._predio))
            {
                var predioProperty = typeof(T).GetProperty("NU_PREDIO");

                if (predioProperty != null && predioProperty.PropertyType == typeof(string))
                {
                    var parameter = Expression.Parameter(typeof(T), "v");
                    var property = Expression.Property(parameter, "NU_PREDIO");

                    if (this._predio == null || this._predio == GeneralDb.PredioSinDefinir)
                    {
                        query = query
                            .GroupJoin(this._context.T_PREDIO_USUARIO,
                                Expression.Lambda<Func<T, string>>(property, parameter),
                                pu => pu.NU_PREDIO,
                                (v, pus) => new { V = v, PUs = pus })
                            .SelectMany(vpus => vpus.PUs.DefaultIfEmpty(), (vpus, pu) => new { V = vpus.V, PU = pu })
                            .Where(vpu => vpu.PU.USERID == this._userId || vpu.PU == null)
                            .Select(vpu => vpu.V);
                    }
                    else
                    {
                        var predioExpression = Expression.Constant(this._predio);
                        var isPredioExpression = Expression.Equal(property, predioExpression);
                        var orExpression = this.GetNullableOr(isPredioExpression, property);

                        query = query.Where(Expression.Lambda<Func<T, bool>>(orExpression, parameter));
                    }
                }
            }

            return query;
        }

        private BinaryExpression GetNullableOr(Expression expression, MemberExpression property)
        {
            var nullExpression = Expression.Constant(null, typeof(object));
            var isNullExpression = Expression.Equal(property, nullExpression);

            var nullValueExpression = Expression.Constant(_predioNullMapValue);
            var isNullValueExpression = Expression.Equal(property, nullValueExpression);
                        
            var orExpresion = Expression.MakeBinary(ExpressionType.Or, expression, isNullExpression);

            return Expression.MakeBinary(ExpressionType.Or, orExpresion, isNullValueExpression);
        }
    }
}
