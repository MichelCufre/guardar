using ClosedXML.Excel;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Configuracion;
using WIS.Exceptions;
using WIS.Extension;
using WIS.GridComponent;
using WIS.GridComponent.Excel;
using WIS.Translation;
using WIS.WMS_API.Controllers.Entrada;

namespace WIS.Domain.General.API
{
    public class GridExcelImporterAPI : ExcelImporterUtils, IDisposable
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly string _fileName;
        protected readonly List<IGridColumn> _columns;
        protected readonly MemoryStream _stream;
        protected readonly XLWorkbook _workbook;
        protected readonly ITranslator _translator;
        protected readonly int _interfazExterna;
        protected readonly int _empresa;
        protected readonly string _referencia;
        protected readonly IFormatProvider _proveedor;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly IOptions<MaxItemsSettings> _configuration;
        protected readonly bool _descargaErrores;

        public GridExcelImporterAPI(IUnitOfWorkFactory uowFactory,
            ITranslator translator,
            string fileName,
            int interfazExterna,
            int empresa,
            string referencia,
            string excelData,
            IFormatProvider proveedor,
            IOptions<MaxItemsSettings> configuration,
            bool descargaErrores)
        {
            _empresa = empresa;
            _fileName = fileName;
            _proveedor = proveedor;
            _translator = translator;
            _referencia = referencia;
            _uowFactory = uowFactory;
            _interfazExterna = interfazExterna;
            _stream = new MemoryStream(Convert.FromBase64String(excelData));
            _workbook = new XLWorkbook(this._stream);
            _configuration = configuration;
            _descargaErrores = descargaErrores;
        }

        public virtual string CreateRequests(out string method)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var json = string.Empty;
            var settings = new JsonSerializerSettings();
            var sheetNames = GetTranslatedValues();

            settings.Culture = CultureInfo.InvariantCulture;
            settings.DateFormatString = "yyyy-MM-dd";

            var interfazExterna = uow.InterfazRepository.GetInterfazExterna(_interfazExterna);
            method = interfazExterna.Endpoint;

            switch (_interfazExterna)
            {
                case CInterfazExterna.Producto:
                    json = JsonConvert.SerializeObject(CreateRequestProductos(), settings);
                    break;
                case CInterfazExterna.CodigoDeBarras:
                    json = JsonConvert.SerializeObject(CreateRequestBarras(), settings);
                    break;
                case CInterfazExterna.Empresas:
                    json = JsonConvert.SerializeObject(CreateRequestEmpresas(), settings);
                    break;
                case CInterfazExterna.Agentes:
                    json = JsonConvert.SerializeObject(CreateRequestAgente(), settings);
                    break;
                case CInterfazExterna.ProductoProveedor:
                    json = JsonConvert.SerializeObject(CreateRequestProductoProveedor(), settings);
                    break;
                case CInterfazExterna.ReferenciaDeRecepcion:
                    json = JsonConvert.SerializeObject(CreateRequestReferenciasRecepcion(sheetNames), settings);
                    break;
                case CInterfazExterna.Agendas:
                    json = JsonConvert.SerializeObject(CreateRequestAgendas(), settings);
                    break;
                case CInterfazExterna.Pedidos:
                    json = JsonConvert.SerializeObject(CreateRequestPedidos(sheetNames), settings);
                    break;
                case CInterfazExterna.Lpn:
                    json = JsonConvert.SerializeObject(CreateRequestLpn(sheetNames), settings);
                    break;
                case CInterfazExterna.Produccion:
                    json = JsonConvert.SerializeObject(CreateRequestProducciones(sheetNames), settings);
                    break;
                case CInterfazExterna.ControlDeCalidad:
                    json = JsonConvert.SerializeObject(CreateRequestControlDeCalidad(sheetNames), settings);
                    break;
                case CInterfazExterna.Facturas:
                    json = JsonConvert.SerializeObject(this.CreateRequestFacturas(sheetNames), settings);
                    break;
                case CInterfazExterna.PickingProducto:
                    json = JsonConvert.SerializeObject(CreateRequestPickingProducto(), settings);
                    break;
            }

            return json;
        }

        public virtual ProductosRequest CreateRequestProductos()
        {
            return CreateApiEntradaRequest<ProductosRequest, ProductoRequest>(CInterfazExterna.Producto, _configuration.Value.Producto, "Alta de productos desde el panel de ejecución");
        }

        public virtual CodigosBarrasRequest CreateRequestBarras()
        {
            return CreateApiEntradaRequest<CodigosBarrasRequest, CodigoBarraRequest>(CInterfazExterna.CodigoDeBarras, _configuration.Value.CodigoBarras, "Alta de códigos de barras desde el panel de ejecución");
        }

        public virtual EmpresasRequest CreateRequestEmpresas()
        {
            return CreateApiEntradaRequest<EmpresasRequest, EmpresaRequest>(CInterfazExterna.Empresas, _configuration.Value.Empresa, "Alta de empresas desde el panel de ejecución");
        }

        public virtual AgentesRequest CreateRequestAgente()
        {
            return CreateApiEntradaRequest<AgentesRequest, AgenteRequest>(CInterfazExterna.Agentes, _configuration.Value.Agente, "Alta de agentes desde el panel de ejecución");
        }

        public virtual AgendasRequest CreateRequestAgendas()
        {
            return CreateApiEntradaRequest<AgendasRequest, AgendaRequest>(CInterfazExterna.Agendas, _configuration.Value.Agenda, "Alta de agendas desde el panel de ejecución");
        }

        public virtual ProductosProveedorRequest CreateRequestProductoProveedor()
        {
            return CreateApiEntradaRequest<ProductosProveedorRequest, ProductoProveedorRequest>(CInterfazExterna.ProductoProveedor, _configuration.Value.ProductoProveedor, "Alta de productos proveedor desde el panel de ejecución");
        }

        public virtual ReferenciasRecepcionRequest CreateRequestReferenciasRecepcion(Dictionary<string, string> sheetNames)
        {
            var cellsToTranslate = new List<IXLCell>();
            var referencias = new Dictionary<string, ReferenciaRecepcionRequest>();
            var detallesReferencia = new Dictionary<string, ReferenciaRecepcionDetalleWithKeysRequest>();
            var dsReferencia = !string.IsNullOrEmpty(_referencia) ? _referencia : "Creación de referencias de recepcion ";

            var request = new ReferenciasRecepcionRequest()
            {
                Empresa = _empresa,
                DsReferencia = dsReferencia.Truncate(200),
                Archivo = "INT050",
            };

            ValidateTemplate(CInterfazExterna.ReferenciaDeRecepcion);

            var rowToReadFrom = 3;
            var rowIndex = 1;
            var cantColumns = 0;
            var colsName = new Dictionary<string, string>();

            var workSheet = this._workbook.Worksheets.First();
            var sheetName = sheetNames.GetValueOrDefault(GridExcelSheetNames.SheetDetailName, "Detalles");
            var detallesSheet = this._workbook.Worksheets.FirstOrDefault(p => p.Name == sheetName);

            ValidateMaxItems(_configuration.Value.ReferenciaRecepcion, workSheet, detallesSheet);

            #region Referencias

            foreach (var row in workSheet.RowsUsed())
            {
                SetColumnsName(row, rowIndex, ref colsName, ref cantColumns);

                if (rowIndex >= rowToReadFrom && !IsRowEmpty(row, cantColumns))
                {
                    var output = AnalyzeRow(GetCellsUsed(row, cantColumns), colsName, cellsToTranslate, typeof(ReferenciaRecepcionRequest));
                    var refereneciaRecepcion = (ReferenciaRecepcionRequest)output;

                    var key = $"{refereneciaRecepcion.Referencia}.{refereneciaRecepcion.TipoReferencia}.{refereneciaRecepcion.CodigoAgente}.{refereneciaRecepcion.TipoAgente}";

                    if (!referencias.ContainsKey(key))
                        referencias.Add(key, refereneciaRecepcion);
                    else
                    {
                        var referenciaSheet = workSheet.Cell(3, 1);
                        referenciaSheet.Style.Font.FontColor = XLColor.Red;
                        AddComment(referenciaSheet, "INT050_Sec0_Error_ImportExcelErrorReferenciaReferenciaRepetida", cellsToTranslate);
                    }
                }

                rowIndex++;
            }

            #endregion

            #region Detalles

            rowIndex = 1;

            foreach (var row in detallesSheet.RowsUsed())
            {
                SetColumnsName(row, rowIndex, ref colsName, ref cantColumns);

                if (rowIndex >= rowToReadFrom && !IsRowEmpty(row, cantColumns))
                {
                    var output = AnalyzeRow(GetCellsUsed(row, cantColumns), colsName, cellsToTranslate, typeof(ReferenciaRecepcionDetalleWithKeysRequest));
                    var refereneciaRecepcionDetalle = (ReferenciaRecepcionDetalleWithKeysRequest)output;

                    var keyReferencia = $"{refereneciaRecepcionDetalle.Referencia}.{refereneciaRecepcionDetalle.TipoReferencia}.{refereneciaRecepcionDetalle.CodigoAgente}.{refereneciaRecepcionDetalle.TipoAgente}";
                    var keyReferenciaDetalle = $"{refereneciaRecepcionDetalle.Referencia}.{refereneciaRecepcionDetalle.TipoReferencia}.{refereneciaRecepcionDetalle.CodigoAgente}.{refereneciaRecepcionDetalle.TipoAgente}.{refereneciaRecepcionDetalle.IdLineaSistemaExterno}.{refereneciaRecepcionDetalle.CodigoProducto}.{refereneciaRecepcionDetalle.Identificador}";

                    if (!detallesReferencia.ContainsKey(keyReferenciaDetalle))
                    {
                        if (referencias.ContainsKey(keyReferencia))
                        {
                            detallesReferencia.Add(keyReferenciaDetalle, refereneciaRecepcionDetalle);
                            referencias[keyReferencia].Detalles.Add(refereneciaRecepcionDetalle);
                        }
                        else
                        {
                            var detallesSheetCell = detallesSheet.Cell(3, 1);
                            detallesSheetCell.Style.Font.FontColor = XLColor.Red;
                            AddComment(detallesSheetCell, "INT050_Sec0_Error_ImportExcelErrorReferenciaDetalles", cellsToTranslate);
                        }
                    }
                    else
                    {
                        var detallesSheetCell = detallesSheet.Cell(3, 1);
                        detallesSheetCell.Style.Font.FontColor = XLColor.Red;
                        AddComment(detallesSheetCell, "INT050_Sec0_Error_ImportExcelErrorReferenciaDetalleRepetido", cellsToTranslate);
                    }
                }

                rowIndex++;
            }

            #endregion

            TranslateCells(cellsToTranslate);

            request.Referencias.Clear();
            request.Referencias.AddRange(referencias.Values);

            return request;
        }

        public virtual PedidosRequest CreateRequestPedidos(Dictionary<string, string> sheetNames)
        {
            var cellsToTranslate = new List<IXLCell>();
            var colPedidos = new Dictionary<string, PedidoRequest>();
            var colDetalles = new Dictionary<string, PedidoDetalleWithKeysRequest>();
            var colDuplicados = new Dictionary<string, PedidoDuplicadoWithKeysRequest>();
            var colDetalleLpn = new Dictionary<string, PedidoDetalleLpnWithKeysRequest>();
            var colLpn = new Dictionary<string, PedidoLpnWithKeysRequest>();
            var colAtributo = new Dictionary<string, PedidoAtributoWithKeysRequest>();
            var colAtributos = new Dictionary<string, PedidoAtributosWithKeysRequest>();
            var colAtributoLpn = new Dictionary<string, PedidoAtributoLpnWithKeysRequest>();
            var colAtributosLpn = new Dictionary<string, PedidoAtributosLpnWithKeysRequest>();
            var dsReferencia = !string.IsNullOrEmpty(_referencia) ? _referencia : "Creación de pedidos ";

            var request = new PedidosRequest()
            {
                Empresa = _empresa,
                DsReferencia = dsReferencia.Truncate(200),
                Archivo = "INT050",
            };

            ValidateTemplate(CInterfazExterna.Pedidos);

            var rowToReadFrom = 3;
            var rowIndex = 1;
            var cantColumns = 0;
            var colsName = new Dictionary<string, string>();

            var workSheet = this._workbook.Worksheets.First();
            var sheetName = sheetNames.GetValueOrDefault(GridExcelSheetNames.SheetDetailName, "Detalles");
            var detallesSheet = this._workbook.Worksheets.FirstOrDefault(p => p.Name == sheetName);

            ValidateMaxItems(_configuration.Value.Pedido, workSheet, detallesSheet);

            #region Pedidos

            foreach (var row in workSheet.RowsUsed())
            {
                SetColumnsName(row, rowIndex, ref colsName, ref cantColumns);

                if (rowIndex >= rowToReadFrom && !IsRowEmpty(row, cantColumns))
                {
                    var output = AnalyzeRow(GetCellsUsed(row, cantColumns), colsName, cellsToTranslate, typeof(PedidoRequest));
                    var pedido = (PedidoRequest)output;

                    var keyPedido = $"{pedido.NroPedido}.{pedido.CodigoAgente}.{pedido.TipoAgente}";

                    if (!colPedidos.ContainsKey(keyPedido))
                        colPedidos.Add(keyPedido, pedido);
                    else
                    {
                        var worksheetCell = workSheet.Cell(3, 1);
                        worksheetCell.Style.Font.FontColor = XLColor.Red;
                        AddComment(worksheetCell, "INT050_Sec0_Error_ImportExcelErrorPedidoPedidoRepetido", cellsToTranslate);
                    }
                }

                rowIndex++;
            }

            #endregion

            #region Detalles

            rowIndex = 1;

            foreach (var row in detallesSheet.RowsUsed())
            {
                SetColumnsName(row, rowIndex, ref colsName, ref cantColumns);

                if (rowIndex >= rowToReadFrom && !IsRowEmpty(row, cantColumns))
                {
                    var output = AnalyzeRow(GetCellsUsed(row, cantColumns), colsName, cellsToTranslate, typeof(PedidoDetalleWithKeysRequest));
                    var detallePedido = (PedidoDetalleWithKeysRequest)output;

                    var keyDetalle = $"{detallePedido.NroPedido}.{detallePedido.CodigoAgente}.{detallePedido.TipoAgente}.{detallePedido.CodigoProducto}.{detallePedido.Identificador}";
                    var keyPedido = $"{detallePedido.NroPedido}.{detallePedido.CodigoAgente}.{detallePedido.TipoAgente}";

                    if (detallePedido != null)
                    {
                        if (!colDetalles.ContainsKey(keyDetalle))
                        {
                            if (colPedidos.ContainsKey(keyPedido))
                            {
                                colDetalles.Add(keyDetalle, detallePedido);
                            }
                            else
                            {
                                var detallesSheetCell = detallesSheet.Cell(3, 1);
                                detallesSheetCell.Style.Font.FontColor = XLColor.Red;
                                AddComment(detallesSheetCell, "INT050_Sec0_Error_ImportExcelErrorDetalles", cellsToTranslate);
                            }
                        }
                        else
                        {
                            var detallesSheetCell = detallesSheet.Cell(3, 1);
                            detallesSheetCell.Style.Font.FontColor = XLColor.Red;
                            AddComment(detallesSheetCell, "INT050_Sec0_Error_ImportExcelErrorPedidoDetalleRepetido", cellsToTranslate);
                        }
                    }
                }

                rowIndex++;
            }

            #endregion

            #region Duplicados

            rowIndex = 1;
            sheetName = sheetNames.GetValueOrDefault(GridExcelSheetNames.SheetDuplicateName, "Duplicados");
            var duplicadosSheet = this._workbook.Worksheets.FirstOrDefault(p => p.Name == sheetName);

            foreach (var row in duplicadosSheet.RowsUsed())
            {
                SetColumnsName(row, rowIndex, ref colsName, ref cantColumns);

                if (rowIndex >= rowToReadFrom && !IsRowEmpty(row, cantColumns))
                {
                    var output = AnalyzeRow(GetCellsUsed(row, cantColumns), colsName, cellsToTranslate, typeof(PedidoDuplicadoWithKeysRequest));
                    var duplicado = (PedidoDuplicadoWithKeysRequest)output;

                    if (duplicado != null)
                    {
                        var keyDetalle = $"{duplicado.NroPedido}.{duplicado.CodigoAgente}.{duplicado.TipoAgente}.{duplicado.CodigoProducto}.{duplicado.Identificador}";
                        var keyDuplicado = $"{duplicado.NroPedido}.{duplicado.CodigoAgente}.{duplicado.TipoAgente}.{duplicado.CodigoProducto}.{duplicado.Identificador}.{duplicado.IdLineaSistemaExterno}";

                        if (!colDuplicados.ContainsKey(keyDuplicado))
                        {
                            if (colDetalles.ContainsKey(keyDetalle))
                            {
                                colDuplicados.Add(keyDuplicado, duplicado);
                                colDetalles[keyDetalle].Duplicados.Add(duplicado);
                            }
                            else
                            {
                                var detallesDuplicadosSheetCell = duplicadosSheet.Cell(3, 1);
                                detallesDuplicadosSheetCell.Style.Font.FontColor = XLColor.Red;
                                AddComment(detallesDuplicadosSheetCell, "INT050_Sec0_Error_ImportExcelErrorDuplicados", cellsToTranslate);
                            }
                        }
                        else
                        {
                            var detallesDuplicadosSheetCell = duplicadosSheet.Cell(3, 1);
                            detallesDuplicadosSheetCell.Style.Font.FontColor = XLColor.Red;
                            AddComment(detallesDuplicadosSheetCell, "INT050_Sec0_Error_ImportExcelErrorPedidoDuplicadoRepetido", cellsToTranslate);
                        }
                    }
                }

                rowIndex++;
            }

            #endregion

            #region Detalles LPN

            rowIndex = 1;
            sheetName = sheetNames.GetValueOrDefault(GridExcelSheetNames.SheetDetalleLpnName, "Detalle LPN");
            var detalleLpnSheet = this._workbook.Worksheets.FirstOrDefault(p => p.Name == sheetName);

            foreach (var row in detalleLpnSheet.RowsUsed())
            {
                SetColumnsName(row, rowIndex, ref colsName, ref cantColumns);

                if (rowIndex >= rowToReadFrom && !IsRowEmpty(row, cantColumns))
                {
                    var output = AnalyzeRow(GetCellsUsed(row, cantColumns), colsName, cellsToTranslate, typeof(PedidoDetalleLpnWithKeysRequest));
                    var detalleLpn = (PedidoDetalleLpnWithKeysRequest)output;

                    if (detalleLpn != null)
                    {
                        var keyDetalle = $"{detalleLpn.NroPedido}.{detalleLpn.CodigoAgente}.{detalleLpn.TipoAgente}.{detalleLpn.CodigoProducto}.{detalleLpn.Identificador}";
                        var keyDetalleAuto = $"{detalleLpn.NroPedido}.{detalleLpn.CodigoAgente}.{detalleLpn.TipoAgente}.{detalleLpn.CodigoProducto}.{ManejoIdentificadorDb.IdentificadorAuto}";
                        var keyDetalleAutoVacio = $"{detalleLpn.NroPedido}.{detalleLpn.CodigoAgente}.{detalleLpn.TipoAgente}.{detalleLpn.CodigoProducto}.";
                        var keyDetalleLpn = $"{detalleLpn.NroPedido}.{detalleLpn.CodigoAgente}.{detalleLpn.TipoAgente}.{detalleLpn.CodigoProducto}.{detalleLpn.Identificador}.{detalleLpn.IdLpnExterno}.{detalleLpn.TipoLpn}";

                        if (!colDetalleLpn.ContainsKey(keyDetalleLpn))
                        {
                            if (colDetalles.ContainsKey(keyDetalle) || colDetalles.ContainsKey(keyDetalleAuto) || colDetalles.ContainsKey(keyDetalleAutoVacio))
                            {
                                if (colDetalles.ContainsKey(keyDetalle))
                                {
                                    colDetalleLpn.Add(keyDetalleLpn, detalleLpn);
                                    colDetalles[keyDetalle].DetallesLpn.Add(detalleLpn);
                                }
                                else if (colDetalles.ContainsKey(keyDetalleAuto))
                                {
                                    colDetalleLpn.Add(keyDetalleLpn, detalleLpn);
                                    colDetalles[keyDetalleAuto].DetallesLpn.Add(detalleLpn);
                                }
                                else
                                {
                                    colDetalleLpn.Add(keyDetalleLpn, detalleLpn);
                                    colDetalles[keyDetalleAutoVacio].DetallesLpn.Add(detalleLpn);
                                }
                            }
                            else
                            {
                                var detallesDetalleLpnSheetCell = detalleLpnSheet.Cell(3, 1);
                                detallesDetalleLpnSheetCell.Style.Font.FontColor = XLColor.Red;
                                AddComment(detallesDetalleLpnSheetCell, "INT050_Sec0_Error_ImportExcelErrorLpn", cellsToTranslate);
                            }
                        }
                        else
                        {
                            var detallesDetalleLpnSheetCell = detalleLpnSheet.Cell(3, 1);
                            detallesDetalleLpnSheetCell.Style.Font.FontColor = XLColor.Red;
                            AddComment(detallesDetalleLpnSheetCell, "INT050_Sec0_Error_ImportExcelErrorLpnDetalleRepetido", cellsToTranslate);
                        }
                    }
                }

                rowIndex++;
            }

            #endregion

            #region LPN

            rowIndex = 1;
            sheetName = sheetNames.GetValueOrDefault(GridExcelSheetNames.SheetLpnName, "LPN");
            var lpnSheet = this._workbook.Worksheets.FirstOrDefault(p => p.Name == sheetName);

            foreach (var row in lpnSheet.RowsUsed())
            {
                SetColumnsName(row, rowIndex, ref colsName, ref cantColumns);

                if (rowIndex >= rowToReadFrom && !IsRowEmpty(row, cantColumns))
                {
                    var output = AnalyzeRow(GetCellsUsed(row, cantColumns), colsName, cellsToTranslate, typeof(PedidoLpnWithKeysRequest));
                    var lpn = (PedidoLpnWithKeysRequest)output;

                    if (lpn != null)
                    {
                        var keyDetalleLpn = $"{lpn.NroPedido}.{lpn.CodigoAgente}.{lpn.TipoAgente}.{lpn.IdExterno}.{lpn.Tipo}";

                        if (!colLpn.ContainsKey(keyDetalleLpn))
                        {
                            colLpn.Add(keyDetalleLpn, lpn);
                        }
                        else
                        {
                            var detallesLpnSheetCell = lpnSheet.Cell(3, 1);
                            detallesLpnSheetCell.Style.Font.FontColor = XLColor.Red;
                            AddComment(detallesLpnSheetCell, "INT050_Sec0_Error_ImportExcelErrorLpn", cellsToTranslate);
                        }
                    }
                    else
                    {
                        var detallesLpnSheetCell = lpnSheet.Cell(3, 1);
                        detallesLpnSheetCell.Style.Font.FontColor = XLColor.Red;
                        AddComment(detallesLpnSheetCell, "INT050_Sec0_Error_ImportExcelErrorLpnRepetido", cellsToTranslate);
                    }
                }

                rowIndex++;
            }

            #endregion

            #region Atributos LPN

            rowIndex = 1;
            sheetName = sheetNames.GetValueOrDefault(GridExcelSheetNames.SheetAtributosLpnName, "Atributos LPN");
            var lpnAttributesSheet = this._workbook.Worksheets.FirstOrDefault(p => p.Name == sheetName);

            foreach (var row in lpnAttributesSheet.RowsUsed())
            {
                SetColumnsName(row, rowIndex, ref colsName, ref cantColumns);

                if (rowIndex >= rowToReadFrom && !IsRowEmpty(row, cantColumns))
                {
                    var output = AnalyzeRow(GetCellsUsed(row, cantColumns), colsName, cellsToTranslate, typeof(PedidoAtributosLpnWithKeysRequest));
                    var configuracion = (PedidoAtributosLpnWithKeysRequest)output;

                    if (configuracion != null)
                    {
                        var keyDetalle = $"{configuracion.NroPedido}.{configuracion.CodigoAgente}.{configuracion.TipoAgente}.{configuracion.CodigoProducto}.{configuracion.Identificador}";
                        var keyDetalleAuto = $"{configuracion.NroPedido}.{configuracion.CodigoAgente}.{configuracion.TipoAgente}.{configuracion.CodigoProducto}.{ManejoIdentificadorDb.IdentificadorAuto}";
                        var keyDetalleAutoVacio = $"{configuracion.NroPedido}.{configuracion.CodigoAgente}.{configuracion.TipoAgente}.{configuracion.CodigoProducto}.";
                        var keyAtributosLpn = $"{configuracion.NroPedido}.{configuracion.CodigoAgente}.{configuracion.TipoAgente}.{configuracion.CodigoProducto}.{configuracion.Identificador}.{configuracion.IdLpnExterno}.{configuracion.TipoLpn}.{configuracion.IdConfiguracion}";

                        if (!colAtributosLpn.ContainsKey(keyAtributosLpn))
                        {
                            if (colDetalles.ContainsKey(keyDetalle) || colDetalles.ContainsKey(keyDetalleAuto) || colDetalles.ContainsKey(keyDetalleAutoVacio))
                            {
                                var keyDetalleLpn = keyDetalle;

                                if (colDetalles.ContainsKey(keyDetalleAuto))
                                {
                                    keyDetalleLpn = keyDetalleAuto;
                                }
                                else if (colDetalles.ContainsKey(keyDetalleAutoVacio))
                                {
                                    keyDetalleLpn = keyDetalleAutoVacio;
                                }

                                keyDetalleLpn += $".{configuracion.IdLpnExterno}.{configuracion.TipoLpn}";

                                if (colDetalleLpn.ContainsKey(keyDetalleLpn))
                                {
                                    colAtributosLpn.Add(keyAtributosLpn, configuracion);
                                    colDetalleLpn[keyDetalleLpn].Atributos.Add(configuracion);
                                }
                                else
                                {
                                    var detallesAtributosLpnSheetCell = lpnAttributesSheet.Cell(3, 1);
                                    detallesAtributosLpnSheetCell.Style.Font.FontColor = XLColor.Red;
                                    AddComment(detallesAtributosLpnSheetCell, "INT050_Sec0_Error_ImportExcelErrorAtributos", cellsToTranslate);
                                }
                            }
                            else
                            {
                                var detallesAtributosLpnSheetCell = lpnAttributesSheet.Cell(3, 1);
                                detallesAtributosLpnSheetCell.Style.Font.FontColor = XLColor.Red;
                                AddComment(detallesAtributosLpnSheetCell, "INT050_Sec0_Error_ImportExcelErrorAtributos", cellsToTranslate);
                            }
                        }
                        else
                        {
                            var detallesAtributosLpnSheetCell = lpnAttributesSheet.Cell(3, 1);
                            detallesAtributosLpnSheetCell.Style.Font.FontColor = XLColor.Red;
                            AddComment(detallesAtributosLpnSheetCell, "INT050_Sec0_Error_ImportExcelErrorAtributosRepetido", cellsToTranslate);
                        }
                    }
                }

                rowIndex++;
            }

            #endregion

            #region Detalle Atributos LPN

            rowIndex = 1;
            sheetName = sheetNames.GetValueOrDefault(GridExcelSheetNames.SheetAtributosLpnDetailName, "Atributos LPN Detalle");
            var lpnAttributeSheet = this._workbook.Worksheets.FirstOrDefault(p => p.Name == sheetName);

            foreach (var row in lpnAttributeSheet.RowsUsed())
            {
                SetColumnsName(row, rowIndex, ref colsName, ref cantColumns);

                if (rowIndex >= rowToReadFrom && !IsRowEmpty(row, cantColumns))
                {
                    var output = AnalyzeRow(GetCellsUsed(row, cantColumns), colsName, cellsToTranslate, typeof(PedidoAtributoLpnWithKeysRequest));
                    var atributo = (PedidoAtributoLpnWithKeysRequest)output;

                    if (atributo != null)
                    {
                        var keyAtributosLpn = $"{atributo.NroPedido}.{atributo.CodigoAgente}.{atributo.TipoAgente}.{atributo.CodigoProducto}.{atributo.Identificador}.{atributo.IdLpnExterno}.{atributo.TipoLpn}.{atributo.IdConfiguracion}";
                        var keyAtributoLpn = $"{keyAtributosLpn}.{atributo.Nombre}";

                        if (!colAtributoLpn.ContainsKey(keyAtributoLpn))
                        {
                            if (colAtributosLpn.ContainsKey(keyAtributosLpn))
                            {
                                colAtributoLpn.Add(keyAtributoLpn, atributo);
                                colAtributosLpn[keyAtributosLpn].Atributos.Add(atributo);
                            }
                            else
                            {
                                var detallesAtributoLpnSheetCell = lpnAttributeSheet.Cell(3, 1);
                                detallesAtributoLpnSheetCell.Style.Font.FontColor = XLColor.Red;
                                AddComment(detallesAtributoLpnSheetCell, "INT050_Sec0_Error_ImportExcelErrorAtributo", cellsToTranslate);
                            }
                        }
                        else
                        {
                            var detallesAtributoLpnSheetCell = lpnAttributeSheet.Cell(3, 1);
                            detallesAtributoLpnSheetCell.Style.Font.FontColor = XLColor.Red;
                            AddComment(detallesAtributoLpnSheetCell, "INT050_Sec0_Error_ImportExcelErrorAtributoRepetido", cellsToTranslate);
                        }
                    }
                }

                rowIndex++;
            }

            #endregion

            #region Atributos

            rowIndex = 1;
            sheetName = sheetNames.GetValueOrDefault(GridExcelSheetNames.SheetAtributosName, "Atributos");
            var attributesSheet = this._workbook.Worksheets.FirstOrDefault(p => p.Name == sheetName);

            foreach (var row in attributesSheet.RowsUsed())
            {
                SetColumnsName(row, rowIndex, ref colsName, ref cantColumns);

                if (rowIndex >= rowToReadFrom && !IsRowEmpty(row, cantColumns))
                {
                    var output = AnalyzeRow(GetCellsUsed(row, cantColumns), colsName, cellsToTranslate, typeof(PedidoAtributosWithKeysRequest));
                    var configuracion = (PedidoAtributosWithKeysRequest)output;

                    if (configuracion != null)
                    {
                        var keyDetalle = $"{configuracion.NroPedido}.{configuracion.CodigoAgente}.{configuracion.TipoAgente}.{configuracion.CodigoProducto}.{configuracion.Identificador}";
                        var keyDetalleAuto = $"{configuracion.NroPedido}.{configuracion.CodigoAgente}.{configuracion.TipoAgente}.{configuracion.CodigoProducto}.{ManejoIdentificadorDb.IdentificadorAuto}";
                        var keyDetalleAutoVacio = $"{configuracion.NroPedido}.{configuracion.CodigoAgente}.{configuracion.TipoAgente}.{configuracion.CodigoProducto}.";
                        var keyAtributos = $"{configuracion.NroPedido}.{configuracion.CodigoAgente}.{configuracion.TipoAgente}.{configuracion.CodigoProducto}.{configuracion.Identificador}.{configuracion.IdConfiguracion}";

                        if (!colAtributos.ContainsKey(keyAtributos))
                        {
                            if (colDetalles.ContainsKey(keyDetalle) || colDetalles.ContainsKey(keyDetalleAuto) || colDetalles.ContainsKey(keyDetalleAutoVacio))
                            {
                                if (colDetalles.ContainsKey(keyDetalle))
                                {
                                    colAtributos.Add(keyAtributos, configuracion);
                                    colDetalles[keyDetalle].Atributos.Add(configuracion);
                                }
                                else if (colDetalles.ContainsKey(keyDetalleAuto))
                                {
                                    colAtributos.Add(keyAtributos, configuracion);
                                    colDetalles[keyDetalleAuto].Atributos.Add(configuracion);
                                }
                                else
                                {
                                    colAtributos.Add(keyAtributos, configuracion);
                                    colDetalles[keyDetalleAutoVacio].Atributos.Add(configuracion);
                                }
                            }
                            else
                            {
                                var detallesAtributosSheetCell = attributesSheet.Cell(3, 1);
                                detallesAtributosSheetCell.Style.Font.FontColor = XLColor.Red;
                                AddComment(detallesAtributosSheetCell, "INT050_Sec0_Error_ImportExcelErrorAtributos", cellsToTranslate);
                            }
                        }
                        else
                        {
                            var detallesAtributosSheetCell = attributesSheet.Cell(3, 1);
                            detallesAtributosSheetCell.Style.Font.FontColor = XLColor.Red;
                            AddComment(detallesAtributosSheetCell, "INT050_Sec0_Error_ImportExcelErrorAtributosRepetido", cellsToTranslate);
                        }
                    }
                }

                rowIndex++;
            }

            #endregion

            #region Detalle Atributos

            rowIndex = 1;
            sheetName = sheetNames.GetValueOrDefault(GridExcelSheetNames.SheetAtributosDetailName, "Atributos Detalle");
            var attributeSheet = this._workbook.Worksheets.FirstOrDefault(p => p.Name == sheetName);

            foreach (var row in attributeSheet.RowsUsed())
            {
                SetColumnsName(row, rowIndex, ref colsName, ref cantColumns);

                if (rowIndex >= rowToReadFrom && !IsRowEmpty(row, cantColumns))
                {
                    var output = AnalyzeRow(GetCellsUsed(row, cantColumns), colsName, cellsToTranslate, typeof(PedidoAtributoWithKeysRequest));
                    var atributo = (PedidoAtributoWithKeysRequest)output;

                    if (atributo != null)
                    {
                        var keyAtributos = $"{atributo.NroPedido}.{atributo.CodigoAgente}.{atributo.TipoAgente}.{atributo.CodigoProducto}.{atributo.Identificador}.{atributo.IdConfiguracion}";
                        var keyAtributo = $"{keyAtributos}.{atributo.Tipo}.{atributo.Nombre}";

                        if (!colAtributo.ContainsKey(keyAtributo))
                        {
                            if (colAtributos.ContainsKey(keyAtributos))
                            {
                                colAtributo.Add(keyAtributo, atributo);
                                colAtributos[keyAtributos].Atributos.Add(atributo);
                            }
                            else
                            {
                                var detallesAtributoSheetCell = attributeSheet.Cell(3, 1);
                                detallesAtributoSheetCell.Style.Font.FontColor = XLColor.Red;
                                AddComment(detallesAtributoSheetCell, "INT050_Sec0_Error_ImportExcelErrorAtributo", cellsToTranslate);
                            }
                        }
                        else
                        {
                            var detallesAtributoSheetCell = attributeSheet.Cell(3, 1);
                            detallesAtributoSheetCell.Style.Font.FontColor = XLColor.Red;
                            AddComment(detallesAtributoSheetCell, "INT050_Sec0_Error_ImportExcelErrorAtributoRepetido", cellsToTranslate);
                        }
                    }
                }

                rowIndex++;
            }

            #endregion

            foreach (var lpn in colLpn.Values)
            {
                var key = $"{lpn.NroPedido}.{lpn.CodigoAgente}.{lpn.TipoAgente}";

                if (colPedidos.ContainsKey(key))
                    colPedidos[key].Lpns.Add(lpn);
            }

            foreach (var detalle in colDetalles.Values)
            {
                var key = $"{detalle.NroPedido}.{detalle.CodigoAgente}.{detalle.TipoAgente}";

                if (colPedidos.ContainsKey(key))
                    colPedidos[key].Detalles.Add(detalle);
            }

            TranslateCells(cellsToTranslate);

            request.Pedidos.Clear();
            request.Pedidos.AddRange(colPedidos.Values);

            return request;
        }

        public virtual LpnsRequest CreateRequestLpn(Dictionary<string, string> sheetNames)
        {
            var cellsToTranslate = new List<IXLCell>();
            var colLpn = new Dictionary<string, LpnRequest>();
            var colBarras = new Dictionary<string, LpnBarrasWithKeysRequest>();
            var colDetalles = new Dictionary<string, LpnDetalleWithKeysRequest>();
            var colAtributos = new Dictionary<string, LpnAtributoWithKeysRequest>();
            var colAtributosDetalle = new Dictionary<string, LpnAtributoDetalleWithKeysRequest>();
            var dsReferencia = !string.IsNullOrEmpty(_referencia) ? _referencia : "Creación de Lpn ";

            var request = new LpnsRequest()
            {
                Empresa = _empresa,
                DsReferencia = dsReferencia.Truncate(200),
                Archivo = "INT050",
            };

            ValidateTemplate(CInterfazExterna.Lpn);

            var rowToReadFrom = 3;
            var rowIndex = 1;
            var cantColumns = 0;
            var colsName = new Dictionary<string, string>();

            var workSheet = this._workbook.Worksheets.First();
            var sheetName = sheetNames.GetValueOrDefault(GridExcelSheetNames.SheetDetailName, "Detalles");
            var detallesSheet = this._workbook.Worksheets.FirstOrDefault(p => p.Name == sheetName);

            sheetName = sheetNames.GetValueOrDefault(GridExcelSheetNames.SheetAtributosDetailName, "Atributos Detalle");
            var atributosDetalleSheet = this._workbook.Worksheets.FirstOrDefault(p => p.Name == sheetName);

            ValidateMaxItems(_configuration.Value.Lpn, workSheet, detallesSheet, atributosDetalleSheet);

            #region LPNs

            foreach (var row in workSheet.RowsUsed())
            {
                SetColumnsName(row, rowIndex, ref colsName, ref cantColumns);

                if (rowIndex >= rowToReadFrom && !IsRowEmpty(row, cantColumns))
                {
                    var output = AnalyzeRow(GetCellsUsed(row, cantColumns), colsName, cellsToTranslate, typeof(LpnRequest));
                    var lpn = (LpnRequest)output;

                    var keyLpn = $"{lpn.IdExterno}";

                    if (!colLpn.ContainsKey(keyLpn))
                        colLpn.Add(keyLpn, lpn);
                    else
                    {
                        var worksheetCell = workSheet.Cell(3, 1);
                        worksheetCell.Style.Font.FontColor = XLColor.Red;
                        AddComment(worksheetCell, "INT050_Sec0_Error_ImportExcelErrorLpnRepetido", cellsToTranslate);
                    }
                }

                rowIndex++;
            }

            #endregion

            #region Detalles

            rowIndex = 1;

            foreach (var row in detallesSheet.RowsUsed())
            {
                SetColumnsName(row, rowIndex, ref colsName, ref cantColumns);

                if (rowIndex >= rowToReadFrom && !IsRowEmpty(row, cantColumns))
                {
                    var output = AnalyzeRow(GetCellsUsed(row, cantColumns), colsName, cellsToTranslate, typeof(LpnDetalleWithKeysRequest));
                    var detalleLpn = (LpnDetalleWithKeysRequest)output;

                    var keyDetalle = $"{detalleLpn.IdExterno}.{detalleLpn.IdLineaSistemaExterno}";
                    var keyLpn = $"{detalleLpn.IdExterno}";

                    if (detalleLpn != null)
                    {
                        if (!colDetalles.ContainsKey(keyDetalle))
                        {
                            if (colLpn.ContainsKey(keyLpn))
                            {
                                colDetalles.Add(keyDetalle, detalleLpn);
                            }
                            else
                            {
                                var detallesSheetCell = detallesSheet.Cell(3, 1);
                                detallesSheetCell.Style.Font.FontColor = XLColor.Red;
                                AddComment(detallesSheetCell, "INT050_Sec0_Error_ImportExcelErrorDetallesLpn", cellsToTranslate);
                            }
                        }
                        else
                        {
                            var detallesSheetCell = detallesSheet.Cell(3, 1);
                            detallesSheetCell.Style.Font.FontColor = XLColor.Red;
                            AddComment(detallesSheetCell, "INT050_Sec0_Error_ImportExcelErrorLpnDetalleRepetido", cellsToTranslate);
                        }
                    }
                }

                rowIndex++;
            }

            #endregion

            #region Atributos Cabezal

            rowIndex = 1;
            sheetName = sheetNames.GetValueOrDefault(GridExcelSheetNames.SheetAtributosName, "Atributos");
            var atributosSheet = this._workbook.Worksheets.FirstOrDefault(p => p.Name == sheetName);

            foreach (var row in atributosSheet.RowsUsed())
            {
                SetColumnsName(row, rowIndex, ref colsName, ref cantColumns);

                if (rowIndex >= rowToReadFrom && !IsRowEmpty(row, cantColumns))
                {
                    var output = AnalyzeRow(GetCellsUsed(row, cantColumns), colsName, cellsToTranslate, typeof(LpnAtributoWithKeysRequest));
                    var atributo = (LpnAtributoWithKeysRequest)output;

                    var keyAtributo = $"{atributo.IdExterno}.{atributo.Nombre}";
                    var keyLpn = $"{atributo.IdExterno}";

                    if (atributo != null)
                    {
                        if (!colAtributos.ContainsKey(keyAtributo))
                        {
                            if (colLpn.ContainsKey(keyLpn))
                            {
                                colAtributos.Add(keyAtributo, atributo);
                            }
                            else
                            {
                                var atributosSheetCell = atributosSheet.Cell(3, 1);
                                atributosSheetCell.Style.Font.FontColor = XLColor.Red;
                                AddComment(atributosSheetCell, "INT050_Sec0_Error_ImportExcelErrorAtributosLpn", cellsToTranslate);
                            }
                        }
                        else
                        {
                            var atributosSheetCell = atributosSheet.Cell(3, 1);
                            atributosSheetCell.Style.Font.FontColor = XLColor.Red;
                            AddComment(atributosSheetCell, "INT050_Sec0_Error_ImportExcelErrorAtributosRepetido", cellsToTranslate);
                        }
                    }
                }

                rowIndex++;
            }

            #endregion

            #region Atributos detalles

            rowIndex = 1;

            foreach (var row in atributosDetalleSheet.RowsUsed())
            {
                SetColumnsName(row, rowIndex, ref colsName, ref cantColumns);

                if (rowIndex >= rowToReadFrom && !IsRowEmpty(row, cantColumns))
                {
                    var output = AnalyzeRow(GetCellsUsed(row, cantColumns), colsName, cellsToTranslate, typeof(LpnAtributoDetalleWithKeysRequest));
                    var atributoDetalle = (LpnAtributoDetalleWithKeysRequest)output;

                    var keyAtributoDetalle = $"{atributoDetalle.IdExterno}.{atributoDetalle.IdLineaSistemaExterno}.{atributoDetalle.Nombre}";
                    var keyDetalle = $"{atributoDetalle.IdExterno}.{atributoDetalle.IdLineaSistemaExterno}";

                    if (atributoDetalle != null)
                    {
                        if (!colAtributosDetalle.ContainsKey(keyAtributoDetalle))
                        {
                            if (colDetalles.ContainsKey(keyDetalle))
                            {
                                colAtributosDetalle.Add(keyAtributoDetalle, atributoDetalle);
                            }
                            else
                            {
                                var atributosDetalleSheetCell = atributosDetalleSheet.Cell(3, 1);
                                atributosDetalleSheetCell.Style.Font.FontColor = XLColor.Red;
                                AddComment(atributosDetalleSheetCell, "INT050_Sec0_Error_ImportExcelErrorAtributosDetalle", cellsToTranslate);
                            }
                        }
                        else
                        {
                            var atributosDetalleSheetCell = atributosDetalleSheet.Cell(3, 1);
                            atributosDetalleSheetCell.Style.Font.FontColor = XLColor.Red;
                            AddComment(atributosDetalleSheetCell, "INT050_Sec0_Error_ImportExcelErrorAtributosDetalleRepetido", cellsToTranslate);
                        }
                    }
                }

                rowIndex++;
            }

            #endregion

            #region Códigos de Barras

            rowIndex = 1;
            sheetName = sheetNames.GetValueOrDefault(GridExcelSheetNames.SheetBarrasName, "Barras");
            var barrasSheet = this._workbook.Worksheets.FirstOrDefault(p => p.Name == sheetName);

            foreach (var row in barrasSheet.RowsUsed())
            {
                SetColumnsName(row, rowIndex, ref colsName, ref cantColumns);

                if (rowIndex >= rowToReadFrom && !IsRowEmpty(row, cantColumns))
                {
                    var output = AnalyzeRow(GetCellsUsed(row, cantColumns), colsName, cellsToTranslate, typeof(LpnBarrasWithKeysRequest));
                    var barra = (LpnBarrasWithKeysRequest)output;

                    var keyBarra = $"{barra.IdExterno}.{barra.CodigoBarras}";
                    var keyLpn = $"{barra.IdExterno}";

                    if (barra != null)
                    {
                        if (!colBarras.ContainsKey(keyBarra))
                        {
                            if (colLpn.ContainsKey(keyLpn))
                            {
                                colBarras.Add(keyBarra, barra);
                            }
                            else
                            {
                                var barrasSheetCell = barrasSheet.Cell(3, 1);
                                barrasSheetCell.Style.Font.FontColor = XLColor.Red;
                                AddComment(barrasSheetCell, "INT050_Sec0_Error_ImportExcelErrorBarras", cellsToTranslate);
                            }
                        }
                        else
                        {
                            var barrasSheetCell = barrasSheet.Cell(3, 1);
                            barrasSheetCell.Style.Font.FontColor = XLColor.Red;
                            AddComment(barrasSheetCell, "INT050_Sec0_Error_ImportExcelErrorBarrasRepetido", cellsToTranslate);
                        }
                    }
                }

                rowIndex++;
            }

            #endregion

            foreach (var atributo in colAtributosDetalle.Values)
            {
                var key = $"{atributo.IdExterno}.{atributo.IdLineaSistemaExterno}";

                if (colDetalles.ContainsKey(key))
                    colDetalles[key].Atributos.Add(atributo);
            }

            foreach (var detalle in colDetalles.Values)
            {
                var key = $"{detalle.IdExterno}";

                if (colLpn.ContainsKey(key))
                    colLpn[key].Detalles.Add(detalle);
            }

            foreach (var atributo in colAtributos.Values)
            {
                var key = $"{atributo.IdExterno}";

                if (colLpn.ContainsKey(key))
                    colLpn[key].Atributos.Add(atributo);
            }

            foreach (var barra in colBarras.Values)
            {
                var key = $"{barra.IdExterno}";

                if (colLpn[key].Barras == null)
                    colLpn[key].Barras = new List<BarrasRequest>();

                if (colLpn.ContainsKey(key))
                    colLpn[key].Barras.Add(barra);
            }

            TranslateCells(cellsToTranslate);

            request.Lpns.Clear();
            request.Lpns.AddRange(colLpn.Values);

            return request;
        }

        public virtual ProduccionesRequest CreateRequestProducciones(Dictionary<string, string> sheetNames)
        {
            var cellsToTranslate = new List<IXLCell>();
            var produccionRequest = new Dictionary<string, ProduccionRequest>();
            var produccionInsumo = new Dictionary<string, List<ProduccionInsumoRequestWithKeyProduccion>>();
            var produccionProducto = new Dictionary<string, List<ProduccionProductoFinalesRequestWithKeyProduccion>>();
            var dsReferencia = !string.IsNullOrEmpty(_referencia) ? _referencia : "Creación de Ingresos Produccion";

            var request = new ProduccionesRequest()
            {
                Empresa = _empresa,
                DsReferencia = dsReferencia.Truncate(200),
                Archivo = "INT050",
            };

            ValidateTemplate(CInterfazExterna.Produccion);

            var rowToReadFrom = 3;
            var rowIndex = 1;
            var cantColumns = 0;
            var colsName = new Dictionary<string, string>();

            var workSheet = this._workbook.Worksheets.First();
            var sheetName = sheetNames.GetValueOrDefault(GridExcelSheetNames.SheetProduccionInsumo, "Insumos");
            var insumosSheet = this._workbook.Worksheets.FirstOrDefault(p => p.Name == sheetName);

            sheetName = sheetNames.GetValueOrDefault(GridExcelSheetNames.SheetProduccionProducto, "Productos");
            var productosSheet = this._workbook.Worksheets.FirstOrDefault(p => p.Name == sheetName);

            ValidateMaxItems(_configuration.Value.Produccion, workSheet, insumosSheet, productosSheet);

            #region Producciones

            foreach (var row in workSheet.RowsUsed())
            {
                SetColumnsName(row, rowIndex, ref colsName, ref cantColumns);

                if (rowIndex >= rowToReadFrom && !IsRowEmpty(row, cantColumns))
                {
                    var output = AnalyzeRow(GetCellsUsed(row, cantColumns), colsName, cellsToTranslate, typeof(ProduccionRequest));
                    var produccion = (ProduccionRequest)output;

                    var keyProduccion = $"{produccion.IdProduccionExterno}";

                    if (!produccionRequest.TryAdd(keyProduccion, produccion))
                    {
                        IXLCell worksheetCell = workSheet.Cell(3, 1);
                        worksheetCell.Style.Font.FontColor = XLColor.Red;
                        AddComment(worksheetCell, "INT050_Sec0_Error_ImportExcelErrorProduccionRepetido", cellsToTranslate);
                    }
                }

                rowIndex++;
            }

            #endregion

            #region Insumos

            rowIndex = 1;

            foreach (var row in insumosSheet.RowsUsed())
            {
                SetColumnsName(row, rowIndex, ref colsName, ref cantColumns);

                if (rowIndex >= rowToReadFrom && !IsRowEmpty(row, cantColumns))
                {
                    var output = AnalyzeRow(GetCellsUsed(row, cantColumns), colsName, cellsToTranslate, typeof(ProduccionInsumoRequestWithKeyProduccion));
                    var insumo = (ProduccionInsumoRequestWithKeyProduccion)output;

                    if (!produccionInsumo.ContainsKey(insumo.IdProduccionExterno))
                    {
                        List<ProduccionInsumoRequestWithKeyProduccion> newList = [insumo];
                        produccionInsumo.Add(insumo.IdProduccionExterno, newList);
                    }
                    else
                        produccionInsumo[insumo.IdProduccionExterno].Add(insumo);
                }

                rowIndex++;
            }

            #endregion

            #region Productos

            rowIndex = 1;

            foreach (var row in productosSheet.RowsUsed())
            {
                SetColumnsName(row, rowIndex, ref colsName, ref cantColumns);

                if (rowIndex >= rowToReadFrom && !IsRowEmpty(row, cantColumns))
                {
                    var output = AnalyzeRow(GetCellsUsed(row, cantColumns), colsName, cellsToTranslate, typeof(ProduccionProductoFinalesRequestWithKeyProduccion));
                    var producto = (ProduccionProductoFinalesRequestWithKeyProduccion)output;

                    if (!produccionProducto.ContainsKey(producto.IdProduccionExterno))
                    {
                        List<ProduccionProductoFinalesRequestWithKeyProduccion> newList = [producto];
                        produccionProducto.Add(producto.IdProduccionExterno, newList);
                    }
                    else
                        produccionProducto[producto.IdProduccionExterno].Add(producto);
                }

                rowIndex++;
            }

            #endregion

            foreach (var produccion in produccionRequest.Values)
            {
                if (produccionInsumo.ContainsKey(produccion.IdProduccionExterno))
                    produccion.Insumos.AddRange(produccionInsumo[produccion.IdProduccionExterno]);

                if (produccionProducto.ContainsKey(produccion.IdProduccionExterno))
                    produccion.Productos.AddRange(produccionProducto[produccion.IdProduccionExterno]);
            }

            TranslateCells(cellsToTranslate);

            request.Ingresos.Clear();
            request.Ingresos.AddRange(produccionRequest.Values);

            return request;
        }

        public virtual ControlCalidadRequest CreateRequestControlDeCalidad(Dictionary<string, string> sheetNames)
        {
            var cellsToTranslate = new List<IXLCell>();
            var controles = new Dictionary<string, ControlCalidadItemRequest>();
            var criterios = new Dictionary<string, List<CriterioSeleccionItemRequestWithKey>>();
            var dsReferencia = !string.IsNullOrEmpty(_referencia) ? _referencia : "Asociar o Aprobar controles de calidad";

            var request = new ControlCalidadRequest()
            {
                Empresa = _empresa,
                DsReferencia = dsReferencia.Truncate(200),
                Archivo = "INT050",
            };

            ValidateTemplate(CInterfazExterna.ControlDeCalidad);

            var rowToReadFrom = 3;
            var rowIndex = 1;
            var cantColumns = 0;
            var colsName = new Dictionary<string, string>();

            var workSheet = this._workbook.Worksheets.First();
            var sheetName = sheetNames.GetValueOrDefault(GridExcelSheetNames.SheetCriterios, "Criterios");
            var criteriosSheet = this._workbook.Worksheets.FirstOrDefault(p => p.Name == sheetName);

            ValidateMaxItems(_configuration.Value.ControlDeCalidad, workSheet, criteriosSheet);

            #region Controles

            foreach (var row in workSheet.RowsUsed())
            {
                SetColumnsName(row, rowIndex, ref colsName, ref cantColumns);

                if (rowIndex >= rowToReadFrom && !IsRowEmpty(row, cantColumns))
                {
                    var output = AnalyzeRow(GetCellsUsed(row, cantColumns), colsName, cellsToTranslate, typeof(ControlCalidadItemRequest));
                    var control = (ControlCalidadItemRequest)output;

                    var keyControl = $"{control.CodigoControlCalidad}${control.Estado}";

                    if (!controles.TryAdd(keyControl, control))
                    {
                        var worksheetCell = workSheet.Cell(3, 1);
                        worksheetCell.Style.Font.FontColor = XLColor.Red;
                        AddComment(worksheetCell, "INT050_Sec0_Error_ImportExcelErrorProduccionRepetido", cellsToTranslate);
                    }
                }

                rowIndex++;
            }

            #endregion

            #region Criterios

            rowIndex = 1;

            foreach (var row in criteriosSheet.RowsUsed())
            {
                SetColumnsName(row, rowIndex, ref colsName, ref cantColumns);

                if (rowIndex >= rowToReadFrom && !IsRowEmpty(row, cantColumns))
                {
                    var output = AnalyzeRow(GetCellsUsed(row, cantColumns), colsName, cellsToTranslate, typeof(CriterioSeleccionItemRequestWithKey));
                    var criterio = (CriterioSeleccionItemRequestWithKey)output;

                    var keyControl = $"{criterio.CodigoControlCalidad}${criterio.Estado}";

                    if (!criterios.ContainsKey(keyControl))
                    {
                        List<CriterioSeleccionItemRequestWithKey> newList = [criterio];
                        criterios.Add(keyControl, newList);
                    }
                    else
                        criterios[keyControl].Add(criterio);
                }

                rowIndex++;
            }

            #endregion

            foreach (var control in controles.Values)
            {
                var keyControl = $"{control.CodigoControlCalidad}${control.Estado}";

                control.CriteriosDeSeleccion = new List<CriterioSeleccionItemRequest>();

                if (criterios.ContainsKey(keyControl))
                    control.CriteriosDeSeleccion.AddRange(criterios[keyControl]);
            }

            TranslateCells(cellsToTranslate);

            request.ControlesDeCalidad.Clear();
            request.ControlesDeCalidad.AddRange(controles.Values);

            return request;
        }

        public virtual FacturasRequest CreateRequestFacturas(Dictionary<string, string> sheetNames)
        {
            var cellsToTranslate = new List<IXLCell>();
            var colFacturas = new Dictionary<string, FacturaRequest>();
            var colDetalles = new Dictionary<string, FacturaDetalleWithKeysRequest>();
            var dsReferencia = !string.IsNullOrEmpty(_referencia) ? _referencia : "Creación de facturas";

            var request = new FacturasRequest()
            {
                Empresa = _empresa,
                DsReferencia = dsReferencia.Truncate(200),
                Archivo = "INT050",
            };

            ValidateTemplate(CInterfazExterna.Facturas);

            var rowToReadFrom = 3;
            var rowIndex = 1;
            var cantColumns = 0;
            var colsName = new Dictionary<string, string>();

            var workSheet = this._workbook.Worksheets.First();
            var sheetName = sheetNames.GetValueOrDefault(GridExcelSheetNames.SheetDetailName, "Detalles");
            var detallesSheet = this._workbook.Worksheets.FirstOrDefault(p => p.Name == sheetName);

            ValidateMaxItems(_configuration.Value.Facturas, workSheet, detallesSheet);

            #region Facturas

            foreach (var row in workSheet.RowsUsed())
            {
                SetColumnsName(row, rowIndex, ref colsName, ref cantColumns);

                if (rowIndex >= rowToReadFrom && !IsRowEmpty(row, cantColumns))
                {
                    var output = AnalyzeRow(GetCellsUsed(row, cantColumns), colsName, cellsToTranslate, typeof(FacturaRequest));
                    var factura = (FacturaRequest)output;

                    var key = $"{factura.Factura}.{factura.Serie}.{factura.CodigoAgente}";

                    if (!colFacturas.ContainsKey(key))
                        colFacturas.Add(key, factura);
                    else
                    {
                        var facturaSheet = workSheet.Cell(3, 1);
                        facturaSheet.Style.Font.FontColor = XLColor.Red;
                        AddComment(facturaSheet, "INT050_Sec0_Error_ImportExcelErrorFacturaRepetida", cellsToTranslate);
                    }
                }

                rowIndex++;
            }

            #endregion

            #region Detalles

            rowIndex = 1;

            foreach (var row in detallesSheet.RowsUsed())
            {
                SetColumnsName(row, rowIndex, ref colsName, ref cantColumns);

                if (rowIndex >= rowToReadFrom && !IsRowEmpty(row, cantColumns))
                {
                    var output = AnalyzeRow(GetCellsUsed(row, cantColumns), colsName, cellsToTranslate, typeof(FacturaDetalleWithKeysRequest));
                    var detalleFactura = (FacturaDetalleWithKeysRequest)output;

                    var keyFactura = $"{detalleFactura.Factura}.{detalleFactura.Serie}.{detalleFactura.CodigoAgente}";
                    var keyFacturaDetalle = $"{detalleFactura.Factura}.{detalleFactura.Serie}.{detalleFactura.CodigoAgente}.{detalleFactura.Producto}.{detalleFactura.Identificador}";

                    if (detalleFactura != null)
                    {
                        if (!colDetalles.ContainsKey(keyFacturaDetalle))
                        {
                            if (colFacturas.ContainsKey(keyFactura))
                            {
                                colDetalles.Add(keyFacturaDetalle, detalleFactura);
                                colFacturas[keyFactura].Detalles.Add(detalleFactura);
                            }
                            else
                            {
                                var detallesSheetCell = detallesSheet.Cell(3, 1);
                                detallesSheetCell.Style.Font.FontColor = XLColor.Red;
                                AddComment(detallesSheetCell, "INT050_Sec0_Error_ImportExcelErrorFacturaDetalles", cellsToTranslate);
                            }
                        }
                        else
                        {
                            var detallesSheetCell = detallesSheet.Cell(3, 1);
                            detallesSheetCell.Style.Font.FontColor = XLColor.Red;
                            AddComment(detallesSheetCell, "INT050_Sec0_Error_ImportExcelErrorFacturaDetalleRepetido", cellsToTranslate);
                        }
                    }
                }

                rowIndex++;
            }

            #endregion

            TranslateCells(cellsToTranslate);

            request.Facturas.Clear();
            request.Facturas.AddRange(colFacturas.Values);

            return request;
        }

        public virtual PickingProductosRequest CreateRequestPickingProducto()
        {
            return CreateApiEntradaRequest<PickingProductosRequest, PickingProductoRequest>(CInterfazExterna.PickingProducto, _configuration.Value.UbicacionesPicking, "Alta de ubicaciones de picking desde el panel de ejecución");
        }

        #region Metodos Auxiliares

        public virtual T1 CreateApiEntradaRequest<T1, T2>(int interfazExterna, int maxItemCount, string referencia)
            where T1 : IApiEntradaRequest
            where T2 : IApiEntradaItemRequest
        {
            var cellsToTranslate = new List<IXLCell>();
            var request = (IApiEntradaRequest)Activator.CreateInstance(typeof(T1));

            request.Empresa = _empresa;
            request.DsReferencia = (_referencia ?? referencia).Truncate(200);
            request.Archivo = "INT050";

            ValidateTemplate(interfazExterna);

            var rowToReadFrom = 3;
            var rowIndex = 1;
            var cantColumns = 0;
            var colsName = new Dictionary<string, string>();

            var workSheet = this._workbook.Worksheets.First();

            ValidateMaxItems(maxItemCount, workSheet);

            foreach (var row in workSheet.RowsUsed())
            {
                SetColumnsName(row, rowIndex, ref colsName, ref cantColumns);

                if (rowIndex >= rowToReadFrom && !IsRowEmpty(row, cantColumns))
                {
                    var output = AnalyzeRow(GetCellsUsed(row, cantColumns), colsName, cellsToTranslate, typeof(T2));
                    var item = (IApiEntradaItemRequest)output;

                    request.Add(item);
                }

                rowIndex++;
            }

            TranslateCells(cellsToTranslate);

            return (T1)request;
        }

        public virtual Dictionary<string, string> GetTranslatedValues()
        {
            var translations = this._translator.Translate(new List<string> {
                GridExcelSheetNames.SheetDataName,
                GridExcelSheetNames.SheetDetailName,
                GridExcelSheetNames.SheetDuplicateName,
                GridExcelSheetNames.SheetAtributosName,
                GridExcelSheetNames.SheetAtributosDetailName,
                GridExcelSheetNames.SheetBarrasName,
                GridExcelSheetNames.SheetLpnName,
                GridExcelSheetNames.SheetDetalleLpnName,
                GridExcelSheetNames.SheetProduccion,
                GridExcelSheetNames.SheetProduccionInsumo,
                GridExcelSheetNames.SheetProduccionProducto,
                GridExcelSheetNames.SheetControles,
                GridExcelSheetNames.SheetCriterios,
            });

            foreach (var key in translations.Keys)
            {
                if (key == translations[key])
                    translations[key] = GridExcelImportTemplate.GetValueDefault(key);
            }

            return translations;
        }

        /// <summary>
        /// Crea un objeto del tipo especificado a partir de una fila de celdas, asignando valores a sus propiedades.
        /// </summary>
        /// <param name="rows">Colección de celdas de la fila.</param>
        /// <param name="colsName">Mapeo de columnas a nombres de propiedades.</param>
        /// <param name="cellsToTranslate">Celdas donde se agregarán comentarios en caso de errores.</param>
        /// <param name="type">Tipo del objeto a crear.</param>
        /// <returns>Objeto del tipo especificado con las propiedades asignadas.</returns>
        public virtual object AnalyzeRow(IEnumerable<IXLCell> rows, Dictionary<string, string> colsName, List<IXLCell> cellsToTranslate, Type type)
        {
            object output = Activator.CreateInstance(type, new object[] { });

            foreach (var cell in rows)
            {
                try
                {
                    var columnName = colsName.GetValueOrDefault(cell.Address.ColumnLetter);
                    var piObj = type.GetProperty(columnName);
                    piObj.SetValue(output, ConverToPropertyType(cell.GetFormattedString(), piObj.PropertyType));
                }
                catch (Exception ex)
                {
                    var worksheetCell = cell;
                    worksheetCell.Style.Font.FontColor = XLColor.Red;
                    AddComment(worksheetCell, ex.Message, cellsToTranslate);
                }
            }

            return output;
        }

        public virtual void TranslateCells(List<IXLCell> cellsPendingTranslation)
        {
            if (cellsPendingTranslation.Any())
            {
                var valuesToTranslate = cellsPendingTranslation.Select(d => d.GetComment().ToString()).Where(d => !string.IsNullOrEmpty(d)).Distinct().ToList();
                var translatedValues = this._translator.Translate(valuesToTranslate);
                var error = string.Empty;

                foreach (var cellPending in cellsPendingTranslation.Distinct())
                {
                    var cellValue = cellPending.GetComment().ToString();

                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        cellPending.GetComment().Delete();
                        cellPending.GetComment().AddText(translatedValues[cellValue]);

                        error = ($"Item {cellPending.Address} - {translatedValues[cellValue]}").Replace(':', ' ');
                    }
                }

                if (_descargaErrores)
                    throw new Exception("INT050_Sec0_Error_ImportExcel");
                else
                    throw new Exception(error);
            }
        }

        public virtual object ConverToPropertyType(string value, Type type)
        {
            try
            {
                string[] formats = { "yyyy-MM-dd", "yyyy/MM/dd", "MM/dd/yyyy", "MM-dd-yyyy" };

                if (type == typeof(string))
                {
                    return value;
                }
                else if (type == typeof(int))
                {
                    if (!int.TryParse(value, out int parsedValue))
                        throw new Exception("General_Sec0_Error_Error14");

                    return parsedValue;
                }
                else if (type == typeof(int?) && !string.IsNullOrEmpty(value))
                {
                    if (!int.TryParse(value, out int parsedValue))
                        throw new Exception("General_Sec0_Error_Error14");

                    return parsedValue;
                }
                else if (type == typeof(short))
                {
                    if (!short.TryParse(value, out short parsedValue))
                        throw new Exception("General_Sec0_Error_Error14");

                    return parsedValue;
                }
                else if (type == typeof(short?) && !string.IsNullOrEmpty(value))
                {
                    if (!short.TryParse(value, out short parsedValue))
                        throw new Exception("General_Sec0_Error_Error14");

                    return parsedValue;
                }
                else if (type == typeof(bool))
                {
                    if (!bool.TryParse(value, out bool parsedValue))
                        throw new Exception("General_Sec0_Error_Error14");

                    return parsedValue;
                }
                else if (type == typeof(bool?) && !string.IsNullOrEmpty(value))
                {
                    if (!bool.TryParse(value, out bool parsedValue))
                        throw new Exception("General_Sec0_Error_Error14");

                    return parsedValue;
                }
                else if (type == typeof(long))
                {
                    if (!long.TryParse(value, out long parsedValue))
                        throw new Exception("General_Sec0_Error_Error14");

                    return parsedValue;
                }
                else if (type == typeof(long?) && !string.IsNullOrEmpty(value))
                {
                    if (!long.TryParse(value, out long parsedValue))
                        throw new Exception("General_Sec0_Error_Error14");

                    return parsedValue;
                }
                else if (type == typeof(double))
                {
                    if (!double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedValue))
                        throw new Exception("General_Sec0_Error_Error14");

                    return parsedValue;
                }
                else if (type == typeof(double?) && !string.IsNullOrEmpty(value))
                {
                    if (!double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedValue))
                        throw new Exception("General_Sec0_Error_Error14");

                    return parsedValue;
                }
                else if (type == typeof(decimal))
                {
                    if (!decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedValue))
                        throw new Exception("General_Sec0_Error_Error14");

                    return parsedValue;
                }
                else if (type == typeof(decimal?) && !string.IsNullOrEmpty(value))
                {
                    if (!decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedValue))
                        throw new Exception("General_Sec0_Error_Error14");

                    return parsedValue;
                }
                else if (type == typeof(DateTime))
                {
                    if (!DateTimeExtension.IsValid_ImportExcel(value, formats, DateTimeStyles.None, _proveedor))
                        throw new Exception("General_Sec0_Error_Error14");

                    return DateTime.ParseExact(value, formats, _proveedor, DateTimeStyles.None);
                }
                else if (type == typeof(DateTime?) && !string.IsNullOrEmpty(value))
                {
                    if (!DateTimeExtension.IsValid_ImportExcel(value, formats, DateTimeStyles.None, _proveedor))
                        throw new Exception("General_Sec0_Error_Error14");

                    return DateTime.ParseExact(value, formats, _proveedor, DateTimeStyles.None);
                }

                return null;
            }
            catch (Exception ex)
            {
                logger.Error($"Error ConvertoPropertyType: {ex}");
                throw ex;
            }
        }

        public virtual void SetErrors(List<ValidationsError> errors)
        {
            if (errors.Count > 0)
            {
                var workSheet = this._workbook.Worksheets.First();
                var dictErrors = new Dictionary<int, List<string>>();

                foreach (var error in errors)
                {
                    if (!dictErrors.ContainsKey(error.ItemId))
                        dictErrors[error.ItemId] = new List<string>();

                    dictErrors[error.ItemId].AddRange(error.Messages.Select(m => m.TrimEnd('.')));
                }

                int rowIndex = 1;
                int rowToReadFrom = 3;
                var colsName = new Dictionary<int, string>();

                foreach (var row in workSheet.RowsUsed())
                {
                    if (rowIndex >= rowToReadFrom)
                    {
                        var itemId = rowIndex - 2;
                        if (dictErrors.ContainsKey(itemId))
                        {
                            var worksheetCell = row.CellsUsed().FirstOrDefault();
                            worksheetCell.Style.Font.FontColor = XLColor.Red;
                            AddComment(worksheetCell, string.Join(". ", dictErrors[itemId]));
                        }
                    }

                    rowIndex++;
                }
            }
        }

        public virtual void CleanErrors()
        {
            var workSheets = this._workbook.Worksheets.ToList();
            if (workSheets != null)
            {
                foreach (var workSheet in workSheets)
                {
                    workSheet.DeleteComments();
                    workSheet.Cells().Style.Font.SetFontColor(XLColor.Black);
                }
            }
        }

        public virtual byte[] GetAsByteArray()
        {
            using (var workbook = new XLWorkbook())
            using (var ms = new MemoryStream())
            {
                //Genero el resultado a partir de una copia del workbook para evitar manejar
                //estilos no soportados por ClosedXml e inyectados por editores de hojas de calculo particulares (ej. WPS Office) 
                foreach (var ws in _workbook.Worksheets)
                {
                    workbook.AddWorksheet(ws);
                }

                workbook.SaveAs(ms, new SaveOptions { EvaluateFormulasBeforeSaving = false, GenerateCalculationChain = false, ValidatePackage = true });
                var data = ms.ToArray();
                return data;
            }
        }

        public void Dispose()
        {
            this._workbook.Dispose();
            this._stream.Dispose();
        }

        public virtual void ValidateTemplate(int codigoInterfazExterna)
        {
            //Hoja de referencias
            var sheetReferencias = _workbook.Worksheets.Last();
            var cellsUsed = sheetReferencias.RowsUsed().CellsUsed();
            var cdInterfazExternaReferencia = string.Empty;
            var index = 1;

            foreach (var cell in cellsUsed)
            {
                if (index == 3)
                {
                    cdInterfazExternaReferencia = cell.Value.ToString();
                    break;
                }

                index++;
            }

            if (codigoInterfazExterna.ToString() != cdInterfazExternaReferencia)
                throw new MissingParameterException("INT050_Sec0_Error_TemplateIncorrecto");
        }

        public virtual void ValidateMaxItems(int maxItems, params object[] sheets)
        {
            var cantItems = 0;

            foreach (var sheet in sheets)
            {
                cantItems += (((IXLWorksheet)sheet).RowsUsed().Count() - 2);
            }

            if (cantItems > maxItems)
                throw new MissingParameterException("INT050_Sec0_Error_CantidadSuperaMaximo", [maxItems.ToString()]);
        }

        #endregion
    }
}
