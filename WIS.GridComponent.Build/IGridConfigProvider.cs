using System;
using System.Collections.Generic;
using System.Text;
using WIS.GridComponent.Build;

namespace WIS.GridComponent.Build
{
    public interface IGridConfigProvider
    {
        List<IGridColumn> GetColumns(string application, int userId, string gridId);
        List<IGridColumn> GetColumnsFromEntity<T>(List<IGridColumn> columnsSource);
        void SaveColumns(Grid grid, string application, int UserId);
        GridFilterData GetFilterData(string application, int userId, string gridId);
        int GetSelectResultLimit();
        Dictionary<string, List<IGridColumn>> GetApiColumns(int interfazExterna, out string filename);
        bool IsModoConsulta(int userId);
        bool IsPantallaModoConsulta(int userid, string pagina);
    }
}
