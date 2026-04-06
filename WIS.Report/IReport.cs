using System;

namespace WIS.Report
{
    public interface IReport
    {
        void Build();
        ReportContent Render();
    }
}
