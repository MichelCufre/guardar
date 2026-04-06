using WIS.Report.Execution;

namespace WIS.Application.Report
{
    public interface IReportDownloadService
    {
        ReportContent GetReport(int user, ReportRequest request);
    }
}
