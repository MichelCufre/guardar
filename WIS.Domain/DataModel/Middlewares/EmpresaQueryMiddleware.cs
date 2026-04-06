using System;
using System.Linq;
using System.Linq.Expressions;
using WIS.Data.Middleware;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Middlewares
{
    public class EmpresaQueryMiddleware : IQueryMiddleware
    {
        private readonly WISDB _context;
        private readonly int _userId;
        private readonly bool _allowNull;

        public EmpresaQueryMiddleware(WISDB context, int userId, bool allowNull)
        {
            this._context = context;
            this._userId = userId;
            this._allowNull = allowNull;
        }

        public IQueryable<T> ApplyFilter<T>(IQueryable<T> query)
        {
            var empresaProperty = typeof(T).GetProperty("CD_EMPRESA");

            if (empresaProperty != null)
            {
                var parameter = Expression.Parameter(typeof(T), "v");
                var property = Expression.Property(parameter, "CD_EMPRESA");

                if (empresaProperty.PropertyType == typeof(int?))
                {
                    if (this._allowNull)
                    {
                        query = query
                            .GroupJoin(this._context.T_EMPRESA_FUNCIONARIO,
                                Expression.Lambda<Func<T, int?>>(property, parameter),
                                ef => (int?)ef.CD_EMPRESA,
                                (v, efs) => new { V = v, EFs = efs })
                            .SelectMany(vefs => vefs.EFs.DefaultIfEmpty(), (vefs, ef) => new { V = vefs.V, EF = ef })
                            .Where(vef => vef.EF.USERID == this._userId || vef.EF == null)
                            .Select(vef => vef.V);
                    }
                    else
                    {
                        query = query
                            .Join(this._context.T_EMPRESA_FUNCIONARIO,
                                Expression.Lambda<Func<T, int?>>(property, parameter),
                                ef => (int?)ef.CD_EMPRESA,
                                (v, ef) => new { V = v, EF = ef })
                            .Where(vef => vef.EF.USERID == this._userId)
                            .Select(vef => vef.V);
                    }
                }
                else if (empresaProperty.PropertyType == typeof(int))
                {
                    query = query
                        .Join(this._context.T_EMPRESA_FUNCIONARIO,
                            Expression.Lambda<Func<T, int>>(property, parameter),
                            ef => ef.CD_EMPRESA,
                            (v, ef) => new { V = v, EF = ef })
                        .Where(vef => vef.EF.USERID == this._userId)
                        .Select(vef => vef.V);
                }
            }

            return query;
        }
    }
}
