using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public abstract class Repository
    {
        protected bool IsEntityAttached<T>(WISDB context, T entity) where T : class
        {
            return context.Set<T>().Local.Any(e => e == entity);
        }
    }
}
