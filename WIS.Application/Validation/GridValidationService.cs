using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Validation;

namespace WIS.Application.Validation
{
    public class GridValidationService : IGridValidationService
    {
        public GridValidationService()
        {
        }

        public Grid Validate(IGridValidationModule module, Grid grid, GridRow row, GridValidationContext context)
        {            
            var validator = new GridValidator(context.Parameters);

            module.Validator = validator;

            module.Validate(row);

            return grid;
        }
    }
}
