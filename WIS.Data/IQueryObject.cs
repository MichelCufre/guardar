using System;
using System.Collections.Generic;
using System.Text;
using WIS.Data.Middleware;
using WIS.Filtering;
using WIS.Sorting;

namespace WIS.Data
{
    public interface IQueryObject<T, ContextDataType>
    {
        void BuildQuery(ContextDataType context);

        void ApplyFilter(IFilterInterpreter filter, string filterString);
        void ApplyFilter(IFilterInterpreter filter, List<FilterCommand> commands);
        void ApplySort(ISortingService sorter, List<SortCommand> commands);
        void ApplyMiddleware(IQueryMiddleware filter);

        void SkipRecords(int recordsToSkip);
        void TakeRecords(int recordsToTake);

        bool Any();

        IList<T> GetResult();
    }
}
