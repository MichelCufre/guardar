using System;
using System.Collections.Generic;
using System.Text;
using WIS.GridComponent.Build;
using WIS.GridComponent.Execution.Serialization;

namespace WIS.Application.GridConfiguration
{
    public interface IGridConfigService
    {
        void UpdateGridConfig(GridWrapper data);
        void SaveFilter(GridWrapper data);
        void RemoveFilter(GridWrapper data);
        List<GridFilterData> GetFilterList(GridWrapper data);
    }
}
