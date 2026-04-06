using ClosedXML.Excel;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent.Build;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel.Configuration;
using WIS.Sorting;
using WIS.Translation;

namespace WIS.GridComponent.Excel
{
    public class GridExcelService : IGridExcelService
    {
        private readonly IGridConfigProvider _configProvider;
        private readonly IFilterInterpreter _filterInterpreter;
        private readonly ITranslator _translator;
        private readonly ISortingService _sorter;
        private readonly IGridExcelBuildService _buildService;

        public GridExcelService(
            IGridConfigProvider configProvider,
            IFilterInterpreter interpreter, 
            ISortingService sorter,
            ITranslator translator, 
            IGridExcelBuildService buildService)
        {
            this._configProvider = configProvider;
            this._filterInterpreter = interpreter;
            this._sorter = sorter;
            this._translator = translator;
            this._buildService = buildService;
        }

        public byte[] GetExcel<T, ContextDataType>(string name, IQueryObject<T, ContextDataType> query, List<IGridColumn> columns, GridExportExcelContext queryParameters, List<SortCommand> defaultSorting)
        {
            if (columns == null || columns.Where(d => !d.IsNew).Count() == 0)
                columns.AddRange(this._configProvider.GetColumnsFromEntity<T>(columns));

            columns = columns.Where(s => s.Name != null).ToList();

            query.ApplyFilter(this._filterInterpreter, queryParameters.Filters);
            query.ApplyFilter(this._filterInterpreter, queryParameters.ExplicitFilter);

            var sorts = queryParameters.Sorts;

            if (!sorts.Any())
                sorts = defaultSorting;

            query.ApplySort(this._sorter, sorts);

            IList<T> data = query.GetResult();

            if (data.Count == 0)
                throw new EntityNotFoundException("General_Sec0_Error_Error08");

            List<string> keys = columns.Select(d => d.Name).ToList();

            Dictionary<string, string> translatedColumns = this._translator.Translate(keys);

            foreach (var column in columns)
            {
                var translation = translatedColumns[column.Name];

                if (!string.IsNullOrEmpty(translation))
                    column.Name = translation;
            }

           return this.BuildExcel<T>(name, columns, data);
        }
        public byte[] GetExcel<T, ContextDataType>(string name, IQueryObject<T, ContextDataType> query, List<IGridColumn> columns, GridExportExcelContext queryParameters, SortCommand defaultSorting)
        {
            var sorting = new List<SortCommand> { defaultSorting };

            return this.GetExcel(name, query, columns, queryParameters, sorting);
        }

        private byte[] BuildExcel<T>(string name, List<IGridColumn> columns, IList<T> data)
        {
            using (XLWorkbook package = new XLWorkbook())
            {
                List<IGridColumn> columnsToExport = columns.ToList();

                columnsToExport = columnsToExport.Where(d => d.Type != GridColumnType.Button
                    && d.Type != GridColumnType.ItemList).OrderBy(d => d.Order).ToList();

                return this._buildService.Build(name, package, columnsToExport, data);
            }
        }
    }
}
