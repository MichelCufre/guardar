using System.Collections.Generic;
using WIS.Components.Common.Select;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;

namespace WIS.GridComponent.Execution
{
    public interface IGridController
    {
        Grid GridInitialize(Grid grid, GridInitializeContext parameters);
        Grid GridFetchRows(Grid grid, GridFetchContext context);
        GridStats GridFetchStats(Grid grid, GridFetchStatsContext context);
        Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context);
        Grid GridCommit(Grid grid, GridFetchContext context);
        byte[] GridExportExcel(Grid grid, GridExportExcelContext context);
        Grid GridImportExcel(Grid grid, GridImportExcelContext context);
        GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext selection);
        GridButtonActionContext GridButtonAction(GridButtonActionContext data);
        List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context);
        GridNotifySelectionContext GridNotifySelection(GridNotifySelectionContext context);
        GridNotifyInvertSelectionContext GridNotifyInvertSelection(GridNotifyInvertSelectionContext context);
        byte[] GridGenerateExcelTemplate(Grid grid, GridImportExcelContext context, int? interfazExterna = null);
        byte[] GridGenerateExcelTemplate(Dictionary<string, List<IGridColumn>> sheetColumns, GridImportExcelContext context, int? interfazExterna = null);

    }
}
