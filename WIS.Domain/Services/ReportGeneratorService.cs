using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.Reportes;

namespace WIS.Domain.Services.Interfaces
{
    public class ReportGeneratorService : IReportGeneratorService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IDapper _dapper;
        protected readonly ILogger<ReportLogic> _logger;
        protected readonly IPrintingService _printingService;

        public ReportGeneratorService(IUnitOfWorkFactory uowFactory,
            IDapper dapper,
            ILogger<ReportLogic> logger,
            IPrintingService printingService)
        {
            _uowFactory = uowFactory;
            _dapper = dapper;
            _logger = logger;
            _printingService = printingService;
        }

        public virtual async Task GeneratePendingReport(long reportId)
        {
            try
            {
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    var logic = new ReportLogic(uow, _dapper, _logger, _printingService);
                    var report = logic.GenerateReport(reportId);
                    await logic.SendReportToPrintAsync(report);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GeneratePendingReport: {reportId}");
            }
        }

        public virtual void GeneratePendingReports()
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var logic = new ReportLogic(uow, _dapper, _logger, _printingService);

                logic.ProcessPending();
                logic.SendToPrintProcessed();
            }
        }

        public virtual List<long> GetPendingReports()
        {
            try
            {
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    return uow.ReporteRepository.GetReportesPendientes();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetPendingReports");
            }

            return new List<long>();
        }
    }
}
