using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;

namespace WIS.GridComponent.Validation
{
    public class GridValidationSchema : Dictionary<string, Func<GridCell, GridRow, List<ComponentParameter>, GridValidationGroup>>
    {
    }
}
