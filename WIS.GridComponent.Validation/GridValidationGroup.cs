using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;
using WIS.Validation;

namespace WIS.GridComponent.Validation
{
    public class GridValidationGroup
    {
        public string ColumnId { get; set; }
        public List<string> Dependencies { get; set; }
        public List<IValidationRule> Rules { get; set; }
        public bool BreakValidationChain { get; set; }
        public Action<GridCell, GridRow, List<ComponentParameter>> OnSuccess { private get; set; }
        public Action<GridCell, GridRow, List<ComponentParameter>> OnFailure { private get; set; }

        public GridValidationGroup()
        {
            this.Rules = new List<IValidationRule>();
            this.Dependencies = new List<string>();
            this.BreakValidationChain = false;
        }

        public ValidationErrorGroup Validate(GridRow row, List<ComponentParameter> parameters)
        {
            var errorGroup = new ValidationErrorGroup();

            GridCell cell = row.GetCell(this.ColumnId);

            foreach (var rule in this.Rules)
            {
                var result = rule.Validate();

                if (result.Count > 0)
                {
                    //Rompe la cadena de validación si una de las validaciones falla
                    errorGroup.AddErrors(result);
                    break;
                }
            }

            if (errorGroup.IsValid && this.OnSuccess != null)
                this.OnSuccess(cell, row, parameters);

            if (!errorGroup.IsValid && this.OnFailure != null)
                this.OnFailure(cell, row, parameters);

            return errorGroup;
        }
    }
}
