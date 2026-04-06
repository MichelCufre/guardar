using System.Collections.Generic;
using WIS.Data;
using WIS.GridComponent.Build.Configuration;
using WIS.Sorting;

namespace WIS.GridComponent.Build
{
    public interface IGridService
    {
        List<GridRow> GetRows<T, ContextDataType>(IQueryObject<T, ContextDataType> query, List<IGridColumn> columns, GridFetchContext queryParameters, SortCommand defaultSorting, List<string> keys, bool enableSkipAndTakeRecords = true);
        List<GridRow> GetRows<T, ContextDataType>(IQueryObject<T, ContextDataType> query, List<IGridColumn> columns, GridFetchContext queryParameters, List<SortCommand> defaultSorting, List<string> keys, bool enableSkipAndTakeRecords = true);
    }
}
