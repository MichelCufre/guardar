using NLog;
using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Produccion.Interfaces;
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
    public class PRD110 : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly ILogicaProduccionFactory _logicaProduccionFactory;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected readonly Dictionary<string, object> GridIngresosProduccionConfig = new Dictionary<string, object>();
        protected readonly Dictionary<string, object> GridPedidosConfig = new Dictionary<string, object>();
        protected readonly Dictionary<string, object> GridDetallesPedidosConfig = new Dictionary<string, object>();

        protected List<string> GridKeys { get; set; }

        public PRD110(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            ITrafficOfficerService concurrencyControl,
            ILogicaProduccionFactory logicaProduccion)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._formValidationService = formValidationService;
            this._filterInterpreter = filterInterpreter;
            this._concurrencyControl = concurrencyControl;
            _logicaProduccionFactory = logicaProduccion;

            GridIngresosProduccionConfig.Add("Id", "PRD110_grid_1");
            GridIngresosProduccionConfig.Add("GridKeys", new List<string> { "NU_PRDC_INGRESO" });
            GridIngresosProduccionConfig.Add("DefaultSort", new List<SortCommand> { new SortCommand("DT_ADDROW", SortDirection.Descending) });

            GridPedidosConfig.Add("Id", "PRD110Pedidos_grid_2");
            GridPedidosConfig.Add("GridKeys", new List<string> { "NU_PEDIDO" });
            GridPedidosConfig.Add("DefaultSort", new List<SortCommand> { new SortCommand("NU_PEDIDO", SortDirection.Ascending) });

            GridDetallesPedidosConfig.Add("Id", "PRD110DetallesPedidos_grid_3");
            GridDetallesPedidosConfig.Add("GridKeys", new List<string> { "NU_IDENTIFICADOR", "CD_PRODUTO", "CD_EMPRESA", "CD_FAIXA" });
            GridDetallesPedidosConfig.Add("DefaultSort", new List<SortCommand> { new SortCommand("CD_PRODUTO", SortDirection.Ascending) });
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = false;
            context.IsAddEnabled = false;
            context.IsCommitEnabled = false;
            context.IsRemoveEnabled = false;

            if (grid.Id == (string)GridIngresosProduccionConfig["Id"])
            {
                grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem>
                {
                    new GridButton("btnAsociarEspacio", "PRD112_grid1_btn_AsociarEspacio", "fas fa-industry"),
                    new GridButton("btnPlanificacionInsumos", "PRD110_grid1_btn_PlanificacionInsumos", "fa fa-paste"),
                    new GridButton("btnDetalles", "PRD110_grid1_btn_Detalles", "fas fa-list"),
                    new GridButton("btnDetallesProducidos", "PRD110_grid1_btn_DetallesProducidos", "fas fa-list"),
					//new GridButton("btnInstrucciones", "PRD500_grid1_btn_Instrucciones", "fas fa-list-ol"),
					new GridButton("btnIniciarProduccion", "PRD110_grid1_btn_IniciarProduccion", "fas fa-play"),
                    new GridButton("btnProducir", "PRD110_grid1_btn_PanelDeProduccion", "fas fa-play"),
                }));
            }

            if (grid.Id == (string)GridPedidosConfig["Id"])
            {
                grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem>
                {
                    new GridButton("btnDetalles", "PRD112_grid1_btn_DetallesPedido", "fas fa-list")
                }));
            }

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            if (grid.Id == (string)GridIngresosProduccionConfig["Id"])
            {
                var dbQuery = new IngresosProduccionQuery();

                uow.HandleQuery(dbQuery);

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, (List<SortCommand>)GridIngresosProduccionConfig["DefaultSort"], (List<string>)GridIngresosProduccionConfig["GridKeys"]);

                AsignarOperacionesPorFilaIngresoProduccion(grid, uow);
            }
            else if (grid.Id == (string)GridPedidosConfig["Id"])
            {
                var idIngreso = context.GetParameter("idIngreso");

                var dbQuery = new IngresoProduccionPedidosQuery(idIngreso);

                uow.HandleQuery(dbQuery);

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, (List<SortCommand>)GridPedidosConfig["DefaultSort"], (List<string>)GridPedidosConfig["GridKeys"]);
            }
            else if (grid.Id == (string)GridDetallesPedidosConfig["Id"])
            {
                var nuPedido = context.GetParameter("nuPedido");
                var cdCliente = context.GetParameter("cdCliente");
                var cdEmpresa = int.Parse(context.GetParameter("cdEmpresa"));

                var dbQuery = new DetallePedidoPanelQuery(cdEmpresa, cdCliente, nuPedido);

                uow.HandleQuery(dbQuery);

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, (List<SortCommand>)GridDetallesPedidosConfig["DefaultSort"], (List<string>)GridDetallesPedidosConfig["GridKeys"]);
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (grid.Id == (string)GridIngresosProduccionConfig["Id"])
                {
                    var dbQuery = new IngresosProduccionQuery();

                    uow.HandleQuery(dbQuery);

                    var defaultSort = new SortCommand("NU_PRDC_INGRESO", SortDirection.Descending);

                    context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                    return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
                }

                if (grid.Id == (string)GridPedidosConfig["Id"])
                {
                    var idIngreso = context.GetParameter("idIngreso");

                    var dbQuery = new IngresoProduccionPedidosQuery(idIngreso);

                    uow.HandleQuery(dbQuery);

                    var defaultSort = new SortCommand("NU_PEDIDO", SortDirection.Ascending);

                    context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                    return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
                }

                if (grid.Id == (string)GridDetallesPedidosConfig["Id"])
                {
                    var nuPedido = context.GetParameter("nuPedido");
                    var cdCliente = context.GetParameter("cdCliente");
                    var cdEmpresa = int.Parse(context.GetParameter("cdEmpresa"));

                    var dbQuery = new DetallePedidoPanelQuery(cdEmpresa, cdCliente, nuPedido);

                    uow.HandleQuery(dbQuery);

                    var defaultSort = new SortCommand("CD_PRODUTO", SortDirection.Ascending);

                    context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                    return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
                }

                return null;
            }
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            if (grid.Id == (string)GridIngresosProduccionConfig["Id"])
            {
                var dbQuery = new IngresosProduccionQuery();
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else if (grid.Id == (string)GridPedidosConfig["Id"])
            {
                var idIngreso = context.GetParameter("idIngreso");

                var dbQuery = new IngresoProduccionPedidosQuery(idIngreso);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else if (grid.Id == (string)GridDetallesPedidosConfig["Id"])
            {
                var nuPedido = context.GetParameter("nuPedido");
                var cdCliente = context.GetParameter("cdCliente");
                var cdEmpresa = int.Parse(context.GetParameter("cdEmpresa"));

                var dbQuery = new DetallePedidoPanelQuery(cdEmpresa, cdCliente, nuPedido);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }

            return null;
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            switch (context.ButtonId)
            {
                case "btnIniciarProduccion":
                    BtnIniciarProduccion(context);

                    break;

                case "btnProducir":
                    context.Redirect("/produccion/PRD113", true, new List<ComponentParameter> {
                            new ComponentParameter("nuIngresoProduccion", context.Row.GetCell("NU_PRDC_INGRESO").Value),
                            new ComponentParameter("cdEmpresa", context.Row.GetCell("CD_EMPRESA").Value)
                        });
                    break;

                case "btnPlanificacionInsumos":
                    context.Redirect("/produccion/PRD112", true, new List<ComponentParameter> {
                            new ComponentParameter("nuIngresoProduccion", context.Row.GetCell("NU_PRDC_INGRESO").Value)
                        });
                    break;

            }
            return context;
        }

        #region Metodos Auxiliares
        public virtual void AsignarOperacionesPorFilaIngresoProduccion(Grid grid, IUnitOfWork uow)
        {
            foreach (GridRow row in grid.Rows)
            {
                short situacion = short.Parse(row.GetCell("CD_SITUACAO").Value);
                var idIngreso = row.GetCell("NU_PRDC_INGRESO").Value;
                var ingreso = uow.IngresoProduccionRepository.GetIngresoById(idIngreso);

                if (ingreso == null)
                    throw new ValidationFailedException("General_Sec0_Error_ProduccionNotFound");

                if (situacion != SituacionDb.PRODUCCION_INICIADA
                    && situacion != SituacionDb.PRODUCCION_PARCIALMENTE_NOTIF
                    && situacion != SituacionDb.PRODUCCION_PENDIENTE_NOTIFICACION_FINAL
                    && situacion != SituacionDb.PRODUCCION_PENDIENTE_NOTIFICACION_PARCIAL
                    && situacion != SituacionDb.PRODUCCION_FINALIZADA
                    && situacion != SituacionDb.INICIO_ALTA_PRODUCTOS_FINALES
                    && situacion != SituacionDb.PRODUCIENDO)
                {
                    row.DisabledButtons = new List<string>() { "btnProducir" };
                }
                else
                {
                    row.DisabledButtons = new List<string>() { "btnIniciarProduccion" };
                }

                if (situacion == SituacionDb.PRODUCCION_FINALIZADA || situacion == SituacionDb.PRODUCCION_PENDIENTE_NOTIFICACION_FINAL || ingreso.Tipo != TipoIngresoProduccion.BlackBox)
                {
                    row.DisabledButtons.Add("btnPlanificacionInsumos");
                }

                if ((situacion != SituacionDb.PRODUCCION_CREADA
                    && situacion != SituacionDb.PEDIDO_GENERADO) || uow.IngresoProduccionRepository.AnyInsumosReales(idIngreso))
                {
                    row.DisabledButtons.Add("btnAsociarEspacio");
                }

                if (!uow.IngresoProduccionRepository.AnyConsumoInsumosReales(idIngreso) && !uow.IngresoProduccionRepository.AnySalidasReales(idIngreso))
                {
                    row.DisabledButtons.Add("btnDetallesProducidos");
                }

                if (ingreso.Tipo == TipoIngresoProduccion.Colector)
                {
                    row.DisabledButtons.Add("btnAsociarEspacio");
                    row.DisabledButtons.Add("btnPlanificacionInsumos");
                    row.DisabledButtons.Add("btnIniciarProduccion");
                    row.DisabledButtons.Add("btnProducir");
                }
            }
        }

        public virtual GridButtonActionContext BtnIniciarProduccion(GridButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string idIngreso = context.Row.GetCell("NU_PRDC_INGRESO").Value;

            if (this._concurrencyControl.IsLocked("T_PRDC_INGRESO", idIngreso, true))
                throw new Exception("General_msg_Error_ProduccionBloqueada");

            var transaction = _concurrencyControl.CreateTransaccion();

            try
            {
                _concurrencyControl.AddLock("T_PRDC_INGRESO", idIngreso, transaction, true);

                var logicaProduccion = _logicaProduccionFactory.GetLogicaProduccion(uow, idIngreso);

                if (!logicaProduccion.PuedeIniciarProduccion(out string error, out List<string> errorArgs))
                {
                    if (errorArgs != null && errorArgs.Count > 0)
                        context.AddErrorNotification(error, errorArgs);
                    else
                        context.AddErrorNotification(error);
                }
                else
                {
                    uow.CreateTransactionNumber("Iniciar Produccion");
                    uow.BeginTransaction();

                    logicaProduccion.IniciarProduccion();

                    uow.SaveChanges();
                    uow.Commit();

                    context.AddSuccessNotification("PRD110_grid1_Msg_ProduccionIniciada");
                }
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
                _logger.Error(ex, ex.Message);
            }
            finally
            {
                _concurrencyControl.DeleteTransaccion(transaction);
            }
            return context;
        }

        #endregion
    }
}
