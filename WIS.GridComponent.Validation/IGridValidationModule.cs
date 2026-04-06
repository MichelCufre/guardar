using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.GridComponent.Validation
{
    public interface IGridValidationModule
    {
        GridValidator Validator { set; }
        void Validate(GridRow row);
    }
}
