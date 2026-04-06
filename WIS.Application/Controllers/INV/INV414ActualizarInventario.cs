using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Security;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Inventario;
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
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.INV
{
    public class INV414ActualizarInventario : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITaskQueueService _taskQueue;
        protected readonly ISecurityService _security;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly ILogger<INV414ActualizarInventario> _logger;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public INV414ActualizarInventario(IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ITaskQueueService taskQueue,
            ISecurityService security,
            ITrafficOfficerService concurrencyControl,
            ILogger<INV414ActualizarInventario> logger)
        {
            this.GridKeys = new List<string>
            {
                "NU_INVENTARIO_ENDERECO_DET"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_INVENTARIO_ENDERECO_DET", SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._taskQueue = taskQueue;
            this._security = security;
            this._concurrencyControl = concurrencyControl;
            this._logger = logger;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsCommitEnabled = false;
            context.IsRemoveEnabled = false;
            context.IsRollbackEnabled = false;
            context.IsAddEnabled = false;

            var nuInventario = context.GetParameter("inventario");
            if (!string.IsNullOrEmpty(nuInventario) && !decimal.TryParse(nuInventario, _identity.GetFormatProvider(), out decimal i))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var result = this._security.CheckPermissions(new List<string>
            {
                SecurityResources.INV414_btn_Access_AceptarDiferencia,
                SecurityResources.INV414_btn_Access_Rechazar,
                SecurityResources.INV414_btn_Access_RechazarGenerarConteo
            });

            if (result[SecurityResources.INV414_btn_Access_AceptarDiferencia])
                grid.MenuItems.Add(new GridButton("btnAceptarDif", "INV414_Sec0_btn_AceptarDiferencia"));

            if (result[SecurityResources.INV414_btn_Access_Rechazar])
                grid.MenuItems.Add(new GridButton("btnRechazar", "INV414_Sec0_btn_Rechazar"));

            if (result[SecurityResources.INV414_btn_Access_RechazarGenerarConteo])
                grid.MenuItems.Add(new GridButton("btnRechazarGenerarConteo", "INV414_Sec0_btn_RechazarGenerarConteo"));

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem>
            {
                new GridButton("btnAtributosCabezal", "INV410_Sec0_btn_AtributosCabezal", "fas fa-list"),
                new GridButton("btnAtributosDetalle", "INV410_Sec0_btn_AtributosDetalle", "fas fa-list"),
                new GridButton("btnAtributosDetalleTemporal", "INV410_Sec0_btn_AtributosDetalle", "fas fa-list"),
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuInventario = decimal.TryParse(context.GetParameter("inventario"), this._identity.GetFormatProvider(), out decimal n) ? n : default(decimal?);

            var dbQuery = new INV414GridQuery(nuInventario);
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, DefaultSort, this.GridKeys);

            grid.Rows.ForEach(row =>
            {
                switch (row.GetCell("ND_ESTADO_INV_ENDERECO_DET").Value)
                {
                    case "SINVEDACT":
                        row.CssClass = "green";
                        row.DisabledSelected = true;
                        break;
                    case "SINVEDFINDIF":
                        row.CssClass = "red";
                        break;
                    case "SINVEDREC":
                        row.CssClass = "blue";
                        break;
                    case "SINVEDFINREC":
                        row.CssClass = "yellow";
                        row.DisabledSelected = true;
                        break;
                    default:
                        break;
                }

                if (string.IsNullOrEmpty(row.GetCell("NU_LPN").Value))
                {
                    row.DisabledButtons.Add("btnAtributosCabezal");
                    row.DisabledButtons.Add("btnAtributosDetalle");
                    row.DisabledButtons.Add("btnAtributosDetalleTemporal");
                }
                else
                {
                    if (string.IsNullOrEmpty(row.GetCell("ID_LPN_DET").Value))
                        row.DisabledButtons.Add("btnAtributosDetalle");
                    else
                        row.DisabledButtons.Add("btnAtributosDetalleTemporal");
                }
            });

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuInventario = decimal.TryParse(context.GetParameter("inventario"), this._identity.GetFormatProvider(), out decimal n) ? n : default(decimal?);

            var dbQuery = new INV414GridQuery(nuInventario);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuInventario = decimal.TryParse(context.GetParameter("inventario"), this._identity.GetFormatProvider(), out decimal n) ? n : default(decimal?);

            var dbQuery = new INV414GridQuery(nuInventario);
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, DefaultSort);
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                switch (context.ButtonId)
                {
                    case "btnAtributosCabezal":
                        context.Redirect("/stock/STO710", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter() { Id = "numeroLPN", Value = context.Row.GetCell("NU_LPN").Value },
                            new ComponentParameter() { Id = "detalle", Value = "false" },
                        });
                        break;

                    case "btnAtributosDetalle":
                        context.Redirect("/stock/STO710", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter() { Id = "numeroLPN", Value = context.Row.GetCell("NU_LPN").Value },
                            new ComponentParameter() { Id = "idDetalle", Value = context.Row.GetCell("ID_LPN_DET").Value },
                            new ComponentParameter() { Id = "detalle", Value = "true" },
                        });
                        break;
                }

            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                _logger.LogError(ex, "GridButtonAction");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GridButtonAction");
                throw;
            }

            return context;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var keys = new List<string>();
            var transactionTO = this._concurrencyControl.CreateTransaccion();

            try
            {
                var logic = new InventarioLogic(this._identity, this._concurrencyControl);
                var nuInventario = decimal.TryParse(context.GetParameter("inventario"), this._identity.GetFormatProvider(), out decimal n) ? n : default(decimal?);

                var keysRowSelected = GetSelectedValues(uow, nuInventario, context, rechazarConteo: context.ButtonId == "btnRechazar");

                if (keysRowSelected != null && keysRowSelected.Count > 0)
                {
                    var inventariosOperacion = uow.InventarioRepository.GetInventarioOperacion(keysRowSelected);

                    if (nuInventario.HasValue)
                    {
                        var inventario = inventariosOperacion.FirstOrDefault(i => i.NumeroInventario == nuInventario.Value);

                        if (inventario == null)
                        {
                            context.AddErrorNotification("WINV410_Sec0_Error_Er003_NoSeEncontroInventarioX", new List<string> { nuInventario.Value.ToString() });
                            return context;
                        }
                        else if (!inventario.EnProceso())
                        {
                            context.AddErrorNotification("INV414_Sec0_Error_NoPermiteActualizar");
                            return context;
                        }
                    }
                    else
                    {
                        if (inventariosOperacion.Any(i => i.Estado != EstadoInventario.EnProceso))
                        {
                            context.AddErrorNotification("INV414_Sec0_Error_NoPermiteActualizarInventario", new List<string> { inventariosOperacion.FirstOrDefault().NumeroInventario.ToString() });
                            return context;
                        }
                    }

                    var cantNoRegenerados = 0;
                    var detallesInventario = uow.InventarioRepository.GetDetallesInventario(keysRowSelected);
                    var inventarioLpns = new Dictionary<decimal, List<Lpn>>();

                    if (detallesInventario != null && detallesInventario.Count() > 0)
                    {
                        BloqueoRegistros(nuInventario, detallesInventario, inventariosOperacion, transactionTO);

                        switch (context.ButtonId)
                        {
                            case "btnAceptarDif":
                                logic.AceptarConteos(uow, detallesInventario, inventariosOperacion, out keys, out inventarioLpns);
                                break;
                            case "btnRechazar":
                                logic.RechazarConteos(uow, detallesInventario, inventariosOperacion, out cantNoRegenerados, out inventarioLpns);
                                break;
                            case "btnRechazarGenerarConteo":
                                logic.RechazarConteos(uow, detallesInventario, inventariosOperacion, out cantNoRegenerados, out inventarioLpns, regenerarConteo: true);
                                inventarioLpns = new Dictionary<decimal, List<Lpn>>();
                                break;
                            default:
                                break;
                        }
                    }
                    var nuTransaccion = uow.GetTransactionNumber();

                    uow.BeginTransaction();
                    foreach (var inventario in inventarioLpns)
                    {
                        uow.InventarioRepository.FinalizarLpnSinStock(inventario.Key, nuTransaccion, inventario.Value);
                    }
                    uow.SaveChanges();
                    uow.Commit();

                    if (cantNoRegenerados > 0)
                        context.AddSuccessNotification("INV414_msg_Info_ConteosRechazadosSinRegenerar");
                    else
                        context.AddSuccessNotification("General_Db_Success_Update");

                }
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                _logger.LogError(ex, "GridMenuItemAction");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GridMenuItemAction");
                throw;
            }
            finally
            {
                this._concurrencyControl.DeleteTransaccion(transactionTO);

                if (_taskQueue.IsEnabled() && keys.Any())
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.AjustesDeStock, keys);
            }

            return context;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            return form;
        }

        #region Metodos auxiliares

        public virtual List<InventarioUbicacionDetalle> GetSelectedValues(IUnitOfWork uow, decimal? nroInventario, GridMenuItemActionContext context, bool rechazarConteo)
        {
            var dbQuery = new INV414GridQuery(nroInventario);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            var keys = new List<decimal>();

            foreach (var key in context.Selection.Keys)
            {
                keys.Add(decimal.Parse(key, _identity.GetFormatProvider()));
            }

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(keys, rechazarConteo);

            return dbQuery.GetSelectedKeys(keys, rechazarConteo);
        }

        public virtual void BloqueoRegistros(decimal? nuInventario, IEnumerable<InventarioUbicacionDetalle> detallesInventario, IEnumerable<InventarioOperacion> inventariosOperacion, TrafficOfficerTransaction transactionTO)
        {
            var keysInventarios = inventariosOperacion
                .GroupBy(i => i.NumeroInventario)
                .Select(i => i.Key.ToString(_identity.GetFormatProvider()))
                .ToList();

            if (keysInventarios.Count() > 0)
            {
                var listLock = this._concurrencyControl.GetLockList("T_INVENTARIO", keysInventarios, transactionTO);

                if (listLock.Count > 0)
                {
                    var keyBloqueo = listLock.FirstOrDefault().Id_Bloqueo;

                    if (!nuInventario.HasValue)
                        nuInventario = decimal.Parse(keyBloqueo, _identity.GetFormatProvider());

                    throw new ValidationFailedException("INV410_msg_Error_InventarioBloqueado", [nuInventario.Value.ToString()]);
                }

                var keysDetallesBloqueos = (from d in detallesInventario.Where(d => !string.IsNullOrEmpty(d.Producto))
                                            join o in inventariosOperacion on d.Id equals o.NumeroInventarioUbicacionDetalle
                                            group d by new { o.NumeroInventarioUbicacion, d.Empresa, d.Producto, d.Faixa, d.Identificador } into i
                                            select $"{i.Key.NumeroInventarioUbicacion}#{i.Key.Empresa}#{i.Key.Producto}#{i.Key.Faixa}#{i.Key.Identificador}")
                                .ToList();

                if (keysDetallesBloqueos.Count > 0)
                {
                    listLock = this._concurrencyControl.GetLockList("T_INVENTARIO_ENDERECO_DET", keysDetallesBloqueos, transactionTO);

                    if (listLock.Count > 0)
                    {
                        var keyBloqueo = listLock.FirstOrDefault().Id_Bloqueo.Split("#");
                        var producto = keyBloqueo[2];

                        if (!nuInventario.HasValue)
                        {
                            var nuInvUbicacion = decimal.Parse(keyBloqueo[0], _identity.GetFormatProvider());
                            nuInventario = inventariosOperacion.FirstOrDefault(i => i.NumeroInventarioUbicacion == nuInvUbicacion)?.NumeroInventario;
                        }

                        throw new ValidationFailedException("INV414_msg_Error_ConteosBloqueados", [nuInventario.Value.ToString(), producto]);
                    }

                    this._concurrencyControl.AddLockList("T_INVENTARIO", keysInventarios, transactionTO, true);
                    this._concurrencyControl.AddLockList("T_INVENTARIO_ENDERECO_DET", keysDetallesBloqueos, transactionTO, true);
                }
            }
        }

        #endregion
    }
}
