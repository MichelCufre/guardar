using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Components.Common;
using WIS.Validation;

namespace WIS.GridComponent.Validation
{
    public class GridValidator
    {
        private readonly List<ComponentParameter> _parameters;

        public GridValidationSchema Schema { get; set; }
        public List<GridValidationGroup> Groups { get; set; }

        public GridValidator(List<ComponentParameter> parameters)
        {
            this._parameters = parameters;
            this.Schema = new GridValidationSchema();
            this.Groups = new List<GridValidationGroup>();
        }

        public void Validate(GridRow row)
        {
            if (this.Schema.Count == 0)
                return;

            foreach (var cell in row.Cells.Where(d => d.ShouldValidate()))
            {
                this.SetValidationRules(cell.Column.Id, row);
            }

            foreach (var validation in this.Groups)
            {
                ValidationErrorGroup result = validation.Validate(row, this._parameters);

                GridCell cell = row.GetCell(validation.ColumnId);

                if (result.IsValid)
                {
                    cell.SetOk();
                }
                else
                {
                    ValidationError error = result.GetMessage();

                    cell.SetError(error.Message, error.Arguments);

                    if (validation.BreakValidationChain)
                        break;
                }
            }
        }

        private void SetValidationRules(string columnId, GridRow row)
        {
            if (this.Groups.Any(d => d.ColumnId == columnId))
                return;

            var cell = row.GetCell(columnId);

            if (cell != null && this.Schema.ContainsKey(columnId))
            {
                var result = this.Schema[columnId](cell, row, this._parameters);

                if (result == null)
                    return;

                result.ColumnId = columnId;

                foreach (var dependency in result.Dependencies)
                {
                    this.SetValidationRules(dependency, row);
                }

                this.Groups.Add(result);
            }
        }
    }
}
