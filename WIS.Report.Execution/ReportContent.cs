using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Report.Execution
{
    public class ReportContent
    {
        public string FileName { get; set; }
        public string Content { get; set; }

        public ReportContent()
        {

        }

        public ReportContent(string fileName, string content)
        {
            this.FileName = fileName;
            this.Content = content;
        }
    }
}
