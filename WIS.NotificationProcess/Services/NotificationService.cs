using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartFormat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Eventos;
using WIS.Domain.General;
using WIS.NotificationProcess.Models;

namespace WIS.NotificationProcess.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly MailSettings _mailSettings;
        private readonly ILogger _logger;

        public NotificationService(IUnitOfWorkFactory uowFactory,
            IOptions<MailSettings> mailSettings,
            ILogger<INotificationService> logger)
        {
            this._uowFactory = uowFactory;
            this._mailSettings = mailSettings.Value;
            this._logger = logger;
        }

        public async Task Run()
        {
            try
            {
                _logger.LogInformation("Application Started");

                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    ProcessScheduledEvents(uow);
                    await NotifyPendingEmails(uow);
                }

                _logger.LogInformation("Application Ended");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Application Error");
            }
        }

        protected virtual void ProcessScheduledEvents(IUnitOfWork uow)
        {
            var instancias = uow.EventoRepository.GetInstanciasProgramadasHabilitadas();

            foreach (var instancia in instancias)
            {
                switch (instancia.NombreEvento)
                {
                    case EventoDb.ProductosAVencerse:
                        ProcessProductosAVencerse(uow, instancia);
                        break;
                }
            }
        }

        protected virtual void ProcessProductosAVencerse(IUnitOfWork uow, Instancia instancia)
        {
            try
            {
                var nuevaEjecucionInstancia = false;
                var ejecucionInstancia = uow.EventoRepository.GetEjecucionInstancia(instancia.Id);

                if (ejecucionInstancia == null)
                {
                    nuevaEjecucionInstancia = true;

                    ejecucionInstancia = new EjecucionInstancia();
                    ejecucionInstancia.NumeroInstancia = instancia.Id;
                }

                var periodicidad = int.Parse(instancia.Parametros.First(x => x.Codigo == EventoParametroDb.QT_PERIODICIDAD).Valor);
                var hoy = DateTime.Today;
                var proximaEjecucion = ejecucionInstancia.FechaUltimaEjecucion?.AddDays(periodicidad);

                if (proximaEjecucion == null || hoy >= proximaEjecucion)
                {
                    var empresa = int.Parse(instancia.Parametros.First(x => x.Codigo == EventoParametroDb.CD_EMPRESA).Valor);
                    var ventana = int.Parse(instancia.Parametros.First(x => x.Codigo == EventoParametroDb.QT_VENTANA).Valor);
                    var predio = instancia.Parametros.FirstOrDefault(x => x.Codigo == EventoParametroDb.NU_PREDIO)?.Valor;
                    var stock = uow.StockRepository.GetStockProximoAVencer(empresa, hoy.AddDays(ventana), predio);

                    if (stock.Count > 0)
                    {
                        var fileName = $"ProductosAVencerse_{hoy.ToString("yyyy-MM-dd")}.xls";
                        AddNotificacionArchivoAsExcel(uow, instancia, empresa, predio, hoy.AddDays(ventana), stock, fileName);
                    }

                    ejecucionInstancia.FechaUltimaEjecucion = hoy;

                    if (nuevaEjecucionInstancia)
                    {
                        uow.EventoRepository.AddEjecucionInstancia(ejecucionInstancia);
                    }
                    else
                    {
                        uow.EventoRepository.UpdateEjecucionInstancia(ejecucionInstancia);
                    }

                    uow.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al procesar la instancia {instancia.Id}");
            }
        }

        protected virtual void AddNotificacionArchivoAsExcel<T>(IUnitOfWork uow, Instancia instancia, int cdEmpresa, string nuPredio, DateTime fecha, IEnumerable<T> data, string fileName)
        {
            var content = GetExcelContent(data);
            var template = uow.EventoRepository.GetEventoTemplate(instancia.NumeroEvento, instancia.IdTipoNotificacion, instancia.Plantilla);
            var contactos = uow.DestinatarioRepository.GetContactos(instancia.Id);
            var empresa = uow.EmpresaRepository.GetEmpresa(cdEmpresa);
            var cuerpo = Smart.Format(template.Cuerpo, GetParametrosPlantillaNotificacion(uow, empresa, nuPredio, fecha));

            foreach (var contacto in contactos)
            {
                uow.NotificacionRepository.AddNotificacionEmail(new NotificacionEmail
                {
                    Asunto = template.Asunto,
                    Cuerpo = cuerpo,
                    EmailRecibe = contacto.Email,
                    Estado = EstadoNotificacion.EST_PEND,
                    FechaCreacion = DateTime.Now,
                    FechaEnvio = DateTime.Now,
                    IsHtml = template.IsHtml,
                    NumeroInstancia = instancia.Id,
                    Archivos = new List<NotificacionArchivo>()
                    {
                        new NotificacionArchivo()
                        {
                            TpReferencia = EventoArchivoTipoReferenciaDb.NOTIFICACION,
                            VlData = content,
                            DsArchivo = fileName,
                            DtAddRow = DateTime.Now,
                            DtUpdateRow = DateTime.Now,
                        }
                    }
                });
            }
        }

        public virtual object GetParametrosPlantillaNotificacion(IUnitOfWork uow, Empresa empresa, string predio, DateTime fecha)
        {
            return new
            {
                empresa = empresa.Nombre + $" [{empresa.Id}]",
                predio = !string.IsNullOrEmpty(predio) ? $" (predio {predio}) " : predio,
                fecha = fecha.ToString("yyyy-MM-dd"),
            };
        }

        private static byte[] GetExcelContent<T>(IEnumerable<T> data)
        {
            var content = new byte[0];

            using (var wb = new XLWorkbook())
            using (var stream = new MemoryStream())
            {
                var ws = wb.AddWorksheet();

                ws.Cell("A1").InsertTable(data);
                ws.Columns().AdjustToContents();
                wb.SaveAs(stream);

                return stream.ToArray();
            }
        }

        protected virtual async Task NotifyPendingEmails(IUnitOfWork uow)
        {
            var notificaciones = uow.NotificacionRepository.GetNotificacionesEmailPendientes();
            var senderName = _mailSettings.SenderName;
            var senderAddress = _mailSettings.SenderAddress;
            var username = _mailSettings.SmtpUser;
            var password = _mailSettings.SmtpPassword;
            var host = _mailSettings.SmtpAddress;
            var port = _mailSettings.SmtpPort;

            foreach (var notificacion in notificaciones)
            {
                if (notificacion.Instancia.EsHabilitado)
                {
                    try
                    {
                        var from = new MailAddress(senderAddress, senderName);
                        var to = new MailAddress(notificacion.EmailRecibe);

                        using (var message = new MailMessage(from, to))
                        {
                            message.Subject = notificacion.Asunto;
                            message.Body = notificacion.Cuerpo;

                            if (notificacion.IsHtml.HasValue)
                                message.IsBodyHtml = notificacion.IsHtml.Value;

                            foreach (var archivo in notificacion.Archivos)
                            {
                                message.Attachments.Add(await GetNewAttachment(notificacion, archivo, uow));
                            }

                            SmtpClient server;

                            if (port.HasValue)
                                server = new SmtpClient(host, port.Value);
                            else
                                server = new SmtpClient(host);

                            using (server)
                            {
                                server.EnableSsl = _mailSettings.EnableSsl;
                                server.DeliveryMethod = SmtpDeliveryMethod.Network;

                                if (_mailSettings.UseDefaultCredentials)
                                    server.UseDefaultCredentials = _mailSettings.UseDefaultCredentials;
                                else
                                    server.Credentials = new NetworkCredential(username, password);

                                server.Send(message);
                            }
                        }

                        _logger.LogTrace("Notification {notificationId} was successfully sent", notificacion.Id);

                        notificacion.Estado = EstadoNotificacion.EST_FIN_CORRECTO;
                        notificacion.FechaEnvio = DateTime.Now;

                        uow.NotificacionRepository.UpdateNotificacionEmail(notificacion);
                    }
                    catch (Exception ex)
                    {
                        notificacion.Estado = EstadoNotificacion.EST_CON_ERRORES;
                        uow.NotificacionRepository.UpdateNotificacionEmail(notificacion);
                        _logger.LogError(ex, "Error while sending notification {notificationId}", notificacion.Id);
                    }

                    uow.SaveChanges();
                }
            }
        }

        private async Task<Attachment> GetNewAttachment(NotificacionEmail notificacion, NotificacionArchivo archivo, IUnitOfWork uow)
        {
            var fileName = archivo.DsArchivo;
            var idReferencia = archivo.IdReferencia;
            var data = new byte[0];

            switch (archivo.TpReferencia)
            {
                case EventoArchivoTipoReferenciaDb.INTERFAZ:
                    data = await GetInterfazData(long.Parse(idReferencia), uow);
                    return new Attachment(new MemoryStream(data), fileName);
                case EventoArchivoTipoReferenciaDb.REPORTE:
                    data = GetReporteData(long.Parse(idReferencia), uow);
                    return new Attachment(new MemoryStream(data), fileName);
                case EventoArchivoTipoReferenciaDb.NOTIFICACION:
                    data = GetNotificacionData(archivo.Id, notificacion.Id, uow);
                    return new Attachment(new MemoryStream(data), fileName);
                default:
                    return new Attachment(fileName);
            }
        }

        protected virtual async Task<byte[]> GetInterfazData(long nroEjecucion, IUnitOfWork uow)
        {
            return (await uow.EjecucionRepository.GetEjecucionData(nroEjecucion)).Data;
        }

        protected virtual byte[] GetReporteData(long nuReporte, IUnitOfWork uow)
        {
            var reporte = uow.ReporteRepository.GetReporte(nuReporte);
            return reporte.Contenido;
        }

        protected virtual byte[] GetNotificacionData(int nuNotificacionArchivo, long nuNotificacion, IUnitOfWork uow)
        {
            return uow.NotificacionRepository.GetNotificacionArchivoData(nuNotificacionArchivo, nuNotificacion)?.VlData;
        }
    }
}
