using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using WIS.Translation;

namespace WIS.GridComponent.Excel
{
    public class GridExcelBuildService : IGridExcelBuildService
    {
        protected readonly ITranslator _translator;

        public GridExcelBuildService(ITranslator translator)
        {
            this._translator = translator;
        }

        public virtual byte[] Build<T>(string name, XLWorkbook workbook, List<IGridColumn> columns, IList<T> data)
        {
            var worksheet = workbook.Worksheets.Add(name.Split("-")[0]);

            var visibleColumns = this.GetVisibleColumns(columns);
            var infoList = this.GetColumnList<T>(visibleColumns);

            var range = LoadFromCollectionInternal(worksheet, data, true, XLWorkbook.DefaultStyle, infoList.ToArray());

            var dateList = new List<int>();
            var dupedList = new List<string>();
            int index = 0;

            List<IXLCell> cellsPendingTranslation = new List<IXLCell>();

            foreach (IGridColumn column in visibleColumns)
            {
                var infoCol = infoList.FirstOrDefault(x =>
                    string.Equals(x.Name, column.Id, StringComparison.OrdinalIgnoreCase));

                if (infoCol == null)
                    continue;

                this.AlterColumn(column, range, worksheet, dupedList, infoList, dateList, index);

                if (column.Translate)
                {
                    var cellsToTranslate = worksheet.Cells().Where(d => d.WorksheetColumn().ColumnNumber() == index + 1).ToList();

                    cellsToTranslate.RemoveAt(0); //Quita cabezal

                    cellsPendingTranslation.AddRange(cellsToTranslate);
                }

                index++;
            }

            this.TranslateCells(cellsPendingTranslation);

            worksheet.Columns().AdjustToContents();
            FreezePanes(worksheet, 1, 0);

            return GetAsByteArray(workbook);
        }

        public virtual IXLRange LoadFromCollectionInternal<T>(IXLWorksheet worksheet, IList<T> collection, bool printHeaders, IXLStyle style, MemberInfo[] memberInfos)
        {
            var defaultBindingFlags = BindingFlags.Public | BindingFlags.Instance;

            LoadFromCollectionParams parameters = new LoadFromCollectionParams
            {
                PrintHeaders = printHeaders,
                TableStyle = style,
                BindingFlags = defaultBindingFlags,
                Members = memberInfos
            };

            return new LoadFromCollection<T>(worksheet, collection, parameters).Load();

        }

        public virtual void FreezePanes(IXLWorksheet worksheet, int rows, int columnns)
        {
            worksheet.SheetView.Freeze(rows, columnns);
        }

        public virtual byte[] GetAsByteArray(XLWorkbook workbook)
        {
            using (var ms = new MemoryStream())
            {
                workbook.SaveAs(ms);
                return ms.ToArray();
            }
        }

        public virtual void TranslateCells(List<IXLCell> cellsPendingTranslation)
        {
            if (cellsPendingTranslation.Any())
            {
                List<string> valuesToTranslate = cellsPendingTranslation.Select(d => d.Value.ToString()).Where(d => !string.IsNullOrEmpty(d)).Distinct().ToList();

                Dictionary<string, string> translatedValues = this._translator.Translate(valuesToTranslate);

                foreach (var cellPending in cellsPendingTranslation)
                {
                    var cellValue = cellPending.Value.ToString();

                    if (!string.IsNullOrEmpty(cellValue))
                        cellPending.Value = translatedValues[cellValue];
                }
            }
        }

        public virtual void AlterColumn(IGridColumn column, IXLRange range, IXLWorksheet worksheet, List<string> dupedList, List<MemberInfo> infoList, List<int> dateList, int index)
        {
            string columnName = column.Name.Replace("\n", "").Trim();

            range.Cell(1, index + 1).Value = columnName;

            int count = range.Cells(c => c.WorksheetRow().RowNumber() == 1 && c.Value.ToString().Equals(columnName, StringComparison.InvariantCultureIgnoreCase)).Count();

            if (count > 1)
            {
                dupedList.Add(columnName);

                range.Cell(1, index + 1).Value = columnName + "~" + dupedList.Count(s => s == columnName);
            }

            var columnInfo = (PropertyInfo)infoList[index];

            if (columnInfo.PropertyType.Equals(typeof(DateTime)) || columnInfo.PropertyType.Equals(typeof(DateTime?)))
            {
                worksheet.Column(index + 1).Style.NumberFormat.Format = "dd/MM/yyyy HH:mm:ss";

                dateList.Add(index + 1);
            }
        }

        public virtual List<MemberInfo> GetColumnList<T>(List<IGridColumn> columns)
        {
            var infoList = new List<MemberInfo>();

            foreach (IGridColumn column in this.GetVisibleColumns(columns))
            {
                PropertyInfo info = typeof(T).GetProperty(column.Id);

                if (info != null)
                    infoList.Add(info);
            }

            return infoList;
        }

        public virtual List<IGridColumn> GetVisibleColumns(List<IGridColumn> columns)
        {
            return columns.Where(d => !d.Hidden).ToList();
        }

    }
}
