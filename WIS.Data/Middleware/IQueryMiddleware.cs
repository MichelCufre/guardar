using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WIS.Data.Middleware
{
    public interface IQueryMiddleware
    {
        IQueryable<T> ApplyFilter<T>(IQueryable<T> query);
    }
}
