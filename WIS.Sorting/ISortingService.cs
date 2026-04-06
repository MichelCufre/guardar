using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WIS.Sorting
{
    public interface ISortingService
    {
        IQueryable<T> ApplySorting<T>(IQueryable<T> query, List<SortCommand> commands);
        IQueryable<T> ApplySorting<T>(IQueryable<T> query, SortCommand command);
    }
}
