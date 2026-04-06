using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Utils;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Sorting;

namespace WIS.Domain.General
{
    public class GridInMemoryUtils
    {
        /// <summary>
        /// Este método retorna los datos de la fila de una grilla en memoria.
        /// </summary>
        /// <param name="row">La fila</param>
        /// <param name="data">El objeto el cual se desea cargar y recibir</param>
        public static T GetDataFromRow<T>(GridRow row, T data)
        {
            foreach (var prop in typeof(T).GetProperties())
            {
                var isCollection = prop.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(prop.PropertyType);

                var isCustomClass = !prop.PropertyType.IsPrimitive && prop.PropertyType.IsClass && !prop.PropertyType.FullName.StartsWith("System");

                if (isCollection || isCustomClass) continue;

                if (string.IsNullOrEmpty(row.GetCell(prop.Name).Value))
                    prop.SetValue(data, null);

                else if (prop.PropertyType.Equals(typeof(Int64)) || prop.PropertyType.Equals(typeof(Nullable<Int64>)))
                {
                    if (long.TryParse(row.GetCell(prop.Name).Value, out long longValue))
                        prop.SetValue(data, longValue);
                }

                else if (prop.PropertyType.Equals(typeof(Int32)) || prop.PropertyType.Equals(typeof(Nullable<Int32>)))
                {
                    if (int.TryParse(row.GetCell(prop.Name).Value, out int intValue))
                        prop.SetValue(data, intValue);
                }

                else if (prop.PropertyType.Equals(typeof(Int16)) || prop.PropertyType.Equals(typeof(Nullable<Int16>)))
                {
                    if (Int16.TryParse(row.GetCell(prop.Name).Value, out Int16 int16Value))
                        prop.SetValue(data, int16Value);
                }

                else if (prop.PropertyType.Equals(typeof(Decimal)) || prop.PropertyType.Equals(typeof(Nullable<Decimal>)))
                {
                    if (decimal.TryParse(row.GetCell(prop.Name).Value, out decimal decimalValue))
                        prop.SetValue(data, decimalValue);
                }

                else if (prop.PropertyType.Equals(typeof(Double)) || prop.PropertyType.Equals(typeof(Nullable<Double>)))
                {
                    if (double.TryParse(row.GetCell(prop.Name).Value, out double doubleValue))
                        prop.SetValue(data, doubleValue);
                }

                else if (prop.PropertyType.Equals(typeof(DateTime)) || prop.PropertyType.Equals(typeof(Nullable<DateTime>)))
                {
                    if (DateTime.TryParse(row.GetCell(prop.Name).Value, out DateTime dateValue))
                        prop.SetValue(data, dateValue);
                }

                else if (prop.PropertyType.Equals(typeof(Boolean)))
                {
                    if (bool.TryParse(row.GetCell(prop.Name).Value, out bool boolValue))
                        prop.SetValue(data, boolValue);
                }

                else if (prop.PropertyType.Equals(typeof(String)))
                {
                    prop.SetValue(data, row.GetCell(prop.Name).Value);
                }
            }

            return data;
        }

        /// <summary>
        /// Este método añade a la Grid todas las columnas faltantes correspondiendo a la entidad T que se le pase.
        /// </summary>
        /// <param name="grid">La grilla en la que se añadirán las columnas</param>
        public static void AddColumnFromEntityIfNotExists<T>(Grid grid)
        {
            foreach (var prop in typeof(T).GetProperties())
            {
                var isCollection = prop.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(prop.PropertyType);

                var isCustomClass = !prop.PropertyType.IsPrimitive && prop.PropertyType.IsClass && !prop.PropertyType.FullName.StartsWith("System");

                if (isCollection || isCustomClass)
                    continue;

                if (!grid.Columns.Any(i => i.Id == prop.Name))
                {
                    grid.AddOrUpdateColumn(new GridColumn
                    {
                        Id = prop.Name,
                        Name = prop.Name,
                        Translate = false,
                        AllowsFiltering = true,
                        AllowsSorting = true,
                        Type = MapProperyTypeToColumnType(prop.PropertyType)
                    });
                }
            }
        }

        public static GridColumnType MapProperyTypeToColumnType(Type type)
        {
            if (type.Equals(typeof(DateTime)) || type.Equals(typeof(DateTime?)))
                return GridColumnType.DateTime;

            return GridColumnType.Text;
        }

        /// <summary>
        /// Este método oculta aquellas columnas que no correspondan a la entidad T que se le pasa.
        /// </summary>
        /// <param name="grid">La grilla en la que se ocultarán las columnas</param>
        public static void HideColumnIfNotApplies<T>(Grid grid)
        {
            grid.Columns.ForEach(i =>
            {
                i.IsNew = false; //NO QUITAR, sino en el GetRows se pueden generar columnas no deseadas.

                if (!typeof(T).GetProperties().Any(prop => prop.Name == i.Id) && i.Type != GridColumnType.Button)
                    i.Hidden = true;
            });
        }

        /// <summary>
        /// Este método carga la grilla en memoria a partir del tipo de dato, registros, lista de órdenes y llaves primarias. Opcionalmente se puede pasar una lista de columnas editables. Si la lista de columnas editables está vacía, ningúna columna será editable. Si no se especifica, por defecto todas las columnas serán editables.
        /// </summary>
        public static List<GridRow> LoadGrid<T>(IGridService gridService, IUnitOfWork uow, Grid grid, GridFetchContext context, List<T> gridRecords, List<SortCommand> sorts, List<string> keys, List<string> editableCells = null)
        {
            var query = new InMemoryQuery<T>(gridRecords, context.Filters, context.Sorts);

            uow.HandleQuery(query);

            context.Filters.RemoveAll(i => true);

            AddColumnFromEntityIfNotExists<T>(grid);

            HideColumnIfNotApplies<T>(grid);

            var defaultSort = context.Sorts.Count > 0 ? context.Sorts : sorts;

            var rows = gridService.GetRows(query, grid.Columns, context, defaultSort, keys);

            grid.Rows = rows;

            if (editableCells == null)
                editableCells = grid.Columns.Where(i => !i.Hidden).Select(s => s.Id).ToList();

            grid.SetEditableCells(editableCells);

            return rows;
        }

        /// <summary>
        /// Este método crea el excel a partir de los datos provistos.
        /// </summary>
        public static byte[] CreateExcel<T>(IGridExcelService excelService, IUnitOfWork uow, Grid grid, GridExportExcelContext context, List<T> records, List<SortCommand> sorts, string application)
        {
            var query = new InMemoryQuery<T>(records, context.Filters, context.Sorts);

            uow.HandleQuery(query);

            context.Filters.RemoveAll(i => true);
            context.Sorts.RemoveAll(i => true);
            grid.Columns.RemoveAll(i => true);

            context.FileName = application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return excelService.GetExcel(context.FileName, query, grid.Columns, context, sorts);
        }

        /// <summary>
        /// Este método obtiene las estadísticas de una grilla a partir de los datos provistos.
        /// </summary>
        public static GridStats FetchStats<T>(GridFetchStatsContext context, IUnitOfWork uow, List<T> records)
        {
            var query = new InMemoryQuery<T>(records, context.Filters, context.Sorts);

            uow.HandleQuery(query);

            return new GridStats()
            {
                Count = query?.GetCount() ?? 0
            };
        }
    }
}

