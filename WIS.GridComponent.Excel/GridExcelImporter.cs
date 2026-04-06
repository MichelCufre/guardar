using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WIS.Translation;

namespace WIS.GridComponent.Excel
{
    public class GridExcelImporter : ExcelImporterUtils, IDisposable
    {
        protected readonly string _fileName;
        protected readonly List<IGridColumn> _columns;
        protected readonly MemoryStream _stream;
        protected readonly ITranslator _translator;
        protected readonly XLWorkbook _workbook;

        public GridExcelImporter(ITranslator translator, string fileName, List<IGridColumn> columns, string excelData)
        {
            this._translator = translator;
            this._fileName = fileName;
            this._columns = columns;
            this._stream = new MemoryStream(Convert.FromBase64String(excelData));
            this._workbook = new XLWorkbook(this._stream);
        }

        public List<GridRow> BuildRows()
        {
            var rows = new List<GridRow>();

            var rowToReadFrom = 3;
            var rowIndex = 1;
            var cantColumns = 0;
            var colsName = new Dictionary<string, string>();

            var workSheet = _workbook.Worksheets.First();

            foreach (var row in workSheet.Rows())
            {
                SetColumnsName(row, rowIndex, ref colsName, ref cantColumns);

                var cells = new List<GridCell>();

                if (rowIndex >= rowToReadFrom && !IsRowEmpty(row, cantColumns))
                {
                    foreach (var cell in GetCellsUsed(row, cantColumns))
                    {
                        var columnName = colsName.GetValueOrDefault(cell.Address.ColumnLetter);
                        var column = this._columns.FirstOrDefault(d => d.Id == columnName);                        
                        var cellValue = cell.GetFormattedString();

                        if (!string.IsNullOrEmpty(cellValue))
                        {
                            cells.Add(new GridCell
                            {
                                Column = column,
                                Editable = true,
                                Value = cellValue,
                                Modified = true
                            });
                        }
                    }

                    if (cells.Count > 0)
                    {
                        rows.Add(new GridRow
                        {
                            Cells = cells,
                            IsNew = true
                        });
                    }
                }

                rowIndex++;
            }

            return rows;
        }

        public void Dispose()
        {
            _workbook.Dispose();
            _stream.Dispose();
        }

        public virtual void SetErrors(Dictionary<int, List<string>> errors)
        {
            if (errors.Count > 0)
            {
                var workSheet = this._workbook.Worksheets.First();
                int rowIndex = 1;
                int rowToReadFrom = 3;
                var colsName = new Dictionary<int, string>();

                foreach (var row in workSheet.RowsUsed())
                {
                    if (rowIndex >= rowToReadFrom)
                    {
                        var itemId = rowIndex - 2;
                        if (errors.ContainsKey(itemId))
                        {
                            var worksheetCell = row.CellsUsed().FirstOrDefault();
                            worksheetCell.Style.Font.FontColor = XLColor.Red;
                            AddComment(worksheetCell, string.Join(". ", errors[itemId]));
                        }
                    }

                    rowIndex++;
                }
            }
        }

        public virtual void CleanErrors()
        {
            var workSheets = this._workbook.Worksheets.ToList();
            if (workSheets != null)
            {
                foreach (var workSheet in workSheets)
                {
                    workSheet.DeleteComments();
                    workSheet.Cells().Style.Font.SetFontColor(XLColor.Black);
                }
            }
        }

        public virtual byte[] GetAsByteArray()
        {
            using (var workbook = new XLWorkbook())
            using (var ms = new MemoryStream())
            {
                //Genero el resultado a partir de una copia del workbook para evitar manejar
                //estilos no soportados por ClosedXml e inyectados por editores de hojas de calculo particulares (ej. WPS Office) 
                foreach (var ws in _workbook.Worksheets)
                {
                    workbook.AddWorksheet(ws);
                }

                workbook.SaveAs(ms, new SaveOptions { EvaluateFormulasBeforeSaving = false, GenerateCalculationChain = false, ValidatePackage = true });
                var data = ms.ToArray();
                return data;
            }
        }
    }
}
