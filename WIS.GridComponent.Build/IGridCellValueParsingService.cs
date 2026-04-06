using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.GridComponent.Build
{
    public interface IGridCellValueParsingService
    {
        string GetValue(DateTime dateValue);
        string GetValue(DateTime? dateValue);
        string GetValue(object value);
    }
}
