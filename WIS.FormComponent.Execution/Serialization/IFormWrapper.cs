using System;
using System.Collections.Generic;
using System.Text;
using WIS.Serialization;

namespace WIS.FormComponent.Execution.Serialization
{
    public interface IFormWrapper : ITransferWrapper
    {
        FormAction Action { get; set; }
        string FormId { get; set; }
    }
}
