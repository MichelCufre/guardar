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
using WIS.Exceptions;
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
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.PRE
{
    public class PRE120AnulacionPreparaciones : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ILogger<PRE120AnulacionPreparaciones> _logger;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IFormValidationService _formValidationService;
        protected readonly ITrafficOfficerService _concurrencyControl;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE120AnulacionPreparaciones(IUnitOfWorkFactory uowFactory, IIdentityService identity, ILogger<PRE120AnulacionPreparaciones> logger, IGridValidationService gridValidationService, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter, IFormValidationService formValidationService, ITrafficOfficerService concurrencyControl)
        {
            this.GridKeys = new List<string>
            {
                "CD_PRODUTO", "CD_EMPRESA", "NU_IDENTIFICADOR", "CD_FAIXA", "NU_PREPARACION", "NU_PEDIDO", "CD_CLIENTE", "CD_ENDERECO", "NU_SEQ_PREPARACION"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_PREPARACION", SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._formValidationService = formValidationService;
            this._concurrencyControl = concurrencyControl;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsEditingEnabled = true;
            query.IsAddEnabled = false;
            query.IsRemoveEnabled = false;
            query.IsRollbackEnabled = true;
            query.IsCommitEnabled = true;

            grid.MenuItems.Add(new GridButton("btnAnular", "PRE120_Sec0_btn_Anular", string.Empty, new ConfirmMessage("PRE120_Sec0_msg_AnularPreparacion")));

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new AnulacionDePreparacionesQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            foreach (var row in grid.Rows)
            {
                row.SetEditableCells(new List<string>
                {
                     "AUXQT_PRODUTO_ANULAR",
                });
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new AnulacionDePreparacionesQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new AnulacionDePreparacionesQuery();

            uow.HandleQuery(dbQuery);

            var defaultSort = new SortCommand("NU_PREPARACION", SortDirection.Descending);

            query.FileName = $"{this._identity.Application}{DateTime.Now:yyyy-MM-dd_HH:mm}.xlsx";

            return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, defaultSort);
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var transactionTO = this._concurrencyControl.CreateTransaccion();

            uow.CreateTransactionNumber("PRE120AnulacionPreparaciones - GridMenuItemAction");

            long nuTransaccion = uow.GetTransactionNumber();

            uow.BeginTransaction();
            IEnumerable<string> detallesPreparacion = null;
            try
            {
                //Variable: 1-Preparacion | 2-Preparacion y Pedido            
                var tipo = int.Parse(context.GetParameter("tipoAnulacion"));

                if (context.ButtonId == "btnAnular")
                {
                    var detalles = GetSelected(uow, context);

                    if (detalles != null && detalles.Any())
                    {
                        var preparaciones = detalles
                            .GroupBy(g => g.NumeroPreparacion)
                            .Select(s => s.Key);

                        detallesPreparacion = detalles
                            .GroupBy(g => new { g.NumeroPreparacion, g.Producto, g.Faixa, g.Lote, g.Empresa, g.Ubicacion, g.Pedido, g.Cliente, g.NumeroSecuencia })
                            .Select(s => new { key = $"{s.Key.NumeroPreparacion}#{s.Key.Producto}#{s.Key.Faixa}#{s.Key.Lote}#{s.Key.Empresa}#{s.Key.Ubicacion}#{s.Key.Pedido}#{s.Key.Cliente}#{s.Key.NumeroSecuencia}" }).Select(x => x.key);

                        var listLock = this._concurrencyControl.GetLockList("T_DET_PICKING", detallesPreparacion.ToList(), transactionTO);

                        if (listLock.Count > 0)
                        {
                            var keyBloqueo = listLock.FirstOrDefault().Id_Bloqueo.Split("#");
                            throw new EntityLockedException("PRE120_msg_Error_PreparacionBloqueada", new string[] { keyBloqueo[0] });
                        }

                        this._concurrencyControl.AddLockList("T_DET_PICKING", detallesPreparacion.ToList(), transactionTO, true);

                        var anulacion = new AnularPreparaciones(uow, this._identity.UserId, tipo, TipoAnulacion.Todo, detalles, preparaciones, nuTransaccion);
                        anulacion.ProcesarAnulacion(out string pendientesExistentes, out string preparacionesEnTraspaso);

                        uow.SaveChanges();
                        uow.Commit();

                        this._concurrencyControl.RemoveLockListByIdLock("T_DET_PICKING", detallesPreparacion.ToList(), _identity.UserId);

                        if (string.IsNullOrEmpty(pendientesExistentes) && string.IsNullOrEmpty(preparacionesEnTraspaso))
                            context.AddSuccessNotification("PRE120_msg_Sucess_Anulacion");
                        else
                        {
                            if (!string.IsNullOrEmpty(pendientesExistentes))
                                context.AddSuccessNotification("PRE120_msg_Sucess_AnulacionAdvertencia", new List<string> { pendientesExistentes });
                            if (!string.IsNullOrEmpty(preparacionesEnTraspaso))
                                context.AddSuccessNotification("PRE120_msg_Sucess_AnulacionAdvertencia2", new List<string> { preparacionesEnTraspaso });
                        }
                    }
                }
            }
            catch (EntityLockedException ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message, ex.StrArguments.ToList());
            }
            catch (Exception ex)
            {
                uow.Rollback();
                this._logger.LogError(ex, "PRE120GridMenuItemAction");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }
            finally
            {
                this._concurrencyControl.DeleteTransaccion(transactionTO);
            }
            return context;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var transactionTO = this._concurrencyControl.CreateTransaccion();

            uow.CreateTransactionNumber("PRE120AnulacionPreparaciones - GridCommit");

            long nuTransaccion = uow.GetTransactionNumber();

            uow.BeginTransaction();

            try
            {
                //Variable tipoAnulacion: 1-Preparacion | 2-Preparacion y Pedido            
                var tipo = int.Parse(context.GetParameter("tipoAnulacion"));

                if (grid.Rows.Any())
                {
                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    var detalles = new List<DetallePreparacion>();

                    foreach (var row in grid.Rows)
                    {
                        var det = new AnulacionDePreparacionesQuery().SelectionQuery(row.Id, _identity.GetFormatProvider());

                        det.CantidadAnular = decimal.Parse(row.GetCell("AUXQT_PRODUTO_ANULAR").Value, _identity.GetFormatProvider());
                        det.Cantidad = decimal.Parse(row.GetCell("QT_PRODUTO").Value, _identity.GetFormatProvider());

                        if (long.TryParse(row.GetCell("ID_DET_PICKING_LPN").Value, out long parsedValue))
                            det.IdDetallePickingLpn = parsedValue;

                        detalles.Add(det);
                    }

                    var preparaciones = detalles
                        .GroupBy(g => g.NumeroPreparacion)
                        .Select(s => s.Key);

                    var detallesPreparacion = detalles
                       .GroupBy(g => new { g.NumeroPreparacion, g.Producto, g.Faixa, g.Lote, g.Empresa, g.Ubicacion, g.Pedido, g.Cliente, g.NumeroSecuencia })
                       .Select(s => new { key = $"{s.Key.NumeroPreparacion}#{s.Key.Producto}#{s.Key.Faixa}#{s.Key.Lote}#{s.Key.Empresa}#{s.Key.Ubicacion}#{s.Key.Pedido}#{s.Key.Cliente}#{s.Key.NumeroSecuencia}" }).Select(x => x.key);

                    var listLock = this._concurrencyControl.GetLockList("T_DET_PICKING", detallesPreparacion.ToList(), transactionTO);

                    if (listLock.Count > 0)
                    {
                        var keyBloqueo = listLock.FirstOrDefault().Id_Bloqueo.Split("#");
                        throw new EntityLockedException("PRE120_msg_Error_PreparacionBloqueada", new string[] { keyBloqueo[0] });
                    }

                    this._concurrencyControl.AddLockList("T_DET_PICKING", detallesPreparacion.ToList(), transactionTO, true);

                    var anulacion = new AnularPreparaciones(uow, this._identity.UserId, tipo, TipoAnulacion.Seleccion, detalles, preparaciones, nuTransaccion);
                    anulacion.ProcesarAnulacion(out string pendientesExistentes, out string preparacionesEnTraspaso);

                    uow.SaveChanges();
                    uow.Commit();

                    this._concurrencyControl.RemoveLockListByIdLock("T_DET_PICKING", detallesPreparacion.ToList(), _identity.UserId);

                    if (string.IsNullOrEmpty(pendientesExistentes))
                        context.AddSuccessNotification("PRE120_msg_Sucess_Anulacion");
                    else
                        if (!string.IsNullOrEmpty(pendientesExistentes))
                        context.AddSuccessNotification("PRE120_msg_Sucess_AnulacionAdvertencia", new List<string> { pendientesExistentes });
                    if (!string.IsNullOrEmpty(preparacionesEnTraspaso))
                        context.AddSuccessNotification("PRE120_msg_Sucess_AnulacionAdvertencia2", new List<string> { preparacionesEnTraspaso });

                }
            }
            catch (EntityLockedException ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message, ex.StrArguments.ToList());
            }
            catch (ExpectedException ex)
            {
                uow.Rollback();
                this._logger.LogWarning(ex, "GridCommit");
                context.AddErrorNotification(ex.Message);
            }
            catch (Exception ex)
            {
                uow.Rollback();
                this._logger.LogError(ex, "PRE120GridCommit");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }
            finally
            {
                this._concurrencyControl.DeleteTransaccion(transactionTO);
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoAnulacionPreparacionesGridValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        public virtual List<DetallePreparacion> GetSelected(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var dbQuery = new AnulacionDePreparacionesQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys, _identity.GetFormatProvider());

            return dbQuery.GetSelectedKeys(context.Selection.Keys, _identity.GetFormatProvider());
        }
    }
}
