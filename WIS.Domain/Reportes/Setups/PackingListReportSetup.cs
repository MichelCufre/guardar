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

namespace WIS.Domain.Reportes.Especificaciones
{
    public class PackingListReportSetup : IReportSetup
    {
        protected string _backupPath;
        protected readonly string _predio;
        protected readonly Camion _camion;

        protected readonly string _tabla = CReporte.TablaReporteCamion;
        protected readonly string _tipoReporte = CReporte.PACKING_LIST;

        protected readonly IUnitOfWork _uow;
        protected readonly IReportKeyService _keyResolver;
        protected readonly IIdentityService _securityService;
        protected readonly IParameterService _parameterService;

        public PackingListReportSetup(IUnitOfWork uow, IParameterService parameterService, IReportKeyService keyResolver, IIdentityService securityService, Camion camion, string predio = null)
        {
            this._uow = uow;
            this._keyResolver = keyResolver;
            this._parameterService = parameterService;
            this._securityService = securityService;
            this._predio = predio;
            this._camion = camion;
        }
        public PackingListReportSetup(IUnitOfWork uow)
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
                logger.LogDebug(ex, $"GenerarReportePackingList - Clave: " + relacion.Clave);

                relacion.Estado = CReporte.Error;
                relacion.FechaModificacion = DateTime.Now;
            }
        }

        public virtual void Generar(IUnitOfWork uow, DbConnection connection, int cdCamion, int cdEmpresa, string cdCliente, string language, out byte[] fileData, out string fileName)
        {
            fileData = null;
            fileName = string.Empty;
            var reporteConLogo = true;

            var resourcePath = uow.ParametroRepository.GetParameter(ParamManager.REPORTE_RESOURCE_PATH);
            var resourceLogo = uow.ParametroRepository.GetParameter(ParamManager.REPORTE_RESOURCE_LOGO);
            var logoPath = resourcePath + resourceLogo;

            var datos = uow.ReporteRepository.GetInfoCamionPackingList(connection, cdCamion);
            var detalles = uow.ReporteRepository.GetInfoCamionPackingListDet(connection, cdCamion, cdEmpresa, cdCliente);
            var detallesLpn = uow.ReporteRepository.GetInfoCamionPackingListDetLpn(connection, cdCamion, cdEmpresa);

            if (!System.IO.File.Exists(logoPath))
                reporteConLogo = false;

            var resourceList = uow.LocalizationRepository.GetTranslation(new List<string>
            {
                "WCOF070_ReportePL_lbl_CD_CLIENTE",
                "WCOF070_ReportePL_lbl_CD_CAMION",
                "WCOF070_ReportePL_lbl_CD_PLACA_CARRO",
                "WCOF070_ReportePL_lbl_CD_TRANSPORTADORA",
                "WCOF070_ReportePL_lbl_DT_CIERRE",
                "WCOF070_ReportePL_lbl_CD_EMPRESA",
                "WCOF070_ReportePL_lbl_Pagina",
                "WCOF070_ReportePL_lbl_SeparadorPagina",
                "WCOF070_ReportePL_lbl_CD_PRODUTO",
                "WCOF070_ReportePL_lbl_DS_PRODUTO",
                "WCOF070_ReportePL_lbl_NU_IDENTIFICADOR",
                "WCOF070_ReportePL_lbl_DT_FABRICACAO_PICKEO",
                "WCOF070_ReportePL_lbl_QT_PRODUTO",
                "WCOF070_ReportePL_lbl_Title"
            }, language);

            if (datos != null && detalles.Any())
            {
                var datosCliente = detalles.First();

                fileName = string.Join("_", new string[]
                {
                    "PackingList",
                    datos.Camion.ToString(),
                    cdEmpresa.ToString(),
                    datosCliente.TipoCliente,
                    datosCliente.Agente,
                    DateTime.Now.ToString(GeneralDb.DATETIME_FILENAME_OK)
                });

                fileName = fileName.Replace("/", "-");
                fileName += ".pdf";

                var writer = new PdfWriter(this._backupPath + fileName);
                var pdf = new PdfDocument(writer);
                var footerEventHandler = new PageEventHandler();

                pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, footerEventHandler);

                var doc = new Document(pdf);

                CreateHeader(doc, resourceList, datos, datosCliente, resourcePath + resourceLogo, reporteConLogo);
                CreateContent(doc, resourceList, detalles, detallesLpn);

                footerEventHandler.WriteTotalPages(pdf);

                doc.Close();
                var filePath = this._backupPath + fileName;
                fileData = System.IO.File.ReadAllBytes(filePath);
            }
        }

        public virtual void CreateHeader(Document doc, List<TranslatedValue> resourceList, DtpReportePackingList datos, DtpReportePackingListDet datosCliente, string filePath, bool reporteConLogo)
        {
            if (reporteConLogo)
            {
                var myImageData = ImageDataFactory.Create(filePath, false);
                var myImage = new Image(myImageData);

                myImage.SetFixedPosition(1, 370, 720);
                myImage.SetAutoScale(true);
                doc.Add(myImage);
            }

            var table = new Table(2).SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(0, 0, 255), 0).SetFontSize(9)
                .SetBorder(new SolidBorder(ColorConstants.BLACK, 1)).SetWidth(250);

            #region Camion
            Cell head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePL_lbl_CD_CAMION")?.Value ?? "Camión").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph(datos.Camion.ToString()).SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);
            #endregion

            #region Cliente
            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePL_lbl_CD_CLIENTE")?.Value ?? "Agente").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph($"{datosCliente.TipoCliente} - {datosCliente.Agente} - {datosCliente.DescripcionCliente}").SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);
            #endregion

            #region Matricula
            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePL_lbl_CD_PLACA_CARRO")?.Value ?? "Matrícula").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph(datos.Matricula ?? string.Empty).SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);
            #endregion

            #region Empresa
            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePL_lbl_CD_EMPRESA")?.Value ?? "Empresa").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph($"{datos.Empresa} - {datos.NombreEmpresa}").SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);
            #endregion

            #region Cierre
            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePL_lbl_DT_CIERRE")?.Value ?? "Fecha cierre").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph(datos.FechaCierre.ToString("dd/MM/yyyy")).SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePL_lbl_CD_TRANSPORTADORA")?.Value ?? "Transportista").SetFontColor(ColorConstants.BLACK).SetPaddingLeft(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);

            head = new Cell();
            head.Add(new Paragraph($"{datos.Transportadora} - {datos.DescripcionTransportadora}").SetFontColor(ColorConstants.BLACK).SetMarginRight(10));
            head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            head.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            table.AddCell(head);
            #endregion

            doc.Add(table);
            doc.Add(new Paragraph("\n"));
        }

        public virtual void CreateContent(Document doc, List<TranslatedValue> resourceList, List<DtpReportePackingListDet> detallesCliente, List<DtpReportePackingListDetLpn> detallesLpn)
        {
            Cell head = new Cell();

            #region Tabla_ProductosExpedidos

            var table = new Table(UnitValue.CreatePercentArray(5)).UseAllAvailableWidth().SetBackgroundColor(new iText.Kernel.Colors.DeviceRgb(0, 0, 255), 0).SetFontSize(9)
                 .SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePL_lbl_CD_PRODUTO")?.Value ?? "Producto").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
            head.SetBorder(Border.NO_BORDER);
            head.SetBackgroundColor(ColorConstants.GRAY);

            table.AddHeaderCell(head);

            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePL_lbl_DS_PRODUTO")?.Value ?? "Desc. Producto").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
            head.SetBorder(Border.NO_BORDER);
            head.SetBackgroundColor(ColorConstants.GRAY);

            table.AddHeaderCell(head);

            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePL_lbl_NU_IDENTIFICADOR")?.Value ?? "Serie/Lote").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
            head.SetBorder(Border.NO_BORDER);
            head.SetBackgroundColor(ColorConstants.GRAY);

            table.AddHeaderCell(head);

            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePL_lbl_DT_FABRICACAO_PICKEO")?.Value ?? "Vencimiento").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
            head.SetBorder(Border.NO_BORDER);
            head.SetBackgroundColor(ColorConstants.GRAY);

            table.AddHeaderCell(head);

            head = new Cell();
            head.Add(new Paragraph(resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePL_lbl_QT_PRODUTO")?.Value ?? "Cantidad").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
            head.SetBorder(Border.NO_BORDER);
            head.SetBackgroundColor(ColorConstants.GRAY);

            table.AddHeaderCell(head);

            foreach (var linea in detallesCliente)
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
                head.Add(new Paragraph(linea.Vencimiento.ToString("dd/MM/yyyy")));
                head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                table.AddCell(head);

                head = new Cell();
                head.Add(new Paragraph(linea.CantidadProducto?.ToString()).SetTextAlignment(TextAlignment.RIGHT));
                head.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                table.AddCell(head);
            }

            doc.Add(table);
            doc.Add(new Paragraph("\n"));

            #endregion

            #region Tabla_DetallesLpn
            if (detallesLpn != null && detallesLpn.Count() > 0)
            {
                table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth().SetBackgroundColor(new DeviceRgb(0, 0, 255), 0).SetFontSize(9)
                    .SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                foreach (var linea in detallesCliente)
                {
                    var dets = detallesLpn
                        .Where(d => d.Producto == linea.Producto
                            && d.Empresa == linea.Empresa
                            && d.Faixa == linea.Faixa
                            && d.Lote == linea.Lote)
                        .GroupBy(d => new { d.IdLpnDet, d.Producto, d.Empresa, d.Faixa, d.Lote, d.CantidadExpedida })
                        .Select(d => d.Key);

                    foreach (var det in dets)
                    {
                        var textProducto = resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePL_lbl_CD_PRODUTO")?.Value ?? "Producto";
                        var textLote = resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePL_lbl_NU_IDENTIFICADOR")?.Value ?? "Serie/Lote";
                        var textCantidad = resourceList.FirstOrDefault(x => x.Key == "WCOF070_ReportePL_lbl_QT_PRODUTO")?.Value ?? "Cantidad";

                        head = new Cell();
                        head.Add(new Paragraph($"{textProducto} : {det.Producto}").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
                        head.SetBorder(Border.NO_BORDER);
                        head.SetBackgroundColor(ColorConstants.GRAY);

                        table.AddCell(head);

                        head = new Cell();
                        head.Add(new Paragraph($"{textLote}: {det.Lote}").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
                        head.SetBorder(Border.NO_BORDER);
                        head.SetBackgroundColor(ColorConstants.GRAY);

                        table.AddCell(head);

                        head = new Cell();
                        head.Add(new Paragraph($"{textCantidad}: {det.CantidadExpedida}").SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.CENTER));
                        head.SetBorder(Border.NO_BORDER);
                        head.SetBackgroundColor(ColorConstants.GRAY);

                        table.AddCell(head);

                        var atributos = detallesLpn
                            .Where(d => d.IdLpnDet == det.IdLpnDet)
                            .Select(d => new { d.NombreAtributo, d.ValorAtributo });

                        foreach (var at in atributos)
                        {
                            Cell cell = new Cell();
                            cell.Add(new Paragraph(at.NombreAtributo));
                            cell.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                            table.AddCell(cell);

                            cell = new Cell(1, 2);
                            cell.Add(new Paragraph(at.ValorAtributo));
                            cell.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

                            table.AddCell(cell);
                        }
                    }
                }

                doc.Add(table);
            }
            #endregion
        }

        public virtual void Notificar(IUnitOfWork uow, long nuReporte, int cdCamion, int cdEmpresa, string cdCliente, string language, byte[] fileData, string fileName)
        {
            var empresa = uow.EmpresaRepository.GetEmpresa(cdEmpresa);
            var agente = uow.AgenteRepository.GetAgente(cdEmpresa, cdCliente);
            var camion = uow.CamionRepository.GetCamion(cdCamion);
            var reporte = uow.ReporteRepository.GetReporte(nuReporte);
            var instancias = uow.EventoRepository.GetInstanciasHabilitadas(EventoDb.PackingList, new Dictionary<string, string>
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

        public virtual List<long> Preparar()
        {
            var reports = new List<long>();
            var empresas = new List<int>();

            if (this._camion.Estado == CamionEstado.Cerrado)
            {
                empresas = _uow.CamionRepository.GetEmpresasCamion(this._camion.Id);
            }
            else
            {
                empresas = this._camion.GetEmpresas();
            }
            
            var puerta = _camion.Puerta.HasValue
                ? _uow.PuertaEmbarqueRepository.GetPuertaEmbarque(_camion.Puerta.Value)
                : null;

            var zonaPuerta = puerta != null
                ? _uow.UbicacionRepository.GetUbicacion(puerta.CodigoUbicacion)?.IdUbicacionZona
                : null;

            foreach (var empresa in empresas)
            {
                var reportes = this._parameterService.GetValueByEmpresa("EXPEDICION_REPORTES", empresa);

                if (string.IsNullOrEmpty(reportes) || !reportes.Contains($"\"{CReporte.PACKING_LIST}\""))
                    continue;

                List<string> clientes;

                if (this._camion.Estado == CamionEstado.Cerrado)
                {
                    clientes = _uow.CamionRepository.GetClientesCamion(this._camion.Id);
                }
                else
                {
                    clientes = this._camion.GetClientes(empresa);
                }

                foreach (var cliente in clientes)
                {
                    string clave = this.GetCompositeId(this._camion.Id, empresa, cliente);

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

        public virtual string GetCompositeId(int camionId, int empresa, string cliente)
        {
            return this._keyResolver.ResolveKey(Convert.ToString(camionId), Convert.ToString(empresa), cliente);
        }
    }
}
