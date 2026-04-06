using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Produccion;
using WIS.Application.Validation.Modules.GridModules.Produccion;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Domain.Picking;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Produccion.Interfaces;
using WIS.Domain.Produccion.Mappers;
using WIS.Domain.Produccion.Models;
using WIS.Exceptions;
using WIS.Extension;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRD
{
    public class PRD110CrearIngreso : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();
        protected readonly IGridValidationService _gridValidationService;
        protected readonly ILogicaProduccionFactory _logicaProduccionFactory;

        protected List<string> GridKeys { get; set; }

        public PRD110CrearIngreso(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            IGridValidationService _gridValidationService,
            ILogicaProduccionFactory logicaProduccionFactory)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._formValidationService = formValidationService;
            this._filterInterpreter = filterInterpreter;
            this._gridValidationService = _gridValidationService;
            this._logicaProduccionFactory = logicaProduccionFactory;


            this.GridKeys = new List<string>
            {
                "CD_PRDC_DEFINICION", "CD_ACCION_INSTANCIA"
            };
        }

        #region Keys

        protected readonly List<string> GridKeysEntrada = new List<string> { "NU_PRDC_DET_TEORICO" };

        protected readonly List<string> GridKeysSalida = new List<string> { "NU_PRDC_DET_TEORICO" };

        protected readonly List<SortCommand> GridSortEntrada = new List<SortCommand> { new SortCommand("NU_PRDC_DET_TEORICO", SortDirection.Descending) };

        protected readonly List<SortCommand> GridSortSalida = new List<SortCommand> { new SortCommand("NU_PRDC_DET_TEORICO", SortDirection.Descending) };

        #endregion

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            var formula = context.GetParameter("isConFormula") == "true";

            if (formula)
            {
                context.IsEditingEnabled = false;
                context.IsAddEnabled = false;
                context.IsCommitEnabled = false;
                context.IsRemoveEnabled = false;
            }
            else
            {
                context.IsEditingEnabled = true;
                context.IsAddEnabled = true;
                context.IsCommitEnabled = false;
                context.IsRemoveEnabled = true;

                var insertableColumns = new List<string> { "CD_PRODUTO", "QT_TEORICO" };

                if (grid.Id == "PRD110CrearIngreso_grid_1")
                    insertableColumns.Add("NU_IDENTIFICADOR");

                grid.SetInsertableColumns(insertableColumns);
            }

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            if (grid.Id == "PRD110CrearIngreso_grid_1")
            {
                var dbQuery = new DetallesIngresoControlAuxQuery(null, CIngresoProduccionDetalleTeorico.TipoDetalleEntrada);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.GridSortEntrada, this.GridKeysEntrada);

                grid.SetEditableCells(new List<string> { "CD_PRODUTO", "NU_IDENTIFICADOR", "QT_TEORICO" });

            }
            else if (grid.Id == "PRD110CrearIngreso_grid_2")
            {
                var dbQuery = new DetallesIngresoControlAuxQuery(null, CIngresoProduccionDetalleTeorico.TipoDetalleSalida);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.GridSortSalida, this.GridKeysSalida);

                grid.SetEditableCells(new List<string> { "CD_PRODUTO", "QT_TEORICO" });

            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            if (grid.Id == "PRD110CrearIngreso_grid_1")
            {
                var dbQuery = new DetallesIngresoControlAuxQuery(null, CIngresoProduccionDetalleTeorico.TipoDetalleEntrada);

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else if (grid.Id == "PRD110CrearIngreso_grid_2")
            {
                var dbQuery = new DetallesIngresoControlAuxQuery(null, CIngresoProduccionDetalleTeorico.TipoDetalleSalida);

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }

            return null;
        }

        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            switch (context.ColumnId)
            {
                case "CD_PRODUTO":
                    return this.SearchProducto(grid, context);
            }

            return new List<SelectOption>();
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            if (grid.Id == "PRD110CrearIngreso_grid_1")
                return this._gridValidationService.Validate(new PRD110DetalleTeoricoEntradaGridValidationModule(uow, _identity.GetFormatProvider()), grid, row, context);
            else if (grid.Id == "PRD110CrearIngreso_grid_2")
                return this._gridValidationService.Validate(new PRD110DetalleTeoricoSalidaGridValidationModule(uow, _identity.GetFormatProvider()), grid, row, context);

            return null;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            var generarPedido = form.GetField("generarPedido");
            var generarPreparacion = form.GetField("generaPreparacion");

            generarPedido.Value = "true";
            generarPreparacion.Value = "true";

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var validationModule = new PRD110FormValidationModule(uow, _identity);
                var parameters = new List<ComponentParameter>();

                FormField selectPredio = form.GetField("predio");

                if (_identity.Predio != GeneralDb.PredioSinDefinir)
                {
                    var predio = uow.PredioRepository.GetPredio(_identity.Predio);

                    selectPredio.Options = new List<SelectOption> { new SelectOption(predio.Numero, $"{predio.Numero} - {predio.Descripcion}") };
                    selectPredio.Value = predio.Numero;
                    selectPredio.ReadOnly = true;

                    parameters.Add(new ComponentParameter() { Id = "predio", Value = predio.Numero });
                    validationModule.ValidatePredioOnSuccess(selectPredio, form, parameters);
                }
                else
                {
                    var predios = uow.PredioRepository.GetPrediosUsuario(_identity.UserId).OrderBy(p => p.Numero);

                    foreach (var predio in predios)
                    {
                        selectPredio.Options.Add(new SelectOption(predio.Numero, $"{predio.Numero} - {predio.Descripcion}"));
                    }
                }

                FormField selectEmpresa = form.GetField("empresa");

                Empresa empresa = uow.EmpresaRepository.GetEmpresaUnicaParaUsuario(_identity.UserId);

                if (empresa != null)
                {
                    selectEmpresa.ReadOnly = true;
                    selectEmpresa.Value = empresa.Id.ToString();
                    selectEmpresa.Options = new List<SelectOption> { new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}") };

                    parameters.Add(new ComponentParameter() { Id = "empresa", Value = empresa.Id.ToString() });
                    validationModule.ValidateEmpresaOnSuccess(selectEmpresa, form, parameters);
                }
                else
                {
                    selectEmpresa.Value = string.Empty;
                    selectEmpresa.Options = new List<SelectOption>();
                }
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            var uow = _uowFactory.GetUnitOfWork();

            var predio = form.GetField("predio").Value;
            var cdFormula = form.GetField("cdFormula")?.Value;
            var tipoIngreso = form.GetField("tpIngreso")?.Value;

            var usaFormula = form.GetField("ingresoConFormula")?.Value == "true";
            var generarPedido = form.GetField("generarPedido")?.Value == "true";
            var generaPreparacion = form.GetField("generaPreparacion")?.Value == "true";
            var idExterno = !string.IsNullOrEmpty(form.GetField("idExterno").Value) ? form.GetField("idExterno").Value : string.Empty;

            try
            {
                uow.CreateTransactionNumber(this._identity.Application);

                if (!usaFormula)
                {
                    ILogicaProduccion logicaProduccion;

                    var empresa = int.Parse(form.GetField("empresa").Value);

                    var rowsEntrada = JsonConvert.DeserializeObject<List<GridRow>>(context.GetParameter("rowsEntrada"))!;
                    var rowsSalida = JsonConvert.DeserializeObject<List<GridRow>>(context.GetParameter("rowsSalida"))!;

                    if (tipoIngreso == TipoIngresoProduccion.BlackBox && uow.EmpresaRepository.IsEmpresaDocumental(empresa))
                        throw new ValidationFailedException("PRD110_Error_msg_EmpresaDocumental");

                    ValidateRows(uow, form, rowsEntrada, rowsSalida);

                    var detalles = new List<IngresoProduccionDetalleTeorico>();

                    var _mapper = new IngresoProduccionMapper();

                    detalles.AddRange(rowsEntrada.Select(s => _mapper.MapRowToObject(s, _identity.GetFormatProvider())));
                    detalles.AddRange(rowsSalida.Select(s => _mapper.MapRowToObject(s, _identity.GetFormatProvider())));

                    if (tipoIngreso == TipoIngresoProduccion.Colector)
                        logicaProduccion = _logicaProduccionFactory.CreateLogicaProduccion(uow, TipoIngresoProduccion.Colector);
                    else
                        logicaProduccion = _logicaProduccionFactory.CreateLogicaProduccion(uow, TipoIngresoProduccion.BlackBox);

                    uow.BeginTransaction();
                    uow.CreateTransactionNumber("PRD110 - Crear Ingreso a Producción");

                    detalles.ForEach(det => det.NumeroTransaccion = uow.GetTransactionNumber());

                    var ingreso = logicaProduccion.CrearIngresoProduccion(tipoIngreso, empresa, predio, detalles, idExterno);

                    logicaProduccion.AddIngresoProduccion();

                    uow.SaveChanges();

                    if (generarPedido)
                    {
                        var pedido = CrearPedido(uow, ingreso, generaPreparacion);
                        uow.PedidoRepository.AddPedidoConDetalle(pedido);

                        ingreso.GeneraPedido = true;
                        ingreso.Situacion = SituacionDb.PEDIDO_GENERADO;
                        ingreso.NuTransaccion = uow.GetTransactionNumber();
                        uow.IngresoProduccionRepository.UpdateIngresoProduccion(ingreso);

                        uow.SaveChanges();

                        context.AddSuccessNotification("PRD110_Sec0_Succes_SeCreoElPedidoProduccion", new List<string>() { pedido.Id });
                    }

                    context.AddSuccessNotification("PRD110_Sec0_Succes_SeCreoElIngresoAProduccion", new List<string>() { ingreso.Id });
                }
                else
                {
                    var detalles = new List<IngresoProduccionDetalleTeorico>();

                    var _mapper = new IngresoProduccionMapper();

                    ILogicaProduccion logicaProduccion;

                    if (tipoIngreso == TipoIngresoProduccion.Colector)
                        logicaProduccion = _logicaProduccionFactory.CreateLogicaProduccion(uow, TipoIngresoProduccion.Colector);
                    else
                        logicaProduccion = _logicaProduccionFactory.CreateLogicaProduccion(uow, TipoIngresoProduccion.BlackBox);

                    uow.BeginTransaction();
                    uow.CreateTransactionNumber("PRD110 - Crear Ingreso a Producción");

                    var cantidadFormula = int.Parse(form.GetField("qtFormula")?.Value);

                    var formula = uow.FormulaRepository.GetFormula(cdFormula);

                    if (tipoIngreso == TipoIngresoProduccion.BlackBox && uow.EmpresaRepository.IsEmpresaDocumental(formula.Empresa))
                        throw new ValidationFailedException("PRD110_Error_msg_EmpresaDocumental");

                    detalles = _mapper.MapFormulaToObject(formula, cantidadFormula, uow);

                    detalles.ForEach(det => det.NumeroTransaccion = uow.GetTransactionNumber());

                    var ingreso = logicaProduccion.CrearIngresoProduccion(tipoIngreso, formula.Empresa, predio, detalles);

                    ingreso.Formula = formula;
                    ingreso.IdFormula = formula.Id;
                    ingreso.CantidadIteracionesFormula = cantidadFormula;

                    if (!string.IsNullOrEmpty(form.GetField("idExterno").Value))
                        ingreso.IdProduccionExterno = form.GetField("idExterno").Value;

                    logicaProduccion.AddIngresoProduccion();

                    uow.SaveChanges();

                    if (generarPedido)
                    {
                        var pedido = CrearPedido(uow, ingreso, generaPreparacion);
                        uow.PedidoRepository.AddPedidoConDetalle(pedido);

                        ingreso.GeneraPedido = true;
                        ingreso.Situacion = SituacionDb.PEDIDO_GENERADO;
                        ingreso.NuTransaccion = uow.GetTransactionNumber();
                        uow.IngresoProduccionRepository.UpdateIngresoProduccion(ingreso);

                        uow.SaveChanges();

                        context.AddSuccessNotification("PRD110_Sec0_Succes_SeCreoElPedidoProduccion", new List<string>() { pedido.Id });
                    }

                    context.AddSuccessNotification("PRD110_Sec0_Succes_SeCreoElIngresoAProduccion", new List<string>() { ingreso.Id });
                }

                uow.SaveChanges();
                uow.Commit();
            }
            catch (ValidationFailedException ex)
            {
                _logger.Error(ex, ex.Message);
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                uow.Rollback();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new PRD110FormValidationModule(uow, this._identity), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "cdFormula":
                    return this.SearchFormula(form, context);
                case "empresa":
                    return this.SearchEmpresa(form, context);
                default:
                    return new List<SelectOption>();
            }
        }

        #region Metodos Auxiliares

        public virtual List<SelectOption> SearchFormula(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();
            var empresa = form.GetField("empresa").Value;

            if (string.IsNullOrEmpty(empresa))
                return opciones;

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                List<Formula> formulas = uow.FormulaRepository.GetFormulasActivasEmpresaByNameOrCode(context.SearchValue, this._identity.UserId, int.Parse(empresa));

                foreach (var formula in formulas)
                {
                    opciones.Add(new SelectOption(formula.Id.ToString(), formula.Nombre));
                }
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var empresas = uow.EmpresaRepository.GetByNombreOrCodePartialForUsuario(context.SearchValue, this._identity.UserId);

            foreach (var empresa in empresas)
            {
                opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
            }

            return opciones;
        }

        public virtual void ValidarConfiguracionFormula(string cdFormula, string tipoIngreso, IUnitOfWork uow)
        {
            if (tipoIngreso == TipoIngresoProduccion.Colector)
            {
                var formula = uow.FormulaRepository.GetFormula(cdFormula);

                if (formula.Entrada.Any(e => e.CantidadConsumir == 0))
                {
                    throw new ValidationFailedException("General_Sec0_Error_ComponenteEntrada");
                }

                if (formula.Salida.Any(s => s.CantidadProducir == 0))
                {
                    throw new ValidationFailedException("General_Sec0_Error_ProductoSalida");
                }

                if (formula.Entrada.Count == 0 || formula.Salida.Count == 0)
                {
                    throw new ValidationFailedException("General_Sec0_Error_FormulaSinProductos");
                }
            }
        }

        public virtual Pedido CrearPedido(IUnitOfWork uow, IngresoProduccion ingreso, bool generaPreparacion)
        {
            var empresa = uow.EmpresaRepository.GetEmpresa((int)ingreso.Empresa);
            var cliente = uow.AgenteRepository.GetAgente(empresa.Id, empresa.CdClienteArmadoKit);

            if (cliente == null)
                throw new ValidationFailedException("General_Sec0_Error_ClienteNoExiste", new string[] { empresa.CdClienteArmadoKit });

            Pedido pedido = new Pedido()
            {
                Id = uow.PedidoRepository.GetNextNuPedidoManual().ToString(),
                ComparteContenedorPicking = $"{ingreso.Id}.{empresa.CdClienteArmadoKit}.{empresa.Id}",
                Empresa = empresa.Id,
                Cliente = empresa.CdClienteArmadoKit,
                Ruta = cliente.Ruta.Id,
                Estado = SituacionDb.PedidoAbierto,
                IsManual = false,
                Agrupacion = Agrupacion.Pedido,
                FechaAlta = DateTime.Now,
                IngresoProduccion = ingreso.Id,
                Memo1 = "Pedido generado para producción en PRD110",
                Origen = "PRD110",
                CondicionLiberacion = CondicionLiberacionDb.SinCondicion,
                Tipo = TipoPedidoDb.Produccion,
                Memo = $"Pedido generado para producción {ingreso.Id}.",
                ConfiguracionExpedicion = new ConfiguracionExpedicionPedido() { Tipo = Domain.DataModel.Mappers.Constants.TipoExpedicion.Produccion },
                NuCarga = null,
                Transaccion = uow.GetTransactionNumber(),
                Predio = ingreso.Predio,
            };

            foreach (var detalleEntrada in ingreso.Detalles.Where(x => x.Tipo == CIngresoProduccionDetalleTeorico.TipoDetalleEntrada).ToList())
            {
                var producto = uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(empresa.Id, detalleEntrada.Producto);

                pedido.Lineas.Add(new DetallePedido()
                {
                    Id = pedido.Id,
                    Cliente = empresa.CdClienteArmadoKit,
                    Empresa = empresa.Id,
                    Producto = detalleEntrada.Producto,
                    Faixa = (decimal)detalleEntrada.Faixa,
                    Identificador = detalleEntrada.Identificador,
                    EspecificaIdentificador = Producto.EspecificaIdentificador(detalleEntrada.Identificador),
                    Agrupacion = Agrupacion.Pedido,
                    Cantidad = detalleEntrada.CantidadTeorica,
                    CantidadLiberada = 0,
                    CantidadAnulada = 0,
                    CantidadAnuladaFactura = 0,
                    CantUndAsociadoCamion = 0,
                    FechaAlta = DateTime.Now,
                    CantidadOriginal = detalleEntrada.CantidadTeorica,
                    Transaccion = pedido.Transaccion,
                });
            }

            if (generaPreparacion)
            {
                var codigoContenedorValidado = uow.ParametroRepository.GetParameter("PRDC_PED_CD_CON_VAL");
                var onda = short.Parse(uow.ParametroRepository.GetParameter("PRD112_LIB_ONDA"));
                var agrupacion = uow.ParametroRepository.GetParameter("PRD112_LIB_AGRUPACION");
                var respetarFifoEnLoteAUTO = uow.ParametroRepository.GetParameter("PRD112_LIB_RESP_FIFO_AUTO") == "S";
                var controlaStockDocumental = uow.ParametroRepository.GetParameter("PRD112_LIB_CTRL_STK_DOCUMENTAL") == "S";
                var cursorStock = uow.ParametroRepository.GetParameter("PRD112_LIB_CURSOR_STOCK");
                var cursorPedido = uow.ParametroRepository.GetParameter("PRD112_LIB_CURSOR_PEDIDO");
                var debeLiberarPorCurvas = uow.ParametroRepository.GetParameter("PRD112_LIB_LIBERAR_CURVAS") == "S";
                var debeLiberarPorUnidades = uow.ParametroRepository.GetParameter("PRD112_LIB_LIBERAR_UNIDADES") == "S";
                var repartirEscasez = uow.ParametroRepository.GetParameter("PRD112_LIB_REPARTIR_ESCASEZ");
                var pickingAgrupCamion = uow.ParametroRepository.GetParameter("PRD112_LIB_AGRUP_CAMION") == "S";
                var prepararSoloConCamion = uow.ParametroRepository.GetParameter("PRD112_LIB_PREP_SOLO_CAMION") == "S";
                var modalPalletCompleto = uow.ParametroRepository.GetParameter("PRD112_LIB_MODO_PALLET_COMPLEO");
                var modalPalletIncompleto = uow.ParametroRepository.GetParameter("PRD112_LIB_MODO_PALLET_INCO");
                var priorizarDesborde = uow.ParametroRepository.GetParameter("PRD112_LIB_PRIORIZAR_DESBORDE") == "S";
                var manejaVidaUtil = uow.ParametroRepository.GetParameter("PRD112_LIB_MANEJA_VIDA_UTIL") == "S";
                var requiereUbicacion = uow.ParametroRepository.GetParameter("PRD112_LIB_PICKING_DOS_FACES") == "S";
                var excluirPicking = uow.ParametroRepository.GetParameter("PRD112_LIB_EXCLUIR_PICKING") == "S";

                var preparacion = new Preparacion()
                {
                    Descripcion = $"Lib Fabricación: {ingreso.IdProduccionExterno} Ped: {pedido.Id}".Truncate(60),
                    Empresa = ingreso.Empresa,
                    Onda = onda,
                    Agrupacion = agrupacion,
                    RespetarFifoEnLoteAUTO = respetarFifoEnLoteAUTO,
                    ControlaStockDocumental = controlaStockDocumental,
                    CursorStock = cursorStock,
                    DebeLiberarPorCurvas = debeLiberarPorCurvas,
                    DebeLiberarPorUnidades = debeLiberarPorUnidades,
                    RepartirEscasez = repartirEscasez,
                    PickingEsAgrupadoPorCamion = pickingAgrupCamion,
                    PrepararSoloConCamion = prepararSoloConCamion,
                    ModalPalletCompleto = modalPalletCompleto,
                    ModalPalletIncompleto = modalPalletIncompleto,
                    CursorPedido = cursorPedido,
                    UsarSoloStkPicking = priorizarDesborde,
                    ManejaVidaUtil = manejaVidaUtil,
                    RequiereUbicacion = requiereUbicacion,
                    FechaInicio = DateTime.Now,
                    ExcluirUbicacionesPicking = excluirPicking,
                    Predio = ingreso.Predio,
                    Transaccion = uow.GetTransactionNumber(),
                    Tipo = TipoPreparacionDb.Normal,
                    Usuario = _identity.UserId,
                    Situacion = SituacionDb.PreparacionPendiente,
                    AceptaMercaderiaAveriada = false,
                    PermitePickVencido = false,
                    ValidarProductoProveedor = false,
                };


                int nuPreparacion = uow.PreparacionRepository.AddPreparacion(preparacion);

                pedido.NumeroOrdenLiberacion = 0;
                pedido.PreparacionProgramada = nuPreparacion;
            }

            return pedido;
        }

        public virtual void ValidateRows(IUnitOfWork uow, Form form, List<GridRow> rowsEntrada, List<GridRow> rowsSalida)
        {
            var insumos = new List<string>();

            foreach (var row in rowsEntrada)
            {
                row.GetCell("TP_REGISTRO").Value = CIngresoProduccionDetalleTeorico.TipoDetalleEntrada;
                row.GetCell("CD_EMPRESA").Value = form.GetField("empresa").Value;
                row.GetCell("CD_FAIXA").Value = "1";

                var cantidad = row.GetCell("QT_TEORICO").Value;
                var cdProducto = row.GetCell("CD_PRODUTO").Value;

                if (string.IsNullOrEmpty(cantidad))
                    throw new ValidationFailedException("PRD110_grid_error_CantidadObligatoria");

                if (string.IsNullOrEmpty(cdProducto))
                    throw new ValidationFailedException("PRD110_grid_error_ProductoObligatorio");

                var producto = uow.ProductoRepository.GetProducto(int.Parse(form.GetField("empresa").Value), cdProducto);

                if (producto.ManejoIdentificador == ManejoIdentificador.Producto)
                    row.GetCell("NU_IDENTIFICADOR").Value = ManejoIdentificadorDb.IdentificadorProducto;
                else if (string.IsNullOrEmpty(row.GetCell("NU_IDENTIFICADOR").Value))
                    row.GetCell("NU_IDENTIFICADOR").Value = ManejoIdentificadorDb.IdentificadorAuto;

                var key = $"{cdProducto}.{row.GetCell("NU_IDENTIFICADOR").Value}";
                var keyAuto = $"{cdProducto}.{ManejoIdentificadorDb.IdentificadorAuto}";

                if (insumos.Contains(key))
                    throw new ValidationFailedException("PRD110_grid_error_ProductosRepetidos");
                else if (insumos.Contains(keyAuto))
                    throw new ValidationFailedException("WMSAPI_msg_Error_EnvioLoteEspecificoyAutoNoPermitido");
                else if (row.GetCell("NU_IDENTIFICADOR").Value == ManejoIdentificadorDb.IdentificadorAuto && insumos.Any(i => i.Contains(cdProducto)))
                    throw new ValidationFailedException("WMSAPI_msg_Error_EnvioLoteEspecificoyAutoNoPermitido");
                else
                    insumos.Add(key);
            }

            var productosProducir = new List<string>();

            foreach (var row in rowsSalida)
            {
                row.GetCell("TP_REGISTRO").Value = CIngresoProduccionDetalleTeorico.TipoDetalleSalida;
                row.GetCell("CD_EMPRESA").Value = form.GetField("empresa").Value;
                row.GetCell("CD_FAIXA").Value = "1";

                var cantidad = row.GetCell("QT_TEORICO").Value;
                var producto = row.GetCell("CD_PRODUTO").Value;

                if (string.IsNullOrEmpty(cantidad))
                    throw new ValidationFailedException("PRD110_grid_error_CantidadObligatoria");

                if (string.IsNullOrEmpty(producto))
                    throw new ValidationFailedException("PRD110_grid_error_ProductoObligatorio");

                if (productosProducir.Any(x => x == producto))
                    throw new ValidationFailedException("PRD110_grid_error_ProductosRepetidos");

                productosProducir.Add(producto);
            }

            if (rowsEntrada.Any(d => !d.IsValid()) || rowsSalida.Any(d => !d.IsValid()))
                throw new ValidationFailedException("PRD100_grid1_error_FormulaDetalleInvalido");
        }

        public virtual List<SelectOption> SearchProducto(Grid grid, GridSelectSearchContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            string paramEmpresa = context.GetParameter("empresa");

            List<SelectOption> opciones = new List<SelectOption>();

            if (string.IsNullOrEmpty(paramEmpresa))
                return opciones;

            int empresa = int.Parse(paramEmpresa);

            List<Producto> productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(empresa, context.SearchValue);

            foreach (Producto producto in productos)
            {
                opciones.Add(new SelectOption(producto.Codigo, $"{producto.Codigo} - {producto.Descripcion}"));
            }

            return opciones;
        }

        #endregion
    }
}
