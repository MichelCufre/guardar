using System;
using System.Collections.Generic;
using System.Text;
using WIS.Serialization;

namespace WIS.GridComponent.Execution.Serialization
{
    public interface IGridWrapper : ITransferWrapper
    {
        GridAction Action { get; set; }
        string GridId { get; set; }
    }
}
