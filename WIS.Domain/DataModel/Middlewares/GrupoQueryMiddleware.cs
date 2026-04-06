using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WIS.Data.Middleware;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Middlewares
{
    public class GrupoQueryMiddleware : IQueryMiddleware
    {
        private readonly WISDB _context;
        private readonly int _userId;
        private readonly bool _allowNull;

        public GrupoQueryMiddleware(WISDB context, int userId, bool allowNull)
        {
            this._context = context;
            this._userId = userId;
            this._allowNull = allowNull;
        }

        public IQueryable<T> ApplyFilter<T>(IQueryable<T> query)
        {
            var grupoProperty = typeof(T).GetProperty("CD_GRUPO_CONSULTA");

            if (grupoProperty != null)
            {
                var parameter = Expression.Parameter(typeof(T), "v");
                var property = Expression.Property(parameter, "CD_GRUPO_CONSULTA");

                if (this._allowNull)
                {
                    query = query
                        .GroupJoin(this._context.T_GRUPO_CONSULTA_FUNCIONARIO,
                            Expression.Lambda<Func<T, string>>(property, parameter),
                            gcf => gcf.CD_GRUPO_CONSULTA,
                            (v, gcfs) => new { V = v, GCFs = gcfs })
                        .SelectMany(vgcfs => vgcfs.GCFs.DefaultIfEmpty(), (vgcfs, gcf) => new { V = vgcfs.V, GCF = gcf })
                        .Where(vgcf => vgcf.GCF.USERID == this._userId || vgcf.GCF == null)
                        .Select(vgcf => vgcf.V);
                }
                else
                {
                    query = query
                        .Join(this._context.T_GRUPO_CONSULTA_FUNCIONARIO,
                            Expression.Lambda<Func<T, string>>(property, parameter),
                            gcf => gcf.CD_GRUPO_CONSULTA,
                            (v, gcf) => new { V = v, GCF = gcf })
                        .Where(vgcf => vgcf.GCF.USERID == this._userId)
                        .Select(vgcf => vgcf.V);
                }
            }

            return query;
        }
    }
}
