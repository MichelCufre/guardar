using ClosedXML.Excel;
using System.Collections.Generic;

namespace WIS.GridComponent.Excel
{
    public interface IGridExcelBuildService
    {
        byte[] Build<T>(string name, XLWorkbook package, List<IGridColumn> columns, IList<T> data);
    }
}
