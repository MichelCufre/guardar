using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Report
{
    public class ReportContent
    {
        public string Name { get; set; }
        public string Data { get; set; }
        public ReportStatus Status { get; set; }
        public string Message { get; set; }

        public ReportContent()
        {
            this.Status = ReportStatus.Ok;
        }
    }
}
