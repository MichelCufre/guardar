using System;
using System.Collections.Generic;
using System.Text;
using WIS.Serialization;

namespace WIS.Report.Execution.Serialization
{
    public interface IReportWrapper : ITransferWrapper
    {
        string ReportId { get; set; }
    }
}
