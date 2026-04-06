using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Application.Security;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.Picking;
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
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRE
{
    public class PRE052PanelLiberacion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<PRE052PanelLiberacion> _logger;

        protected List<string> GridKeys { get; }

        protected List<SortCommand> DefaultSort { get; }

        public PRE052PanelLiberacion(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            ISecurityService security,
            ISecurityService segurity,
            IFilterInterpreter filterInterpreter,
            ILogger<PRE052PanelLiberacion> logger)
        {
            this.GridKeys = new List<string>
            {
                "NU_PREPARACION"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_PREPARACION", SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._security = security;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
        }

        public override PageContext PageLoad(PageContext data)
        {
            var listaPermisos = new List<string>()
            {
                SecurityResources.PreparacionManualLibre,
                SecurityResources.PreparacionManualPedido,
                SecurityResources.PreparacionManualAdm,
           };

            var resultado = this._security.CheckPermissions(listaPermisos);

            foreach (var res in resultado)
            {
                data.Parameters.Add(new ComponentParameter(res.Key, res.Value.ToString()));
            }

            return data;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            var resultadoPermisions = this._security.CheckPermissions(new List<string>() { SecurityResources.AsociarPedidosPrepManual });

            var gridButtons = new List<IGridItem>()
            {
                new GridButton("btnDetallePreparacion", "PRE052_Sec0_btn_DetallePreparacion", "fas fa-list-alt"),
                new GridButton("btnFinalizarPedido", "PRE052_grid1_btn_FinalizarPreparacion", "fas fa-clipboard-check"),
            };

            if (resultadoPermisions[SecurityResources.AsociarPedidosPrepManual])
                gridButtons.Add(new GridButton("btnAsociarPedidos", "PRE052_Sec0_btn_AsociarPedidos", "fas fa-clipboard-list"));

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_ARRAY", gridButtons));

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY_2", new List<GridButton>
            {
                new GridButton("btnAnalisisRechazo", "PRE052_grid1_btn_AnalisisRechazo", "fas fa-exclamation-triangle"),
                new GridButton("btnAnalisisRechazoPedido", "PRE052_grid1_btn_AnalisisRechazoPedido", "fas fa-exclamation-circle")
            }));

            context.AddLink("CD_EMPRESA", "registro/REG100", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PreparacionesQuery();
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY_2", new List<GridButton>{
                new GridButton("btnAnalisisRechazo", "PRE052_grid1_btn_AnalisisRechazo", "fas fa-exclamation-triangle"),
                new GridButton("btnAnalisisRechazoPedido", "PRE052_grid1_btn_AnalisisRechazoPedido", "fas fa-exclamation-circle")
            }));

            SetConfigurationRows(grid);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PreparacionesQuery();
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PreparacionesQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            try
            {
                if (context.ButtonId == "btnLiberacionPedido")
                {
                    if (this._security.IsUserAllowed(SecurityResources.WPRE300_Page_Access_PreparacionManual))
                    {
                        context.Redirect("/preparacion/PRE300", new List<ComponentParameter>()
                    {
                        new ComponentParameter() { Id = "preparacion", Value = context.Row.GetCell("NU_PREPARACION").Value },
                        new ComponentParameter() { Id = "tipo", Value = context.Row.GetCell("TP_PREPARACION").Value }
                    });
                    }
                    else
                        context.AddErrorNotification("PRE052_Sec0_Error_UsuarioSinPermisos");
                }
                else if (context.ButtonId == "btnFinalizarPedido")
                {
                    FinalizarPreparacion(context.Row);

                    context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
                    context.AddParameter("PrepaFinalizada", "finalizada");

                }
                else if (context.ButtonId == "btnAnalisisRechazo")
                {
                    var estadoPreparaciones = new List<short> { SituacionDb.EnProceso, SituacionDb.PreparacionPendiente };

                    if (estadoPreparaciones.Contains(short.Parse(context.Row.GetCell("CD_SITUACAO").Value)))
                        throw new ValidationFailedException("WPRE052_Sec0_Error_PrepSinEjecutarRechazos");

                    context.Redirect("/preparacion/PRE170", true, new List<ComponentParameter>()
                    {
                       new ComponentParameter() { Id = "preparacion", Value = context.Row.GetCell("NU_PREPARACION").Value }
                    });
                }
                else if (context.ButtonId == "btnAnalisisRechazoPedido")
                {
                    var estadoPreparaciones = new List<short> { SituacionDb.EnProceso, SituacionDb.PreparacionPendiente };

                    if (estadoPreparaciones.Contains(short.Parse(context.Row.GetCell("CD_SITUACAO").Value)))
                        throw new ValidationFailedException("WPRE052_Sec0_Error_PrepSinEjecutarRechazos");

                    context.Redirect("/preparacion/PRE080", true, new List<ComponentParameter>()
                    {
                       new ComponentParameter() { Id = "preparacion", Value = context.Row.GetCell("NU_PREPARACION").Value }
                    });
                }
                else if (context.ButtonId == "btnDetallePreparacion")
                {
                    context.Redirect("/preparacion/PRE130", true, new List<ComponentParameter>()
                    {
                        new ComponentParameter() { Id = "FROM_PRE052", Value = "PRE052" },
                        new ComponentParameter() { Id = "preparacion", Value = context.Row.GetCell("NU_PREPARACION").Value },
                        new ComponentParameter() { Id = "empresa", Value = context.Row.GetCell("CD_EMPRESA").Value }
                    });
                }
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                _logger.LogError(ex, "PRE052PanelLiberacion - GridButtonAction");
            }
            catch (Exception ex)
            {
                context.AddErrorNotification("General_Sec0_Error_Operacion");
                _logger.LogError(ex, "PRE052PanelLiberacion - GridButtonAction");
            }

            return context;
        }

        #region Metodos Auxiliares

        public virtual void SetConfigurationRows(Grid grid)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var estadosSinAlerta = new List<short> { SituacionDb.EnProceso, SituacionDb.HabilitadoParaPickear, SituacionDb.PreparacionFinalizada };

            //TODO: En la tabla T_SITUACAO no hay ninguna de las tres situaciones de abajo, "hardcode", a revisar posteriormente
            var estadoAlertaAlta = new List<short> { 113, 114, 115 };

            foreach (var row in grid.Rows)
            {
                List<string> botonesDeshabiltiados = new List<string>();

                decimal.TryParse(row.GetCell("QT_RECHAZOS")?.Value, NumberStyles.Number, this._identity.GetFormatProvider(), out decimal cantRechazos);

                if (cantRechazos == 0)
                {
                    botonesDeshabiltiados.Add("btnAnalisisRechazo");
                    botonesDeshabiltiados.Add("btnAnalisisRechazoPedido");
                }
                else
                {
                    row.DisabledButtons.Remove("btnAnalisisRechazo");
                    row.DisabledButtons.Remove("btnAnalisisRechazoPedido");
                }

                var tipoPreparacion = row.GetCell("TP_PREPARACION").Value;
                var situacion = short.Parse(row.GetCell("CD_SITUACAO").Value);

                if (estadoAlertaAlta.Contains(situacion))
                    row.CssClass = row.CssClass + "alertaAlta";
                else if (!estadosSinAlerta.Contains(situacion))
                    row.CssClass = row.CssClass + " alertaBaja";

                var tiposPreparacionManual = new List<string> { TipoPreparacionDb.Libre, TipoPreparacionDb.Pedido };

                if (!this._security.IsUserAllowed(SecurityResources.WPRE052_grid1_btn_EditarPreparacion) ||
                    !tiposPreparacionManual.Contains(tipoPreparacion) ||
                    situacion == SituacionDb.PreparacionFinalizada)
                {
                    botonesDeshabiltiados.Add("btnFinalizarPedido");
                    botonesDeshabiltiados.Add("btnLiberacionPedido");
                }

                if (!this._security.IsUserAllowed(SecurityResources.WPRE052_grid1_btn_AsociarPedidos) ||
                    tipoPreparacion != TipoPreparacionDb.Pedido ||
                    situacion == SituacionDb.PreparacionFinalizada)
                {
                    botonesDeshabiltiados.Add("btnAsociarPedidos");
                }

                var prep = int.Parse(row.GetCell("NU_PREPARACION").Value);
                var emp = int.Parse(row.GetCell("CD_EMPRESA").Value);

                if (!uow.PreparacionRepository.AnyDetPicking(prep, emp))
                    botonesDeshabiltiados.Add("btnDetallePreparacion");

                row.DisabledButtons = botonesDeshabiltiados;
            }
        }

        public virtual void FinalizarPreparacion(GridRow row)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuPreparacion = int.Parse(row.GetCell("NU_PREPARACION").Value);

            if (uow.PreparacionRepository.GetConfiguracionPreparacionManual().ControlTotal)
            {
                var dbQuery = new DetallePedidoSalidaQuery();
                var pedidosPendientes = dbQuery.GetPedidosPendientesPreparar(nuPreparacion);

                if (pedidosPendientes != null || pedidosPendientes.Count > 0)
                {
                    var msg = "PRE052_msg_Error_PedidoPreparadoParcial";
                    var args = pedidosPendientes.FirstOrDefault();

                    if (pedidosPendientes.Count > 1)
                    {
                        msg = "PRE052_msg_Error_PedidosPreparadosParcialmente";
                        args = string.Join(";", pedidosPendientes);
                    }

                    throw new ValidationFailedException(msg, [args]);
                }
            }

            var preparacion = uow.PreparacionRepository.GetPreparacionPorNumero(nuPreparacion);
            if (preparacion != null)
            {
                if ((preparacion.Situacion ?? 0) == SituacionDb.PreparacionFinalizada)
                    throw new ValidationFailedException("PRE052_Sec0_Error_PreparacionYaFinalizada");
            }

            uow.CreateTransactionNumber("Finalizar preparación manual");

            preparacion.Situacion = SituacionDb.PreparacionFinalizada;
            preparacion.Transaccion = uow.GetTransactionNumber();
            preparacion.FechaFin = DateTime.Now;

            uow.PedidoRepository.LiberarPedidosConPendiente(nuPreparacion, uow.GetTransactionNumber());
            
            uow.PreparacionRepository.UpdatePreparacion(preparacion);
            uow.SaveChanges();
        }

        #endregion
    }
}
