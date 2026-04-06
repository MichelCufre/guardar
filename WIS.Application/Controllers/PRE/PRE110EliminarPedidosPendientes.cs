using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRE
{
    public class PRE110EliminarPedidosPendientes : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly ILogger<PRE110EliminarPedidosPendientes> _logger;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITrackingService _trackingService;
        protected readonly ITaskQueueService _taskQueue;
          
        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE110EliminarPedidosPendientes(
            IIdentityService identity,
            ILogger<PRE110EliminarPedidosPendientes> logger,
            IUnitOfWorkFactory uowFactory,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ITrackingService trackingService,
            ITaskQueueService taskQueue)
        {
            this.GridKeys = new List<string>
            {
                "NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA", "CD_PRODUTO", "CD_FAIXA", "NU_IDENTIFICADOR", "ID_ESPECIFICA_IDENTIFICADOR"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("DT_ADDROW", SortDirection.Descending),
            };

            this._logger = logger;
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._trackingService = trackingService;
            _taskQueue = taskQueue;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = true;
            context.IsAddEnabled = false;

            grid.MenuItems = new List<IGridItem>
            {
                new GridButton("btnEliminar", "PRE110_grid1_btn_eliminarLineas")
            };

            context.AddLink("CD_EMPRESA", "registro/REG100", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            context.AddLink("CD_PRODUTO", "registro/REG009", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_EMPRESA", "empresa"), new GridColumnLinkMapping("CD_PRODUTO", "producto") });

            context.AddLink("CD_AGENTE", "registro/REG220", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_CLIENTE", "cliente"), new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new EliminarPedidosPendiente();
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> { "AUXQT_ANULADO", "AUXDS_MOTIVO" });

            return grid;
        }
        
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new EliminarPedidosPendiente();
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }
        
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new EliminarPedidosPendiente();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            return context;
        }
        
        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber("PRE110 - Anulación Detalle de Pedido");
            uow.BeginTransaction();
            try
            {
                var keys = new List<string>();
                var cache = new List<Pedido>();
                var cacheAgente = new List<Agente>();
                var keysProduccion = new List<string>();

                foreach (var detPedido in GetSelectedLineasPedidos(uow, context))
                {
                    var pedidoId = detPedido[0];
                    var empresa = int.Parse(detPedido[2]);
                    var cliente = detPedido[1];

                    var pedido = cache.FirstOrDefault(d => d.Id == pedidoId && d.Empresa == empresa && d.Cliente == cliente);
                    var agente = cacheAgente.FirstOrDefault(d => d.CodigoInterno == cliente && d.Empresa == empresa);

                    if (pedido == null)
                    {
                        pedido = uow.PedidoRepository.GetPedido(empresa, cliente, pedidoId);
                        cache.Add(pedido);
                    }

                    if (agente == null)
                    {
                        agente = uow.AgenteRepository.GetAgente(empresa, cliente);
                        cacheAgente.Add(agente);
                    }

                    var pedidoAnulado = AnularDetallePedidoTotal(uow, pedido, detPedido[3], detPedido[5], decimal.Parse(detPedido[4], _identity.GetFormatProvider()));

                    uow.SaveChanges();

                    _trackingService.CerrarPedido(uow, pedido, agente, true);

                    AnulacionDePreparaciones.FinalizarProduccion(uow, pedido, out bool isProduccionFinalizada);

                    if (isProduccionFinalizada && !keysProduccion.Any(x => x == pedido.IngresoProduccion))
                        keysProduccion.Add(pedido.IngresoProduccion);

                    if (pedidoAnulado.InterfazEjecucion == -1)
                    {
                        var keyPedido = $"{pedido.Id}#{pedido.Cliente}#{pedido.Empresa}";
                        if (!keys.Contains(keyPedido))
                            keys.Add(keyPedido);
                    }
                }

                uow.Commit();
                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");


                if (_taskQueue.IsEnabled() && keysProduccion.Count() > 0)
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.ConfirmacionProduccion, keysProduccion);

                if (_taskQueue.IsEnabled() && keys.Any())
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.PedidosAnulados, keys);
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }
            return context;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("PRE110 - Anulación Detalle de Pedido");
            uow.BeginTransaction();

            try
            {

                var keys = new List<string>();
                var keysProduccion = new List<string>();

                if (grid.Rows.Any())
                {
                    var cache = new List<Pedido>();
                    var cacheAgente = new List<Agente>();

                    foreach (var row in grid.Rows)
                    {
                        var pedidoId = row.GetCell("NU_PEDIDO").Value;
                        var empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                        var cliente = row.GetCell("CD_CLIENTE").Value;

                        var pedido = cache.FirstOrDefault(d => d.Id == pedidoId && d.Empresa == empresa && d.Cliente == cliente);
                        var agente = cacheAgente.FirstOrDefault(d => d.CodigoInterno == cliente && d.Empresa == empresa);

                        if (pedido == null)
                        {
                            pedido = uow.PedidoRepository.GetPedido(empresa, cliente, pedidoId);
                            cache.Add(pedido);
                        }

                        if (agente == null)
                        {
                            agente = uow.AgenteRepository.GetAgente(empresa, cliente);
                            cacheAgente.Add(agente);
                        }

                        PedidoAnulado pedidoAnulado = null;
                        if (row.IsDeleted)
                        {
                            var producto = row.GetCell("CD_PRODUTO").Value;
                            var identificador = row.GetCell("NU_IDENTIFICADOR").Value;
                            var faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value, _identity.GetFormatProvider());

                            pedidoAnulado = AnularDetallePedidoTotal(uow, pedido, producto, identificador, faixa);
                        }
                        else if (row.IsModified)
                            pedidoAnulado = AnularDetallePedidoParcial(uow, pedido, row);

                        uow.SaveChanges();

                        _trackingService.CerrarPedido(uow, pedido, agente, true);

                        AnulacionDePreparaciones.FinalizarProduccion(uow, pedido, out bool isProduccionFinalizada);

                        if (isProduccionFinalizada && !keysProduccion.Any(x => x == pedido.IngresoProduccion))
                            keysProduccion.Add(pedido.IngresoProduccion);

                        if (pedidoAnulado.InterfazEjecucion == -1)
                        {
                            var keyPedido = $"{pedido.Id}#{pedido.Cliente}#{pedido.Empresa}";
                            if (!keys.Contains(keyPedido))
                                keys.Add(keyPedido);
                        }
                    }
                }

                uow.Commit();
                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");

                if (_taskQueue.IsEnabled() && keysProduccion.Count() > 0)
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.ConfirmacionProduccion, keysProduccion);

                if (_taskQueue.IsEnabled() && keys.Any())
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.PedidosAnulados, keys);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "PRE110GridCommit");
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
                throw;
            }
            return grid;
        }
        
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoEliminacionPedidosPendienteValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        #region Metodos Auxiliares

        public virtual PedidoAnulado AnularDetallePedidoTotal(IUnitOfWork uow, Pedido pedido, string producto, string identificador, decimal faixa)
        {
            var detalle = pedido.Lineas.FirstOrDefault(d => d.Producto == producto && d.Identificador == identificador && d.Faixa == faixa);

            var anulacionPedido = new AnulacionPedidoPendiente(uow, pedido, detalle, "Anulación total de Detalle de Pedido", this._identity.UserId, this._identity.Application);

            return anulacionPedido.Anular();
        }

        public virtual PedidoAnulado AnularDetallePedidoParcial(IUnitOfWork uow, Pedido pedido, GridRow row)
        {
            var producto = row.GetCell("CD_PRODUTO").Value;
            var identificador = row.GetCell("NU_IDENTIFICADOR").Value;
            var faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value, _identity.GetFormatProvider());
            var cantidadAnular = decimal.Parse(row.GetCell("AUXQT_ANULADO").Value, _identity.GetFormatProvider());
            var motivo = string.IsNullOrEmpty(row.GetCell("AUXDS_MOTIVO").Value) ? "Anulación parcial de Detalle de Pedido" : row.GetCell("AUXDS_MOTIVO").Value;

            var detalle = pedido.Lineas.FirstOrDefault(d => d.Producto == producto && d.Identificador == identificador && d.Faixa == faixa);

            var anulacionPedido = new AnulacionPedidoPendiente(uow, pedido, detalle, motivo, this._identity.UserId, this._identity.Application);

            return anulacionPedido.Anular(cantidadAnular);
        }

        public virtual List<string[]> GetSelectedLineasPedidos(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var dbQuery = new EliminarPedidosPendiente();

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys, _identity.GetFormatProvider());

            return dbQuery.GetSelectedKeys(context.Selection.Keys, _identity.GetFormatProvider());
        }

        #endregion
    }
}
