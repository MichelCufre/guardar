using System.Collections.Generic;
using System.Linq;
using WIS.GridComponent.Items;

namespace WIS.GridComponent
{
    public class Grid
    {
        public string Id { get; set; }
        public List<IGridColumn> Columns { get; set; }
        public List<GridRow> Rows { get; set; }
        public List<IGridItem> MenuItems { get; set; }

        public Grid()
        {
            this.Columns = new List<IGridColumn>();
            this.Rows = new List<GridRow>();
            this.MenuItems = new List<IGridItem>();
        }
        public Grid(string id) : this()
        {
            this.Id = id;
        }

        public GridRow AddRow()
        {
            return AddEditableRow(false);
        }

        public GridRow AddEditableRow(bool isEditable)
        {
            var row = new GridRow();

            this.Rows.Add(row);

            foreach (var column in this.Columns)
            {
                row.Cells.Add(new GridCell
                {
                    Column = column,
                    Editable = isEditable
                });
            }

            return row;
        }

        public IGridColumn GetColumn(string columnId)
        {
            return this.Columns.Where(d => d.Id == columnId).FirstOrDefault();
        }

        public List<GridRow> GetDeletedRows()
        {
            return this.Rows.Where(d => d.IsDeleted).ToList();
        }

        public void AddOrUpdateColumn(IGridColumn column)
        {
            var existingColumn = this.Columns.Where(d => d.Id == column.Id).FirstOrDefault();

            if (existingColumn != null)
            {
                existingColumn.UpdateSpecificValues(column);
            }
            else
            {
                column.IsNew = true;

                this.Columns.Add(column);
            }
        }

        public void SetColumnDefaultValues(Dictionary<string, string> values)
        {
            foreach (var column in this.Columns)
            {
                if (values.ContainsKey(column.Id))
                    column.DefaultValue = values[column.Id];
            }
        }

        public void SetInsertableColumns(List<string> insertableColumns)
        {
            foreach (var column in this.Columns)
            {
                column.Insertable = insertableColumns.Any(d => d == column.Id);
            }
        }

        public void SetEditableCells(List<string> editableColumns)
        {
            foreach (var row in this.Rows)
            {
                row.SetEditableCells(editableColumns);
            }
        }

        public bool HasNewDuplicates(List<string> keys)
        {
            var cache = new List<string>();

            foreach (var row in this.Rows.Where(d => d.IsNew))
            {
                var values = new List<string>();

                foreach (var key in keys)
                {
                    values.Add(row.GetCell(key).Value);
                }

                var value = string.Join("~", values);

                if (cache.Any(d => d == value))
                    return true;

                cache.Add(value);
            }

            return false;
        }

        public bool HasDuplicates(List<string> keys)
        {
            var cache = new List<string>();

            foreach (var row in this.Rows.Where(d => d.Cells.Any(c => c.Modified)))
            {
                var values = new List<string>();

                foreach (var key in keys)
                {
                    values.Add(row.GetCell(key).Value);
                }

                var value = string.Join("~", values);

                if (cache.Any(d => d == value))
                    return true;

                cache.Add(value);
            }

            return false;
        }
    }
}
