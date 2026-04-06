using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.GridComponent.Execution
{
    public enum GridAction
    {
        Unknown,
        Initialize,
        FetchRows,
        UpdateConfig,
        ValidateRow,
        Commit,
        ButtonAction,
        MenuItemAction,
        ExportExcel,
        ImportExcel,
        GenerateExcelTemplate,
        SelectSearch,
        SaveFilter,
        RemoveFilter,
        GetFilterList,
        NotifySelection,
        NotifyInvertSelection,
        FetchStats
    }
}
