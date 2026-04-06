using System;
using System.Collections.Generic;
using System.Text;
using WIS.Serialization;

namespace WIS.PageComponent.Execution.Serialization
{
    public interface IPageWrapper : ITransferWrapper
    {
        PageAction Action { get; set; }
    }
}
