using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules.Expedicion;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Expedicion;
using WIS.Domain.Expedicion.EXP110EmpaquetadoPicking;
using WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Extension;
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

namespace WIS.Application.Controllers.EXP
{
    public class EXP110InformacionPedidoMesaEmpaque : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly ITrackingService _trackingService;
        protected readonly IBarcodeService _barcodeService;
        protected readonly ITaskQueueService _taskQueue;

        protected readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected readonly EmpaquetadoPickingLogic _logic;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public EXP110InformacionPedidoMesaEmpaque(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            IGridValidationService gridValidationService,
            IPrintingService printingService,
            ITrackingService trackingService,
            IBarcodeService barcodeService,
            ITaskQueueService taskQueue)
        {
            this.GridKeys = new List<string>
            {
                "CD_EMPRESA","CD_CLIENTE","NU_PEDIDO","TP_EXPEDICION"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_PEDIDO", SortDirection.Descending)
            };

            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._gridValidationService = gridValidationService;
            this._trackingService = trackingService;
            this._barcodeService = barcodeService;
            this._taskQueue = taskQueue;

            this._logic = new EmpaquetadoPickingLogic(printingService, trackingService, barcodeService, identity, Logger);
        }


        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsAddEnabled = false;
            context.IsRemoveEnabled = false;
            context.IsCommitEnabled = true;

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("btnIr", "General_Sec0_btn_Ir", "fa fa-arrow-right"),
                new GridButton("btnInfoPedido", "EXP110_grid1_btn_InfoPedido", "fas fa-list"),
            }));

            grid.AddOrUpdateColumn(new GridColumnSelect("TP_EXPEDICION", this.OptionSelectTipoExpedicion()));

            grid.SetEditableCells(new List<string> { "TP_EXPEDICION" });

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var ubicacion = GetUbicacionEstacion(context);
            var dbQuery = new EXP110InformacionPedidoMesaEmpaqueQuery(ubicacion);
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            grid.Rows.ForEach(row =>
            {
                var empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                var cliente = row.GetCell("CD_CLIENTE").Value;
                var ped = row.GetCell("NU_PEDIDO").Value;

                Pedido pedido = uow.PedidoRepository.GetPedido(empresa, cliente, ped);

                if (!string.IsNullOrEmpty(row.GetCell("CD_CAMION").Value))
                    row.GetCell("TP_EXPEDICION").Editable = false;
                else if (uow.EmpaquetadoPickingRepository.AnyCamionPedido(ped, cliente, empresa) != -1)
                    row.GetCell("TP_EXPEDICION").Editable = false;
                else
                    row.GetCell("TP_EXPEDICION").Editable = true;


                if (!pedido.FueLiberadoCompletamente(out decimal QT_LIBERADA))
                    row.CssClass = "redBlack";
                else if (!uow.EmpaquetadoPickingRepository.PedidoTodoPickeado(empresa, cliente, ped))
                    row.CssClass = "yellow";
                else if (!uow.EmpaquetadoPickingRepository.PedidoTodoEmpaquetado(empresa, cliente, ped))
                    row.CssClass = "blue";
                else
                    row.CssClass = "green";

            });

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var ubicacion = GetUbicacionEstacion(context);
            var dbQuery = new EXP110InformacionPedidoMesaEmpaqueQuery(ubicacion);
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var ubicacion = GetUbicacionEstacion(context);
            var dbQuery = new EXP110InformacionPedidoMesaEmpaqueQuery(ubicacion);
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
            uow.BeginTransaction();
            try
            {
                if (grid.Rows.Any())
                {
                    foreach (var row in grid.Rows)
                    {
                        var empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                        var cliente = row.GetCell("CD_CLIENTE").Value;
                        var ped = row.GetCell("NU_PEDIDO").Value;
                        var tpExpdicion = row.GetCell("TP_EXPEDICION").Value;

                        var pedido = uow.PedidoRepository.GetPedido(empresa, cliente, ped);
                        pedido.ConfiguracionExpedicion.Tipo = tpExpdicion;
                        pedido.FechaModificacion = DateTime.Now;
                        uow.PedidoRepository.UpdatePedido(pedido);
                    }
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                uow.Rollback();
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }
            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            return this._gridValidationService.Validate(new EXP110InfoPedidoMesaEmpaqueGridValidationModule(uow), grid, row, context);
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.ButtonId == "btnIr")
            {
                var creoCamion = true;
                var pedido = uow.PedidoRepository.GetPedido(context.Row.GetCell("CD_EMPRESA").Value.ToNumber<int>(),
                                                              context.Row.GetCell("CD_CLIENTE").Value,
                                                              context.Row.GetCell("NU_PEDIDO").Value);

                var cdCamion = string.IsNullOrEmpty(context.Row.GetCell("CD_CAMION").Value) ? null : context.Row.GetCell("CD_CAMION").Value.ToNumber<int?>();

                if (!uow.EmpaquetadoPickingRepository.ExisteCamionPedidoEmpaque(pedido.Empresa, pedido.Cliente, pedido.Id, cdCamion))
                {
                    context.AddInfoNotification("EXP110_InfPedidoEmpaque_Info_DebeRefrescarLaGrilla");
                    return context;
                }

                if (cdCamion != null)
                    creoCamion = false;

                if (creoCamion && pedido.ConfiguracionExpedicion.TipoArmadoEgreso == TipoArmadoEgreso.Empaque)
                {
                    uow.CreateTransactionNumber("EXP110InfoPedidoMesa - GridButtonAction");
                    uow.BeginTransaction();

                    _logic.AddCargaCamionPedidoUnico(uow, pedido.Empresa, pedido.Cliente, pedido.Id, _identity.Predio, out cdCamion);
                    
                    uow.SaveChanges();
                    uow.Commit();
                }

                if (cdCamion != null)
                {
                    context.Redirect("/expedicion/EXP040", true, new List<ComponentParameter>()
                    {
                        new ComponentParameter("CD_CAMION", cdCamion.ToString())
                    });
                }
                else
                {
                    context.AddErrorNotification("EXP110_InfPedidoEmpaque_Error_PedidoSinEgresoAsociado");
                }
            }
            else if (context.ButtonId == "btnInfoPedido")
            {
                context.AddParameter("BTNID", "btnInfoPedido");
                context.AddParameter("NU_PEDIDO", context.Row.GetCell("NU_PEDIDO").Value);
                context.AddParameter("CD_CLIENTE", context.Row.GetCell("CD_CLIENTE").Value);
                context.AddParameter("CD_EMPRESA", context.Row.GetCell("CD_EMPRESA").Value);
            }

            return context;
        }

        #region Metodos Auxiliares

        public virtual string GetUbicacionEstacion(ComponentContext context)
        {
            var confInicialParam = context.GetParameter("CONF_INICIAL");

            if (string.IsNullOrEmpty(confInicialParam))
                return string.Empty;
            else
            {
                var confInicial = JsonConvert.DeserializeObject<ConfiguracionInicial>(confInicialParam);
                return confInicial?.Ubicacion;
            }
        }

        public virtual List<SelectOption> OptionSelectTipoExpedicion()
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var opciones = new List<SelectOption>();

            var configuraciones = uow.PedidoRepository.GetConfiguracionesExpedicion();

            foreach (var configuracion in configuraciones)
            {
                if (configuracion.Tipo == TipoExpedicion.ReservasPrepManual)
                    continue;

                opciones.Add(new SelectOption(configuracion.Tipo, configuracion.Descripcion));
            }

            return opciones;
        }

        #endregion

    }
}