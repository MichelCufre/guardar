using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using WIS.Application.Report;
using WIS.Exceptions;
using WIS.Report.Execution;
using WIS.Report.Execution.Serialization;

namespace WIS.BackendService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportDownloadService _reportDownloadService;
        private readonly ILogger<ReportController> _logger;

        public ReportController(IReportDownloadService reportDownloadService, ILogger<ReportController> logger)
        {
            this._reportDownloadService = reportDownloadService;
            this._logger = logger;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult GetReport(ReportWrapper wrapper)
        {
            ReportWrapper response = new ReportWrapper();

            using (MappedDiagnosticsLogicalContext.SetScoped("UserId", wrapper.User))
            using (MappedDiagnosticsLogicalContext.SetScoped("Application", wrapper.Application))
            {
                ReportRequest request = wrapper.GetData<ReportRequest>();

                try
                {
                    ReportContent reportContent = this._reportDownloadService.GetReport(wrapper.User, request);

                    response.SetData(reportContent);
                }
                catch (InvalidUserException ex)
                {
                    this._logger.LogError(ex, "Report - GetReport - Unauthenticated");
                    return Unauthorized();
                }
                catch (UserNotAllowedException ex)
                {
                    this._logger.LogError(ex, "Report - GetReport - Forbidden");
                    return Unauthorized();
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "Report - GetReport");
                    response.SetError(ex.Message);
                }

                return Ok(response);
            }
        }
    }
}
