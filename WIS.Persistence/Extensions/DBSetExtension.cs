using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Extensions
{
    public static class DBSetExtension
    {
        public static bool IsAttached<T>(this DbSet<T> app, T entity) where T : class
        {
            return app.Local.Any(d => d == entity);
        }
    }
}
