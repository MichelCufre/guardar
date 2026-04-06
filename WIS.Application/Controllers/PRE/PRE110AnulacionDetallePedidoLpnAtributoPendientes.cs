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
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRE
{
    public class PRE110AnulacionDetallePedidoLpnAtributoPendientes : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly ILogger<PRE110AnulacionDetallePedidoLpnAtributoPendientes> _logger;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITrackingService _trackingService;
        protected readonly ITaskQueueService _taskQueue;
          
        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE110AnulacionDetallePedidoLpnAtributoPendientes(
            IIdentityService identity,
            ILogger<PRE110AnulacionDetallePedidoLpnAtributoPendientes> logger,
            IUnitOfWorkFactory uowFactory,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ITrackingService trackingService,
            ITaskQueueService taskQueue)
        {
            this.GridKeys = new List<string>
            {"NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA", "CD_PRODUTO", "CD_FAIXA", "NU_IDENTIFICADOR", "ID_ESPECIFICA_IDENTIFICADOR","TP_LPN_TIPO", "ID_LPN_EXTERNO","NU_DET_PED_SAI_ATRIB"};

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

            EliminarPedidosLpnAtributosPendientesQuery dbQuery;

            if (context.Parameters.Count > 0)
            {
                var pedido = context.Parameters.FirstOrDefault(x => x.Id == "Pedido").Value.ToString();
                var empresa = int.Parse(context.Parameters.FirstOrDefault(x => x.Id == "Empresa").Value.ToString());
                var cliente = context.Parameters.FirstOrDefault(x => x.Id == "Cliente").Value.ToString();
                var idEspecificaIdentificador = context.Parameters.FirstOrDefault(x => x.Id == "IdEspecificaIdentificador").Value.ToString();
                var idLpnExteno = context.Parameters.FirstOrDefault(x => x.Id == "IdLpnExteno").Value.ToString();
                var lpnTipo = context.Parameters.FirstOrDefault(x => x.Id == "LpnTipo").Value.ToString();
                var producto = context.Parameters.FirstOrDefault(x => x.Id == "Producto").Value.ToString();
                var identificador = context.Parameters.FirstOrDefault(x => x.Id == "Identificador").Value.ToString();
                var faixa = decimal.Parse(context.Parameters.FirstOrDefault(x => x.Id == "Faixa").Value.ToString(), _identity.GetFormatProvider());

                dbQuery = new EliminarPedidosLpnAtributosPendientesQuery(pedido, empresa, cliente, idEspecificaIdentificador, idLpnExteno, lpnTipo, producto, identificador, faixa);
                uow.HandleQuery(dbQuery);
            }
            else
            {
                dbQuery = new EliminarPedidosLpnAtributosPendientesQuery();
                uow.HandleQuery(dbQuery);
            }

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            grid.SetEditableCells(new List<string> { "AUXQT_ANULADO", "AUXDS_MOTIVO" });

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            if (context.Parameters.Count > 0)
            {
                var pedido = context.Parameters.FirstOrDefault(x => x.Id == "Pedido").Value.ToString();
                var empresa = int.Parse(context.Parameters.FirstOrDefault(x => x.Id == "Empresa").Value.ToString());
                var cliente = context.Parameters.FirstOrDefault(x => x.Id == "Cliente").Value.ToString();
                var idEspecificaIdentificador = context.Parameters.FirstOrDefault(x => x.Id == "IdEspecificaIdentificador").Value.ToString();
                var idLpnExteno = context.Parameters.FirstOrDefault(x => x.Id == "IdLpnExteno").Value.ToString();
                var lpnTipo = context.Parameters.FirstOrDefault(x => x.Id == "LpnTipo").Value.ToString();
                var producto = context.Parameters.FirstOrDefault(x => x.Id == "Producto").Value.ToString();
                var identificador = context.Parameters.FirstOrDefault(x => x.Id == "Identificador").Value.ToString();
                var faixa = decimal.Parse(context.Parameters.FirstOrDefault(x => x.Id == "Faixa").Value.ToString(), _identity.GetFormatProvider());

                var dbQuery = new EliminarPedidosLpnAtributosPendientesQuery(pedido, empresa, cliente, idEspecificaIdentificador, idLpnExteno, lpnTipo, producto, identificador, faixa);

                uow.HandleQuery(dbQuery);
                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }
            else
            {
                var dbQuery = new EliminarPedidosLpnAtributosPendientesQuery();
                uow.HandleQuery(dbQuery);
                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.Parameters.Count > 0)
            {
                var pedido = context.Parameters.FirstOrDefault(x => x.Id == "Pedido").Value.ToString();
                var empresa = int.Parse(context.Parameters.FirstOrDefault(x => x.Id == "Empresa").Value.ToString());
                var cliente = context.Parameters.FirstOrDefault(x => x.Id == "Cliente").Value.ToString();
                var idEspecificaIdentificador = context.Parameters.FirstOrDefault(x => x.Id == "IdEspecificaIdentificador").Value.ToString();
                var idLpnExteno = context.Parameters.FirstOrDefault(x => x.Id == "IdLpnExteno").Value.ToString();
                var lpnTipo = context.Parameters.FirstOrDefault(x => x.Id == "LpnTipo").Value.ToString();
                var producto = context.Parameters.FirstOrDefault(x => x.Id == "Producto").Value.ToString();
                var identificador = context.Parameters.FirstOrDefault(x => x.Id == "Identificador").Value.ToString();
                var faixa = decimal.Parse(context.Parameters.FirstOrDefault(x => x.Id == "Faixa").Value.ToString(), _identity.GetFormatProvider());

                var dbQuery = new EliminarPedidosLpnAtributosPendientesQuery(pedido, empresa, cliente, idEspecificaIdentificador, idLpnExteno, lpnTipo, producto, identificador, faixa);

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else
            {
                var dbQuery = new EliminarPedidosLpnAtributosPendientesQuery();
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            if (context.Parameters.Count > 0)
            {
                var pedidoId = context.GetParameter("Pedido");
                var productoId = context.GetParameter("Producto");
                var clienteId = context.GetParameter("Cliente");
                var empresaId = int.Parse(context.GetParameter("Empresa"));
                var identificador = context.GetParameter("Identificador");

                var pedido = uow.PedidoRepository.GetPedido(empresaId, clienteId, pedidoId);
                var empresa = uow.EmpresaRepository.GetEmpresa(empresaId);
                var cliente = uow.AgenteRepository.GetAgente(empresaId, clienteId);
                var dsproducto = uow.ProductoRepository.GetDescripcion(empresaId, productoId);

                form.GetField("pedido").Value = pedidoId;
                form.GetField("codEmpresa").Value = empresaId.ToString();
                form.GetField("empresa").Value = empresa.Nombre;
                form.GetField("codCliente").Value = clienteId;
                form.GetField("cliente").Value = cliente.Descripcion;
                form.GetField("codProducto").Value = productoId;
                form.GetField("producto").Value = dsproducto;
                form.GetField("identificador").Value = identificador;
            }
            return form;
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            context.Parameters.Add(new ComponentParameter("NU_PEDIDO", context.Row.GetCell("NU_PEDIDO").Value));
            context.Parameters.Add(new ComponentParameter("CD_EMPRESA", context.Row.GetCell("CD_EMPRESA").Value));
            context.Parameters.Add(new ComponentParameter("CD_CLIENTE", context.Row.GetCell("CD_CLIENTE").Value));
            context.Parameters.Add(new ComponentParameter("NU_IDENTIFICADOR", context.Row.GetCell("NU_IDENTIFICADOR").Value));
            context.Parameters.Add(new ComponentParameter("ID_LPN_EXTERNO", context.Row.GetCell("ID_LPN_EXTERNO").Value));
            context.Parameters.Add(new ComponentParameter("TP_LPN_TIPO", context.Row.GetCell("TP_LPN_TIPO").Value));
            context.Parameters.Add(new ComponentParameter("CD_PRODUTO", context.Row.GetCell("CD_PRODUTO").Value));
            context.Parameters.Add(new ComponentParameter("ID_ESPECIFICA_IDENTIFICADOR", context.Row.GetCell("ID_ESPECIFICA_IDENTIFICADOR").Value));
            context.Parameters.Add(new ComponentParameter("CD_FAIXA", context.Row.GetCell("CD_FAIXA").Value));
            context.Parameters.Add(new ComponentParameter("NU_DET_PED_SAI_ATRIB", context.Row.GetCell("NU_DET_PED_SAI_ATRIB").Value));
            return context;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("PRE110 - Anulación Detalle de Pedido LPN Atributo");
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
                    var cliente = detPedido[1];
                    var empresa = int.Parse(detPedido[2]);
                    var producto = detPedido[3];
                    var faixa = decimal.Parse(detPedido[4], _identity.GetFormatProvider());
                    var identificador = detPedido[5];
                    var idEspecificaIdentificador = detPedido[6];
                    var tipoLpn = detPedido[7];
                    var idExterno = detPedido[8];
                    var idConfiguracion = long.Parse(detPedido[9]);

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

                    var detallePedidoLpnAtributo = uow.ManejoLpnRepository.GetDetallePedidoLpnAtributo(pedidoId, cliente, empresa, producto, faixa, identificador, idEspecificaIdentificador, tipoLpn, idExterno, idConfiguracion);
                    var pedidoAnulado = AnularDetallePedidoTotal(uow, pedido, detallePedidoLpnAtributo);

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
            uow.CreateTransactionNumber("PRE110 - Anulación Detalle de Pedido LPN Atributo");
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

                        var producto = row.GetCell("CD_PRODUTO").Value;
                        var identificador = row.GetCell("NU_IDENTIFICADOR").Value;
                        var faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value, _identity.GetFormatProvider());
                        var idExterno = row.GetCell("ID_LPN_EXTERNO").Value;
                        var tipoLpn = row.GetCell("TP_LPN_TIPO").Value;
                        var idEspecificaIdentificador = row.GetCell("ID_ESPECIFICA_IDENTIFICADOR").Value;
                        var idConfiguracion = long.Parse(row.GetCell("NU_DET_PED_SAI_ATRIB").Value);

                        var detallePedidoLpnAtributo = uow.ManejoLpnRepository.GetDetallePedidoLpnAtributo(pedidoId, cliente, empresa, producto, faixa, identificador, idEspecificaIdentificador, tipoLpn, idExterno, idConfiguracion);

                        PedidoAnulado pedidoAnulado = null;
                        if (row.IsDeleted)
                            pedidoAnulado = AnularDetallePedidoTotal(uow, pedido, detallePedidoLpnAtributo);
                        else if (row.IsModified)
                        {
                            var cantidadAnular = decimal.Parse(row.GetCell("AUXQT_ANULADO").Value, _identity.GetFormatProvider());
                            var motivo = string.IsNullOrEmpty(row.GetCell("AUXDS_MOTIVO").Value) ? "Anulación parcial Detalle de Pedido de LPN Atributo" : row.GetCell("AUXDS_MOTIVO").Value;

                            pedidoAnulado = AnularDetallePedidoParcial(uow, pedido, detallePedidoLpnAtributo, motivo, cantidadAnular);
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

        #region Metodos Auxiiares

        public virtual PedidoAnulado AnularDetallePedidoTotal(IUnitOfWork uow, Pedido pedido, DetallePedidoLpnAtributo detalle)
        {
            var anulacionPedido = new AnulacionPedidoPendienteLpnAtributo(uow, pedido, detalle, "Anulación total de Detalle de Pedido de LPN Atributo", this._identity.UserId, this._identity.Application);

            return anulacionPedido.Anular();
        }

        public virtual PedidoAnulado AnularDetallePedidoParcial(IUnitOfWork uow, Pedido pedido, DetallePedidoLpnAtributo detalle, string motivo, decimal cantidadAnular)
        {
            var anulacionPedido = new AnulacionPedidoPendienteLpnAtributo(uow, pedido, detalle, motivo, this._identity.UserId, this._identity.Application);

            return anulacionPedido.Anular(cantidadAnular);
        }
        
        public virtual List<string[]> GetSelectedLineasPedidos(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            if (context.Parameters.Count > 0)
            {
                var pedido = context.Parameters.FirstOrDefault(x => x.Id == "Pedido").Value.ToString();
                var empresa = int.Parse(context.Parameters.FirstOrDefault(x => x.Id == "Empresa").Value.ToString());
                var cliente = context.Parameters.FirstOrDefault(x => x.Id == "Cliente").Value.ToString();
                var idEspecificaIdentificador = context.Parameters.FirstOrDefault(x => x.Id == "IdEspecificaIdentificador").Value.ToString();
                var idLpnExteno = context.Parameters.FirstOrDefault(x => x.Id == "IdLpnExteno").Value.ToString();
                var lpnTipo = context.Parameters.FirstOrDefault(x => x.Id == "LpnTipo").Value.ToString();
                var producto = context.Parameters.FirstOrDefault(x => x.Id == "Producto").Value.ToString();
                var identificador = context.Parameters.FirstOrDefault(x => x.Id == "Identificador").Value.ToString();
                var faixa = decimal.Parse(context.Parameters.FirstOrDefault(x => x.Id == "Faixa").Value.ToString(), _identity.GetFormatProvider());

                var dbQuery = new EliminarPedidosLpnAtributosPendientesQuery(pedido, empresa, cliente, idEspecificaIdentificador, idLpnExteno, lpnTipo, producto, identificador, faixa);

                uow.HandleQuery(dbQuery);

                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                if (context.Selection.AllSelected)
                    return dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys, _identity.GetFormatProvider());

                return dbQuery.GetSelectedKeys(context.Selection.Keys, _identity.GetFormatProvider());
            }
            else
            {
                var dbQuery = new EliminarPedidosLpnAtributosPendientesQuery();

                uow.HandleQuery(dbQuery);

                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                if (context.Selection.AllSelected)
                    return dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys, _identity.GetFormatProvider());

                return dbQuery.GetSelectedKeys(context.Selection.Keys, _identity.GetFormatProvider());
            }

        }

        #endregion
    }
}
