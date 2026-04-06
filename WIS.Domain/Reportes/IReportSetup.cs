using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data.Common;
using WIS.Domain.DataModel;
using WIS.Domain.Reportes.Dtos;

namespace WIS.Domain.Reportes
{
    public interface IReportSetup
    {
        List<long> Preparar();
        void GenerarReporte(IUnitOfWork uow, DbConnection connection, ILogger<ReportLogic> logger, DtoReporteRelacion reporteRelacion);
    }
}
