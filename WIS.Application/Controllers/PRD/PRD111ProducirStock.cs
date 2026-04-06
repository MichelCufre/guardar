using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules.Produccion;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Domain.General;
using WIS.Domain.Inventario;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
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
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.PRD
{
    public class PRD111ProducirStock : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly ITaskQueueService _taskQueue;

        protected readonly ILogger<PRD111ProducirStock> _logger;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRD111ProducirStock(IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IGridValidationService gridValidationService,
            IFilterInterpreter filterInterpreter,
            ITrafficOfficerService concurrencyControl,
            ITaskQueueService taskQueue,
            ILogger<PRD111ProducirStock> logger)
        {
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

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._gridValidationService = gridValidationService;
            this._filterInterpreter = filterInterpreter;
            this._concurrencyControl = concurrencyControl;
            this._taskQueue = taskQueue;
            this._logger = logger;
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
            context.IsAddEnabled = true;
            context.IsRemoveEnabled = false;
            context.IsCommitEnabled = true;
            context.IsEditingEnabled = true;
            context.IsRollbackEnabled = true;

            var codigoEspacio = context.GetParameter("codigoEspacio");

            var column = new GridColumnSelect("CD_ENDERECO", OptionSelectUbicaciones(codigoEspacio));
            if (column.Options.Count == 1)
                column.DefaultValue = column.Options.FirstOrDefault().Value;

            grid.AddOrUpdateColumn(column);

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var codigoEspacio = context.GetParameter("codigoEspacio");

            var dbQuery = new StockEspacioDeProduccionQuery(codigoEspacio);
            uow.HandleQuery(dbQuery);

            grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, DefaultSort, this.GridKeys);

            grid.SetInsertableColumns(new List<string> { "CD_ENDERECO", "CD_EMPRESA", "CD_PRODUTO", "NU_IDENTIFICADOR", "QT_AJUSTAR", "DT_FABRICACAO" });

            foreach (var row in grid.Rows)
            {
                var codigoProducto = row.GetCell("CD_PRODUTO").Value;
                var empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                var producto = uow.ProductoRepository.GetProducto(empresa, codigoProducto);

                if (producto.IsIdentifiedBySerie())
                    row.SetEditableCells(new List<string>());
                else
                    row.SetEditableCells(new List<string> { "QT_AJUSTAR" });
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

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var keysAjustes = new List<string>();
            var transactionTO = this._concurrencyControl.CreateTransaccion();

            uow.BeginTransaction();

            try
            {
                if (grid.Rows.Any())
                {
                    var keys = new List<Stock>();
                    uow.CreateTransactionNumber("GridCommit - Producir Stock");

                    foreach (var row in grid.Rows)
                    {
                        DateTime? vencimiento = null;
                        if (!string.IsNullOrEmpty(row.GetCell("DT_FABRICACAO").Value) && DateTime.TryParse(row.GetCell("DT_FABRICACAO").Value, this._identity.GetFormatProvider(), DateTimeStyles.None, out DateTime fecha))
                        {
                            vencimiento = fecha.Date;
                        }

                        var faixa = string.IsNullOrEmpty(row.GetCell("CD_FAIXA").Value) ? 1 :
                                    decimal.Parse(row.GetCell("CD_FAIXA").Value, _identity.GetFormatProvider());

                        keys.Add(new Stock()
                        {
                            Ubicacion = row.GetCell("CD_ENDERECO").Value,
                            Empresa = int.Parse(row.GetCell("CD_EMPRESA").Value),
                            Producto = row.GetCell("CD_PRODUTO").Value,
                            Faixa = faixa,
                            Identificador = row.GetCell("NU_IDENTIFICADOR").Value,
                            CantidadAjustar = decimal.Parse(row.GetCell("QT_AJUSTAR").Value, _identity.GetFormatProvider()),
                            Vencimiento = vencimiento
                        });
                    }

                    keys = keys.GroupBy(s => new { s.Ubicacion, s.Empresa, s.Producto, s.Faixa, s.Identificador })
                        .Select(s => new Stock()
                        {
                            Ubicacion = s.Key.Ubicacion,
                            Empresa = s.Key.Empresa,
                            Producto = s.Key.Producto,
                            Faixa = s.Key.Faixa,
                            Identificador = s.Key.Identificador,
                            CantidadAjustar = s.Sum(s => s.CantidadAjustar),
                            Vencimiento = s.Min(s => s.Vencimiento),
                            NumeroTransaccion = uow.GetTransactionNumber()
                        }).ToList();

                    var idsBloqueos = keys.Select(s => s.GetLockId(_identity.GetFormatProvider())).ToList();

                    var listLock = this._concurrencyControl.GetLockList("T_STOCK", idsBloqueos, transactionTO);

                    if (listLock.Count > 0)
                    {
                        var keyBloqueo = listLock.FirstOrDefault().Id_Bloqueo.Split("#");
                        throw new EntityLockedException("PRD111ConsumirStock_grid1_msg_RegistrosBloqueados", [keyBloqueo[0], keyBloqueo[1], keyBloqueo[2], keyBloqueo[4],]);
                    }

                    this._concurrencyControl.AddLockList("T_STOCK", idsBloqueos, transactionTO, true);

                    var codigoEspacio = context.GetParameter("codigoEspacio");

                    keysAjustes = ProcesarAltaStock(uow, codigoEspacio, keys);

                    uow.SaveChanges();
                    uow.Commit();

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
                throw;
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

            return this._gridValidationService.Validate(new PRD111ProducirStockGridValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext query)
        {
            switch (query.ColumnId)
            {
                case "CD_EMPRESA":
                    return this.SearchEmpresa(grid, row, query);
                case "CD_PRODUTO":
                    return this.SearchProduto(grid, row, query);
            }

            return new List<SelectOption>();
        }

        #region Metodos Auxiliares
        public virtual List<string> ProcesarAltaStock(IUnitOfWork uow, string codigoEspacio, List<Stock> keys)
        {
            var keyAjustes = new List<string>();

            var stocksExistentes = uow.StockRepository.GetStock(keys);

            foreach (var keyStock in keys)
            {
                var producto = uow.ProductoRepository.GetProducto(keyStock.Empresa, keyStock.Producto);

                var stock = stocksExistentes
                    .FirstOrDefault(s => s.Ubicacion == keyStock.Ubicacion
                        && s.Empresa == keyStock.Empresa
                        && s.Producto == keyStock.Producto
                        && s.Faixa == keyStock.Faixa
                        && s.Identificador == keyStock.Identificador);

                if (stock != null)
                {
                    DateTime? vencimiento = stock.Vencimiento;
                    if (producto.IsFefo())
                        vencimiento = InventarioLogic.ResolverVencimiento(stock.Vencimiento, keyStock.Vencimiento);

                    stock.Cantidad = (stock.Cantidad ?? 0) + (keyStock.CantidadAjustar ?? 0);
                    stock.FechaModificacion = DateTime.Now;
                    stock.NumeroTransaccion = uow.GetTransactionNumber();
                    stock.Vencimiento = vencimiento;

                    uow.StockRepository.UpdateStock(stock);
                }
                else
                {
                    DateTime? vencimiento = null;

                    if (producto.IsFifo())
                        vencimiento = DateTime.Now;
                    else if (producto.IsFefo())
                        vencimiento = keyStock.Vencimiento;

                    if (producto.IsIdentifiedBySerie())
                        keyStock.CantidadAjustar = 1;

                    var espacioProduccion = uow.EspacioProduccionRepository.GetEspacioProduccion(codigoEspacio);
                    stock = new Stock()
                    {
                        Ubicacion = keyStock.Ubicacion,
                        Empresa = keyStock.Empresa,
                        Producto = keyStock.Producto,
                        Faixa = keyStock.Faixa,
                        Identificador = producto.IsIdentifiedByProducto() ? ManejoIdentificadorDb.IdentificadorProducto : keyStock.Identificador,
                        Cantidad = keyStock.CantidadAjustar,
                        ReservaSalida = 0,
                        CantidadTransitoEntrada = 0,
                        Vencimiento = vencimiento,
                        Averia = "N",
                        Inventario = "R",
                        ControlCalidad = EstadoControlCalidad.Controlado,
                        FechaModificacion = DateTime.Now,
                        NumeroTransaccion = uow.GetTransactionNumber(),
                        Predio = espacioProduccion.Predio,
                    };

                    uow.StockRepository.AddStock(stock);
                }

                uow.SaveChanges();

                var ajuste = new AjusteStock
                {
                    NuAjusteStock = uow.AjusteRepository.GetNextNuAjuste(),
                    Ubicacion = stock.Ubicacion,
                    Empresa = stock.Empresa,
                    Producto = stock.Producto,
                    Faixa = stock.Faixa,
                    Identificador = stock.Identificador,
                    QtMovimiento = keyStock.CantidadAjustar,
                    FechaVencimiento = stock.Vencimiento,
                    FechaRealizado = DateTime.Now,
                    TipoAjuste = TipoAjusteDb.Stock,
                    CdMotivoAjuste = MotivoAjusteDb.Produccion,
                    DescMotivo = $"Producir stock de Espacio de Producción {codigoEspacio}",
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

        public virtual List<SelectOption> SearchEmpresa(Grid grid, GridRow row, GridSelectSearchContext query)
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var empresasAsignadasUsuario = uow.EmpresaRepository.GetByNombreOrCodePartialForUsuario(query.SearchValue, this._identity.UserId);

            foreach (var emp in empresasAsignadasUsuario)
            {
                opciones.Add(new SelectOption(emp.Id.ToString(), $"{emp.Id} - {emp.Nombre}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> SearchProduto(Grid grid, GridRow row, GridSelectSearchContext query)
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var productos = new List<Producto>();

            if (!string.IsNullOrEmpty(row.GetCell("CD_EMPRESA").Value))
                productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(int.Parse(row.GetCell("CD_EMPRESA").Value), query.SearchValue);
            else
                row.GetCell("CD_EMPRESA").SetError(new ComponentError("General_Sec0_Error_Error25", new List<string>()));

            foreach (var prod in productos)
            {
                opciones.Add(new SelectOption(prod.Codigo, $"{prod.Codigo} - {prod.Descripcion}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> OptionSelectUbicaciones(string codigoEspacio)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var ubicaciones = uow.EspacioProduccionRepository.GetUbicacionesDeProduccion(codigoEspacio);

            foreach (var ubicacion in ubicaciones)
            {
                opciones.Add(new SelectOption(ubicacion, ubicacion));
            }
            return opciones;
        }
        #endregion

    }
}
