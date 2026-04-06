using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WIS.Data;
using WIS.Filtering;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.Security;
using WIS.Sorting;

namespace WIS.GridComponent.Build
{
    public class GridService : IGridService
    {
        private const string RowIdSeparator = "$";

        private readonly IGridConfigProvider _configProvider;
        private readonly IFilterInterpreter _filterInterpreter;
        private readonly ISortingService _sorter;
        private readonly IGridCellValueParsingService _valueParser;
        private readonly IIdentityService _identity;

        public GridService(IGridConfigProvider configProvider, IFilterInterpreter filterInterpreter, ISortingService sortingService, IGridCellValueParsingService valueParser, IIdentityService identity)
        {
            this._configProvider = configProvider;
            this._filterInterpreter = filterInterpreter;
            this._sorter = sortingService;
            this._valueParser = valueParser;
            this._identity = identity;
        }

        public List<GridRow> GetRows<T, ContextDataType>(IQueryObject<T, ContextDataType> query, List<IGridColumn> columns, GridFetchContext queryParameters, SortCommand defaultSorting, List<string> keys, bool enableSkipAndTakeRecords = true)
        {
            var sorting = new List<SortCommand> { defaultSorting };

            return this.GetRows(query, columns, queryParameters, sorting, keys, enableSkipAndTakeRecords);
        }
        public List<GridRow> GetRows<T, ContextDataType>(IQueryObject<T, ContextDataType> query, List<IGridColumn> columns, GridFetchContext queryParameters, List<SortCommand> defaultSorting, List<string> keys, bool enableSkipAndTakeRecords = true)
        {
            if (columns == null || columns.Where(d => !d.IsNew).Count() == 0)
                columns.AddRange(this._configProvider.GetColumnsFromEntity<T>(columns));

            IList<T> data = new List<T>();

            if (!queryParameters.IsGridInitialize || queryParameters.Filters.Count > 0 || !string.IsNullOrEmpty(queryParameters.ExplicitFilter) || (queryParameters.IsGridInitialize && (!_configProvider.IsModoConsulta(_identity.UserId) && !_configProvider.IsPantallaModoConsulta(_identity.UserId, _identity.Application))))
            {
                query.ApplyFilter(this._filterInterpreter, queryParameters.Filters);
                query.ApplyFilter(this._filterInterpreter, queryParameters.ExplicitFilter);

                var sorts = queryParameters.Sorts;

                if (!sorts.Any())
                    sorts = defaultSorting;

                sorts.AddRange(keys.Select(x => new SortCommand(x, SortDirection.Ascending)));

                query.ApplySort(this._sorter, sorts);

                //Agregar paginado
                if (enableSkipAndTakeRecords)
                {
                    query.SkipRecords(queryParameters.RowsToSkip);

                    query.TakeRecords(queryParameters.RowsToFetch);
                }

                data = query.GetResult();
            }

            //Reiniciar filas
            return this.BuildRows(data, columns, keys);
        }

        private List<GridRow> BuildRows<T>(IList<T> data, List<IGridColumn> columns, List<string> keys)
        {
            var rows = new List<GridRow>();

            foreach (var item in data)
            {
                var newRow = new GridRow();

                var rowIdList = new Dictionary<int, string>();

                var properties = item.GetType().GetProperties();

                var itemType = item.GetType();

                foreach (var col in columns)
                {
                    var prop = itemType.GetProperty(col.Id, BindingFlags.Public | BindingFlags.Instance);

                    var value = string.Empty;

                    if (prop != null)
                    {
                        if (prop.PropertyType == typeof(DateTime))
                            value = this._valueParser.GetValue((DateTime)prop.GetValue(item));
                        else if (Nullable.GetUnderlyingType(prop.PropertyType) == typeof(DateTime))
                            value = this._valueParser.GetValue((DateTime?)prop.GetValue(item));
                        else
                            value = this._valueParser.GetValue(prop.GetValue(item));

                        if (keys.Any(k => k == prop.Name))
                            rowIdList.Add(keys.IndexOf(prop.Name), value);
                    }

                    newRow.Cells.Add(new GridCell
                    {
                        Column = col,
                        Value = value,
                        Old = value
                    });
                }

                newRow.Id = this.BuildRowId(this.GetSortedIdColumnList(rowIdList));

                rows.Add(newRow);
            }

            return rows;
        }

        private List<string> GetSortedIdColumnList(Dictionary<int, string> idList)
        {
            var result = new List<string>();
            var sortedKeys = idList.Keys.ToList();

            sortedKeys.Sort();

            foreach (var key in sortedKeys)
            {
                result.Add(idList[key]);
            }

            return result;
        }

        public string BuildRowId(List<string> rowIdList)
        {
            return string.Join(RowIdSeparator, rowIdList);
        }
        public string[] SplitRowId(string rowId)
        {
            return rowId.Split(new[] { RowIdSeparator }, StringSplitOptions.None);
        }
    }
}
