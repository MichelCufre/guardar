using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Expedicion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Expedicion;
using WIS.Domain.Expedicion.EXP110EmpaquetadoPicking;
using WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto;
using WIS.Domain.General;
using WIS.Domain.Impresiones;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Tracking.Models;
using WIS.Domain.Validation;
using WIS.Exceptions;
using WIS.Extension;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.EXP
{
    public class EXP110SeleccionProducto : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITaskQueueService _taskQueue;
        protected readonly ITrackingService _trackingService;
        protected readonly IBarcodeService _barcodeService;

        protected readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected readonly EmpaquetadoPickingLogic _logic;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public EXP110SeleccionProducto(
            ISecurityService security,
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IFormValidationService formValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ITaskQueueService taskQueue,
            IPrintingService printingService,
            ITrackingService trackingService,
            IBarcodeService barcodeService)
        {
            this.GridKeys = new List<string>
            {
                "CD_PRODUTO",
                "CD_EMPRESA",
                "NU_IDENTIFICADOR",
                "CD_FAIXA",
                "NU_PREPARACION",
                "NU_PEDIDO",
                "CD_CLIENTE",
                "CD_ENDERECO",
                "NU_SEQ_PREPARACION"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_PEDIDO", SortDirection.Ascending),
                new SortCommand("NU_CONTENEDOR", SortDirection.Ascending)
            };

            _security = security;
            _uowFactory = uowFactory;
            _identity = identity;
            _formValidationService = formValidationService;
            _gridService = gridService;
            _excelService = excelService;
            _filterInterpreter = filterInterpreter;
            _taskQueue = taskQueue;
            _trackingService = trackingService;
            _barcodeService = barcodeService;

            _logic = new EmpaquetadoPickingLogic(printingService, trackingService, barcodeService, identity, Logger);
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            var nuContenedorOrigen = context.GetParameter("AUX_CONT_ORIGEN_NU_CONTENEDOR");
            var nuPreparacionOrigen = context.GetParameter("AUX_CONT_ORIGEN_NU_PREPARACION");
            var confInicial = context.GetParameter("CONF_INICIAL");
            var dataProducto = context.GetParameter("AUX_PROD_LEIDO");

            if (string.IsNullOrEmpty(confInicial) ||
                string.IsNullOrEmpty(nuContenedorOrigen) ||
                string.IsNullOrEmpty(nuPreparacionOrigen) ||
                string.IsNullOrEmpty(dataProducto))
            {
                return form;
            }

            var producto = JsonConvert.DeserializeObject<Producto>(dataProducto);

            form.GetField("codigoProducto").Value = producto.Codigo;
            form.GetField("descripcionProducto").Value = producto.Descripcion;

            using var uow = this._uowFactory.GetUnitOfWork();

            var contenedorDestino = !string.IsNullOrEmpty(context.GetParameter("CONT_DESTINO_DATA")) ?
                JsonConvert.DeserializeObject<ContenedorDestinoData>(context.GetParameter("CONT_DESTINO_DATA")) : null;

            var filtrarComparteContenedorEntrega = (contenedorDestino != null ? true : false);

            if (!uow.EmpaquetadoPickingRepository.TieneMasDeUnPedidoProductoContenedor(nuContenedorOrigen.ToNumber<int>(), nuPreparacionOrigen.ToNumber<int>(), producto.Codigo, filtrarComparteContenedorEntrega, contenedorDestino?.CompartContenedorEntrega) &&
              !uow.EmpaquetadoPickingRepository.TieneMasDeUnPedidoProductoLote(nuContenedorOrigen.ToNumber<int>(), nuPreparacionOrigen.ToNumber<int>(), producto.Codigo, filtrarComparteContenedorEntrega, contenedorDestino?.CompartContenedorEntrega, out decimal qtProducto, out string identificador))
            {
                PedidoProductoContenedor auxPedidoProductoContenedor = null;


                var contenedorOrigen = !string.IsNullOrEmpty(context.GetParameter("CONT_ORIGEN_DATA")) ?
                    JsonConvert.DeserializeObject<DatosClientePedidoOriginal>(context.GetParameter("CONT_ORIGEN_DATA")) : null;

                if (contenedorDestino == null && contenedorOrigen == null)
                    auxPedidoProductoContenedor = uow.EmpaquetadoPickingRepository.GetOnlyPedidoProductoContenedor(nuContenedorOrigen.ToNumber<int>(), nuPreparacionOrigen.ToNumber<int>(), producto.Codigo);

                var nuPedido = contenedorDestino == null ? contenedorOrigen == null ? auxPedidoProductoContenedor.NumeroPedido : contenedorOrigen.NumeroPedido : contenedorDestino.NumeroPedido;
                var cdCliente = contenedorDestino == null ? contenedorOrigen == null ? auxPedidoProductoContenedor.CodigoCliente : contenedorOrigen.CodigoCliente : contenedorDestino.CodigoCliente;
                var empresa = contenedorDestino == null ? contenedorOrigen == null ? auxPedidoProductoContenedor.Empresa : contenedorOrigen.Empresa : contenedorDestino.CodigoEmpresa;

                form.GetField("anexo4").Value = uow.EmpaquetadoPickingRepository.GetDsAnexo4DetallePedido(nuPedido, cdCliente, empresa, producto.Codigo, 1, identificador);
                form.GetField("cantidadProducto").Value = qtProducto.ToString(_identity.GetFormatProvider());

                context.AddOrUpdateParameter("AUX_TIENE_SOLO_UN_REGISTRO", "S");
            }
            else
            {
                var pedidoProductoLote = context.GetParameter("AUX_ROW_SELECTED_PEDPRODLOTE");
                var rowSelectedPedProdCont = context.GetParameter("AUX_ROW_SELECTED_PEDPRODCONT");
                var onlyOneRow = context.GetParameter("AUX_TIENE_UNA_ROW");

                if (!string.IsNullOrEmpty(pedidoProductoLote))
                {
                    var keys = pedidoProductoLote.Split('$');

                    var nuPedido = keys[5];
                    var cdProducto = keys[0];
                    var cdCliente = keys[6];
                    var nuIdentificador = keys[2];
                    var empresa = keys[1].ToNumber<int>();

                    form.GetField("anexo4").Value = uow.EmpaquetadoPickingRepository.GetDsAnexo4DetallePedido(nuPedido, cdCliente, empresa, producto.Codigo, 1, nuIdentificador);
                    form.GetField("cantidadProducto").Value = keys[7];
                }
                else if (!string.IsNullOrEmpty(rowSelectedPedProdCont) && !string.IsNullOrEmpty(onlyOneRow))
                {
                    var onlyRow = context.GetParameter("AUX_TIENE_UNA_ROW").Split('$');
                    var keys = rowSelectedPedProdCont.Split('$');

                    var empresa = keys[1].ToNumber<int>();
                    var nuPedido = keys[3];
                    var cdCliente = keys[4];
                    var auxIdentificador = onlyRow[0];

                    form.GetField("anexo4").Value = uow.EmpaquetadoPickingRepository.GetDsAnexo4DetallePedido(nuPedido, cdCliente, empresa, producto.Codigo, 1, auxIdentificador);
                    form.GetField("cantidadProducto").Value = onlyRow[1];
                }
                else
                {
                    form.GetField("anexo4").Value = string.Empty;
                    form.GetField("cantidadProducto").Value = string.Empty;
                }
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            switch (context.ButtonId)
            {
                case "btnSubmitConfirmar":
                    return SubmitConfirmarBtn(form, context);
                case "BtnEmpaquetarTodo":
                    return SubmitEmpaquetarTodo(form, context);
                case "BtnEmpaquetarProducto":
                    return SubmitEmpaquetarProducto(form, context);
                default: return form;
            }
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new EXP110SeleccionProductoValidationModule(uow, _identity.GetFormatProvider(), this._identity.UserId, this._identity.Predio), form, context);
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            var queryData = GetPickingProductoQueryData(context);

            if (queryData == null)
                return grid;

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PickingProductoQuery(queryData);
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            var queryData = GetPickingProductoQueryData(context);

            if (queryData == null)
                return Array.Empty<byte>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PickingProductoQuery(queryData);
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            var queryData = GetPickingProductoQueryData(context);

            if (queryData == null)
            {
                return new GridStats
                {
                    Count = 0
                };
            }

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PickingProductoQuery(queryData);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        #region Metodos Auxiliares

        public virtual Form SubmitConfirmarBtn(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("Confirmar selección producto");
            uow.BeginTransaction();

            Contenedor contenedorDestino = null;

            int nuContenedor = context.GetParameter("AUX_CONT_ORIGEN_NU_CONTENEDOR").ToNumber<int>();
            int nuPreparacion = context.GetParameter("AUX_CONT_ORIGEN_NU_PREPARACION").ToNumber<int>();
            string prodLeido = context.GetParameter("AUX_PROD_LEIDO");
            string contDestinoData = context.GetParameter("CONT_DESTINO_DATA");
            string contenedorDestinoCreado = context.GetParameter("AUX_CONTENEDOR_DESTINO_JSON");
            bool creoContenedor = (string.IsNullOrEmpty(contDestinoData) && string.IsNullOrEmpty(contenedorDestinoCreado)) ? true : false;

            string confInicial = context.GetParameter("CONF_INICIAL");
            ConfiguracionInicial configuracionInicial = JsonConvert.DeserializeObject<ConfiguracionInicial>(confInicial);

            string dataClientePedido = context.GetParameter("CONT_ORIGEN_DATA");
            string rowSelectedPedProdCont = context.GetParameter("AUX_ROW_SELECTED_PEDPRODCONT");

            DatosClientePedidoOriginal data = string.IsNullOrEmpty(dataClientePedido) ? !string.IsNullOrEmpty(rowSelectedPedProdCont)
                                             ? _logic.GetDatosByPedidoSelected(uow, rowSelectedPedProdCont)
                                             : uow.EmpaquetadoPickingRepository.GetDatosClientePedidoOriginal(nuContenedor, nuPreparacion)
                                             : JsonConvert.DeserializeObject<DatosClientePedidoOriginal>(dataClientePedido);

            if (data == null)
                throw new ValidationFailedException("EXP110_form1_Error_DataFaltante");

            if (creoContenedor)
                contenedorDestino = _logic.CreoContenedorEmpaquetado(uow, configuracionInicial, data, prodLeido, nuPreparacion, nuContenedor, _identity.UserId);
            else
            {
                contenedorDestino = !string.IsNullOrEmpty(contenedorDestinoCreado) ? JsonConvert.DeserializeObject<Contenedor>(contenedorDestinoCreado) : _logic.GetContenedorEmpaquetado(uow, contDestinoData);
            }


            if (contenedorDestino != null && !creoContenedor && !_logic.ContenedorDestinoValido(uow, contenedorDestino.Numero, contenedorDestino.NumeroPreparacion, contenedorDestino.Ubicacion, out string mensajeError))
            {
                context.AddOrUpdateParameter("CONTENEDOR_INVALIDO", "S");
                throw new ValidationFailedException(mensajeError);
            }

            int cantidadLineas = uow.EmpaquetadoPickingRepository.GetCantidadLineasContenedor(nuContenedor, nuPreparacion);
            bool flEmpaquetaProducto = uow.PedidoRepository.GetConfiguracionExpedicion(data.TipoExpedicion).FlEmpaquetaSoloUnProducto;

            if (cantidadLineas > 1 && !flEmpaquetaProducto)
            {
                context.AddOrUpdateParameter("CONFIRMATION_MSG_EMPAQUETAR_TODO", "S");
            }
            else
            {
                context.AddOrUpdateParameter("SUMMIT_EMPAQUETAR_PROD", "S");
            }

            uow.SaveChanges();
            uow.Commit();

            if (contenedorDestino != null)
            {
                var contDestinoDataParam = uow.EmpaquetadoPickingRepository.GetContenedorDestinoData(contenedorDestino.Numero, contenedorDestino.NumeroPreparacion);

                context.AddOrUpdateParameter("CONT_DESTINO_DATA", JsonConvert.SerializeObject(contDestinoDataParam));
            }

            context.AddOrUpdateParameter("AUX_CONTENEDOR_NUEVO", contenedorDestino.Numero.ToString());
            context.AddOrUpdateParameter("AUX_ID_EXTERNO_CONTENEDOR_NUEVO", contenedorDestino.IdExterno);
            context.AddOrUpdateParameter("AUX_PREPARACION_NUEVO", contenedorDestino.NumeroPreparacion.ToString());
            context.AddOrUpdateParameter("AUX_CONTENEDOR_DESTINO", JsonConvert.SerializeObject(contenedorDestino));

            return form;
        }

        public virtual Form SubmitEmpaquetarTodo(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            (bool Notificar, int? Camion) interfazFacturacion = (false, null);

            uow.CreateTransactionNumber("EXP110SeleccionProducto SubmitEmpaquetarTodo");
            uow.BeginTransaction();

            var datosTrackingOrigen = new EgresoContenedorTracking();
            var datosTrackingDestino = new EgresoContenedorTracking();

            var pedProdContSelect = context.GetParameter("AUX_ROW_SELECTED_PEDPRODCONT");
            var contenedorDestinoJson = context.GetParameter("AUX_CONTENEDOR_DESTINO");
            var nuContenedorOrigen = context.GetParameter("AUX_CONT_ORIGEN_NU_CONTENEDOR").ToNumber<int>();
            var nuPreparacionOrigen = context.GetParameter("AUX_CONT_ORIGEN_NU_PREPARACION").ToNumber<int>();

            var contenedorDestino = JsonConvert.DeserializeObject<Contenedor>(contenedorDestinoJson);
            var contenedorOrigen = uow.ContenedorRepository.GetContenedor(nuPreparacionOrigen, nuContenedorOrigen);

            var dataClientePedido = context.GetParameter("CONT_ORIGEN_DATA");
            var rowSelectedPedProdCont = context.GetParameter("AUX_ROW_SELECTED_PEDPRODCONT");

            DatosClientePedidoOriginal data = string.IsNullOrEmpty(dataClientePedido) ? !string.IsNullOrEmpty(rowSelectedPedProdCont) ? _logic.GetDatosByPedidoSelected(uow, rowSelectedPedProdCont)
                : uow.EmpaquetadoPickingRepository.GetDatosClientePedidoOriginal(nuContenedorOrigen, nuPreparacionOrigen) : JsonConvert.DeserializeObject<DatosClientePedidoOriginal>(dataClientePedido);

            var cdCliente = pedProdContSelect != null ? pedProdContSelect.Split('$')[4] : data.CodigoCliente;
            var nuPedido = pedProdContSelect != null ? pedProdContSelect.Split('$')[3] : data.NumeroPedido;

            var camionContenedorOrigen = uow.ContenedorRepository.GetCamionAsignado(nuContenedorOrigen, nuPreparacionOrigen);
            var camionContenedorDestino = uow.ContenedorRepository.GetCamionAsignado(contenedorDestino.Numero, contenedorDestino.NumeroPreparacion);

            datosTrackingOrigen.Egreso = camionContenedorOrigen;
            datosTrackingOrigen.Contenedor = contenedorOrigen;

            _logic.EmpaquetarTodosProductosContenedor(uow, nuContenedorOrigen, contenedorDestino.Numero, nuPreparacionOrigen, contenedorDestino.NumeroPreparacion,
                                                      cdCliente, data.Empresa, nuPedido, contenedorOrigen.Ubicacion, contenedorDestino.Ubicacion, out bool isContenedorOrigenVacio);

            context.AddOrUpdateParameter("AUX_PESO_NUEVO", GetPeso(uow, contenedorDestino.Numero, contenedorDestino.NumeroPreparacion));

            datosTrackingOrigen.Baja = isContenedorOrigenVacio;

            datosTrackingDestino.Egreso = camionContenedorDestino;
            datosTrackingDestino.Contenedor = contenedorDestino;
            datosTrackingDestino.Baja = false;

            uow.SaveChanges();
            uow.Commit();

            if (contenedorDestino != null)
            {
                var contDestinoDataParam = uow.EmpaquetadoPickingRepository.GetContenedorDestinoData(contenedorDestino.Numero, contenedorDestino.NumeroPreparacion);

                context.AddOrUpdateParameter("CONT_DESTINO_DATA", JsonConvert.SerializeObject(contDestinoDataParam));
            }

            _logic.GenerarEgresoYFacturacion(uow, form, context, contenedorDestino, contenedorOrigen.Numero, nuPreparacionOrigen, cdCliente, 
                                            data.Empresa, nuPedido, isContenedorOrigenVacio, "SubmitEmpaquetarTodo", 
                                            camionContenedorOrigen, camionContenedorDestino, out interfazFacturacion);
            
            var auxContenedorImpresion = context.GetParameter("AUX_CONT_IMPRESION").ToNumber<int?>();
            var confInicial = context.GetParameter("CONF_INICIAL");

            if (auxContenedorImpresion == null || auxContenedorImpresion != contenedorDestino.Numero)
            {
                _logic.ImprimirEtiqueta(uow, _identity.UserId, confInicial, auxContenedorImpresion, contenedorDestino, cdCliente, data.Empresa, nuPedido, out string infoCambio);

                if (!string.IsNullOrEmpty(infoCambio))
                {
                    EtiquetasEmpaquetadoPickingQuery dbQuery = new EtiquetasEmpaquetadoPickingQuery();
                    uow.HandleQuery(dbQuery);
                    List<EtiquetaEstilo> listaEstilos = dbQuery.GetEtiquetasEstilo();
                    EtiquetaEstilo estiloSeleccionado = listaEstilos.FirstOrDefault(f => f.Id == infoCambio.Split('#')[1]);

                    List<Impresora> listaImpresoras;
                    if (this._identity.Predio.Equals(GeneralDb.PredioSinDefinir))
                        listaImpresoras = uow.ImpresoraRepository.GetListaImpresorasPredio(form.GetField("predio").Value);
                    else
                    {
                        listaImpresoras = uow.ImpresoraRepository.GetListaImpresorasPredio(this._identity.Predio);
                    }

                    Impresora impresoraSeleccionada = listaImpresoras.FirstOrDefault(f => f.Id == infoCambio.Split('#')[2]);

                    context.AddInfoNotification(infoCambio.Split('#')[0], new List<string> { estiloSeleccionado.Id, estiloSeleccionado.Descripcion, impresoraSeleccionada.Id, impresoraSeleccionada.Descripcion });
                }

                context.AddSuccessNotification("EXP110SelecProd_form_Msg_SeEnvioEtiquetaImpresora", new List<string> { contenedorDestino.TipoContenedor, contenedorDestino.IdExterno });
                context.AddOrUpdateParameter("AUX_CONT_IMPRESION", contenedorDestino.Numero.ToString());
            }

            if (_logic.IsPedidoCompleto(uow, data.Empresa, cdCliente, nuPedido, out Error _))
            {
                context.AddOrUpdateParameter("AUX_PED_COMPLETO_IMP_RESUMEN", "S");
                context.AddOrUpdateParameter("AUX_PEDIDO_COMPLETO", "S");
            }

            context.AddOrUpdateParameter("SUMMIT_EMPAQUETAR_TODO", "S");
            context.AddOrUpdateParameter("SUCCESS_SUMMIT", "S");

            _trackingService.RegularizarEgresosMesaEmpaque(uow, datosTrackingOrigen, datosTrackingDestino);
            uow.SaveChanges();

            if (interfazFacturacion.Notificar && _taskQueue.IsEnabled())
                _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.Facturacion, interfazFacturacion.Camion.ToString());

            return form;
        }

        public virtual string GetPeso(IUnitOfWork uow, int nuContenedor, int nuPreparacion)
        {
            return uow.ContenedorRepository.GetPesoTotalContenedor(nuPreparacion, nuContenedor).ToString();
        }

        public virtual Form SubmitEmpaquetarProducto(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            (bool Notificar, int? Camion) interfazFacturacion = (false, null);

            var contenedorDestinoJson = context.GetParameter("AUX_CONTENEDOR_DESTINO");
            var contenedorDestino = JsonConvert.DeserializeObject<Contenedor>(contenedorDestinoJson);

            if (uow.ContenedorRepository.IsFacturado(contenedorDestino.Numero, contenedorDestino.NumeroPreparacion))
            {
                context.AddInfoNotification("EXP110SelecProd_form_Msg_ContenedorYaFacturado");
                context.AddOrUpdateParameter("AUX_FACTURO_CONTENEDOR_EMPAQUE", "S");
                return form;
            }

            uow.CreateTransactionNumber("EXP110SubmitEmpaquetarProducto");
            uow.BeginTransaction();

            var datosTrackingOrigen = new EgresoContenedorTracking();
            var datosTrackingDestino = new EgresoContenedorTracking();

            var pedProdContSelect = context.GetParameter("AUX_ROW_SELECTED_PEDPRODCONT");
            var pedProdLoteSelect = context.GetParameter("AUX_ROW_SELECTED_PEDPRODLOTE");

            var nuContenedorOrigen = context.GetParameter("AUX_CONT_ORIGEN_NU_CONTENEDOR").ToNumber<int>();
            var nuPreparacionOrigen = context.GetParameter("AUX_CONT_ORIGEN_NU_PREPARACION").ToNumber<int>();
            var contenedorOrigen = uow.ContenedorRepository.GetContenedor(nuPreparacionOrigen, nuContenedorOrigen);
            var dataClientePedido = context.GetParameter("CONT_ORIGEN_DATA");
            var rowSelectedPedProdCont = context.GetParameter("AUX_ROW_SELECTED_PEDPRODCONT");

            DatosClientePedidoOriginal data = string.IsNullOrEmpty(dataClientePedido) ? !string.IsNullOrEmpty(rowSelectedPedProdCont) ? _logic.GetDatosByPedidoSelected(uow, rowSelectedPedProdCont)
                : uow.EmpaquetadoPickingRepository.GetDatosClientePedidoOriginal(nuContenedorOrigen, nuPreparacionOrigen) : JsonConvert.DeserializeObject<DatosClientePedidoOriginal>(dataClientePedido);

            var cantidad = decimal.Parse(form.GetField("cantidadProducto").Value, _identity.GetFormatProvider());
            var cdProducto = form.GetField("codigoProducto").Value;
            var onlyOneRow = context.GetParameter("AUX_TIENE_UNA_ROW");

            var nuPedido = pedProdContSelect != null ? pedProdContSelect.Split('$')[3] : data.NumeroPedido;
            var cdCliente = pedProdContSelect != null ? pedProdContSelect.Split('$')[4] : data.CodigoCliente;
            decimal cdFaixa = 1;
            var nuIdentificador = pedProdLoteSelect != null ? pedProdLoteSelect.Split('$')[2] : onlyOneRow != null ? onlyOneRow.Split("$")[0] : "";

            if (string.IsNullOrEmpty(nuIdentificador))
            {
                var pedidoDestino = uow.PedidoRepository.GetPedidosByPreparacionAndContenedor(contenedorDestino.NumeroPreparacion, contenedorDestino.Numero).FirstOrDefault();

                var filtrarComparteContenedorEntrega = (pedidoDestino != null ? true : false);

                uow.EmpaquetadoPickingRepository.TieneMasDeUnPedidoProductoLote(nuContenedorOrigen, nuPreparacionOrigen, cdProducto, filtrarComparteContenedorEntrega, pedidoDestino?.ComparteContenedorEntrega, out decimal qtProducto, out nuIdentificador);
            }

            var camionContenedorOrigen = uow.ContenedorRepository.GetCamionAsignado(nuContenedorOrigen, nuPreparacionOrigen);
            var camionContenedorDestino = uow.ContenedorRepository.GetCamionAsignado(contenedorDestino.Numero, contenedorDestino.NumeroPreparacion);

            datosTrackingOrigen.Egreso = camionContenedorOrigen;
            datosTrackingOrigen.Contenedor = contenedorOrigen;

            var isContenedorOrigenVacio = true;
            _logic.EmpaquetarProductoContenedor(uow, nuContenedorOrigen, contenedorDestino.Numero, nuPreparacionOrigen, contenedorDestino.NumeroPreparacion,
                                                cdCliente, data.Empresa, nuPedido, contenedorOrigen.Ubicacion, contenedorDestino.Ubicacion, cdProducto,
                                                nuIdentificador, cdFaixa, cantidad, out isContenedorOrigenVacio);

            datosTrackingOrigen.Baja = isContenedorOrigenVacio;

            datosTrackingDestino.Egreso = camionContenedorDestino;
            datosTrackingDestino.Contenedor = contenedorDestino;
            datosTrackingDestino.Baja = false;

            var isEnderecoEquipoManual = uow.EquipoRepository.GetEquipoManualByEndereco(contenedorOrigen.Ubicacion, out Equipo equipoContenedor);
            if (isEnderecoEquipoManual && !uow.EmpaquetadoPickingRepository.AnyContenedoresEquipo(contenedorOrigen.Ubicacion))
            {
                uow.EquipoRepository.ModificarEquipo(equipoContenedor, _identity.UserId, true);
            }

            uow.SaveChanges();

            _logic.GenerarEgresoYFacturacion(uow, form, context, contenedorDestino, contenedorOrigen.Numero, nuPreparacionOrigen, cdCliente,
                                            data.Empresa, nuPedido, isContenedorOrigenVacio, "SubmitEmpaquetarProducto",
                                            camionContenedorOrigen, camionContenedorDestino, out interfazFacturacion);

            uow.Commit();

            if (contenedorDestino != null)
            {
                var contDestinoDataParam = uow.EmpaquetadoPickingRepository.GetContenedorDestinoData(contenedorDestino.Numero, contenedorDestino.NumeroPreparacion);

                context.AddOrUpdateParameter("CONT_DESTINO_DATA", JsonConvert.SerializeObject(contDestinoDataParam));
            }

            var auxContenedorImpresion = context.GetParameter("AUX_CONT_IMPRESION").ToNumber<int?>();
            var confInicial = context.GetParameter("CONF_INICIAL");

            if (auxContenedorImpresion == null || auxContenedorImpresion != contenedorDestino.Numero)
            {
                try
                {
                    _logic.ImprimirEtiqueta(uow, _identity.UserId, confInicial, auxContenedorImpresion, contenedorDestino, cdCliente, data.Empresa, nuPedido);

                    context.AddSuccessNotification("EXP110SelecProd_form_Msg_SeEnvioEtiquetaImpresora", new List<string> { contenedorDestino.TipoContenedor, contenedorDestino.IdExterno });
                    context.AddOrUpdateParameter("AUX_CONT_IMPRESION", contenedorDestino.Numero.ToString());
                }
                catch (Exception ex)
                {
                    context.AddErrorNotification("EXP110SelecProd_form_Msg_FalloEnvioEtiquetaImpresora", new List<string> { contenedorDestino.TipoContenedor, contenedorDestino.IdExterno });
                    context.AddOrUpdateParameter("AUX_CONT_IMPRESION", contenedorDestino.Numero.ToString());
                }
            }

            if (_logic.IsPedidoCompleto(uow, data.Empresa, cdCliente, nuPedido, out Error _))
            {
                if (uow.ParametroRepository.GetParameter("MODAL_IMPRESION_EMPAQUETADO") == "S")
                {
                    context.AddOrUpdateParameter("AUX_PEDIDO_COMPLETO", "S");
                    context.AddOrUpdateParameter("AUX_PED_COMPLETO_IMP_RESUMEN", "S");
                }
            }

            context.AddOrUpdateParameter("SUMMIT_EMPAQUETAR_PROD", "S");
            context.AddOrUpdateParameter("SUCCESS_SUMMIT", "S");

            _trackingService.RegularizarEgresosMesaEmpaque(uow, datosTrackingOrigen, datosTrackingDestino);
            uow.SaveChanges();

            if (interfazFacturacion.Notificar && _taskQueue.IsEnabled())
                _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.Facturacion, interfazFacturacion.Camion.ToString());

            return form;
        }

        public virtual PickingProductoQueryData GetPickingProductoQueryData(ComponentContext context)
        {
            var nuContenedorOrigen = context.GetParameter("AUX_CONT_ORIGEN_NU_CONTENEDOR");
            var nuPreparacionOrigen = context.GetParameter("AUX_CONT_ORIGEN_NU_PREPARACION");
            var confInicial = context.GetParameter("CONF_INICIAL");
            var dataProducto = context.GetParameter("AUX_PROD_LEIDO");
            var dataContenedorDestino = !string.IsNullOrEmpty(context.GetParameter("CONT_DESTINO_DATA")) ? JsonConvert.DeserializeObject<ContenedorDestinoData>(context.GetParameter("CONT_DESTINO_DATA")) : null;

            if (string.IsNullOrEmpty(confInicial) ||
                string.IsNullOrEmpty(nuContenedorOrigen) ||
                string.IsNullOrEmpty(nuPreparacionOrigen) ||
                string.IsNullOrEmpty(dataProducto))
            {
                return null;
            }

            var producto = JsonConvert.DeserializeObject<Producto>(dataProducto);

            return new PickingProductoQueryData
            {
                Contenedor = nuContenedorOrigen.ToNumber<int>(),
                Preparacion = nuPreparacionOrigen.ToNumber<int>(),
                Producto = producto.Codigo,
                FiltrarComparteContenedorEntrega = (dataContenedorDestino != null ? true : false),
                ComparteContenedorEntregaDestino = dataContenedorDestino?.CompartContenedorEntrega,
            };
        }

        #endregion
    }
}
