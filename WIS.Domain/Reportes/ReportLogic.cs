using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Impresiones;
using WIS.Domain.Reportes.Dtos;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Reportes
{
    public class ReportLogic
    {
        protected readonly IDapper _dapper;
        protected readonly IUnitOfWork _uow;
        protected readonly ILogger<ReportLogic> _logger;
        protected readonly IPrintingService _printingService;

        public ReportLogic(IUnitOfWork uow,
            IDapper dapper,
            ILogger<ReportLogic> logger,
            IPrintingService printingService)
        {
            _uow = uow;
            _logger = logger;
            _dapper = dapper;
            _printingService = printingService;
        }

        public virtual Reporte GenerateReport(long reportId)
        {
            var report = _uow.ReporteRepository.GetReporte(reportId);

            if (report.Estado == CReporte.Pendiente || report.Estado == CReporte.PendienteReprocesar)
            {
                var reportSetup = report.GetTipoReporte(_uow, report.Tipo);
                var reportRelation = report.RelacionEntidad.FirstOrDefault();

                if (reportRelation != null)
                {
                    var relation = new DtoReporteRelacion
                    {
                        IdReporteRelacion = reportRelation.Id,
                        Id = reportId,
                        Clave = reportRelation.Clave,
                        Tabla = reportRelation.Tabla,
                        Tipo = report.Tipo,
                        Usuario = report.Usuario,
                        FechaAlta = report.FechaAlta,
                        NombreArchivo = report.NombreArchivo,
                        Estado = report.Estado,
                        FechaModificacion = report.FechaModificacion,
                        Predio = report.Predio,
                        Zona = report.Zona,
                    };

                    using (var connection = this._dapper.GetDbConnection())
                    {
                        reportSetup.GenerarReporte(_uow, connection, _logger, relation);
                    }

                    report.NombreArchivo = relation.NombreArchivo;
                    report.Contenido = relation.Contenido;
                    report.Estado = relation.Estado;
                    report.FechaModificacion = report.FechaModificacion;
                    report.Zona = relation.Zona;

                    _uow.ReporteRepository.UpdateReporte(report);
                    _uow.SaveChanges();
                }
            }

            return report;
        }

        public virtual void ProcessPending()
        {
            //Procesar Recepción
            this.ProcessRecepcion();

            //Procesar Expedición
            this.ProcessExpedicion();
        }

        public virtual void ProcessRecepcion()
        {
            try
            {
                ProcessReport(CReporte.TablaReporteAgenda);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, $"ProcessRecepcion");
            }
        }

        public virtual void ProcessExpedicion()
        {
            try
            {
                ProcessReport(CReporte.TablaReporteCamion);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, $"ProcessExpedicion");
            }
        }

        protected virtual void ProcessReport(string tablaReporte)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                var relaciones = _uow.ReporteRepository.GetReportesPendientes(connection, tablaReporte);

                foreach (var relacion in relaciones)
                {
                    var report = new Reporte();
                    var reportSetup = report.GetTipoReporte(_uow, relacion.Tipo);

                    if (reportSetup != null)
                        reportSetup.GenerarReporte(_uow, connection, _logger, relacion);

                    _uow.ReporteRepository.UpdateReporte(connection, relacion);
                    _uow.SaveChanges();
                }
            }
        }

        public virtual void SendToPrintProcessed()
        {
            var reportesImprimir = new List<string>
            {
                CReporte.PACKING_LIST,
                CReporte.NOTA_DEVOLUCION,
                CReporte.CONFIRMACION_RECEPCION,
                CReporte.CONTENEDORES_CAMION,
                CReporte.CONTROL_CAMBIO,
                CReporte.PACKING_LIST_SIN_LPN,
            };

            var situacionesImprimir = new List<string>
            {
                CReporte.Procesado,
                CReporte.PendienteReimprimir,
                CReporte.PendienteReimprimir,
            };

            var reportesProcesados = _uow.ReporteRepository.GetReportePendienteImprimir(reportesImprimir, situacionesImprimir);

            foreach (var reporte in reportesProcesados)
            {
                this.SendReportToPrint(reporte);
            }

            _uow.SaveChanges();
        }

        public virtual void SendReportToPrint(Reporte reporte)
        {
            var nuImpresion = CreateReportPrint(reporte);

            _printingService.SendToPrint(_uow, nuImpresion);
        }

        public virtual async Task SendReportToPrintAsync(Reporte reporte)
        {
            var nuImpresion = CreateReportPrint(reporte);

            await _printingService.SendToPrintAsync(_uow, nuImpresion);
        }

        public virtual int CreateReportPrint(Reporte reporte)
        {
            int nuImpresion = -1;

            try
            {
                if (reporte.Contenido == null)
                    throw new Exception("No existen datos para procesar");

                if (string.IsNullOrEmpty(reporte.NombreArchivo))
                    throw new Exception("Nombre de archivo no definido");

                string paramName = string.Empty;

                switch (reporte.Tipo)
                {
                    case CReporte.PACKING_LIST:
                        paramName = ParamManager.REPIMP_PACKING_LIST;
                        break;
                    case CReporte.CONTENEDORES_CAMION:
                        paramName = ParamManager.REPIMP_CONTENEDORES_CAMION;
                        break;
                    case CReporte.CONTROL_CAMBIO:
                        paramName = ParamManager.REPIMP_CONTROL_CAMBIO;
                        break;
                    case CReporte.CONFIRMACION_RECEPCION:
                        paramName = ParamManager.REPIMP_CONFIRMACION_RECEPCION;
                        break;
                    case CReporte.NOTA_DEVOLUCION:
                        paramName = ParamManager.REPIMP_NOTA_DEVOLUCION;
                        break;
                    case CReporte.PACKING_LIST_SIN_LPN:
                        paramName = ParamManager.REPIMP_PACKING_LIST_SIN_LPN;
                        break;
                }

                var parametros = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(reporte.Zona))
                {
                    var zona = _uow.ZonaUbicacionRepository.GetZona(reporte.Zona);
                    parametros[ParamManager.PARAM_ZONA] = string.Format("{0}_{1}", ParamManager.PARAM_ZONA, zona.IdInterno);
                }

                var printerName = _uow.ParametroRepository.GetParameter(paramName, parametros);

                if (string.IsNullOrEmpty(printerName))
                    throw new Exception("Impresora no especificada");

                var impresion = new Impresion();
                var detalle = new DetalleImpresion();

                impresion.Id = _uow.ImpresionRepository.GetSecuenciaImpresion();
                impresion.Usuario = 0;
                impresion.Predio = reporte.Predio;
                impresion.CodigoImpresora = printerName;
                impresion.NombreImpresora = printerName;
                impresion.Referencia = $"Impresión de reporte: {reporte.Id}";
                impresion.Generado = DateTime.Now;
                impresion.Estado = _printingService.GetEstadoInicial();
                impresion.CantRegistros = 1;

                _uow.ImpresionRepository.AddImpresion(impresion);
                _uow.SaveChanges();

                nuImpresion = impresion.Id;

                detalle.NumeroImpresion = impresion.Id;
                detalle.Registro = 1;
                detalle.Contenido = "reporte:///" + reporte.Id;
                detalle.Estado = _printingService.GetEstadoInicial();
                detalle.FechaProcesado = DateTime.Now;

                _uow.ImpresionRepository.AddDetalleImpresion(detalle);

                reporte.Estado = CReporte.EnviadoImpresion;
                reporte.FechaModificacion = DateTime.Now;
            }
            catch (Exception ex)
            {
                reporte.Estado = CReporte.ErrorImpresion;
                reporte.FechaModificacion = DateTime.Now;

                _logger.LogDebug(ex, $"SendReportToPrint. Reporte: " + reporte.Id);
            }

            _uow.ReporteRepository.UpdateReporte(reporte);
            _uow.SaveChanges();

            return nuImpresion;
        }
    }
}
