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
using WIS.Domain.Expedicion;
using WIS.Domain.Expedicion.Enums;
using WIS.Domain.General;
using WIS.Domain.Reportes.Dtos;
using WIS.Domain.Services.Interfaces;
using WIS.Extension;
using WIS.Report;
using WIS.Security;
using WIS.Translation;

namespace WIS.Domain.Reportes.Setups
{
    public class PackingListSinLpnReportSetup : IReportSetup
    {
        protected string _backupPath;
        protected string _predio;
        protected Camion _camion;

        protected readonly string _tabla = CReporte.TablaReporteCamion;
        protected readonly string _tipoReporte = CReporte.PACKING_LIST_SIN_LPN;

        protected IUnitOfWork _uow;
        protected IReportKeyService _keyResolver;
        protected IIdentityService _securityService;
        protected IParameterService _parameterService;

        public PackingListSinLpnReportSetup(IUnitOfWork uow, IParameterService parameterService, IReportKeyService keyResolver, IIdentityService securityService, Camion camion, string predio = null)
        {
            this._uow = uow;
            this._keyResolver = keyResolver;
            this._parameterService = parameterService;
            this._securityService = securityService;
            this._predio = predio;
            this._camion = camion;
        }

        public PackingListSinLpnReportSetup(IUnitOfWork uow)
        {
            _backupPath = uow.ParametroRepository.GetParameter(ParamManager.REPORTE_BACKUP_PATH);

            _uow = uow;

            if (!Directory.Exists(_backupPath)) Directory.CreateDirectory(_backupPath);
        }

        public virtual List<long> Preparar()
        {
            var reports = new List<long>();
            var empresas = new List<int>();

            if (this._camion.Estado == CamionEstado.Cerrado)
                empresas = _uow.CamionRepository.GetEmpresasCamion(this._camion.Id);
            else
                empresas = this._camion.GetEmpresas();
            
            var puerta = _camion.Puerta.HasValue
                ? _uow.PuertaEmbarqueRepository.GetPuertaEmbarque(_camion.Puerta.Value)
                : null;

            var zonaPuerta = puerta != null
                ? _uow.UbicacionRepository.GetUbicacion(puerta.CodigoUbicacion)?.IdUbicacionZona
                : null;

            foreach (var empresa in empresas)
            {
                var reportes = this._parameterService.GetValueByEmpresa("EXPEDICION_REPORTES", empresa);

                if (string.IsNullOrEmpty(reportes) || !reportes.Contains($"\"{CReporte.PACKING_LIST_SIN_LPN}\""))
                    continue;

                List<string> clientes;

                if (this._camion.Estado == CamionEstado.Cerrado)
                    clientes = _uow.CamionRepository.GetClientesCamion(this._camion.Id);
                else
                    clientes = this._camion.GetClientes(empresa);

                foreach (var cliente in clientes)
                {
                    string clave = this.GetCompositeId(empresa, cliente);

                    if (this._uow.ReporteRepository.AnyReporte(clave, this._tabla, this._tipoReporte))
                        continue;

                    var nuevoReporte = new Reporte
                    {
                        Id = -1,
                        Tipo = this._tipoReporte,
                        Usuario = this._securityService.UserId,
                        NombreArchivo = string.Empty,
                        Estado = CReporte.Pendiente,
                        Predio = this._predio ?? this._securityService.Predio,
                        Zona = zonaPuerta //Se utiliza para determinar donde se imprime el reporte (Antes venia de la puerta de la agenda, ahora la agenda no tiene puerta al momento de la creacion, entonces se setea el predio del usuario)                            
                    };

                    nuevoReporte.AddRelacion(this._tabla, clave);

                    _uow.ReporteRepository.AddReporte(nuevoReporte);

                    if (nuevoReporte.Id != -1)
                    {
                        reports.Add(nuevoReporte.Id);
                    }
                }
            }

            return reports;
        }

        public virtual void GenerarReporte(IUnitOfWork uow, DbConnection connection, ILogger<ReportLogic> logger, DtoReporteRelacion relacion)
        {
            try
            {
                string situacionReporte;
                var tmpClave = Reporte.SplitCompositeId(relacion.Clave);

                if (tmpClave != null && tmpClave.Any())
                {
                    var cdCamion = Convert.ToInt32(tmpClave[0]);
                    var cdEmpresa = Convert.ToInt32(tmpClave[1]);
                    var cdCliente = tmpClave[2];
                    var userLanguage = uow.SecurityRepository.GetUserLanguage(relacion.Usuario ?? 0);

                    Generar(uow, connection, cdCamion, cdEmpresa, cdCliente, userLanguage, out byte[] fileData, out string fileName);

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
                        var camion = uow.CamionRepository.GetCamion(cdCamion);
                        var puerta = _camion.Puerta.HasValue
                            ? uow.PuertaEmbarqueRepository.GetPuertaEmbarque(camion.Puerta.Value)
                            : null;
                        var zonaPuerta = puerta != null
                            ? uow.UbicacionRepository.GetUbicacion(puerta.CodigoUbicacion)?.IdUbicacionZona
                            : null;
                        relacion.Zona = zonaPuerta;
                    }

                    Notificar(uow, relacion.Id, cdCamion, cdEmpresa, cdCliente, userLanguage, fileData, fileName);
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, $"GenerarReportePackingListSinLpn - Clave: " + relacion.Clave);

                relacion.Estado = CReporte.Error;
                relacion.FechaModificacion = DateTime.Now;
            }
        }

        public virtual void Generar(IUnitOfWork uow, DbConnection connection, int cdCamion, int cdEmpresa, string cdCliente, string language, out byte[] fileData, out string fileName)
        {
            fileData = null;
            fileName = string.Empty;
            var reporteConLogo = true;

            string resourcePath = uow.ParametroRepository.GetParameter(ParamManager.REPORTE_RESOURCE_PATH);
            string resourceLogo = uow.ParametroRepository.GetParameter(ParamManager.REPORTE_RESOURCE_LOGO);
            string logoPath = resourcePath + resourceLogo;

            var predio = uow.CamionRepository.GetCamion(cdCamion).Predio;

            var data = uow.ReporteRepository.GetDetallesPackingListSinLpnReporte(cdCamion, cdEmpresa, cdCliente);

            if (!System.IO.File.Exists(logoPath))
                reporteConLogo = false;

            var resourceList = uow.LocalizationRepository.GetTranslation(new List<string>
            {
                "WCOF070_ReportePLSLPN_lbl_CD_AGENTE",
                "WCOF070_ReportePLSLPN_lbl_CD_CAMION",
                "WCOF070_ReportePLSLPN_lbl_CD_TRANSPORTADORA",
                "WCOF070_ReportePLSLPN_lbl_CD_EMPRESA",
                "WCOF070_ReportePLSLPN_lbl_CLI_DS_ENDERECO",
                "WCOF070_ReportePLSLPN_lbl_CD_ROTA",
                "WCOF070_ReportePLSLPN_lbl_FIRMA",
                "WCOF070_ReportePLSLPN_lbl_ACLARACION",
                "WCOF070_ReportePLSLPN_lbl_DT_FIRMA",
                "WCOF070_ReportePLSLPN_lbl_ENTREGADO_POR",
                "WCOF070_ReportePLSLPN_lbl_RECIBIDO_POR",
                "WCOF070_ReportePLSLPN_lbl_FECHA",
                "WCOF070_ReportePLSLPN_lbl_CD_PLACA_CARRO",
                "WCOF070_ReportePLSLPN_lbl_NU_PEDIDO",
                "WCOF070_ReportePLSLPN_lbl_CD_PRODUTO",
                "WCOF070_ReportePLSLPN_lbl_DS_PRODUTO",
                "WCOF070_ReportePLSLPN_lbl_NU_IDENTIFICADOR",
                "WCOF070_ReportePLSLPN_lbl_DT_VENCIMIENTO",
                "WCOF070_ReportePLSLPN_lbl_QT_PRODUTO",
                "WCOF070_ReportePLSLPN_lbl_QT_TOTAL",
                "WCOF070_ReportePLSLPN_lbl_TP_BULTO",
                "WCOF070_ReportePLSLPN_lbl_QT_BULTO",
                "WCOF070_ReportePLSLPN_lbl_QT_UND_BULTO",
                "WCOF070_ReportePLSLPN_lbl_BULTO"
            }, language);

            if (data != null && data.Any())
            {
                var firstItem = data.First();
                fileName = string.Join("_", new string[]
                {
                    "PackingListSinLPN",
                    firstItem.CodigoCamion.ToString(),
                    cdEmpresa.ToString(),
                    firstItem.TipoAgente,
                    firstItem.CodigoAgente,
                    DateTime.Now.ToString(GeneralDb.DATETIME_FILENAME_OK)
                });

                fileName = fileName.Replace("/", "-");
                fileName += ".pdf";

                PdfWriter writer = new PdfWriter(_backupPath + fileName);
                PdfDocument pdf = new PdfDocument(writer);

                var footerEventHandler = new PageEventHandler();
                pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, footerEventHandler);

                Document doc = new Document(pdf);

                CreateHeader(doc, resourceList, firstItem, resourcePath + resourceLogo, reporteConLogo);

                foreach (var item in data)
                {
                    CreateContent(doc, resourceList, item);
                }

                CreateFooter(doc, resourceList);

                footerEventHandler.WriteTotalPages(pdf);
                doc.Close();

                string filePath = _backupPath + fileName;
                fileData = System.IO.File.ReadAllBytes(filePath);
            }
        }

        public virtual void CreateHeader(Document doc, List<TranslatedValue> resourceList, DtoDetallesPackingListSinLpn data, string filePath, bool reporteConLogo)
        {
            var cantidadColumnas = reporteConLogo ? 2 : 1;

            Table table = new Table(cantidadColumnas).SetBackgroundColor(new DeviceRgb(0, 0, 255), 0)
                                                     .SetFontSize(9)
                                                     .SetBorder(Border.NO_BORDER)
                                                     .UseAllAvailableWidth();
            if (reporteConLogo)
            {
                var logoCell = new Cell().SetBorder(Border.NO_BORDER);

                ImageData myImageData = ImageDataFactory.Create(filePath, false);
                Image myImage = new Image(myImageData);

                myImage.SetWidth(150);
                logoCell.Add(myImage);
                table.AddCell(logoCell);
            }

            doc.Add(table);
            doc.Add(new Paragraph("\n"));


            var leftTable = new Cell().SetBorder(Border.NO_BORDER);
            var rightTable = new Cell().SetBorder(Border.NO_BORDER);

            table = new Table(2).SetBackgroundColor(new DeviceRgb(0, 0, 255), 0)
                                      .SetWidth(390)
                                      .SetFontSize(9)
                                      .SetBorder(new SolidBorder(ColorConstants.BLACK, 1))
                                      .SetHorizontalAlignment(HorizontalAlignment.LEFT)
                                      .SetTextAlignment(TextAlignment.CENTER);

            #region Agente
            Cell head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePLSLPN_lbl_CD_AGENTE")?.Value ?? "Agente").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph($"{data.CodigoAgente} - {data.DescripcionAgente}").SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            #endregion

            #region Camion
            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePLSLPN_lbl_CD_CAMION")?.Value ?? "Camión").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph($"{data.CodigoCamion} - {data.DescripcionCamion}").SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            #endregion

            #region Transportista
            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePLSLPN_lbl_CD_TRANSPORTADORA")?.Value ?? "Transportista").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph($"{data.CodigoTransportista} - {data.DescripcionTransportista}").SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            #endregion

            #region Empresa
            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePLSLPN_lbl_CD_EMPRESA")?.Value ?? "Empresa").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph($"{data.Empresa} - {data.DescripcionEmpresa}").SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            #endregion

            #region Direccion

            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePLSLPN_lbl_CLI_DS_ENDERECO")?.Value ?? "Dirección Entrega").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph(data.DireccionEntrega ?? "").SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            #endregion

            #region Ruta
            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePLSLPN_lbl_CD_ROTA")?.Value ?? "Ruta").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph($"{data.Ruta} - {data.DescripcionRuta}").SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            #endregion

            leftTable.Add(table);

            table = new Table(2).SetBackgroundColor(new DeviceRgb(0, 0, 255), 0)
                                      .SetFontSize(9)
                                      .SetWidth(120)
                                      .SetBorder(new SolidBorder(ColorConstants.BLACK, 1))
                                      .SetHorizontalAlignment(HorizontalAlignment.RIGHT)
                                      .SetTextAlignment(TextAlignment.CENTER);

            #region Cierre

            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePLSLPN_lbl_FECHA")?.Value ?? "Fecha").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph(data.Fecha.ToString("dd/MM/yyyy")).SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            #endregion

            #region Matricula

            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePLSLPN_lbl_CD_PLACA_CARRO")?.Value ?? "Matricula").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph(data.Matricula).SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            #endregion

            rightTable.Add(table);

            table = new Table(2).SetBackgroundColor(new DeviceRgb(0, 0, 255), 0)
                                      .SetFontSize(9)
                                      .SetBorder(Border.NO_BORDER)
                                      .UseAllAvailableWidth();

            table.AddCell(leftTable);
            table.AddCell(rightTable);

            table.SetMarginBottom(15);

            doc.Add(table);
        }

        public virtual void CreateContent(Document doc, List<TranslatedValue> resourceList, DtoDetallesPackingListSinLpn item)
        {
            var table = new Table(1);
            Cell head = new Cell();

            foreach (var contenedor in item.Contenedores)
            {
                table = new Table(6).SetBackgroundColor(new DeviceRgb(0, 0, 255), 0)
                                                          .SetFontSize(9)
                                                          .SetBorder(new SolidBorder(ColorConstants.BLACK, 1))
                                                          .UseAllAvailableWidth()
                                                          .SetTextAlignment(TextAlignment.CENTER);

                var especificacionBulto = resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePLSLPN_lbl_BULTO")?.Value ?? "Bulto:";
                var title = $"{especificacionBulto} {contenedor.IdExternoContenedor}";

                head = new Cell(1, 6);
                head.Add(new Paragraph(title ?? "Bulto:").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
                head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1)).SetBold().SetFontSize(12);

                table.AddHeaderCell(head);

                head = new Cell(1, 1);
                head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePLSLPN_lbl_NU_PEDIDO")?.Value ?? "Pedido").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
                head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1)).SetBold().SetFontSize(9);

                table.AddHeaderCell(head);

                head = new Cell(1, 1);
                head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePLSLPN_lbl_CD_PRODUTO")?.Value ?? "Producto").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
                head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1)).SetBold().SetFontSize(9);

                table.AddHeaderCell(head);

                head = new Cell(1, 1);
                head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePLSLPN_lbl_DS_PRODUTO")?.Value ?? "Descripción").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
                head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1)).SetBold().SetFontSize(9);

                table.AddHeaderCell(head);

                head = new Cell(1, 1);
                head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePLSLPN_lbl_NU_IDENTIFICADOR")?.Value ?? "Identificador").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
                head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1)).SetBold().SetFontSize(9);

                table.AddHeaderCell(head);

                head = new Cell(1, 1);
                head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePLSLPN_lbl_DT_VENCIMIENTO")?.Value ?? "Vencimiento").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
                head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1)).SetBold().SetFontSize(9);

                table.AddHeaderCell(head);

                head = new Cell(1, 1);
                head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePLSLPN_lbl_QT_PRODUTO")?.Value ?? "Cantidad").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
                head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1)).SetBold().SetFontSize(9);

                table.AddHeaderCell(head);

                foreach (var linea in contenedor.Detalles)
                {
                    Cell cell = new Cell();
                    cell.Add(new Paragraph(linea.Pedido));
                    cell.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                    table.AddCell(cell);

                    cell = new Cell();
                    cell.Add(new Paragraph(linea.Producto));
                    cell.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                    table.AddCell(cell);

                    cell = new Cell();
                    cell.Add(new Paragraph(linea.Descripcion));
                    cell.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                    table.AddCell(cell);

                    cell = new Cell();
                    cell.Add(new Paragraph(linea.Identificador));
                    cell.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                    table.AddCell(cell);

                    cell = new Cell();
                    cell.Add(new Paragraph(linea.Vencimiento.ToString(CDateFormats.DATETIME_24H)));
                    cell.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                    table.AddCell(cell);

                    cell = new Cell();
                    cell.Add(new Paragraph(linea.Cantidad.ToString()));
                    cell.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                    table.AddCell(cell);
                }

                if (contenedor.Detalles.Count > 1)
                {
                    var colspan = table.GetNumberOfColumns() - 1;

                    Cell cell = new Cell(1, colspan);
                    cell.Add(new Paragraph((resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePLSLPN_lbl_QT_TOTAL")?.Value ?? "" ?? "")));
                    cell.SetBorder(new SolidBorder(ColorConstants.BLACK, 1)).SetBold();

                    cell.SetTextAlignment(TextAlignment.LEFT);

                    table.AddCell(cell);

                    cell = new Cell(1, colspan);
                    cell.Add(new Paragraph(contenedor.GetTotal().ToString()));
                    cell.SetBorder(new SolidBorder(ColorConstants.BLACK, 1)).SetBold();

                    table.AddCell(cell);
                }

                table.SetMarginBottom(15);

                doc.Add(table);
            }

            table = new Table(3).SetBackgroundColor(new DeviceRgb(0, 0, 255), 0)
                                                      .SetFontSize(9)
                                                      .SetWidth(UnitValue.CreatePercentValue(50))
                                                      .SetBorder(new SolidBorder(ColorConstants.BLACK, 1))
                                                      .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                                                      .SetTextAlignment(TextAlignment.CENTER);

            head = new Cell(1, 1);
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePLSLPN_lbl_TP_BULTO")?.Value ?? "Tipo Bulto").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1)).SetBold().SetFontSize(9);

            table.AddHeaderCell(head);

            head = new Cell(1, 1);
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePLSLPN_lbl_QT_BULTO")?.Value ?? "Bultos").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1)).SetBold().SetFontSize(9);

            table.AddHeaderCell(head);

            head = new Cell(1, 1);
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePLSLPN_lbl_QT_UND_BULTO")?.Value ?? "Unidades").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1)).SetBold().SetFontSize(9);

            table.AddHeaderCell(head);

            var totalesPorTipo = item.GetTotalContenedorPorTipo();

            foreach (var entry in totalesPorTipo)
            {
                var unidades = item.Contenedores.Where(i => i.TipoContenedor == entry.Key).Sum(s => s.GetTotal());

                var cell = new Cell();
                cell.Add(new Paragraph(entry.Key));
                cell.SetHorizontalAlignment(HorizontalAlignment.CENTER);

                table.AddCell(cell);

                cell = new Cell();
                cell.Add(new Paragraph(entry.Value.ToString()));
                cell.SetHorizontalAlignment(HorizontalAlignment.CENTER);

                table.AddCell(cell);


                cell = new Cell();
                cell.Add(new Paragraph(unidades.ToString()));
                cell.SetHorizontalAlignment(HorizontalAlignment.CENTER);

                table.AddCell(cell);
            }

            if (totalesPorTipo.Count > 1)
            {
                var colspan = table.GetNumberOfColumns() - 1;

                Cell cell = new Cell(1, colspan);
                cell.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePLSLPN_lbl_QT_TOTAL")?.Value ?? ""));
                cell.SetBorder(new SolidBorder(ColorConstants.BLACK, 1)).SetBold();
                cell.SetTextAlignment(TextAlignment.LEFT);

                table.AddCell(cell);

                cell = new Cell(1, colspan);
                cell.Add(new Paragraph(totalesPorTipo.Sum(s => s.Value).ToString()));
                cell.SetBorder(new SolidBorder(ColorConstants.BLACK, 1)).SetBold();

                table.AddCell(cell);
            }

            doc.Add(table);

            doc.Add(new Paragraph("\n"));
        }

        public virtual void CreateFooter(Document doc, List<TranslatedValue> resourceList)
        {
            Table outerTable = new Table(UnitValue.CreatePercentArray(2))
                .UseAllAvailableWidth()
                .SetFontSize(10);

            outerTable.SetBorderCollapse(BorderCollapsePropertyValue.SEPARATE);
            outerTable.SetHorizontalBorderSpacing(10);
            outerTable.SetVerticalBorderSpacing(30);

            outerTable.AddCell(GetConfirmationTable(resourceList.FirstOrDefault(i => i.Key == "WCOF070_ReportePLSLPN_lbl_ENTREGADO_POR").Value, resourceList));
            outerTable.AddCell(GetConfirmationTable(resourceList.FirstOrDefault(i => i.Key == "WCOF070_ReportePLSLPN_lbl_RECIBIDO_POR")?.Value, resourceList));

            doc.Add(outerTable);
        }

        public virtual Cell GetConfirmationTable(string concept, List<TranslatedValue> resourceList)
        {
            var confirmationTableCell = new Cell();
            var cell = new Cell();
            confirmationTableCell.SetBorder(Border.NO_BORDER);

            Table innerTable = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();

            Cell conceptualCell = new Cell();
            conceptualCell.Add(new Paragraph(concept ?? ""));
            conceptualCell.SetBorder(Border.NO_BORDER).SetBold();
            innerTable.AddCell(conceptualCell);

            cell = new Cell();
            cell.Add(new Paragraph(CReporte.GAP).SetTextAlignment(TextAlignment.LEFT));
            cell.SetBorder(Border.NO_BORDER);
            innerTable.AddCell(cell);

            Cell signatureCell = new Cell();
            signatureCell.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePLSLPN_lbl_FIRMA")?.Value ?? ""));
            signatureCell.SetBorder(Border.NO_BORDER).SetBold();
            innerTable.AddCell(signatureCell);

            cell = new Cell();
            cell.Add(new Paragraph(CReporte.GAP).SetTextAlignment(TextAlignment.LEFT));
            cell.SetBorder(Border.NO_BORDER);
            innerTable.AddCell(cell);

            Cell clarificationCell = new Cell();
            clarificationCell.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePLSLPN_lbl_ACLARACION")?.Value ?? ""));
            clarificationCell.SetBorder(Border.NO_BORDER).SetBold();
            innerTable.AddCell(clarificationCell);

            cell = new Cell();
            cell.Add(new Paragraph(CReporte.GAP).SetTextAlignment(TextAlignment.LEFT));
            cell.SetBorder(Border.NO_BORDER);
            innerTable.AddCell(cell);

            Cell dateCell = new Cell();
            dateCell.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePLSLPN_lbl_DT_FIRMA")?.Value ?? ""));
            dateCell.SetBorder(Border.NO_BORDER).SetBold();
            innerTable.AddCell(dateCell);

            cell = new Cell();
            cell.Add(new Paragraph(CReporte.GAP).SetTextAlignment(TextAlignment.LEFT));
            cell.SetBorder(Border.NO_BORDER);
            innerTable.AddCell(cell);

            return confirmationTableCell.Add(innerTable);
        }

        public virtual string GetCompositeId(int empresa, string cliente)
        {
            return _keyResolver.ResolveKey(Convert.ToString(_camion.Id), Convert.ToString(empresa), cliente);
        }

        public virtual void Notificar(IUnitOfWork uow, long nuReporte, int cdCamion, int cdEmpresa, string cdCliente, string language, byte[] fileData, string fileName)
        {
            var empresa = uow.EmpresaRepository.GetEmpresa(cdEmpresa);
            var agente = uow.AgenteRepository.GetAgente(cdEmpresa, cdCliente);
            var camion = uow.CamionRepository.GetCamion(cdCamion);
            var reporte = uow.ReporteRepository.GetReporte(nuReporte);
            var instancias = uow.EventoRepository.GetInstanciasHabilitadas(EventoDb.PackingListSinLpn, new Dictionary<string, string>
            {
                [EventoParametroDb.CD_EMPRESA] = cdEmpresa.ToString(),
                [EventoParametroDb.TP_AGENTE] = agente.Tipo,
                [EventoParametroDb.CD_AGENTE] = agente.Codigo,
                [EventoParametroDb.NU_PREDIO] = camion.Predio,
            });

            foreach (var instancia in instancias)
            {
                var template = uow.EventoRepository.GetEventoTemplate(instancia.NumeroEvento, instancia.IdTipoNotificacion, instancia.Plantilla);
                var contactos = uow.DestinatarioRepository.GetContactos(instancia.Id);
                var cuerpo = Smart.Format(template.Cuerpo, GetParametrosPlantillaNotificacion(uow, empresa, agente, camion));
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

        public virtual object GetParametrosPlantillaNotificacion(IUnitOfWork uow, Empresa empresa, Agente agente, Camion camion)
        {
            return new
            {
                empresa = empresa.Nombre + $" [{empresa.Id}]",
                agente = agente.Descripcion + $" [{agente.Tipo}-{agente.Codigo}]",
                predio = camion.Predio,
                camion = camion.Id,
            };
        }
    }
}
