using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Application.Security;
using WIS.Application.Validation;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.Automatismo;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Interfaz;
using WIS.Domain.General;
using WIS.Domain.General.API;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Interfaces;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.INT
{
    public class INT050PanelEjecucionesInterfaz : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IWmsApiService _wmsApiService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly ITaskQueueService _taskQueueService;
        protected readonly IOptions<MaxItemsSettings> _configuration;
        protected readonly ICodigoBarrasService _codigoBarrasService;
        protected readonly IProductoService _productoService;
        protected readonly CodigoBarrasMapper _barcodeMapper;
        protected readonly ProductoMapper _productoMapper;
        protected readonly ILogger<INT050PanelEjecucionesInterfaz> _logger;
        protected readonly ISecurityService _security;

        protected List<string> GridKeys { get; }

        protected List<SortCommand> DefaultSort { get; }

        public INT050PanelEjecucionesInterfaz(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter, IWmsApiService wmsApiService, IFormValidationService formValidationService, ITaskQueueService taskQueueService, IOptions<MaxItemsSettings> configuration, ICodigoBarrasService codigoBarrasService, IProductoService productoService, ILogger<INT050PanelEjecucionesInterfaz> logger, ISecurityService security)
        {
            this.GridKeys = new List<string>
            {
                "NU_INTERFAZ_EJECUCION"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_INTERFAZ_EJECUCION",SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._wmsApiService = wmsApiService;
            this._formValidationService = formValidationService;
            this._taskQueueService = taskQueueService;
            this._configuration = configuration;
            this._codigoBarrasService = codigoBarrasService;
            this._productoService = productoService;
            this._barcodeMapper = new CodigoBarrasMapper();
            this._productoMapper = new ProductoMapper();
            this._logger = logger;
            this._security = security;
        }

        public override PageContext PageLoad(PageContext data)
        {
            if (this._security.IsUserAllowed(SecurityResources.INT050AdministrarBloqueos_Page_Access_Allow))
                data.AddParameter("PermisoBtnAdministrarBloqueo", "true");

            return data;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("btnViewData", "INT050_grid1_btn_Data", "fas fa-arrow-down"),
                new GridButton("btnViewError", "INT050_grid1_btn_Error", "fas fa-exclamation-triangle"),
            }));


            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_ACTIONS", new List<IGridItem>
            {
                new GridButton("btnViewDetail", "INT050_grid1_btn_Detail"),
                new GridButton("btnReprocess", "INT050_grid1_btn_Reprocess"),
                new GridButton("btnReprocessNotificacion", "INT050_grid1_btn_ReprocessNotificacion")
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            InterfazEjecucionQuery dbQuery;

            if (context.Parameters.Any(s => s.Id == "interfaz"))
            {
                if (!long.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "interfaz")?.Value, out long nuInterfaz))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new InterfazEjecucionQuery(nuInterfaz);
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            }
            else
            {
                dbQuery = new InterfazEjecucionQuery();
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            }

            foreach (var row in grid.Rows)
            {
                var nuInterfaz = long.Parse(row.GetCell("NU_INTERFAZ_EJECUCION").Value);

                var interfazEjecQuery = new InterfazEjecDataQuery(nuInterfaz);
                uow.HandleQuery(interfazEjecQuery);

                var deshabilitados = new List<string>();

                var idEntradaSalida = row.GetCell("ID_ENTRADA_SALIDA").Value;
                var situacion = short.Parse(row.GetCell("CD_SITUACAO").Value);
                var actionReprocesamiento = row.GetCell("VL_ENDPOINT_REPROCESS").Value;
                var interfazExterna = int.Parse(row.GetCell("CD_INTERFAZ_EXTERNA").Value);
                var errorCarga = !string.IsNullOrEmpty(row.GetCell("FL_ERROR_CARGA").Value) ? row.GetCell("FL_ERROR_CARGA").Value : "N";
                var errorProcedimiento = !string.IsNullOrEmpty(row.GetCell("FL_ERROR_PROCEDIMIENTO").Value) ? row.GetCell("FL_ERROR_PROCEDIMIENTO").Value : "N";

                var isAPI = SituacionDb.IsAPI(situacion);
                var isSalida = (idEntradaSalida == "S" || CInterfazExterna.IsSalida(interfazExterna));

                if (errorCarga == "N" && errorProcedimiento == "N")
                    deshabilitados.Add("btnViewError");

                if (interfazEjecQuery.GetCount() == 0)
                    deshabilitados.Add("btnViewData");

                if (!ValidateDetail(interfazExterna, nuInterfaz))
                    deshabilitados.Add("btnViewDetail");

                if ((isAPI && (situacion != SituacionDb.ProcesadoConError || (!isSalida && string.IsNullOrEmpty(actionReprocesamiento)))) ||
                        (!isAPI && (situacion != SituacionDb.ArchivoRespaldado || (errorCarga == "N" && errorProcedimiento == "N"))))
                    deshabilitados.Add("btnReprocess");

                if (isAPI && (situacion != SituacionDb.ErrorNotificacionAutomatismo))
                    deshabilitados.Add("btnReprocessNotificacion");

                row.DisabledButtons = deshabilitados;
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            InterfazEjecucionQuery dbQuery;
            if (context.Parameters.Count > 0)
            {
                if (!long.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "interfaz")?.Value, out long nuInterfaz))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new InterfazEjecucionQuery(nuInterfaz);
            }
            else
            {
                dbQuery = new InterfazEjecucionQuery();
            }

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            InterfazEjecucionQuery dbQuery;
            if (context.Parameters.Count > 0)
            {
                if (!long.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "interfaz")?.Value, out long nuInterfaz))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new InterfazEjecucionQuery(nuInterfaz);
            }
            else
            {
                dbQuery = new InterfazEjecucionQuery();
            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            switch (context.ButtonId)
            {
                case "btnViewData":
                    context.Redirect("/api/File/Download", true, new List<ComponentParameter> {
                        new ComponentParameter() { Id = "fileId", Value = context.Row.GetCell("NU_INTERFAZ_EJECUCION").Value } ,
                        new ComponentParameter() { Id = "application", Value = _identity.Application }
                    });
                    break;
                case "btnViewDetail":
                    context.Redirect(this.RedireccionCondicionada(context.Row), new List<ComponentParameter>
                    {
                        new ComponentParameter() { Id = "interfaz", Value = context.Row.GetCell("NU_INTERFAZ_EJECUCION").Value }
                    });
                    break;
                case "btnReprocess":
                    return this.Reprocesar(context.Row.GetCell("NU_INTERFAZ_EJECUCION").Value, context);
                case "btnReprocessNotificacion":
                    return this.ReprocessNotificacion(context.Row.GetCell("NU_INTERFAZ_EJECUCION").Value, context);
                case "btnViewError":
                    context.AddParameter("interfaz", context.Row.GetCell("NU_INTERFAZ_EJECUCION").Value);
                    break;
            }
            return context;
        }

        public override Grid GridImportExcel(Grid grid, GridImportExcelContext context)
        {
            if (context.Payload == null)
                throw new MissingParameterException("Datos nulos");

            if (!int.TryParse(context.FetchContext.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int empresa))
                throw new MissingParameterException("INT050_Sec0_Error_EmpresaRequerida");

            if (!int.TryParse(context.FetchContext.Parameters.FirstOrDefault(s => s.Id == "api")?.Value, out int api))
                throw new MissingParameterException("INT050_Sec0_Error_NoSeEnvioParametroApi");

            string referencia = context.FetchContext.Parameters.FirstOrDefault(s => s.Id == "referencia")?.Value;
            grid = this.GridFetchRows(grid, context.FetchContext);

            using var uow = this._uowFactory.GetUnitOfWork();

            var descargaErrores = (uow.ParametroRepository.GetParameter(ParamManager.IMPORT_EXCEL_DESCARGA_ERRORES, new Dictionary<string, string>
            {
                [ParamManager.PARAM_EMPR] = $"{ParamManager.PARAM_EMPR}_{empresa}"
            }) ?? "S") == "S";

            var payloadAux = context.Payload;
            using (var excelImporter = new GridExcelImporterAPI(_uowFactory, context.Translator, context.FileName, api, empresa, referencia, context.Payload, this._identity.GetFormatProvider(), _configuration, descargaErrores))
            {
                try
                {
                    excelImporter.CleanErrors();

                    var payload = excelImporter.CreateRequests(out string method);

                    if (!_wmsApiService.IsEnabled())
                        throw new Exception("INT050_Sec0_Error_ApiDeshabilitada");

                    string result = _wmsApiService.CallService(method, payload);
                    context.AddSuccessNotification(result);
                }
                catch (MissingParameterException ex)
                {
                    _logger.LogError(ex, "GridImportExcel");
                    throw;
                }
                catch (ApiEntradaValidationException ex)
                {
                    _logger.LogError(ex, "GridImportExcel");

                    if (descargaErrores)
                    {
                        excelImporter.SetErrors(ex.Errors);
                        context.Payload = Convert.ToBase64String(excelImporter.GetAsByteArray());

                        throw;
                    }
                    else
                    {
                        var msg = ($"{ex.Message} - Item {ex.Errors?.LastOrDefault().ItemId} - {ex.Errors?.LastOrDefault()?.Messages?.LastOrDefault()}").Replace(':', ' ');
                        throw new ValidationFailedException(msg);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "GridImportExcel");

                    if (descargaErrores)
                        context.Payload = Convert.ToBase64String(excelImporter.GetAsByteArray());

                    throw;
                }
            }
            return grid;
        }


        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            FormField selectApi = form.GetField("api");
            selectApi.Options = new List<SelectOption>();

            Dictionary<string, bool> result = this._security.CheckPermissions(new List<string>
            {
                SecurityResources.INT050APIAgendas_Page_Access_Allow,
                SecurityResources.INT050APIAgentes_Page_Access_Allow,
                SecurityResources.INT050APICodigoDeBarras_Page_Access_Allow,
                SecurityResources.INT050APIEmpresas_Page_Access_Allow,
                SecurityResources.INT050APILpn_Page_Access_Allow,
                SecurityResources.INT050APIPedidos_Page_Access_Allow,
                SecurityResources.INT050APIProducto_Page_Access_Allow,
                SecurityResources.INT050APIProductoProveedor_Page_Access_Allow,
                SecurityResources.INT050APIReferenciaDeRecepcion_Page_Access_Allow,
                SecurityResources.INT050APIProduccion_Page_Access_Allow,
                SecurityResources.INT050APIControlDeCalidad_Page_Access_Allow,
                SecurityResources.INT050APIFacturas_Page_Access_Allow,
                SecurityResources.INT050APIPickingProducto_Page_Access_Allow,
            });

            if (result[SecurityResources.INT050APIAgendas_Page_Access_Allow])
                selectApi.Options.Add(new SelectOption(CInterfazExterna.Agendas.ToString(), "INT050_Sec0_api_Agendas"));

            if (result[SecurityResources.INT050APIAgentes_Page_Access_Allow])
                selectApi.Options.Add(new SelectOption(CInterfazExterna.Agentes.ToString(), "INT050_Sec0_api_Agentes"));

            if (result[SecurityResources.INT050APICodigoDeBarras_Page_Access_Allow])
                selectApi.Options.Add(new SelectOption(CInterfazExterna.CodigoDeBarras.ToString(), "INT050_Sec0_api_CodigoBarras"));

            if (result[SecurityResources.INT050APIEmpresas_Page_Access_Allow])
                selectApi.Options.Add(new SelectOption(CInterfazExterna.Empresas.ToString(), "INT050_Sec0_api_Empresas"));

            if (result[SecurityResources.INT050APILpn_Page_Access_Allow])
                selectApi.Options.Add(new SelectOption(CInterfazExterna.Lpn.ToString(), "INT050_Sec0_api_Lpn"));

            if (result[SecurityResources.INT050APIPedidos_Page_Access_Allow])
                selectApi.Options.Add(new SelectOption(CInterfazExterna.Pedidos.ToString(), "INT050_Sec0_api_Pedidos"));

            if (result[SecurityResources.INT050APIProducto_Page_Access_Allow])
                selectApi.Options.Add(new SelectOption(CInterfazExterna.Producto.ToString(), "INT050_Sec0_api_Producto"));

            if (result[SecurityResources.INT050APIProductoProveedor_Page_Access_Allow])
                selectApi.Options.Add(new SelectOption(CInterfazExterna.ProductoProveedor.ToString(), "INT050_Sec0_api_ProductoProveedor"));

            if (result[SecurityResources.INT050APIReferenciaDeRecepcion_Page_Access_Allow])
                selectApi.Options.Add(new SelectOption(CInterfazExterna.ReferenciaDeRecepcion.ToString(), "INT050_Sec0_api_ReferenciaRecepcion"));

            if (result[SecurityResources.INT050APIProduccion_Page_Access_Allow])
                selectApi.Options.Add(new SelectOption(CInterfazExterna.Produccion.ToString(), "INT050_Sec0_api_Produccion"));

            if (result[SecurityResources.INT050APIControlDeCalidad_Page_Access_Allow])
                selectApi.Options.Add(new SelectOption(CInterfazExterna.ControlDeCalidad.ToString(), "INT050_Sec0_api_ControlDeCalidad"));

            if (result[SecurityResources.INT050APIFacturas_Page_Access_Allow])
                selectApi.Options.Add(new SelectOption(CInterfazExterna.Facturas.ToString(), "INT050_Sec0_api_Facturas"));

            if (result[SecurityResources.INT050APIPickingProducto_Page_Access_Allow])
                selectApi.Options.Add(new SelectOption(CInterfazExterna.PickingProducto.ToString(), "INT050_Sec0_api_Picking"));

            if (selectApi.Options.Any(o => o.Value == CInterfazExterna.Producto.ToString()))
            {
                selectApi.Value = CInterfazExterna.Producto.ToString();
            }
            else if (selectApi.Options.Any())
            {
                selectApi.Value = selectApi.Options.First().Value;
            }

            FormField fieldEmpresa = form.GetField("empresa");
            fieldEmpresa.Disabled = false;

            var empresas = uow.EmpresaRepository.GetEmpresasAsignadasUsuario(_identity.UserId);

            string empAux = string.Empty; ;
            if (empresas != null && empresas.Count == 1)
            {
                fieldEmpresa.Options = SearchEmpresa(form, new FormSelectSearchContext()
                {
                    SearchValue = empresas.FirstOrDefault().ToString()
                });

                fieldEmpresa.Value = empresas.FirstOrDefault().ToString();
                fieldEmpresa.Disabled = true;
                empAux = empresas.FirstOrDefault().ToString();
            }
            context.AddParameter("empresaUnica", empAux);

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            return form;
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "empresa": return this.SearchEmpresa(form, context);
                default: return new List<SelectOption>();
            }
        }

        #region Metodos Auxiliares

        public virtual string RedireccionCondicionada(GridRow row)
        {
            var interfazExterna = int.Parse(row.GetCell("CD_INTERFAZ_EXTERNA").Value);

            if (interfazExterna == CInterfazExterna.Pedidos)
                return "/interfaz/INT100";

            else if (interfazExterna == CInterfazExterna.Producto)
                return "/interfaz/INT101";

            else if (interfazExterna == CInterfazExterna.CodigoDeBarras)
                return "/interfaz/INT102";

            else if (interfazExterna == CInterfazExterna.ReferenciaDeRecepcion)
                return "/interfaz/INT103";

            else if (interfazExterna == CInterfazExterna.PedidosAnulados)
                return "/interfaz/INT104";

            else if (interfazExterna == CInterfazExterna.AjustesDeStock)
                return "/interfaz/INT105";

            else if (interfazExterna == CInterfazExterna.Agentes)
                return "/interfaz/INT106";

            else if (interfazExterna == CInterfazExterna.ConfirmacionDePedido)
                return "/interfaz/INT107";

            else if (interfazExterna == CInterfazExterna.ProductoProveedor)
                return "/interfaz/INT108";

            else if (interfazExterna == CInterfazExterna.Facturacion)
                return "/interfaz/INT109";
            else
                return string.Empty;
        }

        public virtual bool ValidateDetail(int interfazExterna, long numeroInterfaz)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (interfazExterna == CInterfazExterna.Pedidos && uow.InterfazEjecDataRepository.EstanPedidoSalida(numeroInterfaz))
                return true;
            else if (interfazExterna == CInterfazExterna.Producto && uow.InterfazEjecDataRepository.EstanProducto(numeroInterfaz))
                return true;
            else if (interfazExterna == CInterfazExterna.CodigoDeBarras && uow.InterfazEjecDataRepository.EstanCodigoBarras(numeroInterfaz))
                return true;
            else if (interfazExterna == CInterfazExterna.ReferenciaDeRecepcion && uow.InterfazEjecDataRepository.EstanRefRecepcion(numeroInterfaz))
                return true;
            else if (interfazExterna == CInterfazExterna.PedidosAnulados && uow.InterfazEjecDataRepository.EstanPedidoAnulado(numeroInterfaz))
                return true;
            else if (interfazExterna == CInterfazExterna.AjustesDeStock && uow.InterfazEjecDataRepository.EstanAjusteStock(numeroInterfaz))
                return true;
            else if (interfazExterna == CInterfazExterna.Agentes && uow.InterfazEjecDataRepository.EstanAgente(numeroInterfaz))
                return true;
            else if (interfazExterna == CInterfazExterna.ConfirmacionDePedido && uow.InterfazEjecDataRepository.EstanConfPedido(numeroInterfaz))
                return true;
            else if (interfazExterna == CInterfazExterna.ProductoProveedor && uow.InterfazEjecDataRepository.EstanConvertedor(numeroInterfaz))
                return true;
            else if (interfazExterna == CInterfazExterna.Facturacion && uow.InterfazEjecDataRepository.EstanFacturaRec(numeroInterfaz))
                return true;
            else
                return false;
        }

        public virtual GridButtonActionContext Reprocesar(string nuInt, GridButtonActionContext context)
        {
            if (!long.TryParse(nuInt, out long nuInterfaz))
            {
                context.AddErrorNotification("INT050_Sec0_Error_Er003_ParamIncorrecto");
                return context;
            }

            using var uow = this._uowFactory.GetUnitOfWork();

            var interfaz = uow.InterfazRepository.GetInterfaz(nuInterfaz);
            if (interfaz == null)
            {
                context.AddErrorNotification("INT050_Sec0_Error_Er001_InterfazNoExiste");
                return context;
            }

            var actionReprocesamiento = interfaz.InterfazExterna.EndpointReprocess;

            if ((interfaz.IsAPI && (interfaz.Situacion != SituacionDb.ProcesadoConError || (!interfaz.IsSalida && string.IsNullOrEmpty(actionReprocesamiento)))) ||
                (!interfaz.IsAPI && (interfaz.Situacion != SituacionDb.ArchivoRespaldado || (interfaz.ErrorCarga == "N" && interfaz.ErrorProcedimiento == "N"))))
            {
                context.AddErrorNotification("INT050_Sec0_Error_Er002_NoPuedeReprocesar");
                return context;
            }

            if (!interfaz.IsAPI || interfaz.IsSalida)
            {
                if (!interfaz.IsAPI && interfaz.InterfazExterna.CodigoInterfazExterna == CInterfazExterna.Pedidos)
                    ReprocesarInterfaz(uow, interfaz);

                BorrarErrores(uow, interfaz);

                interfaz.Situacion = interfaz.IsAPI ? SituacionDb.ProcesadoPendiente : (interfaz.IsSalida ? SituacionDb.ProcesandoInterfaz : SituacionDb.ArchivoProcesado);
                interfaz.ErrorCarga = "N";
                interfaz.ErrorProcedimiento = "N";
                interfaz.FechaSituacion = DateTime.Now;

                uow.InterfazRepository.Update(interfaz);

                if (interfaz.IsAPI && interfaz.IsSalida && interfaz.Empresa.HasValue)
                    uow.EmpresaRepository.UpdateLock(interfaz.Empresa.Value, false).Wait();

                uow.SaveChanges();

                if (interfaz.IsAPI && interfaz.IsSalida)
                {
                    _taskQueueService.Restart();
                }
            }

            if (interfaz.IsAPI && !interfaz.IsSalida)
            {
                ReprocesarApiEntrada(actionReprocesamiento, interfaz);
            }

            context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            return context;
        }

        public virtual GridButtonActionContext ReprocessNotificacion(string nuInt, GridButtonActionContext context)
        {
            if (!long.TryParse(nuInt, out long nuInterfaz))
            {
                context.AddErrorNotification("INT050_Sec0_Error_Er003_ParamIncorrecto");
                return context;
            }

            using var uow = this._uowFactory.GetUnitOfWork();

            InterfazEjecucion interfaz = uow.InterfazRepository.GetInterfaz(nuInterfaz);
            if (interfaz == null)
            {
                context.AddErrorNotification("INT050_Sec0_Error_Er001_InterfazNoExiste");
                return context;
            }

            if (interfaz.IsAPI && (interfaz.Situacion != SituacionDb.ErrorNotificacionAutomatismo))
            {
                context.AddErrorNotification("INT050_Sec0_Error_Er002_NoPuedeReprocesarNotificacion");
                return context;
            }

            interfaz.Situacion = SituacionDb.EnProcesoNotificacionAutomatismo;
            uow.EjecucionRepository.Update(interfaz);

            uow.SaveChanges();

            try
            {
                var itfzData = uow.InterfazRepository.GetEjecucionData(nuInterfaz);
                if (interfaz.CdInterfazExterna == CInterfazExterna.Producto)
                {
                    var request = JsonConvert.DeserializeObject<ProductosRequest>(Encoding.UTF8.GetString(itfzData.Data));
                    List<Producto> codigos = _productoMapper.Map(request);
                    _productoService.NotificarAutomatismo(uow, codigos);
                }
                else if (interfaz.CdInterfazExterna == CInterfazExterna.CodigoDeBarras)
                {
                    var request = JsonConvert.DeserializeObject<CodigosBarrasRequest>(Encoding.UTF8.GetString(itfzData.Data));
                    List<CodigoBarras> codigos = _barcodeMapper.Map(request);
                    _codigoBarrasService.NotificarAutomatismo(uow, codigos);
                }
            }
            catch (AutomatismoException ex)
            {
                interfaz.Situacion = SituacionDb.ErrorNotificacionAutomatismo;
                uow.EjecucionRepository.Update(interfaz);
            }
            if (interfaz.Situacion != SituacionDb.ErrorNotificacionAutomatismo)
            {
                interfaz.Situacion = SituacionDb.ProcesadoOK;
                uow.EjecucionRepository.Update(interfaz);
            }

            uow.SaveChanges();

            return context;
        }

        public virtual void ReprocesarApiEntrada(string action, InterfazEjecucion interfaz)
        {
            var request = JsonConvert.SerializeObject(new ReprocesamientoRequest
            {
                Empresa = interfaz.Empresa ?? -1,
                Interfaz = interfaz.Id
            });

            string result = _wmsApiService.CallService(action, request);
        }

        public virtual void ReprocesarInterfaz(IUnitOfWork uow, InterfazEjecucion interfaz)
        {
            var colDet = uow.InterfazRepository.GetIntzPedidosDetalles(interfaz.Id);

            foreach (EstanPedidoSalidaDet obj in colDet)
            {
                obj.IdProcesado = "C";
                uow.InterfazRepository.UpdateColDetPedido(obj);
            }

            List<EstanPedidoSalida> col = uow.InterfazRepository.GetIntzPedidos(interfaz.Id);

            foreach (EstanPedidoSalida obj in col)
            {
                obj.IdProcesado = "C";
                uow.InterfazRepository.UpdateColPedido(obj);
            }

            uow.SaveChanges();
        }

        public virtual void BorrarErrores(IUnitOfWork uow, InterfazEjecucion interfaz)
        {
            List<InterfazError> errores = uow.InterfazRepository.GetInterfazError(interfaz.Id);
            foreach (InterfazError err in errores)
            {
                uow.InterfazRepository.RemoveInterfazError(err);
            }

            uow.SaveChanges();
        }

        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Empresa> empresas = uow.EmpresaRepository.GetByNombreOrCodePartial(context.SearchValue);

            foreach (var empresa in empresas)
            {
                opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
            }

            return opciones;
        }

        #endregion
    }
}
