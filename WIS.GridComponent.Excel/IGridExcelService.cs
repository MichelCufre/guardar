using System;
using System.Collections.Generic;
using System.Text;
using WIS.Data;
using WIS.GridComponent.Excel.Configuration;
using WIS.Sorting;

namespace WIS.GridComponent.Excel
{
    public interface IGridExcelService
    {
        byte[] GetExcel<T, ContextDataType>(string name, IQueryObject<T, ContextDataType> query, List<IGridColumn> columns, GridExportExcelContext queryParameters, List<SortCommand> defaultSorting);
        byte[] GetExcel<T, ContextDataType>(string name, IQueryObject<T, ContextDataType> query, List<IGridColumn> columns, GridExportExcelContext queryParameters, SortCommand defaultSorting);
    }
}
