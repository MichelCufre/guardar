using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Report;

namespace WIS.Domain.Services.Interfaces
{
    public interface IReportGeneratorService
    {
        Task GeneratePendingReport(long reportId);
        void GeneratePendingReports();
        List<long> GetPendingReports();
    }
}
