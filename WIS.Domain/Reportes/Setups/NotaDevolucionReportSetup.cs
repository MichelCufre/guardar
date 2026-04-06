using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.ManejoStock.Constants;
using WIS.Domain.Recepcion;
using WIS.Domain.Reportes.Dtos;
using WIS.Extension;
using WIS.Report;
using WIS.Translation;

namespace WIS.Domain.Reportes.Especificaciones
{
    public class NotaDevolucionReportSetup : IReportSetup
    {
        protected string _backupPath;
        protected readonly int usuario;
        protected readonly int _idAgenda;
        protected readonly string _predio;
        protected readonly string _aplicacion;

        protected readonly string _tabla = CReporte.TablaReporteAgenda;
        protected readonly string _tipoReporte = CReporte.NOTA_DEVOLUCION;

        protected readonly IUnitOfWork _uow;

        public NotaDevolucionReportSetup(IUnitOfWork uow, int usuario, string aplicacion, string predio, int idAgenda)
        {
            this._uow = uow;
            this.usuario = usuario;
            this._aplicacion = aplicacion;
            this._predio = predio;
            this._idAgenda = idAgenda;
        }

        public NotaDevolucionReportSetup(IUnitOfWork uow)
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
                        var agenda = uow.AgendaRepository.GetAgenda(nroAgenda);
                        var puerta = GetPuertaEmbarque(uow, agenda);
                        var zonaPuerta = uow.UbicacionRepository.GetUbicacion(puerta.CodigoUbicacion).IdUbicacionZona;
                        relacion.Zona = zonaPuerta;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, $"GenerarReporteNotaDevolucion - Clave: " + relacion.Clave);

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
                var prepararReporte = new PrepararReporte(_uow, usuario, _aplicacion, _predio, zonaPuerta);
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
            {   "WCOF070_ReporteCR_lbl_NU_AGENDA",
                "WCOF070_ReporteND_lbl_Title",
                "WCOF070_ReporteND_lbl_NM_EMPRESA",
                "WCOF070_ReporteND_lbl_DS_ENDERECO",
                "WCOF070_ReporteND_lbl_NU_TELEFONE",
                "WCOF070_ReporteND_lbl_CD_AGENTE",
                "WCOF070_ReporteND_lbl_Pagina",
                "WCOF070_ReporteND_lbl_SeparadorPagina",
                "WCOF070_ReporteND_lbl_DT_ADDROW",
                "WCOF070_ReporteND_lbl_NU_FACTURA",
                "WCOF070_ReporteND_lbl_CD_PRODUTO",
                "WCOF070_ReporteND_lbl_CD_EXTERNO",
                "WCOF070_ReporteND_lbl_DS_PRODUTO",
                "WCOF070_ReporteND_lbl_CD_UNIDADE_MEDIDA",
                "WCOF070_ReporteND_lbl_QT_FACTURADA",
                "WCOF070_ReporteND_lbl_QT_VALIDADA",
                "WCOF070_ReporteND_lbl_QT_RECIBIDA",
                "WCOF070_ReporteND_lbl_QT_FALTANTE",
                "WCOF070_ReporteND_lbl_Proveedor",
                "WCOF070_ReporteND_lbl_Cantidad",
            }, language);

            var agenda = uow.AgendaRepository.GetAgenda(nroAgenda);

            Agente proveedor = uow.AgenteRepository.GetAgente(agenda.IdEmpresa, agenda.CodigoInternoCliente);

            Empresa empresaPropia = uow.EmpresaRepository.GetEmpresa(agenda.IdEmpresa);

            var listaLineas = uow.ReporteRepository.GetInfoFacturasDevolucion(connection, nroAgenda);
            var listaFacturas = listaLineas.GroupBy(l => l.RecepcionFactura).Select(x => x.Key).ToList();

            if (listaFacturas.Count > 0)
            {
                fileName = string.Join("_", new string[]
                {
                    "NotaDevolucion",
                    Convert.ToString(agenda.Id),
                    DateTime.Now.ToString(GeneralDb.DATETIME_FILENAME_OK)
                });

                fileName = fileName.Replace("/", "-");
                fileName += ".pdf";

                PdfWriter writer = new PdfWriter(this._backupPath + fileName);
                PdfDocument pdf = new PdfDocument(writer);
                var footerEventHandler = new PageEventHandler();

                pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, footerEventHandler);

                Document doc = new Document(pdf);

                CreateHeader(doc, resourceList, agenda, proveedor, empresaPropia, logoPath, reporteConLogo);
                CreateContent(uow, doc, resourceList, listaLineas, listaFacturas, proveedor, logoPath);

                footerEventHandler.WriteTotalPages(pdf);

                doc.Close();

                var filePath = this._backupPath + fileName;
                fileData = System.IO.File.ReadAllBytes(filePath);
            }
        }

        public virtual void CreateContent(IUnitOfWork uow, Document doc, List<TranslatedValue> resourceList, List<DtpReporteNotaDevolucionDet> DetallesAgenda, List<int> DetallesAgrpados, Agente cliente, string logoPath)
        {
            foreach (var factura in DetallesAgrpados)
            {
                var detallesFactura = DetallesAgenda.Where(x => x.RecepcionFactura == factura).ToList();
                var DetalleFactura = detallesFactura.FirstOrDefault();

                doc.Add(new Paragraph((resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteND_lbl_NU_FACTURA")?.Value ?? "") + ": " + DetalleFactura.Serie + "-" + DetalleFactura.Factura).SetFontColor(ColorConstants.BLACK).SetMarginTop(60).SetMarginBottom(10).
                                  SetBorderTop(new SolidBorder(ColorConstants.GRAY, 1)));

                Table table = new Table(UnitValue.CreatePercentArray(8)).UseAllAvailableWidth().SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(0, 0, 255), 0).SetFontSize(9)
               .SetBorder(new SolidBorder(ColorConstants.BLACK, 1));


                Cell head = new Cell();
                head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteND_lbl_CD_PRODUTO")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
                head.SetBorder(Border.NO_BORDER);
                head.SetBackgroundColor(ColorConstants.GRAY);

                table.AddHeaderCell(head);

                head = new Cell();
                head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteND_lbl_CD_EXTERNO")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
                head.SetBorder(Border.NO_BORDER);
                head.SetBackgroundColor(ColorConstants.GRAY);

                table.AddHeaderCell(head);

                head = new Cell();
                head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteND_lbl_DS_PRODUTO")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
                head.SetBorder(Border.NO_BORDER);
                head.SetBackgroundColor(ColorConstants.GRAY);

                table.AddHeaderCell(head);
                head = new Cell();
                head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteND_lbl_CD_UNIDADE_MEDIDA")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
                head.SetBorder(Border.NO_BORDER);
                head.SetBackgroundColor(ColorConstants.GRAY);

                table.AddHeaderCell(head);

                head = new Cell();
                head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteND_lbl_QT_FACTURADA")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
                head.SetBorder(Border.NO_BORDER);
                head.SetBackgroundColor(ColorConstants.GRAY);

                table.AddHeaderCell(head);

                head = new Cell();
                head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteND_lbl_QT_VALIDADA")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
                head.SetBorder(Border.NO_BORDER);
                head.SetBackgroundColor(ColorConstants.GRAY);

                table.AddHeaderCell(head);

                head = new Cell();
                head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteND_lbl_QT_RECIBIDA")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
                head.SetBorder(Border.NO_BORDER);
                head.SetBackgroundColor(ColorConstants.GRAY);

                table.AddHeaderCell(head);

                head = new Cell();
                head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteND_lbl_QT_FALTANTE")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
                head.SetBorder(Border.NO_BORDER);
                head.SetBackgroundColor(ColorConstants.GRAY);

                table.AddHeaderCell(head);

                foreach (var linea in detallesFactura)
                {
                    Producto producto = uow.ProductoRepository.GetProducto(linea.Empresa, linea.Producto);
                    var prodconv = uow.ProductoRepository.GetProductoProveedor(linea.Empresa, linea.Producto, cliente.CodigoInterno);

                    Cell cell = new Cell();
                    cell.Add(new Paragraph(linea.Producto));
                    cell.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                    table.AddCell(cell);

                    cell = new Cell();
                    cell.Add(new Paragraph((prodconv == null ? "" : prodconv.CodigoExterno)));
                    cell.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                    table.AddCell(cell);

                    cell = new Cell();
                    cell.Add(new Paragraph(producto.Descripcion));
                    cell.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                    table.AddCell(cell);


                    head = new Cell();
                    head.Add(new Paragraph(producto.UnidadMedida ?? ""));
                    head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                    table.AddCell(head);

                    head = new Cell();
                    head.Add(new Paragraph(Convert.ToString(linea.CantidadFacturada ?? 0)).SetTextAlignment(TextAlignment.RIGHT));
                    head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                    table.AddCell(head);

                    head = new Cell();
                    head.Add(new Paragraph(Convert.ToString(linea.CantidadValidada ?? 0)).SetTextAlignment(TextAlignment.RIGHT));
                    head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                    table.AddCell(head);

                    head = new Cell();
                    head.Add(new Paragraph(Convert.ToString(linea.CantidadRecibida ?? 0)).SetTextAlignment(TextAlignment.RIGHT));
                    head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                    table.AddCell(head);

                    head = new Cell();
                    head.Add(new Paragraph(Convert.ToString((linea.CantidadFacturada ?? 0) - (linea.CantidadRecibida ?? 0))).SetTextAlignment(TextAlignment.RIGHT));
                    head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                    table.AddCell(head);

                }

                doc.Add(table);


            }



        }

        public virtual void CreateHeader(Document doc, List<TranslatedValue> resourceList, Agenda agenda, Agente cliente, Empresa empresa, string filePath, bool reporteConLogo)
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
            head.Add(new Paragraph(agenda.Id.ToString()).SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);
            #endregion

            #region Empresa
            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteND_lbl_NM_EMPRESA")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph(empresa.Id.ToString() + " " + empresa.Nombre).SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);
            #endregion

            #region Agente
            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteND_lbl_CD_AGENTE")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph(cliente.Tipo + " " + agenda.CodigoInternoCliente + " " + cliente.Descripcion).SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);
            #endregion

            #region EnderecoAgente
            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteND_lbl_DS_ENDERECO")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph(empresa.Direccion).SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);
            #endregion

            #region EnderecoAgente
            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteND_lbl_NU_TELEFONE")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph(empresa.Telefono ?? "").SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);
            #endregion

            #region Cierre
            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReporteND_lbl_DT_ADDROW")?.Value ?? "").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph(agenda.FechaCierre.ToString(CDateFormats.DATE_ONLY)).SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);
            #endregion

            doc.Add(table);
        }
    }
}
