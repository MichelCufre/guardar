using System;
using WIS.Domain.DataModel;
using WIS.Domain.Reportes;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Report.Execution;

namespace WIS.Application.Report
{
    public class ReportDownloadService : IReportDownloadService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;

        public ReportDownloadService(IUnitOfWorkFactory uowFactory,
            IDapper dapper,
            IPrintingService printingService)
        {
            _uowFactory = uowFactory;
        }

        public virtual ReportContent GetReport(int user, ReportRequest request)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            //TODO: Comprobar si usuario puede imprimir reporte (en base a la empresa)

            Reporte reporte = uow.ReporteRepository.GetReporte(int.Parse(request.ReportId));

            if (reporte == null)
                throw new ValidationFailedException("Reporte no encontrado");

            string reporteContenido = string.Empty;

            if (reporte.Contenido != null)
                reporteContenido = Convert.ToBase64String(reporte.Contenido);

            return new ReportContent(reporte.NombreArchivo, reporteContenido);
        }
    }
}
