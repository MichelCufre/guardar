using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common;
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
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRE
{
    public class PRE110AnulacionDetallePedidoAtributoPendientes : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly ILogger<PRE110AnulacionDetallePedidoAtributoPendientes> _logger;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITrackingService _trackingService;
        protected readonly ITaskQueueService _taskQueue;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE110AnulacionDetallePedidoAtributoPendientes(
            IIdentityService identity,
            ILogger<PRE110AnulacionDetallePedidoAtributoPendientes> logger,
            IUnitOfWorkFactory uowFactory,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ITrackingService trackingService,
            ITaskQueueService taskQueue)
        {
            this.GridKeys = new List<string>
            {"NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA", "CD_PRODUTO", "CD_FAIXA", "NU_IDENTIFICADOR", "ID_ESPECIFICA_IDENTIFICADOR","NU_DET_PED_SAI_ATRIB"};

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

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("BtnDetalles", "PRE110_grid1_btn_EspecificacionAtributos", "fas fa-list"),
            }));

            context.AddLink("CD_EMPRESA", "registro/REG100", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            context.AddLink("CD_PRODUTO", "registro/REG009", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_EMPRESA", "empresa"), new GridColumnLinkMapping("CD_PRODUTO", "producto") });

            context.AddLink("CD_AGENTE", "registro/REG220", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_CLIENTE", "cliente"), new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            return this.GridFetchRows(grid, context.FetchContext);
        }
        
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new EliminarPedidosAtributosPendientesQuery();
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            grid.SetEditableCells(new List<string> { "AUXQT_ANULADO", "AUXDS_MOTIVO" });

            return grid;
        }
        
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new EliminarPedidosAtributosPendientesQuery();
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }
        
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new EliminarPedidosAtributosPendientesQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            context.Parameters.Add(new ComponentParameter("NU_PEDIDO", context.Row.GetCell("NU_PEDIDO").Value));
            context.Parameters.Add(new ComponentParameter("CD_EMPRESA", context.Row.GetCell("CD_EMPRESA").Value));
            context.Parameters.Add(new ComponentParameter("CD_CLIENTE", context.Row.GetCell("CD_CLIENTE").Value));
            context.Parameters.Add(new ComponentParameter("NU_IDENTIFICADOR", context.Row.GetCell("NU_IDENTIFICADOR").Value));
            context.Parameters.Add(new ComponentParameter("CD_PRODUTO", context.Row.GetCell("CD_PRODUTO").Value));
            context.Parameters.Add(new ComponentParameter("ID_ESPECIFICA_IDENTIFICADOR", context.Row.GetCell("ID_ESPECIFICA_IDENTIFICADOR").Value));
            context.Parameters.Add(new ComponentParameter("CD_FAIXA", context.Row.GetCell("CD_FAIXA").Value));
            context.Parameters.Add(new ComponentParameter("NU_DET_PED_SAI_ATRIB", context.Row.GetCell("NU_DET_PED_SAI_ATRIB").Value));
            return context;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("PRE110 - Anulación Detalle de Pedido Atributo");
            uow.BeginTransaction();

            var keysProduccion = new List<string>();

            try
            {
                var keys = new List<string>();
                var cache = new List<Pedido>();
                var cacheAgente = new List<Agente>();

                foreach (var detPedido in GetSelectedLineasPedidos(uow, context))
                {
                    var pedidoId = detPedido[0];
                    var cliente = detPedido[1];
                    var empresa = int.Parse(detPedido[2]);
                    var producto = detPedido[3];
                    var faixa = decimal.Parse(detPedido[4], _identity.GetFormatProvider());
                    var identificador = detPedido[5];
                    var idEspecificaIdentificador = detPedido[6];
                    var idConfiguracion = long.Parse(detPedido[7]);

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

                    var detallePedidoAtributo = uow.ManejoLpnRepository.GetDetallePedidoAtributo(pedidoId, cliente, empresa, producto, faixa, identificador, idEspecificaIdentificador, idConfiguracion);
                    var pedidoAnulado = AnularDetallePedidoTotal(uow, pedido, detallePedidoAtributo);

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
            uow.CreateTransactionNumber("PRE110 - Anulación Detalle de Pedido Atributo");
            uow.BeginTransaction();

            var keysProduccion = new List<string>();

            try
            {
                var keys = new List<string>();
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

                        var producto = row.GetCell("CD_PRODUTO").Value;
                        var identificador = row.GetCell("NU_IDENTIFICADOR").Value;
                        var faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value, _identity.GetFormatProvider());
                        var idEspecificaIdentificador = row.GetCell("ID_ESPECIFICA_IDENTIFICADOR").Value;
                        var idConfiguracion = long.Parse(row.GetCell("NU_DET_PED_SAI_ATRIB").Value);

                        var detallePedidoAtributo = uow.ManejoLpnRepository.GetDetallePedidoAtributo(pedidoId, cliente, empresa, producto, faixa, identificador, idEspecificaIdentificador, idConfiguracion);

                        PedidoAnulado pedidoAnulado = null;
                        if (row.IsDeleted)
                            pedidoAnulado = AnularDetallePedidoTotal(uow, pedido, detallePedidoAtributo);
                        else if (row.IsModified)
                        {
                            var cantidadAnular = decimal.Parse(row.GetCell("AUXQT_ANULADO").Value, _identity.GetFormatProvider());
                            var motivo = string.IsNullOrEmpty(row.GetCell("AUXDS_MOTIVO").Value) ? "Anulación parcial de Detalle de Pedido Atributo" : row.GetCell("AUXDS_MOTIVO").Value;

                            pedidoAnulado = AnularDetallePedidoParcial(uow, pedido, detallePedidoAtributo, motivo, cantidadAnular);
                        }

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

            return this._gridValidationService.Validate(new MantenimientoEliminacionPedidosPendienteAtributoValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        #region Auxs

        public virtual PedidoAnulado AnularDetallePedidoTotal(IUnitOfWork uow, Pedido pedido, DetallePedidoAtributo detalle)
        {
            var anulacionPedido = new AnulacionPedidoPendienteAtributo(uow, pedido, detalle, "Anulación total de Detalle de Pedido de Atributo", this._identity.UserId, this._identity.Application);

            return anulacionPedido.Anular();
        }

        public virtual PedidoAnulado AnularDetallePedidoParcial(IUnitOfWork uow, Pedido pedido, DetallePedidoAtributo detalle, string motivo, decimal cantidadAnular)
        {
            var anulacionPedido = new AnulacionPedidoPendienteAtributo(uow, pedido, detalle, motivo, this._identity.UserId, this._identity.Application);

            return anulacionPedido.Anular(cantidadAnular);
        }

        public virtual List<string[]> GetSelectedLineasPedidos(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var dbQuery = new EliminarPedidosAtributosPendientesQuery();

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys, _identity.GetFormatProvider());

            return dbQuery.GetSelectedKeys(context.Selection.Keys, _identity.GetFormatProvider());
        }

        #endregion
    }
}
