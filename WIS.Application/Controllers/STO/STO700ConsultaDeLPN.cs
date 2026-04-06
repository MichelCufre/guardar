using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Security;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Stock;
using WIS.Domain.Services.Interfaces;
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

namespace WIS.Application.Controllers.STO
{
    public class STO700ConsultaDeLPN : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IIdentityService _identity;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ISecurityService _security;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly ILogger<STO700ConsultaDeLPN> _logger;
        protected readonly IParameterService _parameterService;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> Sorts { get; }

        public STO700ConsultaDeLPN(
            IGridExcelService excelService,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IIdentityService identity,
            IFilterInterpreter filterInterpreter,
            ISecurityService security,
            ITrafficOfficerService concurrencyControl,
            ILogger<STO700ConsultaDeLPN> logger,
            IParameterService parameterService,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "NU_LPN"
            };

            this.Sorts = new List<SortCommand> {
                new SortCommand("NU_LPN", SortDirection.Descending),
            };

            this._uowFactory = uowFactory;
            this._gridService = gridService;
            _excelService = excelService;
            _identity = identity;
            _filterInterpreter = filterInterpreter;
            _security = security;
            _concurrencyControl = concurrencyControl;
            _logger = logger;
            _parameterService = parameterService;
            _gridValidationService = gridValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = false;
            context.IsAddEnabled = false;
            context.IsCommitEnabled = true;

            var items = new List<IGridItem> {
                new GridButton("btnAtributos", "STO700_grd1_btn_Atributos", "fas fa-list"),
                new GridButton("btnContenido", "STO700_grd1_btn_Contenido", "fas fa-list"),
                new GridButton("btnHistorial", "STO700_grd1_btn_Historial", "fas fa-clock"),
                new GridButton("btnCodBarras", "STO700_grd1_btn_CodBarras", "fas fa-barcode"),
            };

            if (this._security.IsUserAllowed(SecurityResources.WSTO700_grd1_btn_Finalizar))
                items.Add(new GridButton("btnFinalizar", "STO700_grd1_btn_Finalizar", "fas fa-clipboard-check", new ConfirmMessage("STO700_grid1_msg_ConfirmarFinalizarLpn")));

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", items));

            grid.MenuItems = new List<IGridItem> {
                new GridButton("btnImprimir", "STO700_grid1_btn_imprimir")
            };

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int? numeroAgenda = null;
            bool.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "lpnsActivos")?.Value, out bool lpnActivos);

            if (int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "agenda")?.Value, out int parsedValue))
                numeroAgenda = parsedValue;

            var dbQuery = new ConsultaDeLpnQuery(numeroAgenda, lpnActivos);
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.Sorts, this.GridKeys);

            grid.Rows.ForEach(row =>
            {
                var estado = row.GetCell("ID_ESTADO").Value;
                var ubicacion = row.GetCell("CD_ENDERECO").Value;
                var agenda = row.GetCell("NU_AGENDA").Value;

                row.DisabledButtons = new List<string>()
                {
                    "btnFinalizar",
                };

                if (estado == EstadosLPN.Importado && string.IsNullOrEmpty(ubicacion) && string.IsNullOrEmpty(agenda))
                    row.DisabledButtons.Remove("btnFinalizar");

                var empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                var permiteEditarIdPacking = this._parameterService.GetValueByEmpresa(ParamManager.FL_MODIFICAR_ID_PACKING_LPN, empresa) == "S";

                if (estado != EstadosLPN.Finalizado && permiteEditarIdPacking)
                    row.GetCell("ID_PACKING").Editable = true;
            });

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int? numeroAgenda = null;
            bool.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "lpnsActivos")?.Value, out bool lpnActivos);

            if (int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "agenda")?.Value, out int parsedValue))
                numeroAgenda = parsedValue;

            var dbQuery = new ConsultaDeLpnQuery(numeroAgenda, lpnActivos);

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

            int? numeroAgenda = null;
            bool.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "lpnsActivos")?.Value, out bool lpnActivos);

            if (int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "agenda")?.Value, out int parsedValue))
                numeroAgenda = parsedValue;

            var dbQuery = new ConsultaDeLpnQuery(numeroAgenda, lpnActivos);

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.Sorts);
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            return this._gridValidationService.Validate(new STO700GridValidationModule(), grid, row, context);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber("STO700 Modificación de Id Packing");
            uow.BeginTransaction();

            try
            {
                foreach (var row in grid.Rows)
                {
                    var nuLpn = long.Parse(row.GetCell("NU_LPN").Value);
                    var lpn = uow.ManejoLpnRepository.GetLpn(nuLpn);

                    if (lpn.Estado == EstadosLPN.Finalizado)
                        throw new ValidationFailedException("STO700_Sec0_Error_LpnFinalizado");

                    lpn.IdPacking = row.GetCell("ID_PACKING").Value;
                    lpn.FechaModificacion = DateTime.Now;
                    lpn.NumeroTransaccion = uow.GetTransactionNumber();

                    uow.ManejoLpnRepository.UpdateLpn(lpn);
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ValidationFailedException ex)
            {
                uow.Rollback();
                _logger.LogError(ex, ex.Message);
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                uow.Rollback();
                _logger.LogError(ex, ex.Message);
                context.AddErrorNotification(ex.Message);
            }

            return grid;
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                switch (context.ButtonId)
                {
                    case "btnAtributos":
                        context.Redirect("/stock/STO710", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "numeroLPN", Value = context.Row.GetCell("NU_LPN").Value },
                            new ComponentParameter(){ Id = "detalle", Value = "false" },
                        });
                        break;
                    case "btnContenido":
                        context.Redirect("/stock/STO720", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "numeroLPN", Value = context.Row.GetCell("NU_LPN").Value },
                        });
                        break;
                    case "btnHistorial":
                        context.Redirect("/stock/STO721", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "tipo", Value = context.Row.GetCell("DS_LPN_TIPO").Value },
                            new ComponentParameter(){ Id = "tipoLpn", Value = context.Row.GetCell("TP_LPN_TIPO").Value },
                            new ComponentParameter(){ Id = "numeroLpn", Value = context.Row.GetCell("NU_LPN").Value },
                            new ComponentParameter(){ Id = "codigoEmpresa", Value = context.Row.GetCell("CD_EMPRESA").Value },
                            new ComponentParameter(){ Id = "empresa", Value = context.Row.GetCell("NM_EMPRESA").Value },
                        });
                        break;
                    case "btnFinalizar":
                        FinalizarLpnImportado(context, uow);
                        break;
                }
            }
            catch (ValidationFailedException ex)
            {
                _logger.LogError(ex, ex.Message);
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.AddErrorNotification(ex.Message);
            }

            return context;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var filasSeleccionadas = this.ObtenerKeyLineasSeleccionadas(uow, context);

            if (filasSeleccionadas.Count > 0)
                context.AddParameter("ListaFilasSeleccionadas", JsonConvert.SerializeObject(filasSeleccionadas));
            else
                throw new MissingParameterException("General_Sec0_Error_impresionSinSeleccion");

            return context;
        }

        #region Metodos Auxiliares

        public virtual void FinalizarLpnImportado(GridButtonActionContext context, IUnitOfWork uow)
        {
            var nuLpn = long.Parse(context.Row.GetCell("NU_LPN").Value);
            var lpn = uow.ManejoLpnRepository.GetLpn(nuLpn);

            var transactionTO = this._concurrencyControl.CreateTransaccion();
            uow.CreateTransactionNumber("STO700 Finalizar LPN importado");
            uow.BeginTransaction();

            try
            {
                if (lpn.Estado != EstadosLPN.Importado || !string.IsNullOrEmpty(lpn.Ubicacion) || lpn.NroAgenda != null)
                    throw new ValidationFailedException("STO700_Sec0_Error_LpnNoFinalizable");

                this._concurrencyControl.AddLock("T_LPN", nuLpn.ToString(), transactionTO, true);

                lpn.Estado = EstadosLPN.Finalizado;
                lpn.FechaModificacion = DateTime.Now;
                lpn.FechaFin = DateTime.Now;
                lpn.NumeroTransaccion = uow.GetTransactionNumber();
                uow.ManejoLpnRepository.UpdateLpn(lpn);

                context.AddSuccessNotification("STO700_Sec0_Success_FinalizarLpn", new List<string> { nuLpn.ToString() });

                uow.SaveChanges();
                uow.Commit();
            }
            catch
            {
                uow.Rollback();
                throw;
            }
            finally
            {
                this._concurrencyControl.DeleteTransaccion(transactionTO);
            }
        }

        public virtual List<string> ObtenerKeyLineasSeleccionadas(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            int? numeroAgenda = null;
            bool.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "lpnsActivos")?.Value, out bool lpnActivos);

            if (int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "agenda")?.Value, out int parsedValue))
                numeroAgenda = parsedValue;

            var dbQuery = new ConsultaDeLpnQuery(numeroAgenda, lpnActivos);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            List<string> resultado = new List<string>();

            string numeroLpns;

            if (context.Selection.AllSelected)
            {
                var selectAll = dbQuery.GetResult().Select(g => new { g.NU_LPN }).ToList();

                foreach (var noSeleccionKeys in context.Selection.Keys)
                {
                    numeroLpns = noSeleccionKeys;

                    selectAll.Remove(selectAll.FirstOrDefault(z => z.NU_LPN == long.Parse(numeroLpns)));
                }

                foreach (var key in selectAll)
                {
                    resultado.Add(string.Join("$", new List<string> { key.NU_LPN.ToString() }));
                }
            }
            else
            {
                foreach (var key in context.Selection.Keys)
                {
                    resultado.Add(key);
                }
            }

            return resultado;
        }

        #endregion
    }
}
