using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.GridComponent.Columns;

namespace WIS.GridComponent
{
    public class GridRow
    {
        public string Id { get; set; }
        public string CssClass { get; set; }
        public List<GridCell> Cells { get; set; }
        public List<string> DisabledButtons { get; set; }
        public bool IsNew { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsModified { get; set; }
        public bool DisabledSelected { get; set; }

        public GridRow()
        {
            this.Cells = new List<GridCell>();
            this.DisabledButtons = new List<string>();
            this.IsNew = false;
            this.IsDeleted = false;
            this.IsModified = false;
        }

        public void AddCell(GridCell cell)
        {
            this.Cells.Add(cell);
        }
        public GridCell GetCell(string column)
        {
            return this.Cells.Where(d => d.Column.Id == column).FirstOrDefault();
        }
        public void SetCellColumn(List<IGridColumn> columns)
        {
            foreach (var cell in this.Cells)
            {
                IGridColumn columUpdate = columns.Where(d => d.Id == cell.Column.Id).FirstOrDefault();
                if (columUpdate != null)
                    cell.Column = columUpdate;
            }
        }
        public void SetEditableCells(List<string> editableColumns)
        {
            foreach (var cell in this.Cells)
            {
                cell.Editable = editableColumns.Any(d => d == cell.Column.Id);
            }
        }
        public bool IsValid()
        {
            return !this.Cells.Any(c => !c.IsValid());
        }
    }
}
