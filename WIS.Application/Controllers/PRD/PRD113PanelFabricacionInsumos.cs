using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules.Produccion;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Domain.General;
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Produccion.Interfaces;
using WIS.Domain.Produccion.Models;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.PRD
{
    public class PRD113PanelFabricacionInsumos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogicaProduccionFactory _logicaProduccionFactory;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly ITaskQueueService _taskQueue;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected List<string> GridKeys { get; set; }

        protected List<SortCommand> DefaultSort { get; set; }

        public PRD113PanelFabricacionInsumos(
            IIdentityService identity,
            ITrafficOfficerService concurrencyControl,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            ILogicaProduccionFactory logicaProduccionFactory,
            IGridValidationService gridValidationService,
            ITaskQueueService taskQueue)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._formValidationService = formValidationService;
            this._filterInterpreter = filterInterpreter;
            this._logicaProduccionFactory = logicaProduccionFactory;
            this._concurrencyControl = concurrencyControl;
            this._gridValidationService = gridValidationService;
            this._taskQueue = taskQueue;

            this.GridKeys = new List<string>
            {
                "NU_PRDC_INGRESO_REAL",
                "NU_PRDC_INGRESO",
                "CD_PRODUTO",
                "CD_EMPRESA",
                "NU_IDENTIFICADOR",
                "FL_CONSUMIBLE"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_ORDEN", SortDirection.Ascending)
            };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsAddEnabled = false;
            context.IsCommitEnabled = false;
            context.IsRemoveEnabled = false;

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem>
            {
                new GridButton("btnParcial", "PRD113_grid1_btn_Parcial", "fas fa-list"),
            }));

            return GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            string nroIngresoProduccion = context.Parameters.Find(x => x.Id == "nuIngresoProduccion")?.Value;
            string empresaStr = context.Parameters.Find(x => x.Id == "cdEmpresa")?.Value;

            if (string.IsNullOrEmpty(nroIngresoProduccion) || !int.TryParse(empresaStr, out int empresa))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            string insumosConsumidos = context.Parameters.Find(x => x.Id == "insumosConsumidos")?.Value;

            var dbQuery = new StockInsumosProduccionQuery(nroIngresoProduccion, empresa, (insumosConsumidos == "S"));
            uow.HandleQuery(dbQuery);

            grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> { "ND_MOTIVO" });
            grid.AddOrUpdateColumn(new GridColumnSelect("ND_MOTIVO", this.SelectMotivos()));

            var motivoParam = uow.ParametroRepository.GetParameter(ParamManager.PRODUCCION_MOT_CONS_DEFAULT);
            var descripcionMotivoParam = uow.DominioRepository.GetDominio(motivoParam).Descripcion;

            foreach (var row in grid.Rows)
            {
                row.GetCell("ND_MOTIVO").Value = motivoParam;
                row.GetCell("DS_MOTIVO").Value = descripcionMotivoParam;

                decimal.TryParse(row.GetCell("QT_REAL").Value, _identity.GetFormatProvider(), out decimal cantidad);

                if (cantidad <= 0)
                    row.DisabledSelected = true;
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            string nroIngresoProduccion = context.Parameters.Find(x => x.Id == "nuIngresoProduccion")?.Value;
            string empresaStr = context.Parameters.Find(x => x.Id == "cdEmpresa")?.Value;

            if (string.IsNullOrEmpty(nroIngresoProduccion) || !int.TryParse(empresaStr, out int empresa))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            string insumosConsumidos = context.Parameters.Find(x => x.Id == "insumosConsumidos")?.Value;

            var dbQuery = new StockInsumosProduccionQuery(nroIngresoProduccion, empresa, (insumosConsumidos == "S"));

            uow.HandleQuery(dbQuery);

            var defaultSort = new List<SortCommand> { new SortCommand("CD_PRODUTO", SortDirection.Ascending), new SortCommand("NU_IDENTIFICADOR", SortDirection.Ascending) };

            context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            string nroIngresoProduccion = context.Parameters.Find(x => x.Id == "nuIngresoProduccion")?.Value;
            string empresaStr = context.Parameters.Find(x => x.Id == "cdEmpresa")?.Value;

            if (string.IsNullOrEmpty(nroIngresoProduccion) || !int.TryParse(empresaStr, out int empresa))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            string insumosConsumidos = context.Parameters.Find(x => x.Id == "insumosConsumidos")?.Value;

            var dbQuery = new StockInsumosProduccionQuery(nroIngresoProduccion, empresa, (insumosConsumidos == "S"));

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            try
            {
                switch (context.ButtonId)
                {
                    case "btnDesafectar":
                        context = BtnDesafectar(context);
                        break;
                    case "btnConsumir":
                        context = BtnConsumir(context);
                        break;

                }
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
                _logger.Error(ex, ex.Message);
            }

            return context;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new PRD113ConsumirInsumosGridValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        #region Metodos Auxiliares
        public virtual List<SelectOption> SelectMotivos()
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var motivos = uow.DominioRepository.GetDominios(TipoIngresoProduccion.MOTIVO_CONSUMO);

            List<SelectOption> opciones = new List<SelectOption>();

            foreach (var motivo in motivos)
            {
                opciones.Add(new SelectOption(motivo.Id, motivo.Id + " - " + motivo.Descripcion));
            }

            return opciones;
        }

        public virtual GridMenuItemActionContext BtnDesafectar(GridMenuItemActionContext context)
        {
            var idIngreso = context.Parameters.FirstOrDefault(x => x.Id == "nuIngresoProduccion").Value;
            using var uow = _uowFactory.GetUnitOfWork();

            var selection = context.Selection.GetSelection(this.GridKeys);

            var idInsumosSeleccionados = selection.Select(item => new IngresoProduccionDetalleReal
            {
                NuPrdcIngresoReal = long.Parse(item["NU_PRDC_INGRESO_REAL"]),
                Consumible = item["FL_CONSUMIBLE"],
            }).ToList();

            if (_concurrencyControl.IsLocked("T_PRDC_INGRESO", idIngreso, true))
                throw new Exception("General_msg_Error_ProduccionBloqueada");

            if (idInsumosSeleccionados.Any(x => x.Consumible == "S"))
                throw new Exception("PRD113_form1_Error_DesafectarConsumible");

            var transactionTO = _concurrencyControl.CreateTransaccion();

            try
            {
                _concurrencyControl.AddLock("T_PRDC_INGRESO", idIngreso, transactionTO, true);

                var logicaProduccion = _logicaProduccionFactory.GetLogicaProduccion(uow, idIngreso);

                if (!logicaProduccion.ProduccionHabilitadaParaFabricar())
                    throw new Exception("PRD113_grid1_Error_ProduccionEnEstadoIncorrecto");

                uow.CreateTransactionNumber("Desafectar insumo producción");
                uow.BeginTransaction();

                var ingreso = logicaProduccion.GetIngresoProduccion();

                var ubicacionProduccion = uow.ProduccionRepository.GetUbicacionProduccion(ingreso.IdEspacioProducion);

                foreach (var insumos in idInsumosSeleccionados)
                {
                    logicaProduccion.DesafectarInsumo(insumos.NuPrdcIngresoReal, ubicacionProduccion, _concurrencyControl, transactionTO);
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("PRD113_grid1_Msg_InsumoDesafectado");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);

                context.AddErrorNotification(ex.Message);
            }
            finally
            {
                _concurrencyControl.DeleteTransaccion(transactionTO);
            }

            return context;
        }

        public virtual GridMenuItemActionContext BtnConsumir(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var keysAjuste = new List<string>();
            var transactionTO = _concurrencyControl.CreateTransaccion();

            uow.BeginTransaction();

            try
            {
                var rows = JsonConvert.DeserializeObject<List<GridRow>>(context.GetParameter("modifiedRows"));

                var nuIngresoProduccion = context.Parameters.FirstOrDefault(x => x.Id == "nuIngresoProduccion").Value;
                var ubicacionProduccion = context.Parameters.FirstOrDefault(s => s.Id == "ubicacionProduccion").Value;

                var ingreso = uow.IngresoProduccionRepository.GetIngresoById(nuIngresoProduccion);

                if (ingreso == null)
                    throw new ValidationFailedException("General_Sec0_Error_ProduccionNotFound");

                var keysSelection = GetSelectedKeys(uow, context, nuIngresoProduccion);

                if (keysSelection.Count == 0)
                    throw new ValidationFailedException("PRD112_Sec0_Error_DebeSeleccionarFila");

                ValidarRows(rows, keysSelection);

                var logicaProduccion = _logicaProduccionFactory.GetLogicaProduccion(uow, nuIngresoProduccion);

                if (!logicaProduccion.ProduccionHabilitadaParaFabricar())
                    throw new ValidationFailedException("PRD113_grid1_Error_ProduccionEnEstadoIncorrecto");

                if (_concurrencyControl.IsLocked("T_PRDC_INGRESO", nuIngresoProduccion, true))
                    throw new EntityLockedException("General_msg_Error_ProduccionBloqueada");

                _concurrencyControl.AddLock("T_PRDC_INGRESO", nuIngresoProduccion, transactionTO, true);

                uow.CreateTransactionNumber("Consumir productos completos");
                var nuTransaccion = uow.GetTransactionNumber();

                var keysStock = keysSelection.Select(s => new Stock()
                {
                    Ubicacion = ubicacionProduccion,
                    Empresa = s.Empresa.Value,
                    Producto = s.Producto,
                    Faixa = 1,
                    Identificador = s.Identificador,
                    NumeroTransaccion = nuTransaccion,
                });

                var idsBloqueos = keysStock.Select(s => s.GetLockId(_identity.GetFormatProvider())).ToList();

                var listLock = this._concurrencyControl.GetLockList("T_STOCK", idsBloqueos, transactionTO);

                if (listLock.Count > 0)
                {
                    var keyBloqueo = listLock.FirstOrDefault().Id_Bloqueo.Split("#");
                    throw new EntityLockedException("PRD111ConsumirStock_grid1_msg_RegistrosBloqueados", [keyBloqueo[0], keyBloqueo[1], keyBloqueo[2], keyBloqueo[4],]);
                }

                this._concurrencyControl.AddLockList("T_STOCK", idsBloqueos, transactionTO, true);

                ingreso.Situacion = SituacionDb.PRODUCIENDO;
                ingreso.NuTransaccion = uow.GetTransactionNumber();

                uow.IngresoProduccionRepository.UpdateIngresoProduccion(ingreso);

                uow.SaveChanges();

                var stocks = uow.StockRepository.GetStock(keysStock).ToList();

                foreach (var keyDetalleInsumo in keysSelection)
                {
                    var motivoConsumo = GetMotivoConsumo(rows, keyDetalleInsumo);

                    if (motivoConsumo == TipoIngresoProduccion.MOT_CONS_ADS)
                    {
                        var keyAjuste = ProcesarBajaStock(uow, stocks, nuIngresoProduccion, keyDetalleInsumo, ubicacionProduccion);
                        keysAjuste.Add(keyAjuste);
                    }
                    else
                        ConsumirCompleto(uow, logicaProduccion, nuIngresoProduccion, keyDetalleInsumo, ubicacionProduccion, motivoConsumo);
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("General_Db_Success_Update");
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                _logger.Error(ex, ex.Message);
                uow.Rollback();
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
                _logger.Error(ex, ex.Message);
            }
            finally
            {
                _concurrencyControl.DeleteTransaccion(transactionTO);

                if (_taskQueue.IsEnabled() && keysAjuste.Any())
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.AjustesDeStock, keysAjuste);
            }

            return context;
        }

        public virtual void ValidarRows(List<GridRow> rows, List<IngresoProduccionDetalleReal> keysSelection)
        {
            foreach (var keyDetalleInsumo in keysSelection)
            {
                var motivoConsumo = GetMotivoConsumo(rows, keyDetalleInsumo);

                if (string.IsNullOrEmpty(motivoConsumo))
                    throw new ValidationFailedException("PRD113_grid1_Error_FaltaMotivoConsumo");
            }
        }

        public virtual string GetMotivoConsumo(List<GridRow> rows, IngresoProduccionDetalleReal detalleInsumo)
        {
            return rows
                .Where(row =>
                    row.GetCell("NU_PRDC_INGRESO_REAL").Value == detalleInsumo.NuPrdcIngresoReal.ToString() &&
                    row.GetCell("NU_PRDC_INGRESO").Value == detalleInsumo.NuPrdcIngreso &&
                    row.GetCell("CD_PRODUTO").Value == detalleInsumo.Producto &&
                    row.GetCell("CD_EMPRESA").Value == detalleInsumo.Empresa.ToString() &&
                    row.GetCell("NU_IDENTIFICADOR").Value == detalleInsumo.Identificador &&
                    row.GetCell("FL_CONSUMIBLE").Value == detalleInsumo.Consumible)
                .Select(row => row.GetCell("ND_MOTIVO").Value)
                .FirstOrDefault();
        }

        public virtual Stock GetStock(List<Stock> stocks, IngresoProduccionDetalleReal detalleInsumo, string ubicacion)
        {
            return stocks.FirstOrDefault(s => s.Ubicacion == ubicacion
                && s.Empresa == detalleInsumo.Empresa
                && s.Producto == detalleInsumo.Producto
                && s.Faixa == 1
                && s.Identificador == detalleInsumo.Identificador);
        }

        public virtual List<IngresoProduccionDetalleReal> GetSelectedKeys(IUnitOfWork uow, GridMenuItemActionContext context, string idIngreso)
        {
            var selection = context.Selection.GetSelection(this.GridKeys);

            var stockInsumos = selection.Select(item => new IngresoProduccionDetalleReal
            {
                NuPrdcIngresoReal = long.Parse(item["NU_PRDC_INGRESO_REAL"]),
                NuPrdcIngreso = item["NU_PRDC_INGRESO"],
                Producto = item["CD_PRODUTO"],
                Empresa = int.Parse(item["CD_EMPRESA"]),
                Identificador = item["NU_IDENTIFICADOR"],
                Consumible = item["FL_CONSUMIBLE"],
            }).ToList();

            int empresa = stockInsumos.FirstOrDefault()?.Empresa.Value ?? -1;

            if (context.Selection.AllSelected)
            {
                var dbQuery = new StockInsumosProduccionQuery(idIngreso, empresa);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                var queryStockInsumo = dbQuery.GetStockInsumo().Where(s => s.QtReal > 0);
                stockInsumos = queryStockInsumo.Except(stockInsumos).ToList();
            }
            else
            {
                var dbQuery = new StockInsumosProduccionQuery(idIngreso, empresa);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                var queryStockInsumo = dbQuery.GetStockInsumo();

                stockInsumos = queryStockInsumo.Join(stockInsumos,
                    (qst) => new { qst.NuPrdcIngresoReal, qst.NuPrdcIngreso, qst.Producto, qst.Empresa, qst.Identificador, qst.Consumible },
                    (si) => new { si.NuPrdcIngresoReal, si.NuPrdcIngreso, si.Producto, si.Empresa, si.Identificador, si.Consumible },
                    (qst, si) => qst).ToList();
            }

            return stockInsumos;
        }

        public virtual void ConsumirCompleto(IUnitOfWork uow, ILogicaProduccion logicaProduccion, string idIngreso, IngresoProduccionDetalleReal keyDetalle, string endereco, string motivo)
        {
            var consumible = keyDetalle.Consumible == "S";
            if (consumible)
            {
                var detalleInsumo = logicaProduccion.ExisteIngresoReal(keyDetalle.Producto, keyDetalle.Identificador);

                if (detalleInsumo == null)
                {
                    detalleInsumo = new IngresoProduccionDetalleReal()
                    {
                        Empresa = logicaProduccion.GetEmpresa(),
                        Faixa = 1,
                        Producto = keyDetalle.Producto,
                        Identificador = keyDetalle.Identificador,
                        QtReal = keyDetalle.QtReal,
                        QtNotificado = 0,
                        NuPrdcIngreso = idIngreso,
                        NuOrden = uow.IngresoProduccionRepository.GetNextValueNuOrdenDetalleReal(idIngreso),
                    };

                    logicaProduccion.AddInsumoProduccion(detalleInsumo);
                }
                else
                {
                    detalleInsumo.QtReal = (detalleInsumo.QtReal ?? 0) + (keyDetalle.QtReal ?? 0);
                }

                keyDetalle.NuPrdcIngresoReal = detalleInsumo.NuPrdcIngresoReal;

                uow.SaveChanges();
            }

            logicaProduccion.ConsumirInsumoCompleto(keyDetalle.NuPrdcIngresoReal, endereco, keyDetalle.QtReal ?? 0, out DateTime? vencimiento, consumible);

            var insumoConsumido = new IngresoProduccionDetalle()
            {
                Empresa = logicaProduccion.GetEmpresa(),
                Faixa = 1,
                Producto = keyDetalle.Producto,
                Identificador = keyDetalle.Identificador,
                Cantidad = keyDetalle.QtReal ?? 0,
                NuPrdcIngreso = idIngreso,
                FechaAlta = DateTime.Now,
                NuTransaccion = uow.GetTransactionNumber(),
                Ubicacion = endereco,
                Vencimiento = vencimiento,
                Motivo = motivo,
                FlPendienteNotificar = "S",
                NuPrdcIngresoReal = keyDetalle.NuPrdcIngresoReal,
            };

            uow.IngresoProduccionRepository.AddMovimientoIngreso(insumoConsumido);
        }

        public virtual string ProcesarBajaStock(IUnitOfWork uow, List<Stock> stocks, string nuIngresoProduccion, IngresoProduccionDetalleReal keyDetalle, string ubicacionProduccion)
        {
            var cantidadConsumir = keyDetalle.QtReal ?? 0;

            var stock = stocks.FirstOrDefault(s => s.Ubicacion == ubicacionProduccion
                && s.Empresa == keyDetalle.Empresa
                && s.Producto == keyDetalle.Producto
                && s.Faixa == 1
                && s.Identificador == keyDetalle.Identificador);

            if (stock == null || stock.Cantidad < cantidadConsumir || cantidadConsumir <= 0)
                throw new ValidationFailedException("PRD113_grid1_Error_InsumoSinSaldo");

            if (!uow.IngresoProduccionRepository.StockInsumoConsumible(nuIngresoProduccion, stock.Producto, stock.Empresa, stock.Faixa, stock.Identificador))
                throw new ValidationFailedException("PRD113_grid1_Error_InsumoSinSaldo");

            var nuevoSaldo = (stock.Cantidad - cantidadConsumir);
            stock.Cantidad = nuevoSaldo < 0 ? 0 : nuevoSaldo;
            stock.NumeroTransaccion = uow.GetTransactionNumber();
            stock.FechaModificacion = DateTime.Now;

            uow.StockRepository.UpdateStock(stock);
            uow.SaveChanges();

            var ajuste = new AjusteStock
            {
                NuAjusteStock = uow.AjusteRepository.GetNextNuAjuste(),
                Ubicacion = stock.Ubicacion,
                Empresa = stock.Empresa,
                Producto = stock.Producto,
                Faixa = stock.Faixa,
                Identificador = stock.Identificador,
                QtMovimiento = (-1) * cantidadConsumir,
                FechaVencimiento = stock.Vencimiento,
                FechaRealizado = DateTime.Now,
                TipoAjuste = TipoAjusteDb.Stock,
                CdMotivoAjuste = MotivoAjusteDb.Produccion,
                DescMotivo = $"Consumo para el Ingreso Nro: {nuIngresoProduccion}",
                NuTransaccion = uow.GetTransactionNumber(),
                Predio = stock.Predio,
                IdAreaAveria = "N",
                FechaMotivo = DateTime.Now,
                Funcionario = _identity.UserId,
                Aplicacion = _identity.Application,
                Metadata = nuIngresoProduccion,
            };

            uow.AjusteRepository.Add(ajuste);
            uow.SaveChanges();

            return ajuste.NuAjusteStock.ToString();
        }

        #endregion
    }
}
