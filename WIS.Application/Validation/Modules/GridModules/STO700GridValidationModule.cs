using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.GridComponent;
using WIS.GridComponent.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class STO700GridValidationModule : GridValidationModule
    {
        public STO700GridValidationModule()
        {
            this.Schema = new GridValidationSchema
            {
                ["ID_PACKING"] = this.ValidateIdPacking,
            };
        }

        public virtual GridValidationGroup ValidateIdPacking(GridCell cell, GridRow row, List<ComponentParameter> list)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = {
                    new StringMaxLengthValidationRule(cell.Value, 50),
                }
            };
        }
    }
}

