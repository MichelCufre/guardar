using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Extensions.Logging;
using SmartFormat;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Eventos;
using WIS.Domain.General;
using WIS.Domain.ManejoStock.Constants;
using WIS.Domain.Recepcion;
using WIS.Domain.Reportes.Dtos;
using WIS.Extension;
using WIS.Report;
using WIS.Translation;

namespace WIS.Domain.Reportes.Especificaciones
{
    public class ConfirmacionRecepcionReportSetup : IReportSetup
    {
        protected string _backupPath;
        protected readonly int _usuario;
        protected readonly int _idAgenda;
        protected readonly string _predio;
        protected readonly string _aplicacion;

        protected readonly string _tabla = CReporte.TablaReporteAgenda;
        protected readonly string _tipoReporte = CReporte.CONFIRMACION_RECEPCION;

        protected readonly IUnitOfWork _uow;

        public ConfirmacionRecepcionReportSetup(IUnitOfWork uow, int usuario, string aplicacion, string predio, int id)
        {
            this._uow = uow;
            this._usuario = usuario;
            this._aplicacion = aplicacion;
            this._predio = predio;
            this._idAgenda = id;
        }

        public ConfirmacionRecepcionReportSetup(IUnitOfWork uow)
        {
            this._backupPath = uow.ParametroRepository.GetParameter(ParamManager.REPORTE_BACKUP_PATH);

            if (!Directory.Exists(this._backupPath))
                Directory.CreateDirectory(this._backupPath);
        }

        public virtual void GenerarReporte(IUnitOfWork uow, DbConnection connection, ILogger<ReportLogic> logger, DtoReporteRelacion relacion)
        {
            try
            {
                string situacionReporte;
                var tmpClave = Reporte.SplitCompositeId(relacion.Clave);

                if (tmpClave != null && tmpClave.Any())
                {
                    var nroAgenda = Convert.ToInt32(tmpClave[0]);
                    var agenda = uow.AgendaRepository.GetAgenda(nroAgenda);
                    var cdEmpresa = agenda.IdEmpresa;
                    var cdCliente = agenda.CodigoInternoCliente;
                    var userLanguage = uow.SecurityRepository.GetUserLanguage(relacion.Usuario ?? 0);

                    Generar(uow, connection, nroAgenda, userLanguage, out byte[] fileData, out string fileName);

                    if (fileData != null)
                        situacionReporte = (relacion.Estado == CReporte.PendienteReprocesar) ? CReporte.Reprocesado : CReporte.Procesado;
                    else
                        situacionReporte = CReporte.Anulado;

                    relacion.NombreArchivo = fileName;
                    relacion.Contenido = fileData;
                    relacion.Estado = situacionReporte;
                    relacion.FechaModificacion = DateTime.Now;

                    if (string.IsNullOrEmpty(relacion.Zona))
                    {
                        var puerta = GetPuertaEmbarque(uow, agenda);
                        var zonaPuerta = uow.UbicacionRepository.GetUbicacion(puerta.CodigoUbicacion).IdUbicacionZona;
                        relacion.Zona = zonaPuerta;
                    }

                    Notificar(uow, relacion.Id, nroAgenda, cdEmpresa, cdCliente, userLanguage, fileData, fileName);
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, $"GenerarReporteConfirmacionRecepcion - Clave: " + relacion.Clave);

                relacion.Estado = CReporte.Error;
                relacion.FechaModificacion = DateTime.Now;
            }
        }

        public virtual List<long> Preparar()
        {
            var reports = new List<long>();

            if (!_uow.ReporteRepository.AnyReporte(this.GetCompositeId(), _tabla, _tipoReporte))
            {
                var agenda = _uow.AgendaRepository.GetAgenda(_idAgenda);
                var puerta = GetPuertaEmbarque(_uow, agenda);
                var zonaPuerta = _uow.UbicacionRepository.GetUbicacion(puerta.CodigoUbicacion).IdUbicacionZona;
                var prepararReporte = new PrepararReporte(_uow, _usuario, _aplicacion, _predio, zonaPuerta);
                var reportId = prepararReporte.CrearReporte(_tabla, _tipoReporte, string.Empty, this.GetCompositeId());

                if (reportId != -1)
                {
                    reports.Add(reportId);
                }
            }

            return reports;
        }

        protected virtual PuertaEmbarque GetPuertaEmbarque(IUnitOfWork uow, Agenda agenda)
        {
            PuertaEmbarque puerta = null;

            if (agenda.CodigoPuerta.HasValue)
            {
                puerta = uow.PuertaEmbarqueRepository.GetPuertaEmbarque(agenda.CodigoPuerta.Value);
            }
            else
            {
                var log = uow.EtiquetaLoteRepository.GetFirstLogEtiqueta(agenda.Id, TiposMovimiento.Recepcion);
                puerta = uow.PuertaEmbarqueRepository.GetPuertaEmbarqueByUbicacion(log.Ubicacion);
            }

            return puerta;
        }

        public virtual string GetCompositeId()
        {
            return $"{_idAgenda}";
        }

        public virtual void Generar(IUnitOfWork uow, DbConnection connection, int nroAgenda, string language, out byte[] fileData, out string fileName)
        {
            fileData = null;
            fileName = string.Empty;
            bool reporteConLogo = true;

            string resourcePath = uow.ParametroRepository.GetParameter(ParamManager.REPORTE_RESOURCE_PATH);
            string resourceLogo = uow.ParametroRepository.GetParameter(ParamManager.REPORTE_RESOURCE_LOGO);
            string logoPath = resourcePath + resourceLogo;

            if (!System.IO.File.Exists(logoPath))
                reporteConLogo = false;

            var resourceList = uow.LocalizationRepository.GetTranslation(new List<string>
            {
                 "WCOF070_ReporteCR_lbl_NU_AGENDA",
                "WCOF070_ReporteCR_lbl_TP_RECEPCION_EXTERNO",
                "WCOF070_ReporteCR_lbl_CD_EMPRESA",
                "WCOF070_ReporteCR_lbl_CD_AGENTE",
                "WCOF070_ReporteCR_lbl_DT_ADDROW",
                "WCOF070_ReporteCR_lbl_DT_CIERRE",
                "WCOF070_ReporteCR_lbl_Pagina",
                "WCOF070_ReporteCR_lbl_SeparadorPagina",
                "WCOF070_ReporteCR_lbl_CD_PRODUTO",
                "WCOF070_ReporteCR_lbl_DS_PRODUTO",
                "WCOF070_ReporteCR_lbl_NU_IDENTIFICADOR",
                "WCOF070_ReporteCR_lbl_QT_AGENDADO",
                "WCOF070_ReporteCR_lbl_QT_RECIBIDA",
                "WCOF070_ReporteCR_lbl_Title"
            }, language);


            var agenda = uow.ReporteRepository.GetAgendaInfoReporte(nroAgenda);
            var detalleAgenda = uow.ReporteRepository.GetDetallesAgendaReporte(nroAgenda);
            if (detalleAgenda.Count > 0)
            {
                fileName = string.Join("_", new string[]
                {
                    "ConfRecepcion",
                    Convert.ToString(agenda.Id),
                    DateTime.Now.ToString(GeneralDb.DATETIME_FILENAME_OK)
                });

                fileName = fileName.Replace("/", "-");
                fileName += ".pdf";

                ReportAtl report = new ReportAtl();

                PdfWriter writer = new PdfWriter(this._backupPath + fileName);
                PdfDocument pdf = new PdfDocument(writer);
                var footerEventHandler = new PageEventHandler();

                pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, footerEventHandler);

                Document doc = new Document(pdf);

                this.CreateHeader(doc, resourceList, new DtoReporteConfRecepcion
                {
                    Id = agenda.Id,
                    Empresa = agenda.Empresa,
                    NombreEmpresa = agenda.NombreEmpresa,
                    Agente = agenda.Agente,
                    TipoAgente = agenda.TipoAgente,
                    DescripcionCliente = agenda.DescripcionCliente,
                    TipoRecepcion = agenda.TipoRecepcion,
                    TipoRecepcionExterno = agenda.TipoRecepcionExterno,
                    DescripcionRecepcionExterno = agenda.DescripcionRecepcionExterno,
                    Anexo1 = agenda.Anexo1,
                    Anexo2 = agenda.Anexo2,
                    FechaCierre = agenda.FechaCierre,
                    FechaAlta = agenda.FechaAlta //DateTime.Now.ToString(Formats.DATE_ddMMyyyy)
                }, resourcePath + resourceLogo, reporteConLogo);

                CreateContent(doc, resourceList, detalleAgenda, logoPath);

                footerEventHandler.WriteTotalPages(pdf);

                doc.Close();
                var filePath = this._backupPath + fileName;
                fileData = System.IO.File.ReadAllBytes(filePath);
            }
        }

        public virtual void CreateContent(Document doc, List<TranslatedValue> resourceList, List<DtoReporteConfRecepcionDetalle> DetallesAgenda, string logoPath)
        {
            Table table = new Table(UnitValue.CreatePercentArray(5)).UseAllAvailableWidth().SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(0, 0, 255), 0).SetFontSize(9)
                .SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

            Cell head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteCR_lbl_CD_PRODUTO")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
            head.SetBorder(Border.NO_BORDER);
            head.SetBackgroundColor(ColorConstants.GRAY);

            table.AddHeaderCell(head);

            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteCR_lbl_DS_PRODUTO")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
            head.SetBorder(Border.NO_BORDER);
            head.SetBackgroundColor(ColorConstants.GRAY);

            table.AddHeaderCell(head);

            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteCR_lbl_NU_IDENTIFICADOR")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
            head.SetBorder(Border.NO_BORDER);
            head.SetBackgroundColor(ColorConstants.GRAY);

            table.AddHeaderCell(head);

            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteCR_lbl_QT_AGENDADO")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
            head.SetBorder(Border.NO_BORDER);
            head.SetBackgroundColor(ColorConstants.GRAY);

            table.AddHeaderCell(head);

            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteCR_lbl_QT_RECIBIDA")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
            head.SetBorder(Border.NO_BORDER);
            head.SetBackgroundColor(ColorConstants.GRAY);

            table.AddHeaderCell(head);

            foreach (var linea in DetallesAgenda)
            {
                Cell cell = new Cell();
                cell.Add(new Paragraph(linea.Producto));
                cell.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
                table.AddCell(cell);

                cell = new Cell();
                cell.Add(new Paragraph(linea.DescripcionProducto));
                cell.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                table.AddCell(cell);

                cell = new Cell();
                cell.Add(new Paragraph(linea.Lote));
                cell.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                table.AddCell(cell);

                head = new Cell();
                head.Add(new Paragraph(Convert.ToString(linea.CantidadAgendada)));
                head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                table.AddCell(head);

                head = new Cell();
                head.Add(new Paragraph(Convert.ToString(linea.CantidadRecibida)).SetTextAlignment(TextAlignment.RIGHT));
                head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                table.AddCell(head);
            }

            doc.Add(table);
        }

        public virtual void Notificar(IUnitOfWork uow, long nuReporte, int nroAgenda, int cdEmpresa, string cdCliente, string language, byte[] fileData, string fileName)
        {
            var empresa = uow.EmpresaRepository.GetEmpresa(cdEmpresa);
            var agente = uow.AgenteRepository.GetAgente(cdEmpresa, cdCliente);
            var agenda = uow.AgendaRepository.GetAgenda(nroAgenda);
            var reporte = uow.ReporteRepository.GetReporte(nuReporte);
            var instancias = uow.EventoRepository.GetInstanciasHabilitadas(EventoDb.ConfirmacionRecepcion, new Dictionary<string, string>
            {
                [EventoParametroDb.CD_EMPRESA] = cdEmpresa.ToString(),
                [EventoParametroDb.TP_AGENTE] = agente.Tipo,
                [EventoParametroDb.CD_AGENTE] = agente.Codigo,
                [EventoParametroDb.NU_PREDIO] = agenda.Predio,
            });

            foreach (var instancia in instancias)
            {
                var template = uow.EventoRepository.GetEventoTemplate(instancia.NumeroEvento, instancia.IdTipoNotificacion, instancia.Plantilla);
                var contactos = uow.DestinatarioRepository.GetContactos(instancia.Id);

                var predio = instancia.Parametros.FirstOrDefault(x => x.Codigo == EventoParametroDb.NU_PREDIO)?.Valor;
                var codigoAgente = instancia.Parametros.FirstOrDefault(x => x.Codigo == EventoParametroDb.CD_AGENTE)?.Valor;
                var tipoAgente = instancia.Parametros.FirstOrDefault(x => x.Codigo == EventoParametroDb.TP_AGENTE)?.Valor;
                var cuerpo = Smart.Format(template.Cuerpo, GetParametrosPlantillaNotificacion(uow, empresa, agenda, predio, codigoAgente, tipoAgente));                
                var asunto = Regex.Replace(template.Asunto, @"\{.*?\}", reporte.Id.ToString());

                foreach (var contacto in contactos)
                {
                    uow.NotificacionRepository.AddNotificacionEmail(new NotificacionEmail
                    {
                        Asunto = asunto,
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
                                IdReferencia = nuReporte.ToString(),
                                TpReferencia = EventoArchivoTipoReferenciaDb.REPORTE,
                                DsArchivo = reporte.NombreArchivo,
                                DtAddRow = DateTime.Now,
                                DtUpdateRow = DateTime.Now,
                            }
                        }
                    });
                }
            }
        }

        public virtual object GetParametrosPlantillaNotificacion(IUnitOfWork uow, Empresa empresa, Agenda agenda, string predio, string codigoAgente, string tipoAgente)
        {
            var listaOpcionales = new List<string>();

            if (!string.IsNullOrEmpty(predio))
            {
                listaOpcionales.Add($"predio: {predio}");
            }

            if (!string.IsNullOrEmpty(codigoAgente))
            {
                listaOpcionales.Add($"agente: {codigoAgente}");
            }

            if (!string.IsNullOrEmpty(tipoAgente))
            {
                listaOpcionales.Add($"tipo agente: {tipoAgente}");
            }

            string opcionales = string.Join(", ", listaOpcionales);

            return new
            {
                agenda = agenda.Id,
                empresa = empresa.Nombre + $" [{empresa.Id}]",
                opcionales = !string.IsNullOrEmpty(opcionales) ? $" ( {opcionales}) " : opcionales,
            };
        }

        public virtual void CreateHeader(Document doc, List<TranslatedValue> resourceList, DtoReporteConfRecepcion dtoReporteConfRecepcion, string filePath, bool reporteConLogo)
        {
            if (reporteConLogo)
            {
                ImageData myImageData = ImageDataFactory.Create(filePath, false);
                Image myImage = new Image(myImageData);

                myImage.SetFixedPosition(1, 370, 720);
                myImage.SetAutoScale(true);
                doc.Add(myImage);
            }

            Table table = new Table(2).SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(0, 0, 255), 0).SetFontSize(9)
               .SetBorder(new SolidBorder(ColorConstants.BLACK, 1)).SetWidth(250);

            #region Agenda
            Cell head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteCR_lbl_NU_AGENDA")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph(dtoReporteConfRecepcion.Id.ToString()).SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);
            #endregion

            #region Empresa
            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteCR_lbl_CD_EMPRESA")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph(dtoReporteConfRecepcion.Empresa.ToString() + " " + dtoReporteConfRecepcion.NombreEmpresa).SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);
            #endregion

            #region Agente
            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteCR_lbl_CD_AGENTE")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph(dtoReporteConfRecepcion.TipoAgente + " " + dtoReporteConfRecepcion.Agente + " " + dtoReporteConfRecepcion.DescripcionCliente).SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);
            #endregion

            #region Tipo de Recepcion
            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteCR_lbl_TP_RECEPCION_EXTERNO")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph(dtoReporteConfRecepcion.TipoRecepcionExterno ?? "" + " " + dtoReporteConfRecepcion.DescripcionRecepcionExterno).SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);
            #endregion

            #region Anexo1

            if (!string.IsNullOrEmpty(dtoReporteConfRecepcion.Anexo1))
            {
                head = new Cell();
                head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteCR_lbl_DS_ANEXO1")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
                head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
                head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                table.AddCell(head);

                head = new Cell();
                head.Add(new Paragraph(dtoReporteConfRecepcion.Anexo1 ?? "").SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
                head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
                head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                table.AddCell(head);
            }
            #endregion

            #region Anexo2
            if (!string.IsNullOrEmpty(dtoReporteConfRecepcion.Anexo2))
            {
                head = new Cell();
                head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteCR_lbl_DS_ANEXO2")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
                head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
                head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                table.AddCell(head);

                head = new Cell();
                head.Add(new Paragraph(dtoReporteConfRecepcion.Anexo2 ?? "").SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
                head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
                head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                table.AddCell(head);
            }
            #endregion

            #region Fecha cierre
            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteCR_lbl_DT_CIERRE")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph(dtoReporteConfRecepcion.FechaCierre.ToString(CDateFormats.DATE_ONLY)).SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);
            #endregion

            #region Fecha alta
            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteCR_lbl_DT_ADDROW")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph(dtoReporteConfRecepcion.FechaAlta.ToString(CDateFormats.DATE_ONLY)).SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);
            #endregion

            doc.Add(table);
            doc.Add(new Paragraph("\n"));
        }
    }
}
