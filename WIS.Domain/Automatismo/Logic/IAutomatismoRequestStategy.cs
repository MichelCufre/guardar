using System.Collections.Generic;
using WIS.GridComponent;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;

namespace WIS.Domain.Automatismo.Logic
{
    public interface IAutomatismoRequestStategy
    {
        T GetAutomatismoRequest<T>();

        byte[] CreateExcel(Grid grid, GridExportExcelContext context);

        List<GridRow> GenerateRowsAndLoadGrid(Grid grid, GridFetchContext context);

        void UpdateRequest(GridRow row, GridFetchContext context);

        GridStats FetchStats(GridFetchStatsContext context);
    }
}
