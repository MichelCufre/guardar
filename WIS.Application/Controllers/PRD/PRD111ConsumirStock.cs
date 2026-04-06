using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules.Produccion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.PRD
{
    public class PRD111ConsumirStock : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly ITaskQueueService _taskQueue;

        protected readonly ILogger<PRD111ConsumirStock> _logger;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRD111ConsumirStock(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IGridValidationService gridValidationService,
            IFilterInterpreter filterInterpreter,
            ITrafficOfficerService concurrencyControl,
            ITaskQueueService taskQueue,
            ILogger<PRD111ConsumirStock> logger)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._gridValidationService = gridValidationService;
            this._filterInterpreter = filterInterpreter;
            this._concurrencyControl = concurrencyControl;
            this._taskQueue = taskQueue;
            this._logger = logger;

            this.GridKeys = new List<string>
            {
                "CD_ENDERECO", "CD_EMPRESA", "CD_PRODUTO",  "CD_FAIXA", "NU_IDENTIFICADOR"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_ENDERECO", SortDirection.Ascending),
                new SortCommand("CD_EMPRESA", SortDirection.Ascending),
                new SortCommand("CD_PRODUTO", SortDirection.Ascending),
                new SortCommand("NU_IDENTIFICADOR", SortDirection.Ascending),
                new SortCommand("QT_ESTOQUE", SortDirection.Ascending)
            };
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var codigoEspacio = context.GetParameter("codigoEspacio");

            if (!string.IsNullOrEmpty(codigoEspacio))
            {
                var espacioProduccion = uow.EspacioProduccionRepository.GetEspacioProduccion(codigoEspacio);

                if (espacioProduccion == null)
                    throw new ValidationFailedException("PRD111_msg_Error_EspacioProduccionNoExiste", new string[] { codigoEspacio });

                form.GetField("codigo").Value = espacioProduccion.Id;
                form.GetField("descripcion").Value = espacioProduccion.Descripcion;

                var tipo = uow.DominioRepository.GetDominio(espacioProduccion.Tipo);
                form.GetField("tipo").Value = tipo?.Descripcion;
            }

            return form;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsAddEnabled = false;
            context.IsRemoveEnabled = false;
            context.IsCommitEnabled = true;
            context.IsEditingEnabled = true;
            context.IsRollbackEnabled = true;

            grid.MenuItems.Add(new GridButton("btnConsumirCompleto", "PRD111ConsumirStock_grid1_btn_ConsumirCompleto", string.Empty, new ConfirmMessage("PRD111ConsumirStock_grid1_msg_ConfirmacionConsumirCompleto")));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var codigoEspacio = context.GetParameter("codigoEspacio");

            var dbQuery = new StockEspacioDeProduccionQuery(codigoEspacio);
            uow.HandleQuery(dbQuery);

            grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> { });

            foreach (var row in grid.Rows)
            {
                var cantidadDisponible = Convert.ToDecimal(row.GetCell("QT_DISPONIBLE").Value, _identity.GetFormatProvider());
                var cantidadReservada = Convert.ToDecimal(row.GetCell("QT_RESERVA_SAIDA").Value, _identity.GetFormatProvider());

                if (cantidadDisponible > 0)
                    row.SetEditableCells(new List<string>() { "QT_AJUSTAR" });

                if (cantidadReservada > 0)
                    row.DisabledSelected = true;
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var codigoEspacio = context.GetParameter("codigoEspacio");

            var dbQuery = new StockEspacioDeProduccionQuery(codigoEspacio);
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var codigoEspacio = context.GetParameter("codigoEspacio");

            var dbQuery = new StockEspacioDeProduccionQuery(codigoEspacio);
            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var transactionTO = this._concurrencyControl.CreateTransaccion();
            var keysAjustes = new List<string>();

            uow.BeginTransaction();

            try
            {
                uow.CreateTransactionNumber("GridMenuItemAction - Consumir Stock");
                var codigoEspacio = context.GetParameter("codigoEspacio");

                if (context.ButtonId == "btnConsumirCompleto")
                {
                    var keys = GetSelectedKeys(uow, context);

                    if (keys.Count == 0)
                        throw new MissingParameterException("PRD111ConsumirStock_msg_Error_ConsumoSinSeleccion");

                    var idsBloqueos = keys.Select(s => s.GetLockId(_identity.GetFormatProvider())).ToList();

                    var listLock = this._concurrencyControl.GetLockList("T_STOCK", idsBloqueos, transactionTO);

                    if (listLock.Count > 0)
                    {
                        var keyBloqueo = listLock.FirstOrDefault().Id_Bloqueo.Split("#");
                        throw new EntityLockedException("PRD111ConsumirStock_grid1_msg_RegistrosBloqueados", [keyBloqueo[0], keyBloqueo[1], keyBloqueo[2], keyBloqueo[4],]);
                    }

                    this._concurrencyControl.AddLockList("T_STOCK", idsBloqueos, transactionTO, true);

                    keysAjustes = ProcesarBajaStock(uow, codigoEspacio, keys, consumoTotal: true, out List<Stock> registrosSinProcesar);

                    uow.SaveChanges();
                    uow.Commit();

                    if (registrosSinProcesar.Any())
                    {
                        var arg = registrosSinProcesar.FirstOrDefault();
                        context.AddInfoNotification("PRD111ConsumirStock_grid1_msg_AjustesSinProcesar", new List<string>() { arg.Empresa.ToString(), arg.Producto, arg.Identificador });
                    }

                    context.AddSuccessNotification("General_Db_Success_Update");
                }
            }
            catch (EntityLockedException ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (ValidationFailedException ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (MissingParameterException ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                uow.Rollback();
                this._logger.LogError(ex, "GridMenuItemAction");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }
            finally
            {
                this._concurrencyControl.DeleteTransaccion(transactionTO);

                if (_taskQueue.IsEnabled() && keysAjustes.Any())
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.AjustesDeStock, keysAjustes);
            }

            return context;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var transactionTO = this._concurrencyControl.CreateTransaccion();
            var keysAjustes = new List<string>();

            uow.BeginTransaction();

            try
            {
                if (grid.Rows.Any())
                {
                    var keys = new List<Stock>();
                    uow.CreateTransactionNumber("GridCommit - Consumir Stock");

                    foreach (var row in grid.Rows)
                    {
                        keys.Add(new Stock()
                        {
                            Ubicacion = row.GetCell("CD_ENDERECO").Value,
                            Empresa = int.Parse(row.GetCell("CD_EMPRESA").Value),
                            Producto = row.GetCell("CD_PRODUTO").Value,
                            Faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value, _identity.GetFormatProvider()),
                            Identificador = row.GetCell("NU_IDENTIFICADOR").Value,
                            CantidadAjustar = decimal.Parse(row.GetCell("QT_AJUSTAR").Value, _identity.GetFormatProvider()),
                            NumeroTransaccion = uow.GetTransactionNumber()
                        });
                    }

                    var idsBloqueos = keys.Select(s => s.GetLockId(_identity.GetFormatProvider())).ToList();

                    var listLock = this._concurrencyControl.GetLockList("T_STOCK", idsBloqueos, transactionTO);

                    if (listLock.Count > 0)
                    {
                        var keyBloqueo = listLock.FirstOrDefault().Id_Bloqueo.Split("#");
                        throw new EntityLockedException("PRD111ConsumirStock_grid1_msg_RegistrosBloqueados", [keyBloqueo[0], keyBloqueo[1], keyBloqueo[2], keyBloqueo[4],]);
                    }

                    this._concurrencyControl.AddLockList("T_STOCK", idsBloqueos, transactionTO, true);

                    var codigoEspacio = context.GetParameter("codigoEspacio");

                    keysAjustes = ProcesarBajaStock(uow, codigoEspacio, keys, consumoTotal: false, out List<Stock> registrosSinProcesar);

                    uow.SaveChanges();
                    uow.Commit();

                    if (registrosSinProcesar.Any())
                    {
                        var arg = registrosSinProcesar.FirstOrDefault();
                        context.AddInfoNotification("PRD111ConsumirStock_grid1_msg_AjustesSinProcesar", new List<string>() { arg.Empresa.ToString(), arg.Producto, arg.Identificador });
                    }

                    context.AddSuccessNotification("General_Db_Success_Update");
                }
            }
            catch (EntityLockedException ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (ValidationFailedException ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                uow.Rollback();
                this._logger.LogError(ex, "GridCommit");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }
            finally
            {
                this._concurrencyControl.DeleteTransaccion(transactionTO);

                if (_taskQueue.IsEnabled() && keysAjustes.Any())
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.AjustesDeStock, keysAjustes);
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new PRD111ConsumirStockGridValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        #region Metodos Auxiliares

        public virtual List<Stock> GetSelectedKeys(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var codigoEspacio = context.GetParameter("codigoEspacio");
            var dbQuery = new StockEspacioDeProduccionQuery(codigoEspacio);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys, _identity.GetFormatProvider(), uow.GetTransactionNumber());

            return dbQuery.GetSelectedKeys(context.Selection.Keys, _identity.GetFormatProvider(), uow.GetTransactionNumber());
        }

        public virtual List<string> ProcesarBajaStock(IUnitOfWork uow, string codigoEspacio, List<Stock> keys, bool consumoTotal, out List<Stock> registrosNoProcesados)
        {
            var keyAjustes = new List<string>();
            registrosNoProcesados = new List<Stock>();

            var stocks = uow.StockRepository.GetStock(keys);

            foreach (var stock in stocks)
            {
                if (consumoTotal)
                {
                    if (stock.ReservaSalida > 0)
                    {
                        registrosNoProcesados.Add(stock);
                        continue;
                    }
                    else
                        stock.CantidadAjustar = stock.Cantidad;
                }
                else
                {
                    if ((stock.Cantidad - stock.ReservaSalida) <= 0)
                    {
                        registrosNoProcesados.Add(stock);
                        continue;
                    }
                    else
                    {
                        stock.CantidadAjustar = keys.FirstOrDefault(s => s.Ubicacion == stock.Ubicacion
                            && s.Empresa == stock.Empresa
                            && s.Producto == stock.Producto
                            && s.Faixa == stock.Faixa
                            && s.Identificador == stock.Identificador)?.CantidadAjustar ?? 0;
                    }
                }

                var nuevoSaldo = (stock.Cantidad ?? 0) - (stock.CantidadAjustar ?? 0);

                if (nuevoSaldo < (stock.ReservaSalida ?? 0))
                {
                    registrosNoProcesados.Add(stock);
                    continue;
                }

                stock.Cantidad = nuevoSaldo;
                stock.FechaModificacion = DateTime.Now;
                stock.NumeroTransaccion = uow.GetTransactionNumber();

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
                    QtMovimiento = (-1) * stock.CantidadAjustar,
                    FechaVencimiento = stock.Vencimiento,
                    FechaRealizado = DateTime.Now,
                    TipoAjuste = TipoAjusteDb.Stock,
                    CdMotivoAjuste = MotivoAjusteDb.Produccion,
                    DescMotivo = $"Consumir stock de Espacio de Producción {codigoEspacio}",
                    NuTransaccion = uow.GetTransactionNumber(),
                    Predio = stock.Predio,
                    IdAreaAveria = "N",
                    FechaMotivo = DateTime.Now,
                    Funcionario = _identity.UserId,
                    Aplicacion = _identity.Application
                };

                uow.AjusteRepository.Add(ajuste);

                keyAjustes.Add(ajuste.NuAjusteStock.ToString());

                uow.SaveChanges();
            }

            return keyAjustes;
        }

        #endregion

    }
}
