using WIS.GridComponent;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Validation;

namespace WIS.Application.Validation
{
    public interface IGridValidationService
    {
        Grid Validate(IGridValidationModule module, Grid grid, GridRow row, GridValidationContext context);
    }
}