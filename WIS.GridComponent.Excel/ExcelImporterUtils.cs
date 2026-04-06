using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.GridComponent.Excel
{
    public class ExcelImporterUtils
    {
        public virtual void SetColumnsName(IXLRow row, int rowIndex, ref Dictionary<string, string> colsName, ref int cantColumns)
        {
            if (rowIndex == 1)
            {
                colsName = new Dictionary<string, string>();

                foreach (var cell in row.CellsUsed())
                {
                    var location = cell.Address.ColumnLetter;
                    colsName.Add(location, cell.GetFormattedString());
                }

                cantColumns = colsName.Count();
            }
        }

        public virtual bool IsRowEmpty(IXLRow row, int lastColumn)
        {
            return !row.Cells(1, lastColumn).Any(cell => !cell.IsEmpty());
        }

        public virtual IEnumerable<IXLCell> GetCellsUsed(IXLRow row, int countColumns)
        {
            return row.CellsUsed().Where(c => c.Address.ColumnNumber <= countColumns);
        }

        public virtual void AddComment(IXLCell cell, string text, List<IXLCell> cellsToTranslate = null)
        {
            cell.CreateComment().AddText(text);

            if (cellsToTranslate != null)
            {
                cellsToTranslate.Add(cell);
            }
        }
    }
}
