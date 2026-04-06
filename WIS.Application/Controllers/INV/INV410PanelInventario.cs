using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Security;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries;
using WIS.Domain.DataModel.Queries.Inventario;
using WIS.Domain.DataModel.Queries.Stock;
using WIS.Domain.General.Enums;
using WIS.Domain.Inventario;
using WIS.Domain.Inventario.Factories;
using WIS.Domain.Services.Interfaces;
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
    public class INV410PanelInventario : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITaskQueueService _taskQueue;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly ILogger<INV410PanelInventario> _logger;

        protected InventarioGridConfig _gridConfig;

        public INV410PanelInventario(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridValidationService gridValidationService,
            IFormValidationService formValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ISecurityService security,
            ITaskQueueService taskQueue,
            ITrafficOfficerService concurrencyControl,
            ILogger<INV410PanelInventario> logger)
        {
            SetGridConfig();

            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridValidationService = gridValidationService;
            this._formValidationService = formValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._security = security;
            this._taskQueue = taskQueue;
            this._concurrencyControl = concurrencyControl;
            this._logger = logger;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            switch (grid.Id)
            {
                case "INV410_grid_1": return InitializeGridInventario(grid, context);

                case "INV410_grid_Ubicacion": return InitializeGridUbicaciones(grid, context);
                case "INV410_grid_UbicacionQuitar": return InitializeGridUbicacionesQuitar(grid, context);

                case "INV410_grid_Registros": return InitializeGridRegistros(grid, context);
                case "INV410_grid_RegistrosQuitar": return InitializeGridRegistrosQuitar(grid, context);

                case "INV410_grid_Lpn": return InitializeGridLpn(grid, context);
                case "INV410_grid_LpnQuitar": return InitializeGridLpnQuitar(grid, context);
                case "INV410_grid_LpnAtrCab": return InitializeGridLpnAtriCab(grid, context);

                case "INV410_grid_DetalleLpn": return InitializeGridDetalleLpn(grid, context);
                case "INV410_grid_DetalleLpnQuitar": return InitializeGridDetalleLpnQuitar(grid, context);
                case "INV410_grid_DetalleLpnAtrCab": return InitializeGridDetalleLpnAtriCab(grid, context);
                case "INV410_grid_DetalleLpnAtrDet": return InitializeGridDetalleLpnAtriDet(grid, context);
            }

            return grid;
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            switch (grid.Id)
            {
                case "INV410_grid_1": return FetchRowsInventario(grid, context);

                case "INV410_grid_Ubicacion": return FetchRowsUbicaciones(grid, context);
                case "INV410_grid_UbicacionQuitar": return FetchRowsUbicacionesQuitar(grid, context);

                case "INV410_grid_Registros": return FetchRowsRegistros(grid, context);
                case "INV410_grid_RegistrosQuitar": return FetchRowsRegistrosQuitar(grid, context);

                case "INV410_grid_Lpn": return FetchRowsLpn(grid, context);
                case "INV410_grid_LpnQuitar": return FetchRowsLpnQuitar(grid, context);
                case "INV410_grid_LpnAtrCab": return FetchRowsLpnAtriCab(grid, context);

                case "INV410_grid_DetalleLpn": return FetchRowsDetalleLpn(grid, context);
                case "INV410_grid_DetalleLpnQuitar": return FetchRowsDetalleLpnQuitar(grid, context);
                case "INV410_grid_DetalleLpnAtrCab": return FetchRowsDetalleLpnAtriCab(grid, context);
                case "INV410_grid_DetalleLpnAtrDet": return FetchRowsDetalleLpnAtriDet(grid, context);
            }

            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            switch (grid.Id)
            {
                case "INV410_grid_1": return FetchStatsInventario(grid, context);

                case "INV410_grid_Ubicacion": return FetchStatsUbicaciones(grid, context);
                case "INV410_grid_UbicacionQuitar": return FetchStatsUbicacionesQuitar(grid, context);

                case "INV410_grid_Registros": return FetchStatsRegistros(grid, context);
                case "INV410_grid_RegistrosQuitar": return FetchStatsRegistrosQuitar(grid, context);

                case "INV410_grid_Lpn": return FetchStatsLpn(grid, context);
                case "INV410_grid_LpnQuitar": return FetchStatsLpnQuitar(grid, context);
                case "INV410_grid_LpnAtrCab": return FetchStatsLpnAtriCab(grid, context);

                case "INV410_grid_DetalleLpn": return FetchStatsDetalleLpn(grid, context);
                case "INV410_grid_DetalleLpnQuitar": return FetchStatsDetalleLpnQuitar(grid, context);
                case "INV410_grid_DetalleLpnAtrCab": return FetchStatsDetalleLpnAtriCab(grid, context);
                case "INV410_grid_DetalleLpnAtrDet": return FetchStatsDetalleLpnAtriDet(grid, context);
            }

            return null;
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            switch (grid.Id)
            {
                case "INV410_grid_1": return ExportExcelInventario(grid, context);

                case "INV410_grid_Ubicacion": return ExportExcelUbicaciones(grid, context);
                case "INV410_grid_UbicacionQuitar": return ExportExcelUbicacionesQuitar(grid, context);

                case "INV410_grid_Registros": return ExportExcelRegistros(grid, context);
                case "INV410_grid_RegistrosQuitar": return ExportExcelRegistrosQuitar(grid, context);

                case "INV410_grid_Lpn": return ExportExcelLpn(grid, context);
                case "INV410_grid_LpnQuitar": return ExportExcelLpnQuitar(grid, context);
                case "INV410_grid_LpnAtrCab": return ExportExcelLpnAtriCab(grid, context);

                case "INV410_grid_DetalleLpn": return ExportExcelDetalleLpn(grid, context);
                case "INV410_grid_DetalleLpnQuitar": return ExportExcelDetalleLpnQuitar(grid, context);
                case "INV410_grid_DetalleLpnAtrCab": return ExportExcelDetalleLpnAtriCab(grid, context);
                case "INV410_grid_DetalleLpnAtrDet": return ExportExcelDetalleLpnAtriDet(grid, context);
            }

            return null;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                uow.CreateTransactionNumber("GridCommit");

                grid.Rows.ForEach(w =>
                {
                    if (decimal.TryParse(w.GetCell("NU_INVENTARIO").Value, this._identity.GetFormatProvider(), out decimal nuInventario))
                    {
                        var inventario = uow.InventarioRepository.GetInventario(nuInventario);

                        if (inventario == null)
                            throw new ValidationFailedException("WINV410_Sec0_Error_Er003_NoSeEncontroInventarioX", new string[] { nuInventario.ToString() });
                        else if (!inventario.IsEditable())
                            throw new ValidationFailedException("WINV410_Sec0_Error_Er004_ImposibleEditarInventarioEstado");

                        if ((w.GetCell("FL_EXCLUIR_LPNS").Value != w.GetCell("FL_EXCLUIR_LPNS").Old || w.GetCell("FL_EXCLUIR_SUELTOS").Value != w.GetCell("FL_EXCLUIR_SUELTOS").Old) && uow.InventarioRepository.TieneUbicaciones(nuInventario))
                        {
                            throw new ValidationFailedException("INV410_Sec0_Error_Er005_ElInventarioYaTieneRegistrosAsociado");
                        }

                        if (w.GetCell("ND_CIERRE_CONTEO").Value == TipoCierreConteoInventario.UnConteo && w.GetCell("FL_GENERAR_PRIMER_CONTEO").Value != "N")
                            throw new ValidationFailedException("INV410_msg_Error_TipoConteoNoPermiteGenerarPrimerConteo");

                        inventario.Descripcion = w.GetCell("DS_INVENTARIO").Value;
                        inventario.ActualizarConteoFin = w.GetCell("FL_ACTUALIZAR_CONTEO_FIN_AUTO").Value == "S";
                        inventario.BloquearConteoConsecutivoUsuario = w.GetCell("FL_BLOQ_USR_CONTEO_SUCESIVO").Value == "S";
                        inventario.ControlarVencimiento = w.GetCell("FL_CONTROLAR_VENCIMIENTO").Value == "S";
                        inventario.ModificarStockEnDiferencia = w.GetCell("FL_MODIFICAR_STOCK_EN_DIF").Value == "S";
                        inventario.PermiteIngresarMotivo = w.GetCell("FL_PERMITE_INGRESAR_MOTIVO").Value == "S";
                        inventario.ExcluirSueltos = w.GetCell("FL_EXCLUIR_SUELTOS").Value == "S";
                        inventario.ExcluirLpns = w.GetCell("FL_EXCLUIR_LPNS").Value == "S";
                        inventario.RestablecerLpnFinalizado = w.GetCell("FL_RESTABLECER_LPN_FINALIZADO").Value == "S";
                        inventario.CierreConteo = w.GetCell("ND_CIERRE_CONTEO").Value;
                        inventario.NumeroTransaccion = uow.GetTransactionNumber();
                        inventario.GenerarPrimerConteo = w.GetCell("FL_GENERAR_PRIMER_CONTEO").Value == "S";
                        inventario.PermiteUbicacionesDeOtrosInventarios = w.GetCell("FL_PERMITE_ASOC_UBIC_OTRO_INV").Value == "S";

                        uow.InventarioRepository.UpdateInventario(inventario);
                        uow.SaveChanges();
                    }
                });

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, new List<string>(ex.StrArguments ?? new string[0]));
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
            }

            return grid;
        }
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new INV410GridValidationModule(uow), grid, row, context);
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                var keys = new List<string>();

                if (!decimal.TryParse(context.Row.GetCell("NU_INVENTARIO").Value, this._identity.GetFormatProvider(), out decimal nuInventario))
                    throw new ValidationFailedException("INV_Format_Error_NuInventario");

                var inventario = uow.InventarioRepository.GetInventario(nuInventario);

                if (inventario == null)
                    throw new ValidationFailedException("INV410_Sec0_Error_InventarioNoExiste", new string[] { context.Row.GetCell("NU_INVENTARIO").Value });

                if (context.GridId == "INV410_grid_1")
                {
                    var logic = new InventarioLogic(this._identity, this._concurrencyControl);

                    switch (context.ButtonId)
                    {
                        case "btnRegistrosStock":
                            {
                                if (inventario.IsEditable())
                                    RedirectInventario(context, "INV413", nuInventario);
                            }
                            break;
                        case "btnUbicacionesDeInventario":
                            {
                                if (inventario.IsEditable())
                                    RedirectInventario(context, "INV411", nuInventario);
                            }
                            break;
                        case "btnAgregarRegistrosLpn":
                            {
                                if (inventario.IsEditable())
                                    RedirectInventario(context, "INV416", nuInventario);
                            }
                            break;
                        case "btnAgregarRegistrosDetalleLpn":
                            {
                                if (inventario.IsEditable())
                                    RedirectInventario(context, "INV417", nuInventario);
                            }
                            break;
                        case "btnHabilitarInventario":
                            {
                                uow.CreateTransactionNumber("HabilitarInventario");
                                logic.HabilitarInventario(uow, inventario, out Domain.Validation.Error info);

                                if (info != null)
                                {
                                    var arguments = info.GetArgumentos();
                                    context.AddInfoNotification(info.Mensaje, arguments);
                                }
                            }
                            break;
                        case "btnDetalleDeConteos":
                            {
                                if (uow.InventarioRepository.TieneConteos(nuInventario))
                                    RedirectInventario(context, "INV412", nuInventario);
                            }
                            break;
                        case "btnConteosPendientes":
                            {
                                if (uow.InventarioRepository.TieneConteoPendiente(nuInventario))
                                    RedirectInventario(context, "INV412", nuInventario, showOnlyPending: true);
                            }
                            break;
                        case "btnAvanceInventario":
                            {
                                if (uow.InventarioRepository.TieneConteos(nuInventario))
                                    RedirectInventario(context, "INV050", nuInventario);
                            }
                            break;
                        case "btnCancelarInventario":
                            {
                                uow.CreateTransactionNumber("CancelarInventario");
                                logic.CancelarInventario(uow, inventario);
                            }
                            break;
                        case "btnCierreParcial":
                            {
                                uow.CreateTransactionNumber("CierreParcial");
                                logic.CierreParcial(uow, inventario, out keys);
                            }
                            break;
                        case "btnCerrarInventario":
                            {
                                uow.CreateTransactionNumber("CerrarInventario");
                                logic.CerrarInventario(uow, inventario, out keys);
                            }
                            break;
                        case "btnRegenerarInventario":
                            {
                                uow.CreateTransactionNumber("RegenerarInventario");
                                logic.RegenerarInventario(uow, inventario);
                            }
                            break;
                        case "btnActualizarInventario":
                            {
                                if (uow.InventarioRepository.TieneConteosFinDif(nuInventario))
                                    RedirectInventario(context, "INV414", nuInventario);
                            }
                            break;
                        case "btnInvEnderecoDetError":
                            {
                                RedirectInventario(context, "INV415", nuInventario);
                            }
                            break;
                        default:
                            break;
                    }

                    uow.SaveChanges();
                    uow.Commit();

                    if (context.Redirection == null)
                        context.AddSuccessNotification("General_Db_Success_Update");
                }
                else if (context.GridId == "INV410_grid_DetalleLpn" || context.GridId == "INV410_grid_DetalleLpnQuitar")
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

                if (_taskQueue.IsEnabled() && keys.Any())
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.AjustesDeStock, keys);
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                _logger.LogError(ex, "GridButtonAction");
                uow.Rollback();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GridButtonAction");
                uow.Rollback();
                throw;
            }

            return context;
        }
        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            switch (context.GridId)
            {
                case "INV410_grid_Ubicacion": return GridMenuItemUbicacion(context);
                case "INV410_grid_UbicacionQuitar": return GridMenuItemUbicacionQuitar(context);

                case "INV410_grid_Registros": return GridMenuItemRegistros(context);
                case "INV410_grid_RegistrosQuitar": return GridMenuItemRegistrosQuitar(context);

                case "INV410_grid_Lpn": return GridMenuItemLpn(context);
                case "INV410_grid_LpnQuitar": return GridMenuItemLpnQuitar(context);

                case "INV410_grid_DetalleLpn": return GridMenuItemDetalleLpn(context);
                case "INV410_grid_DetalleLpnQuitar": return GridMenuItemDetalleLpnQuitar(context);
            }

            return context;
        }
        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            switch (context.ColumnId)
            {
                case "CD_EMPRESA":
                    return this.SearchEmpresa(grid, row, context);
            }

            return new List<SelectOption>();
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ResetForm(uow, form);

            InicializarSelects(uow, form);

            return form;
        }
        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            if (context.ButtonId != "btnAplicarFiltro")
            {
                try
                {
                    using var uow = this._uowFactory.GetUnitOfWork();

                    uow.CreateTransactionNumber("FormSubmit");

                    var fieldNroInventario = form.GetField("nuInventario");
                    var objInventario = GenerarInventario(uow, form, context);

                    if (objInventario.CierreConteo == TipoCierreConteoInventario.UnConteo && objInventario.GenerarPrimerConteo)
                        throw new ValidationFailedException("INV410_msg_Error_TipoConteoNoPermiteGenerarPrimerConteo");

                    if (decimal.TryParse(fieldNroInventario.Value, _identity.GetFormatProvider(), out decimal nroInventario))
                    {
                        var inventario = uow.InventarioRepository.GetInventario(nroInventario);
                        if (inventario == null)
                            throw new ValidationFailedException("WINV410_Sec0_Error_Er003_NoSeEncontroInventarioX", new string[] { nroInventario.ToString(_identity.GetFormatProvider()) });

                        inventario.Descripcion = objInventario.Descripcion;
                        inventario.Predio = objInventario.Predio;
                        inventario.ActualizarConteoFin = objInventario.ActualizarConteoFin;
                        inventario.BloquearConteoConsecutivoUsuario = objInventario.BloquearConteoConsecutivoUsuario;
                        inventario.ControlarVencimiento = objInventario.ControlarVencimiento;
                        inventario.ModificarStockEnDiferencia = objInventario.ModificarStockEnDiferencia;
                        inventario.PermiteIngresarMotivo = objInventario.PermiteIngresarMotivo;
                        inventario.CierreConteo = objInventario.CierreConteo;
                        inventario.NumeroTransaccion = objInventario.NumeroTransaccion;
                        inventario.ExcluirSueltos = objInventario.ExcluirSueltos;
                        inventario.ExcluirLpns = objInventario.ExcluirLpns;
                        inventario.RestablecerLpnFinalizado = objInventario.RestablecerLpnFinalizado;
                        inventario.GenerarPrimerConteo = objInventario.GenerarPrimerConteo;
                        inventario.PermiteUbicacionesDeOtrosInventarios = objInventario.PermiteUbicacionesDeOtrosInventarios;

                        uow.InventarioRepository.UpdateInventario(inventario);
                        uow.SaveChanges();
                    }
                    else
                    {
                        nroInventario = uow.InventarioRepository.GetNextNuInventario();

                        objInventario.NumeroInventario = nroInventario;

                        var descripcion = form.GetField("descInventario").Value;

                        if (string.IsNullOrEmpty(objInventario.Descripcion))
                            objInventario.Descripcion = $"Inventario Nro. {nroInventario}";

                        uow.InventarioRepository.AddInventario(objInventario);

                        uow.SaveChanges();

                        fieldNroInventario.Value = nroInventario.ToString(_identity.GetFormatProvider());
                        form.GetField("descInventario").Value = objInventario.Descripcion;
                        form.GetField("empresa").Disabled = true;
                    }
                }
                catch (Exception ex)
                {
                    context.AddErrorNotification("INV_Db_Error_Insert");
                    throw ex;
                }
            }

            return form;
        }
        public override Form FormButtonAction(Form form, FormButtonActionContext context)
        {
            if (context.ButtonId == "showFormButton")
            {
                using var uow = this._uowFactory.GetUnitOfWork();
                ResetForm(uow, form);
                InicializarSelects(uow, form);
            }
            else if (context.ButtonId == "btnLoadSummary")
            {
                var nroInventario = decimal.Parse(context.GetParameter("nuInventario"), _identity.GetFormatProvider());

                using var uow = this._uowFactory.GetUnitOfWork();

                var inventario = uow.InventarioRepository.GetInventario(nroInventario);

                if (inventario == null)
                    throw new ValidationFailedException("WINV410_Sec0_Error_Er003_NoSeEncontroInventarioX", new string[] { nroInventario.ToString() });

                //No es lo mejor, pero funciona por ahora
                context.Parameters.Clear();

                context.AddParameter("inventario", inventario.NumeroInventario.ToString(_identity.GetFormatProvider()));
                context.AddParameter("empresa", inventario.Empresa?.ToString());
                context.AddParameter("cierreConteo", inventario.CierreConteo);
                context.AddParameter("descripcion", inventario.Descripcion);
                context.AddParameter("nombreEmpresa", uow.EmpresaRepository.GetNombre(inventario.Empresa ?? -1));
                context.AddParameter("cierreConteoDescripcion", uow.DominioRepository.GetDominio(inventario.CierreConteo)?.Descripcion);
                context.AddParameter("actualizarConteoFin", inventario.ActualizarConteoFin ? "S" : "N");
                context.AddParameter("bloquearConteosSucesivos", inventario.BloquearConteoConsecutivoUsuario ? "S" : "N");
                context.AddParameter("controlarVencimiento", inventario.ControlarVencimiento ? "S" : "N");
                context.AddParameter("modificarStockEnDiferencia", inventario.ModificarStockEnDiferencia ? "S" : "N");
                context.AddParameter("permiteIngresarMotivo", inventario.PermiteIngresarMotivo ? "S" : "N");
                context.AddParameter("excluirSueltos", inventario.ExcluirSueltos ? "S" : "N");
                context.AddParameter("excluirLpns", inventario.ExcluirLpns ? "S" : "N");
                context.AddParameter("restablecerLpnFinalizado", inventario.RestablecerLpnFinalizado ? "S" : "N");
                context.AddParameter("generarPrimerConteo", inventario.GenerarPrimerConteo ? "S" : "N");
                context.AddParameter("permiteAsociarUbicOtrosInv", inventario.PermiteUbicacionesDeOtrosInventarios ? "S" : "N");
            }
            else if (context.ButtonId == "btnHabilitar")
            {
                using var uow = this._uowFactory.GetUnitOfWork();
                uow.BeginTransaction();

                try
                {
                    var logic = new InventarioLogic(this._identity, this._concurrencyControl);

                    var nuInv = context.GetParameter("nuInventario");

                    if (!decimal.TryParse(nuInv, this._identity.GetFormatProvider(), out decimal nroInventario))
                        throw new ValidationFailedException("INV_Format_Error_NuInventario");

                    var inventario = uow.InventarioRepository.GetInventario(nroInventario);
                    if (inventario == null)
                        throw new ValidationFailedException("INV410_Sec0_Error_InventarioNoExiste", new string[] { nuInv });

                    if (!inventario.IsEditable())
                        throw new ValidationFailedException("INV410_Sec0_Error_ImposibleEditarInv");

                    uow.CreateTransactionNumber("FormButtonAction - Habilitar");

                    logic.HabilitarInventario(uow, inventario, out Domain.Validation.Error info);

                    uow.SaveChanges();
                    uow.Commit();

                    if (info != null)
                    {
                        var arguments = info.GetArgumentos();
                        context.AddInfoNotification(info.Mensaje, arguments);
                    }
                    else
                        context.AddSuccessNotification("INV410_Sec0_Success_InventarioXHabilitado", new List<string> { nroInventario.ToString() });
                }
                catch (ValidationFailedException ex)
                {
                    context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                    _logger.LogError(ex, "FormButtonAction - Habilitar");
                    uow.Rollback();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "FormButtonAction - Habilitar");
                    uow.Rollback();
                    throw;
                }
            }

            return form;
        }
        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new INV410FormValidationModule(uow, this._identity), form, context);
        }
        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "empresa":
                    return this.SearchEmpresa(form, context);
                default:
                    return new List<SelectOption>();
            }
        }

        #region Metodos Auxiliares

        #region GridInitialize

        public virtual Grid InitializeGridInventario(Grid grid, GridInitializeContext context)
        {
            List<IGridItem> items = new List<IGridItem>();

            context.IsEditingEnabled = true;
            context.IsAddEnabled = false;
            context.IsRemoveEnabled = false;

            using var uow = this._uowFactory.GetUnitOfWork();

            if (this._security.IsUserAllowed(SecurityResources.INV411_Page_Access_Allow))
                items.Add(new GridButton("btnUbicacionesDeInventario", "INV410_Sec0_btn_SeleccionUbicaciones", "fas fa-wrench"));

            if (this._security.IsUserAllowed(SecurityResources.INV413_Page_Access_Allow))
                items.Add(new GridButton("btnRegistrosStock", "INV410_Sec0_btn_SeleccionRegistros", "fas fa-wrench"));

            if (this._security.IsUserAllowed(SecurityResources.INV416_Page_Access_Allow))
                items.Add(new GridButton("btnAgregarRegistrosLpn", "INV410_Sec0_btn_AgregarRegistrosLpn", "fas fa-wrench"));

            if (this._security.IsUserAllowed(SecurityResources.INV417_Page_Access_Allow))
                items.Add(new GridButton("btnAgregarRegistrosDetalleLpn", "INV410_Sec0_btn_AgregarRegistrosDetalleLpn", "fas fa-wrench"));

            if (this._security.IsUserAllowed(SecurityResources.WINV410_grid1_btn_HabilitarInventario))
                items.Add(new GridButton("btnHabilitarInventario", "INV410_Sec0_btn_Habilitar", "fas fa-wrench"));

            if (this._security.IsUserAllowed(SecurityResources.INV412_Page_Access_Allow))
            {
                items.Add(new GridButton("btnDetalleDeConteos", "INV410_Sec0_btn_DetallesConteos", "fas fa-wrench"));
                items.Add(new GridButton("btnConteosPendientes", "INV410_Sec0_btn_ConteosPendientes", "fas fa-wrench"));
            }

            if (this._security.IsUserAllowed(SecurityResources.WINV050_Page_Access_AvanceInventario))
                items.Add(new GridButton("btnAvanceInventario", "INV410_Sec0_btn_VerAvance", "fas fa-wrench"));

            if (this._security.IsUserAllowed(SecurityResources.WINV410_grid1_btn_CancelarInventario))
                items.Add(new GridButton("btnCancelarInventario", "INV410_Sec0_btn_Cancelar", "fas fa-wrench"));

            if (this._security.IsUserAllowed(SecurityResources.WINV410_grid1_btn_CerrarInventarioParcial))
                items.Add(new GridButton("btnCierreParcial", "INV410_Sec0_btn_CierreParcial", "fas fa-wrench", new ConfirmMessage("INV410_Sec0_msg_FinalizarInventario")));

            if (this._security.IsUserAllowed(SecurityResources.WINV410_grid1_btn_CerrarInventario))
                items.Add(new GridButton("btnCerrarInventario", "INV410_Sec0_btn_Cerrar", "fas fa-wrench", new ConfirmMessage("INV410_Sec0_msg_FinalizarInventario")));

            if (this._security.IsUserAllowed(SecurityResources.WINV410_grid1_btn_RegenerarInventario))
                items.Add(new GridButton("btnRegenerarInventario", "INV410_Sec0_btn_Regenerar", "fas fa-wrench"));

            if (this._security.IsUserAllowed(SecurityResources.INV414_Page_Access_Allow))
                items.Add(new GridButton("btnActualizarInventario", "INV410_Sec0_btn_ActualizarInventario", "fas fa-wrench"));

            if (this._security.IsUserAllowed(SecurityResources.INV415_Page_Access_Allow))
                items.Add(new GridButton("btnInvEnderecoDetError", "WINV410_Sec0_btn_InvEnderecoDetError", "fas fa-wrench"));

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", items));
            grid.AddOrUpdateColumn(new GridColumnSelect("ND_CIERRE_CONTEO", this.OptionSelectCierreConteo()));
            grid.AddOrUpdateColumn(new GridColumnSelect("TP_INVENTARIO", this.OptionSelectTiposInventario()));
            grid.AddOrUpdateColumn(new GridColumnSelect("ND_ESTADO_INVENTARIO", this.OptionSelectEstadoInventario()));

            context.AddLink("CD_EMPRESA", "/registro/REG100");

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public virtual Grid InitializeGridUbicaciones(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems.Add(new GridButton
            {
                Id = "btnConfirmarUbicacion",
                Label = "INV410_Sec0_btn_AgregarUbicacion"
            });

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public virtual Grid InitializeGridUbicacionesQuitar(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems.Add(new GridButton
            {
                Id = "btnConfirmarUbicacion",
                Label = "INV410_Sec0_btn_QuitarUbicacion"
            });

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public virtual Grid InitializeGridRegistros(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems.Add(new GridButton
            {
                Id = "btnConfirmarRegistros",
                Label = "INV410_Sec0_btn_AgregarRegistro"
            });

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public virtual Grid InitializeGridRegistrosQuitar(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems.Add(new GridButton
            {
                Id = "btnConfirmarRegistros",
                Label = "INV410_Sec0_btn_QuitarRegistro"
            });

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public virtual Grid InitializeGridLpn(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems.Add(new GridButton
            {
                Id = "btnAgregar",
                Label = "INV410_Sec0_btn_AgregarRegistro"
            });

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public virtual Grid InitializeGridLpnQuitar(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems.Add(new GridButton
            {
                Id = "btnEliminar",
                Label = "INV410_Sec0_btn_QuitarRegistro"
            });

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public virtual Grid InitializeGridLpnAtriCab(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsAddEnabled = false;
            context.IsCommitEnabled = false;
            context.IsRemoveEnabled = false;
            context.IsRollbackEnabled = false;

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public virtual Grid InitializeGridDetalleLpn(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem>
            {
                new GridButton("btnAtributosCabezal", "INV410_Sec0_btn_AtributosCabezal", "fas fa-list"),
                new GridButton("btnAtributosDetalle", "INV410_Sec0_btn_AtributosDetalle", "fas fa-list"),
            }));

            grid.MenuItems.Add(new GridButton
            {
                Id = "btnAgregar",
                Label = "INV410_Sec0_btn_AgregarRegistro"
            });
            return this.GridFetchRows(grid, context.FetchContext);
        }
        public virtual Grid InitializeGridDetalleLpnQuitar(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem>
            {
                new GridButton("btnAtributosCabezal", "INV410_Sec0_btn_AtributosCabezal", "fas fa-list"),
                new GridButton("btnAtributosDetalle", "INV410_Sec0_btn_AtributosDetalle", "fas fa-list"),
            }));

            grid.MenuItems.Add(new GridButton
            {
                Id = "btnEliminar",
                Label = "INV410_Sec0_btn_QuitarRegistro"
            });

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public virtual Grid InitializeGridDetalleLpnAtriCab(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsAddEnabled = false;
            context.IsCommitEnabled = false;
            context.IsRemoveEnabled = false;
            context.IsRollbackEnabled = false;

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public virtual Grid InitializeGridDetalleLpnAtriDet(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsAddEnabled = false;
            context.IsCommitEnabled = false;
            context.IsRemoveEnabled = false;
            context.IsRollbackEnabled = false;

            return this.GridFetchRows(grid, context.FetchContext);
        }

        #endregion

        #region GridFetchRows

        public virtual Grid FetchRowsInventario(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new InventarioQuery();
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, _gridConfig.DefaultSortsInventario, _gridConfig.GridKeysInventario);

            grid.SetEditableCells(new List<string> { "DS_INVENTARIO", "ND_CIERRE_CONTEO", "FL_ACTUALIZAR_CONTEO_FIN_AUTO", "FL_CONTROLAR_VENCIMIENTO", "FL_BLOQ_USR_CONTEO_SUCESIVO", "FL_MODIFICAR_STOCK_EN_DIF", "FL_PERMITE_INGRESO_MOTIVO", "FL_RESTABLECER_LPN_FINALIZADO", "FL_GENERAR_PRIMER_CONTEO" });

            grid.Rows.ForEach(w =>
            {
                var nuInventario = decimal.Parse(w.GetCell("NU_INVENTARIO").Value, _identity.GetFormatProvider());
                var estadoInventario = w.GetCell("ND_ESTADO_INVENTARIO").Value;

                w.DisabledButtons = new List<string>()
                {
                    "btnRegistrosStock",
                    "btnUbicacionesDeInventario",
                    "btnHabilitarInventario",
                    "btnAvanceInventario",
                    "btnCancelarInventario",
                    "btnCierreParcial",
                    "btnCerrarInventario",
                    "btnActualizarInventario",
                    "btnDetalleDeConteos",
                    "btnConteosPendientes",
                    "btnRegenerarInventario",
                    "btnInvEnderecoDetError",
                    "btnAgregarRegistrosLpn",
                    "btnAgregarRegistrosDetalleLpn"
                };

                if (estadoInventario != EstadoInventario.EnEdicion)
                    w.SetEditableCells(new List<string>());

                if (estadoInventario == EstadoInventario.EnProceso)
                {
                    w.DisabledButtons.Remove("btnCancelarInventario");

                    if (uow.InventarioRepository.TieneConteos(nuInventario))
                        w.DisabledButtons.Remove("btnAvanceInventario");

                    if (uow.InventarioRepository.TieneConteoPendiente(nuInventario) && uow.InventarioRepository.TieneConteoFinalizado(nuInventario))
                        w.DisabledButtons.Remove("btnCierreParcial");

                    if (!uow.InventarioRepository.TieneConteoPendiente(nuInventario))
                        w.DisabledButtons.Remove("btnCerrarInventario");

                    if (uow.InventarioRepository.TieneConteosFinDif(nuInventario))
                        w.DisabledButtons.Remove("btnActualizarInventario");
                }
                else if (estadoInventario == EstadoInventario.EnEdicion)
                {
                    w.DisabledButtons.Remove("btnCancelarInventario");

                    if (uow.InventarioRepository.TieneUbicaciones(nuInventario))
                        w.DisabledButtons.Remove("btnHabilitarInventario");
                    else
                    {
                        w.GetCell("FL_EXCLUIR_SUELTOS").Editable = true;
                        w.GetCell("FL_EXCLUIR_LPNS").Editable = true;
                    }

                    if (w.GetCell("TP_INVENTARIO").Value == TipoInventario.Ubicacion)
                        w.DisabledButtons.Remove("btnUbicacionesDeInventario");

                    if (w.GetCell("TP_INVENTARIO").Value == TipoInventario.Registro)
                        w.DisabledButtons.Remove("btnRegistrosStock");

                    if (w.GetCell("TP_INVENTARIO").Value == TipoInventario.Lpn)
                        w.DisabledButtons.Remove("btnAgregarRegistrosLpn");

                    if (w.GetCell("TP_INVENTARIO").Value == TipoInventario.DetalleLpn)
                        w.DisabledButtons.Remove("btnAgregarRegistrosDetalleLpn");
                }

                if (uow.InventarioRepository.TieneConteos(nuInventario))
                    w.DisabledButtons.Remove("btnDetalleDeConteos");

                if (uow.InventarioRepository.TieneConteoPendiente(nuInventario))
                    w.DisabledButtons.Remove("btnConteosPendientes");

                if (((estadoInventario == EstadoInventario.Cancelado) || (estadoInventario == EstadoInventario.Cerrado)) && uow.InventarioRepository.HayConteosFinalizadosOActualizados(nuInventario))
                    w.DisabledButtons.Remove("btnRegenerarInventario");

                if (uow.InventarioRepository.TieneErroresEnUbicaciones(nuInventario))
                    w.DisabledButtons.Remove("btnInvEnderecoDetError");

            });

            return grid;
        }

        public virtual Grid FetchRowsUbicaciones(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nroInventario = context.GetParameter("nuInventario");

            if (string.IsNullOrEmpty(nroInventario))
                return grid;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioUbicacionQuery(filtros);
            uow.HandleQuery(dbQuery, filterEmpresa: false);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, _gridConfig.DefaultSortsUbicacion, _gridConfig.GridKeysUbicacion);

            return grid;
        }
        public virtual Grid FetchRowsUbicacionesQuitar(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nroInventario = context.GetParameter("nuInventario");

            if (string.IsNullOrEmpty(nroInventario))
                return grid;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioUbicacionQuitarQuery(filtros);
            uow.HandleQuery(dbQuery, filterEmpresa: false);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, _gridConfig.DefaultSortsUbicacionQuitar, _gridConfig.GridKeysUbicacionQuitar);

            return grid;
        }

        public virtual Grid FetchRowsRegistros(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nroInventario = context.GetParameter("nuInventario");

            if (string.IsNullOrEmpty(nroInventario))
                return grid;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioRegistrosQuery(filtros);
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, _gridConfig.DefaultSortsRegistro, _gridConfig.GridKeysRegistro);

            return grid;
        }
        public virtual Grid FetchRowsRegistrosQuitar(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nroInventario = context.GetParameter("nuInventario");

            if (string.IsNullOrEmpty(nroInventario))
                return grid;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioRegistroQuitarQuery(filtros);
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, _gridConfig.DefaultSortsRegistroQuitar, _gridConfig.GridKeysRegistroQuitar);

            return grid;
        }

        public virtual Grid FetchRowsLpn(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (string.IsNullOrEmpty(context.GetParameter("nuInventario")))
                return grid;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioLpnDisponiblesQuery(filtros);
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, _gridConfig.DefaultSortsLpn, _gridConfig.GridKeysLpn);

            return grid;
        }
        public virtual Grid FetchRowsLpnQuitar(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (string.IsNullOrEmpty(context.GetParameter("nuInventario")))
                return grid;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioLpnSeleccionadosQuery(filtros);
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, _gridConfig.DefaultSortsLpnQuitar, _gridConfig.GridKeysLpnQuitar);

            return grid;
        }
        public virtual Grid FetchRowsLpnAtriCab(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new InventarioConsultaAtributosLpnQuery();

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, _gridConfig.DefaultSortsAtributos, _gridConfig.GridKeysAtributo);

            grid.SetEditableCells(new List<string>
            {
                "VL_ATRIBUTO"
            });

            return grid;
        }

        public virtual Grid FetchRowsDetalleLpn(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (string.IsNullOrEmpty(context.GetParameter("nuInventario")))
                return grid;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioLpnDetallesDisponiblesQuery(filtros);
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, _gridConfig.DefaultSortsDetalleLpn, _gridConfig.GridKeysDetalleLpn);

            grid.Rows.ForEach(row =>
            {
                row.DisabledButtons = new List<string>()
                {
                    "btnAtributosCabezal",
                    "btnAtributosDetalle",
                };

                if (!string.IsNullOrEmpty(row.GetCell("NU_LPN").Value) && row.GetCell("NU_LPN").Value != "-" &&
                    !string.IsNullOrEmpty(row.GetCell("ID_LPN_DET").Value) && row.GetCell("ID_LPN_DET").Value != "-")
                {
                    row.DisabledButtons = new List<string>();
                }
            });

            return grid;
        }
        public virtual Grid FetchRowsDetalleLpnQuitar(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (string.IsNullOrEmpty(context.GetParameter("nuInventario")))
                return grid;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioLpnDetallesSeleccionadosQuery(filtros);
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, _gridConfig.DefaultSortsDetalleLpnQuitar, _gridConfig.GridKeysDetalleLpnQuitar);

            grid.Rows.ForEach(row =>
            {
                row.DisabledButtons = new List<string>()
                {
                    "btnAtributosCabezal",
                    "btnAtributosDetalle",
                };

                if (!string.IsNullOrEmpty(row.GetCell("NU_LPN").Value) && row.GetCell("NU_LPN").Value != "-" &&
                    !string.IsNullOrEmpty(row.GetCell("ID_LPN_DET").Value) && row.GetCell("ID_LPN_DET").Value != "-")
                {
                    row.DisabledButtons = new List<string>();
                }
            });

            return grid;
        }
        public virtual Grid FetchRowsDetalleLpnAtriCab(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new InventarioConsultaAtributosDetalleLpnCabezalQuery();

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, _gridConfig.DefaultSortsAtributos, _gridConfig.GridKeysAtributo);

            grid.SetEditableCells(new List<string>
            {
                "VL_ATRIBUTO"
            });

            return grid;
        }
        public virtual Grid FetchRowsDetalleLpnAtriDet(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new InventarioConsultaAtributosDetalleLpnDetQuery();

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, _gridConfig.DefaultSortsAtributos, _gridConfig.GridKeysAtributo);

            grid.SetEditableCells(new List<string>
            {
                "VL_ATRIBUTO"
            });

            return grid;
        }
        #endregion

        #region GridFetchStats

        public virtual GridStats FetchStatsInventario(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new InventarioQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public virtual GridStats FetchStatsUbicaciones(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nroInventario = context.GetParameter("nuInventario");

            if (string.IsNullOrEmpty(nroInventario))
                return null;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioUbicacionQuery(filtros);

            uow.HandleQuery(dbQuery, filterEmpresa: false);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public virtual GridStats FetchStatsUbicacionesQuitar(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nroInventario = context.GetParameter("nuInventario");

            if (string.IsNullOrEmpty(nroInventario))
                return null;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioUbicacionQuitarQuery(filtros);
            uow.HandleQuery(dbQuery, filterEmpresa: false);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public virtual GridStats FetchStatsRegistros(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nroInventario = context.GetParameter("nuInventario");

            if (string.IsNullOrEmpty(nroInventario))
                return null;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioRegistrosQuery(filtros);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public virtual GridStats FetchStatsRegistrosQuitar(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nroInventario = context.GetParameter("nuInventario");

            if (string.IsNullOrEmpty(nroInventario))
                return null;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioRegistroQuitarQuery(filtros);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public virtual GridStats FetchStatsLpn(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (string.IsNullOrEmpty(context.GetParameter("nuInventario")))
                return null;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioLpnDisponiblesQuery(filtros);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public virtual GridStats FetchStatsLpnQuitar(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (string.IsNullOrEmpty(context.GetParameter("nuInventario")))
                return null;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioLpnSeleccionadosQuery(filtros);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public virtual GridStats FetchStatsLpnAtriCab(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new InventarioConsultaAtributosLpnQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public virtual GridStats FetchStatsDetalleLpn(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (string.IsNullOrEmpty(context.GetParameter("nuInventario")))
                return null;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioLpnDetallesDisponiblesQuery(filtros);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public virtual GridStats FetchStatsDetalleLpnQuitar(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (string.IsNullOrEmpty(context.GetParameter("nuInventario")))
                return null;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioLpnDetallesSeleccionadosQuery(filtros);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public virtual GridStats FetchStatsDetalleLpnAtriCab(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new InventarioConsultaAtributosDetalleLpnCabezalQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public virtual GridStats FetchStatsDetalleLpnAtriDet(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new InventarioConsultaAtributosDetalleLpnDetQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        #endregion

        #region GridExportExcel
        public virtual byte[] ExportExcelInventario(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new InventarioQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, _gridConfig.DefaultSortsInventario);
        }

        public virtual byte[] ExportExcelUbicaciones(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nroInventario = context.GetParameter("nuInventario");

            if (string.IsNullOrEmpty(nroInventario))
                return null;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioUbicacionQuery(filtros);
            uow.HandleQuery(dbQuery, filterEmpresa: false);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, _gridConfig.DefaultSortsUbicacion);
        }
        public virtual byte[] ExportExcelUbicacionesQuitar(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nroInventario = context.GetParameter("nuInventario");

            if (string.IsNullOrEmpty(nroInventario))
                return null;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioUbicacionQuitarQuery(filtros);
            uow.HandleQuery(dbQuery, filterEmpresa: false);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, _gridConfig.DefaultSortsUbicacionQuitar);
        }

        public virtual byte[] ExportExcelRegistros(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nroInventario = context.GetParameter("nuInventario");

            if (string.IsNullOrEmpty(nroInventario))
                return null;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioRegistrosQuery(filtros);
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, _gridConfig.DefaultSortsRegistro);
        }
        public virtual byte[] ExportExcelRegistrosQuitar(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nroInventario = context.GetParameter("nuInventario");

            if (string.IsNullOrEmpty(nroInventario))
                return null;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioRegistroQuitarQuery(filtros);
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, _gridConfig.DefaultSortsRegistroQuitar);
        }

        public virtual byte[] ExportExcelLpn(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (string.IsNullOrEmpty(context.GetParameter("nuInventario")))
                return null;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioLpnDisponiblesQuery(filtros);
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, _gridConfig.DefaultSortsLpn);
        }
        public virtual byte[] ExportExcelLpnQuitar(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (string.IsNullOrEmpty(context.GetParameter("nuInventario")))
                return null;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioLpnSeleccionadosQuery(filtros);
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, _gridConfig.DefaultSortsLpnQuitar);
        }
        public virtual byte[] ExportExcelLpnAtriCab(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new InventarioConsultaAtributosLpnQuery();
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, _gridConfig.DefaultSortsAtributos);
        }

        public virtual byte[] ExportExcelDetalleLpn(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (string.IsNullOrEmpty(context.GetParameter("nuInventario")))
                return null;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioLpnDetallesDisponiblesQuery(filtros);
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, _gridConfig.DefaultSortsDetalleLpn);
        }
        public virtual byte[] ExportExcelDetalleLpnQuitar(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (string.IsNullOrEmpty(context.GetParameter("nuInventario")))
                return null;

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioLpnDetallesSeleccionadosQuery(filtros);
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, _gridConfig.DefaultSortsDetalleLpnQuitar);
        }
        public virtual byte[] ExportExcelDetalleLpnAtriCab(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new InventarioConsultaAtributosDetalleLpnCabezalQuery();
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, _gridConfig.DefaultSortsAtributos);
        }
        public virtual byte[] ExportExcelDetalleLpnAtriDet(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new InventarioConsultaAtributosDetalleLpnDetQuery();
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, _gridConfig.DefaultSortsAtributos);
        }

        #endregion

        #region GridMenuItemAction

        public virtual GridMenuItemActionContext GridMenuItemUbicacion(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                var logic = new InventarioLogic(this._identity, this._concurrencyControl);

                var nuInv = context.GetParameter("nuInventario");

                if (string.IsNullOrEmpty(nuInv))
                    throw new ValidationFailedException("INV411_Sec0_Info_PerdidaNuInventario");

                if (!decimal.TryParse(nuInv, this._identity.GetFormatProvider(), out decimal nroInventario))
                    throw new ValidationFailedException("INV_Format_Error_NuInventario");

                var inventario = uow.InventarioRepository.GetInventario(nroInventario);
                if (inventario == null)
                    throw new ValidationFailedException("INV410_Sec0_Error_InventarioNoExiste", new string[] { nuInv });

                if (!inventario.IsEditable())
                    throw new ValidationFailedException("INV410_Sec0_Error_ImposibleEditarInv");

                var cantNotAdd = 0;

                var keysRowSelected = GetSelectedUbicaciones(uow, context);

                uow.CreateTransactionNumber("GridMenuItemUbicacion");

                keysRowSelected.ForEach(x =>
                {
                    if (!logic.AgregarUbicacion(uow, inventario, x))
                        cantNotAdd++;
                });

                uow.SaveChanges();
                uow.Commit();

                if (cantNotAdd > 0)
                {
                    if (cantNotAdd == keysRowSelected.Count)
                        context.AddErrorNotification("INV410_Sec0_Info_UbicacionesNoAgregados");
                    else
                        context.AddWarningNotification("INV410_Sec0_Error_AgregadoParcialdeUbicaciones", new List<string> { cantNotAdd.ToString() });
                }
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                _logger.LogError(ex, "GridMenuItemUbicacion");
                uow.Rollback();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GridMenuItemUbicacion");
                uow.Rollback();
                throw;
            }
            return context;
        }
        public virtual GridMenuItemActionContext GridMenuItemUbicacionQuitar(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                var nuInv = context.GetParameter("nuInventario");

                if (string.IsNullOrEmpty(nuInv))
                    throw new ValidationFailedException("INV411_Sec0_Info_PerdidaNuInventario");

                if (!decimal.TryParse(nuInv, this._identity.GetFormatProvider(), out decimal nroInventario))
                    throw new ValidationFailedException("INV_Format_Error_NuInventario");

                var inventario = uow.InventarioRepository.GetInventario(nroInventario);
                if (inventario == null)
                    throw new ValidationFailedException("INV410_Sec0_Error_InventarioNoExiste", new string[] { nuInv });

                if (!inventario.IsEditable())
                    throw new ValidationFailedException("INV410_Sec0_Error_ImposibleEditarInv");

                var keysRowSelected = GetSelectedUbicacionesQuitar(uow, context);

                uow.CreateTransactionNumber("GridMenuItemUbicacionQuitar");

                keysRowSelected.ForEach(x =>
                {
                    var inventarioUbicacion = uow.InventarioRepository.GetInventarioEndereco(x);

                    inventarioUbicacion.NumeroTransaccion = uow.GetTransactionNumber();
                    inventarioUbicacion.NumeroTransaccionDelete = inventarioUbicacion.NumeroTransaccion;

                    uow.InventarioRepository.UpdateInventarioEndereco(inventarioUbicacion);
                    uow.SaveChanges();

                    uow.InventarioRepository.DeleteInventarioEndereco(inventarioUbicacion);
                    uow.SaveChanges();
                });

                uow.SaveChanges();
                uow.Commit();
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                _logger.LogError(ex, "GridMenuItemUbicacionQuitar");
                uow.Rollback();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GridMenuItemUbicacionQuitar");
                uow.Rollback();
                throw;
            }

            return context;
        }

        public virtual GridMenuItemActionContext GridMenuItemRegistros(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                var logic = new InventarioLogic(this._identity, this._concurrencyControl);

                var nuInv = context.GetParameter("nuInventario");

                if (string.IsNullOrEmpty(nuInv))
                    throw new ValidationFailedException("INV411_Sec0_Info_PerdidaNuInventario");

                if (!decimal.TryParse(nuInv, this._identity.GetFormatProvider(), out decimal nroInventario))
                    throw new ValidationFailedException("INV_Format_Error_NuInventario");

                var inventario = uow.InventarioRepository.GetInventario(nroInventario);
                if (inventario == null)
                    throw new ValidationFailedException("INV410_Sec0_Error_InventarioNoExiste", new string[] { nuInv });

                if (!inventario.IsEditable())
                    throw new ValidationFailedException("INV410_Sec0_Error_ImposibleEditarInv");

                var cantInOtherInventario = 0;
                var cantNotAdd = 0;

                var keysRowSelected = GetSelectedRegistros(uow, context)
                    .OrderBy(k => k[0])
                    .ThenBy(k => k[1])
                    .ThenBy(k => k[2])
                    .ToList();

                uow.CreateTransactionNumber("GridMenuItemRegistros");

                keysRowSelected.ForEach(keys =>
                {
                    var registro = new InventarioSelectRegistroLpn()
                    {
                        Ubicacion = keys[0],
                        Producto = keys[1],
                        Identificador = keys[2],
                        Empresa = int.Parse(keys[3]),
                        Faixa = decimal.Parse(keys[4], this._identity.GetFormatProvider())
                    };

                    var result = logic.AgregarRegistros(uow, inventario, registro, out bool regEnOtroInventario);

                    if (!result)
                    {
                        cantNotAdd++;
                    }
                    else if (regEnOtroInventario)
                    {
                        cantInOtherInventario++;
                    }
                });

                uow.SaveChanges();
                uow.Commit();

                if (cantNotAdd > 0)
                {
                    if (cantNotAdd == keysRowSelected.Count)
                        context.AddErrorNotification("INV410_Sec0_Info_RegistrosNoAgregados");
                    else
                        context.AddWarningNotification("INV410_Sec0_Error_AgregadoParcialdeRegistros", new List<string> { cantInOtherInventario.ToString() });
                }
                else if (cantInOtherInventario > 0)
                    context.AddInfoNotification("INV413_Sec0_Info_CantRegistroEnOtroInventario", new List<string> { cantInOtherInventario.ToString() });
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                _logger.LogError(ex, "GridMenuItemRegistros");
                uow.Rollback();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GridMenuItemRegistros");
                uow.Rollback();
                throw;
            }

            return context;
        }
        public virtual GridMenuItemActionContext GridMenuItemRegistrosQuitar(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                var logic = new InventarioLogic(this._identity, this._concurrencyControl);

                var nuInv = context.GetParameter("nuInventario");

                if (string.IsNullOrEmpty(nuInv))
                    throw new ValidationFailedException("INV411_Sec0_Info_PerdidaNuInventario");

                if (!decimal.TryParse(nuInv, this._identity.GetFormatProvider(), out decimal nroInventario))
                    throw new ValidationFailedException("INV_Format_Error_NuInventario");

                var inventario = uow.InventarioRepository.GetInventario(nroInventario);
                if (inventario == null)
                    throw new ValidationFailedException("INV410_Sec0_Error_InventarioNoExiste", new string[] { nuInv });

                if (!inventario.IsEditable())
                    throw new ValidationFailedException("INV410_Sec0_Error_ImposibleEditarInv");

                uow.CreateTransactionNumber("GridMenuItemRegistrosQuitar");

                var keysRowSelected = GetSelectedRegistrosQuitar(uow, context);

                var detallesToDelete = new Dictionary<decimal, List<InventarioSelectRegistroLpn>>();

                foreach (var key in keysRowSelected)
                {
                    if (!detallesToDelete.ContainsKey(key.NuInventarioUbicacion))
                        detallesToDelete[key.NuInventarioUbicacion] = new List<InventarioSelectRegistroLpn>();

                    detallesToDelete[key.NuInventarioUbicacion].Add(key);
                }

                var nuTransaccion = uow.GetTransactionNumber();

                foreach (var ubicacion in detallesToDelete.Keys)
                {
                    var inventarioUbicacion = uow.InventarioRepository.GetInventarioEndereco(ubicacion);

                    var amountDeleted = 0;
                    var detallesInventario = uow.InventarioRepository.GetDetallesInventarioReales(detallesToDelete[ubicacion]);

                    foreach (var detalle in detallesInventario)
                    {
                        detalle.NumeroTransaccion = nuTransaccion;
                        detalle.NumeroTransaccionDelete = nuTransaccion;
                        detalle.UserId = _identity.UserId;

                        uow.InventarioRepository.UpdateInventarioEnderecoDetalle(detalle);
                        uow.SaveChanges();

                        uow.InventarioRepository.DeleteInventarioEnderecoDetalle(detalle);
                        uow.SaveChanges();

                        amountDeleted++;
                    }

                    if (inventarioUbicacion.Detalles.Count == amountDeleted)
                    {
                        inventarioUbicacion.NumeroTransaccion = nuTransaccion;
                        inventarioUbicacion.NumeroTransaccionDelete = nuTransaccion;

                        uow.InventarioRepository.UpdateInventarioEndereco(inventarioUbicacion);
                        uow.SaveChanges();

                        uow.InventarioRepository.DeleteInventarioEndereco(inventarioUbicacion);
                        uow.SaveChanges();
                    }
                }

                uow.SaveChanges();
                uow.Commit();
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                _logger.LogError(ex, "GridMenuItemRegistrosQuitar");
                uow.Rollback();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GridMenuItemRegistrosQuitar");
                uow.Rollback();
                throw;
            }

            return context;
        }

        public virtual GridMenuItemActionContext GridMenuItemLpn(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                var logic = new InventarioLogic(this._identity, this._concurrencyControl);

                var nuInv = context.GetParameter("nuInventario");

                if (string.IsNullOrEmpty(nuInv))
                    throw new ValidationFailedException("INV411_Sec0_Info_PerdidaNuInventario");

                if (!decimal.TryParse(nuInv, this._identity.GetFormatProvider(), out decimal nroInventario))
                    throw new ValidationFailedException("INV_Format_Error_NuInventario");

                var inventario = uow.InventarioRepository.GetInventario(nroInventario);
                if (inventario == null)
                    throw new ValidationFailedException("INV410_Sec0_Error_InventarioNoExiste", new string[] { nuInv });

                if (!inventario.IsEditable())
                    throw new ValidationFailedException("INV410_Sec0_Error_ImposibleEditarInv");

                var cantInOtherInventario = 0;
                var cantNotAdd = 0;

                uow.CreateTransactionNumber("GridMenuItemLpn");

                var registros = GetSelectedRegistrosLpn(uow, context)
                    .OrderBy(r => r.Ubicacion)
                    .ThenBy(r => r.Producto)
                    .ThenBy(r => r.Identificador)
                    .ToList();

                foreach (var registro in registros)
                {
                    var result = logic.AgregarRegistros(uow, inventario, registro, out bool enOtroIventario);

                    if (!result)
                    {
                        cantNotAdd++;
                    }
                    else if (enOtroIventario)
                    {
                        cantInOtherInventario++;
                    }
                }

                uow.SaveChanges();
                uow.Commit();

                if (cantNotAdd > 0)
                {
                    if (cantNotAdd == registros.Count)
                        context.AddErrorNotification("INV410_Sec0_Info_RegistrosNoAgregados");
                    else
                        context.AddWarningNotification("INV410_Sec0_Error_AgregadoParcialdeRegistros", new List<string> { cantInOtherInventario.ToString() });
                }
                else if (cantInOtherInventario > 0)
                    context.AddInfoNotification("INV413_Sec0_Info_CantRegistroEnOtroInventario", new List<string> { cantInOtherInventario.ToString() });
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                _logger.LogError(ex, "GridMenuItemLpn");
                uow.Rollback();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GridMenuItemLpn");
                uow.Rollback();
                throw;
            }
            return context;
        }
        public virtual GridMenuItemActionContext GridMenuItemLpnQuitar(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                var logic = new InventarioLogic(this._identity, this._concurrencyControl);

                var nuInv = context.GetParameter("nuInventario");

                if (string.IsNullOrEmpty(nuInv))
                    throw new ValidationFailedException("INV411_Sec0_Info_PerdidaNuInventario");

                if (!decimal.TryParse(nuInv, this._identity.GetFormatProvider(), out decimal nroInventario))
                    throw new ValidationFailedException("INV_Format_Error_NuInventario");

                var inventario = uow.InventarioRepository.GetInventario(nroInventario);
                if (inventario == null)
                    throw new ValidationFailedException("INV410_Sec0_Error_InventarioNoExiste", new string[] { nuInv });

                if (!inventario.IsEditable())
                    throw new ValidationFailedException("INV410_Sec0_Error_ImposibleEditarInv");

                uow.CreateTransactionNumber("GridMenuItemLpnQuitar");

                var keysRowSelected = GetSelectedRegistrosLpnQuitar(uow, context);

                var detallesToDelete = new Dictionary<decimal, List<InventarioSelectRegistroLpn>>();

                foreach (var key in keysRowSelected)
                {
                    if (!detallesToDelete.ContainsKey(key.NuInventarioUbicacion))
                        detallesToDelete[key.NuInventarioUbicacion] = new List<InventarioSelectRegistroLpn>();

                    detallesToDelete[key.NuInventarioUbicacion].Add(key);
                }

                var nuTransaccion = uow.GetTransactionNumber();

                foreach (var ubicacion in detallesToDelete.Keys)
                {
                    var inventarioUbicacion = uow.InventarioRepository.GetInventarioEndereco(ubicacion);

                    var amountDeleted = 0;
                    var detallesInventario = new List<InventarioUbicacionDetalle>();

                    var keyDetallesSueltos = detallesToDelete[ubicacion].Where(i => (i.NroLpn == "-" || string.IsNullOrEmpty(i.NroLpn)))
                        .Select(i => new InventarioUbicacionDetalle() { Id = i.NuInventarioUbicacionDetalle });

                    var detalleSueltos = inventarioUbicacion.Detalles.Join(keyDetallesSueltos,
                        id => id.Id,
                        ds => ds.Id,
                        (ks, s) => ks).ToList();

                    detallesInventario.AddRange(detalleSueltos);

                    var keysDetallesLpn = detallesToDelete[ubicacion].Where(i => (i.NroLpn != "-" && !string.IsNullOrEmpty(i.NroLpn)));

                    if (keysDetallesLpn.Any())
                    {
                        var detallesLpn = uow.InventarioRepository.GetDetallesInventarioRealesLpn(keysDetallesLpn);
                        detallesInventario.AddRange(detallesLpn);
                    }

                    foreach (var detalle in detallesInventario)
                    {
                        detalle.NumeroTransaccion = nuTransaccion;
                        detalle.NumeroTransaccionDelete = nuTransaccion;
                        detalle.UserId = _identity.UserId;

                        uow.InventarioRepository.UpdateInventarioEnderecoDetalle(detalle);
                        uow.SaveChanges();

                        uow.InventarioRepository.DeleteInventarioEnderecoDetalle(detalle);
                        uow.SaveChanges();

                        amountDeleted++;
                    }

                    if (inventarioUbicacion.Detalles.Count == amountDeleted)
                    {
                        inventarioUbicacion.NumeroTransaccion = nuTransaccion;
                        inventarioUbicacion.NumeroTransaccionDelete = nuTransaccion;

                        uow.InventarioRepository.UpdateInventarioEndereco(inventarioUbicacion);
                        uow.SaveChanges();

                        uow.InventarioRepository.DeleteInventarioEndereco(inventarioUbicacion);
                        uow.SaveChanges();
                    }
                }

                uow.SaveChanges();
                uow.Commit();
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                _logger.LogError(ex, "GridMenuItemLpnQuitar");
                uow.Rollback();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GridMenuItemLpnQuitar");
                uow.Rollback();
                throw;
            }

            return context;
        }

        public virtual GridMenuItemActionContext GridMenuItemDetalleLpn(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                var logic = new InventarioLogic(this._identity, this._concurrencyControl);

                var nuInv = context.GetParameter("nuInventario");

                if (string.IsNullOrEmpty(nuInv))
                    throw new ValidationFailedException("INV411_Sec0_Info_PerdidaNuInventario");

                if (!decimal.TryParse(nuInv, this._identity.GetFormatProvider(), out decimal nroInventario))
                    throw new ValidationFailedException("INV_Format_Error_NuInventario");

                var inventario = uow.InventarioRepository.GetInventario(nroInventario);
                if (inventario == null)
                    throw new ValidationFailedException("INV410_Sec0_Error_InventarioNoExiste", new string[] { nuInv });

                if (!inventario.IsEditable())
                    throw new ValidationFailedException("INV410_Sec0_Error_ImposibleEditarInv");

                var cantInOtherInventario = 0;
                var cantNotAdd = 0;

                uow.CreateTransactionNumber("GridMenuItemDetalleLpn");

                var registros = GetSelectedRegistrosDetalleLpn(uow, context)
                    .OrderBy(r => r.Ubicacion)
                    .ThenBy(r => r.Producto)
                    .ThenBy(r => r.Identificador)
                    .ToList();

                foreach (var registro in registros)
                {
                    var result = logic.AgregarRegistros(uow, inventario, registro, out bool enOtroIventario);

                    if (!result)
                    {
                        cantNotAdd++;
                    }
                    else if (enOtroIventario)
                    {
                        cantInOtherInventario++;
                    }
                }

                uow.SaveChanges();
                uow.Commit();

                if (cantNotAdd > 0)
                {
                    if (cantNotAdd == registros.Count)
                        context.AddErrorNotification("INV410_Sec0_Info_RegistrosNoAgregados");
                    else
                        context.AddWarningNotification("INV410_Sec0_Error_AgregadoParcialdeRegistros", new List<string> { cantInOtherInventario.ToString() });
                }
                else if (cantInOtherInventario > 0)
                    context.AddInfoNotification("INV413_Sec0_Info_CantRegistroEnOtroInventario", new List<string> { cantInOtherInventario.ToString() });
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                _logger.LogError(ex, "GridMenuItemDetalleLpn");
                uow.Rollback();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GridMenuItemDetalleLpn");
                uow.Rollback();
                throw;
            }

            return context;
        }
        public virtual GridMenuItemActionContext GridMenuItemDetalleLpnQuitar(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                var logic = new InventarioLogic(this._identity, this._concurrencyControl);

                var nuInv = context.GetParameter("nuInventario");

                if (string.IsNullOrEmpty(nuInv))
                    throw new ValidationFailedException("INV411_Sec0_Info_PerdidaNuInventario");

                if (!decimal.TryParse(nuInv, this._identity.GetFormatProvider(), out decimal nroInventario))
                    throw new ValidationFailedException("INV_Format_Error_NuInventario");

                var inventario = uow.InventarioRepository.GetInventario(nroInventario);
                if (inventario == null)
                    throw new ValidationFailedException("INV410_Sec0_Error_InventarioNoExiste", new string[] { nuInv });

                if (!inventario.IsEditable())
                    throw new ValidationFailedException("INV410_Sec0_Error_ImposibleEditarInv");

                uow.CreateTransactionNumber("GridMenuItemDetalleLpnQuitar");

                var keysRowSelected = GetSelectedRegistrosDetalleLpnQuitar(uow, context);

                var detallesToDelete = new Dictionary<decimal, List<InventarioSelectRegistroLpn>>();

                foreach (var key in keysRowSelected)
                {
                    if (!detallesToDelete.ContainsKey(key.NuInventarioUbicacion))
                        detallesToDelete[key.NuInventarioUbicacion] = new List<InventarioSelectRegistroLpn>();

                    detallesToDelete[key.NuInventarioUbicacion].Add(key);
                }

                var nuTransaccion = uow.GetTransactionNumber();

                foreach (var ubicacion in detallesToDelete.Keys)
                {
                    var amountDeleted = 0;
                    var inventarioUbicacion = uow.InventarioRepository.GetInventarioEndereco(ubicacion);

                    foreach (var key in detallesToDelete[ubicacion])
                    {
                        var detalle = inventarioUbicacion.Detalles.FirstOrDefault(d => d.Id == key.NuInventarioUbicacionDetalle);

                        detalle.NumeroTransaccion = nuTransaccion;
                        detalle.NumeroTransaccionDelete = nuTransaccion;
                        detalle.UserId = _identity.UserId;

                        uow.InventarioRepository.UpdateInventarioEnderecoDetalle(detalle);
                        uow.SaveChanges();

                        uow.InventarioRepository.DeleteInventarioEnderecoDetalle(detalle);
                        uow.SaveChanges();

                        amountDeleted++;
                    }

                    if (inventarioUbicacion.Detalles.Count == amountDeleted)
                    {
                        inventarioUbicacion.NumeroTransaccion = nuTransaccion;
                        inventarioUbicacion.NumeroTransaccionDelete = nuTransaccion;

                        uow.InventarioRepository.UpdateInventarioEndereco(inventarioUbicacion);
                        uow.SaveChanges();

                        uow.InventarioRepository.DeleteInventarioEndereco(inventarioUbicacion);
                        uow.SaveChanges();
                    }
                }

                uow.SaveChanges();
                uow.Commit();
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                _logger.LogError(ex, "GridMenuItemDetalleLpnQuitar");
                uow.Rollback();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GridMenuItemDetalleLpnQuitar");
                uow.Rollback();
                throw;
            }

            return context;
        }

        #endregion

        #region GridSelectedKeys

        public virtual List<string> GetSelectedUbicaciones(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioUbicacionQuery(filtros);

            ((UnitOfWork)uow).HandleQuery(dbQuery, filterEmpresa: false);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys);

            return dbQuery.GetSelectedKeys(context.Selection.Keys);
        }
        public virtual List<decimal> GetSelectedUbicacionesQuitar(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioUbicacionQuitarQuery(filtros);

            ((UnitOfWork)uow).HandleQuery(dbQuery, filterEmpresa: false);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            var keys = new List<decimal>();

            foreach (var key in context.Selection.Keys)
            {
                keys.Add(decimal.Parse(key, _identity.GetFormatProvider()));
            }

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys, _identity.GetFormatProvider());

            return dbQuery.GetSelectedKeys(context.Selection.Keys, _identity.GetFormatProvider());
        }

        public virtual List<string[]> GetSelectedRegistros(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioRegistrosQuery(filtros);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys, _identity);

            return dbQuery.GetSelectedKeys(context.Selection.Keys, _identity);
        }
        public virtual List<InventarioSelectRegistroLpn> GetSelectedRegistrosQuitar(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var keysSelected = context.Selection.GetSelection(_gridConfig.GridKeysRegistroQuitar)
                .Select(item => new InventarioSelectRegistroLpn
                {
                    NuInventario = decimal.Parse(item["NU_INVENTARIO"], _identity.GetFormatProvider()),
                    NuInventarioUbicacion = decimal.Parse(item["NU_INVENTARIO_ENDERECO"], _identity.GetFormatProvider()),
                    NuInventarioUbicacionDetalle = decimal.Parse(item["NU_INVENTARIO_ENDERECO_DET"], _identity.GetFormatProvider()),
                }).ToList();

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioRegistroQuitarQuery(filtros);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            var seleccion = dbQuery.GetRegistros();

            if (context.Selection.AllSelected)
            {
                var registros = seleccion.Join(keysSelected,
                    ks => new { ks.NuInventario, ks.NuInventarioUbicacion, ks.NuInventarioUbicacionDetalle },
                    s => new { s.NuInventario, s.NuInventarioUbicacion, s.NuInventarioUbicacionDetalle },
                    (ks, s) => ks).ToList();

                return seleccion.Except(registros).ToList();
            }
            else
            {
                return seleccion.Join(keysSelected,
                    ks => new { ks.NuInventario, ks.NuInventarioUbicacion, ks.NuInventarioUbicacionDetalle },
                    s => new { s.NuInventario, s.NuInventarioUbicacion, s.NuInventarioUbicacionDetalle },
                    (ks, s) => ks).ToList();
            }
        }

        public virtual List<InventarioSelectRegistroLpn> GetSelectedRegistrosLpn(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var keysSelected = context.Selection.GetSelection(_gridConfig.GridKeysLpn)
                .Select(item => new InventarioSelectRegistroLpn
                {
                    Ubicacion = item["CD_ENDERECO"],
                    Empresa = int.Parse(item["CD_EMPRESA"]),
                    Producto = item["CD_PRODUTO"],
                    Faixa = decimal.Parse(item["CD_FAIXA"], _identity.GetFormatProvider()),
                    Identificador = item["NU_IDENTIFICADOR"],
                    NroLpn = item["NU_LPN"]
                }).ToList();

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioLpnDisponiblesQuery(filtros);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            var seleccion = dbQuery.GetRegistrosLpn();

            if (context.Selection.AllSelected)
            {
                var registros = seleccion.Join(keysSelected,
                    ks => new { ks.Ubicacion, ks.Producto, ks.Empresa, ks.Identificador, ks.Faixa, ks.NroLpn },
                    s => new { s.Ubicacion, s.Producto, s.Empresa, s.Identificador, s.Faixa, s.NroLpn },
                    (ks, s) => ks).ToList();

                return seleccion.Except(registros).ToList();
            }
            else
            {
                return seleccion.Join(keysSelected,
                    ks => new { ks.Ubicacion, ks.Producto, ks.Empresa, ks.Identificador, ks.Faixa, ks.NroLpn },
                    s => new { s.Ubicacion, s.Producto, s.Empresa, s.Identificador, s.Faixa, s.NroLpn },
                    (ks, s) => ks).ToList();
            }
        }
        public virtual List<InventarioSelectRegistroLpn> GetSelectedRegistrosLpnQuitar(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var keysSelected = context.Selection.GetSelection(_gridConfig.GridKeysLpnQuitar)
                .Select(item => new InventarioSelectRegistroLpn
                {
                    NuInventario = decimal.Parse(item["NU_INVENTARIO"], _identity.GetFormatProvider()),
                    NuInventarioUbicacion = decimal.Parse(item["NU_INVENTARIO_ENDERECO"], _identity.GetFormatProvider()),
                    NuInventarioUbicacionDetalle = decimal.Parse(item["NU_INVENTARIO_ENDERECO_DET"], _identity.GetFormatProvider()),
                }).ToList();

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioLpnSeleccionadosQuery(filtros);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            var seleccion = dbQuery.GetRegistrosLpn();
            if (context.Selection.AllSelected)
            {
                var registros = seleccion.Join(keysSelected,
                    ks => new { ks.NuInventario, ks.NuInventarioUbicacion, ks.NuInventarioUbicacionDetalle },
                    s => new { s.NuInventario, s.NuInventarioUbicacion, s.NuInventarioUbicacionDetalle },
                    (ks, s) => ks).ToList();

                return seleccion.Except(registros).ToList();
            }
            else
            {
                return seleccion.Join(keysSelected,
                    ks => new { ks.NuInventario, ks.NuInventarioUbicacion, ks.NuInventarioUbicacionDetalle },
                    s => new { s.NuInventario, s.NuInventarioUbicacion, s.NuInventarioUbicacionDetalle },
                    (ks, s) => ks).ToList();
            }
        }

        public virtual List<InventarioSelectRegistroLpn> GetSelectedRegistrosDetalleLpn(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var keysSelected = context.Selection.GetSelection(_gridConfig.GridKeysDetalleLpn)
                .Select(item => new InventarioSelectRegistroLpn
                {
                    Ubicacion = item["CD_ENDERECO"],
                    Empresa = int.Parse(item["CD_EMPRESA"]),
                    Producto = item["CD_PRODUTO"],
                    Faixa = decimal.Parse(item["CD_FAIXA"], _identity.GetFormatProvider()),
                    Identificador = item["NU_IDENTIFICADOR"],
                    NroLpn = item["NU_LPN"],
                    IdDetalleLpn = item["ID_LPN_DET"],
                }).ToList();

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioLpnDetallesDisponiblesQuery(filtros);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            var seleccion = dbQuery.GetRegistrosLpn();

            if (context.Selection.AllSelected)
            {
                var registros = seleccion.Join(keysSelected,
                    ks => new { ks.Ubicacion, ks.Producto, ks.Empresa, ks.Identificador, ks.Faixa, ks.NroLpn, ks.IdDetalleLpn },
                    s => new { s.Ubicacion, s.Producto, s.Empresa, s.Identificador, s.Faixa, s.NroLpn, s.IdDetalleLpn },
                    (ks, s) => ks).ToList();

                return seleccion.Except(registros).ToList();
            }
            else
            {
                return seleccion.Join(keysSelected,
                    ks => new { ks.Ubicacion, ks.Producto, ks.Empresa, ks.Identificador, ks.Faixa, ks.NroLpn, ks.IdDetalleLpn },
                    s => new { s.Ubicacion, s.Producto, s.Empresa, s.Identificador, s.Faixa, s.NroLpn, s.IdDetalleLpn },
                    (ks, s) => ks).ToList();
            }
        }
        public virtual List<InventarioSelectRegistroLpn> GetSelectedRegistrosDetalleLpnQuitar(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var keysSelected = context.Selection.GetSelection(_gridConfig.GridKeysDetalleLpnQuitar)
                .Select(item => new InventarioSelectRegistroLpn
                {
                    NuInventario = decimal.Parse(item["NU_INVENTARIO"], _identity.GetFormatProvider()),
                    NuInventarioUbicacion = decimal.Parse(item["NU_INVENTARIO_ENDERECO"], _identity.GetFormatProvider()),
                    NuInventarioUbicacionDetalle = decimal.Parse(item["NU_INVENTARIO_ENDERECO_DET"], _identity.GetFormatProvider()),
                }).ToList();

            var filtros = InventarioLogic.GetFiltros(uow, context, _identity);
            var dbQuery = new InventarioLpnDetallesSeleccionadosQuery(filtros);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            var seleccion = dbQuery.GetRegistrosLpn();
            if (context.Selection.AllSelected)
            {
                var registros = seleccion.Join(keysSelected,
                    ks => new { ks.NuInventario, ks.NuInventarioUbicacion, ks.NuInventarioUbicacionDetalle },
                    s => new { s.NuInventario, s.NuInventarioUbicacion, s.NuInventarioUbicacionDetalle },
                    (ks, s) => ks).ToList();

                return seleccion.Except(registros).ToList();
            }
            else
            {
                return seleccion.Join(keysSelected,
                    ks => new { ks.NuInventario, ks.NuInventarioUbicacion, ks.NuInventarioUbicacionDetalle },
                    s => new { s.NuInventario, s.NuInventarioUbicacion, s.NuInventarioUbicacionDetalle },
                    (ks, s) => ks).ToList();
            }
        }
        #endregion

        public virtual List<SelectOption> SearchEmpresa(Grid grid, GridRow row, GridSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var empresas = uow.EmpresaRepository.GetByNombreOrCodePartial(context.SearchValue);

            foreach (var empresa in empresas)
            {
                opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
            }

            return opciones;
        }

        public virtual List<SelectOption> OptionSelectCierreConteo()
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var dominios = uow.DominioRepository.GetDominios(CodigoDominioDb.TipoCierreConteoInventario);

            foreach (var dominio in dominios)
            {
                opciones.Add(new SelectOption(dominio.Id, dominio.Descripcion));
            }

            return opciones;
        }

        public virtual List<SelectOption> OptionSelectEstadoInventario()
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var dominios = uow.DominioRepository.GetDominios(CodigoDominioDb.EstadoInventario);

            foreach (var dominio in dominios)
            {
                opciones.Add(new SelectOption(dominio.Id, dominio.Descripcion));
            }

            return opciones;
        }

        public virtual List<SelectOption> OptionSelectTiposInventario()
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var dominios = uow.DominioRepository.GetDominios(CodigoDominioDb.TipoInventario);

            foreach (var dominio in dominios)
            {
                opciones.Add(new SelectOption(dominio.Valor, dominio.Descripcion));
            }

            return opciones;
        }

        public virtual void InicializarSelects(IUnitOfWork uow, Form form)
        {
            InicializarSelectPredio(uow, form);

            var selectCierreConteo = form.GetField("tipoCierreConteo");
            selectCierreConteo.Options = new List<SelectOption>();

            var optionsCierreConteo = uow.DominioRepository.GetDominios(CodigoDominioDb.TipoCierreConteoInventario);

            foreach (var option in optionsCierreConteo)
            {
                selectCierreConteo.Options.Add(new SelectOption(option.Id, option.Descripcion));
            }

            var selectEmpresa = form.GetField("empresa");
            var empresa = uow.EmpresaRepository.GetEmpresaUnicaParaUsuario(_identity.UserId);

            if (empresa != null)
            {
                selectEmpresa.ReadOnly = true;
                selectEmpresa.Value = empresa.Id.ToString();
                selectEmpresa.Options = new List<SelectOption> { new SelectOption(selectEmpresa.Value, empresa.Nombre) };
            }
            else
            {
                selectEmpresa.Value = string.Empty;
                selectEmpresa.Options = new List<SelectOption>();
            }
        }

        public virtual void InicializarSelectPredio(IUnitOfWork uow, Form form)
        {
            var selectPredio = form.GetField("predio");
            selectPredio.Options = new List<SelectOption>();

            var dbQuery = new GetPrediosUsuarioQuery();
            uow.HandleQuery(dbQuery);

            var predios = dbQuery.GetPrediosUsuario(_identity.UserId).OrderBy(x => x.Numero);
            foreach (var pred in predios)
            {
                selectPredio.Options.Add(new SelectOption(pred.Numero, $"{pred.Numero} - {pred.Descripcion}"));
            }

            if (predios.Count() == 1)
                selectPredio.Value = predios.FirstOrDefault().Numero;

            if (this._identity.Predio != GeneralDb.PredioSinDefinir)
                selectPredio.Value = this._identity.Predio;
        }

        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var empresas = uow.EmpresaRepository.GetByNombreOrCodePartial(context.SearchValue);

            foreach (var empresa in empresas)
            {
                opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
            }

            return opciones;
        }

        public virtual IInventario GenerarInventario(IUnitOfWork uow, Form form, FormSubmitContext context)
        {
            var tipoInventario = context.GetParameter("tipoInventario");

            var inventario = InventarioFactory.Create(tipoInventario);

            if (int.TryParse(form.GetField("empresa")?.Value, out int cdEmpresa))
                inventario.Empresa = cdEmpresa;

            inventario.Descripcion = form.GetField("descInventario").Value;
            inventario.CierreConteo = form.GetField("tipoCierreConteo").Value;
            inventario.Predio = form.GetField("predio").Value;
            inventario.IsCreacionWeb = true;
            inventario.FechaAlta = DateTime.Now;
            inventario.Estado = EstadoInventario.EnEdicion;
            inventario.NumeroTransaccion = uow.GetTransactionNumber();
            inventario.NumeroConteo = 1;

            inventario.ExcluirLpns = form.GetField("excluirLpns").IsChecked();
            inventario.ExcluirSueltos = form.GetField("excluirSueltos").IsChecked();
            inventario.ActualizarConteoFin = form.GetField("actualizarConteoFin").IsChecked();
            inventario.ControlarVencimiento = form.GetField("controlarVencimiento").IsChecked();
            inventario.PermiteIngresarMotivo = form.GetField("permiteIngresoMotivo").IsChecked();
            inventario.ModificarStockEnDiferencia = form.GetField("modificarStockEnDif").IsChecked();
            inventario.RestablecerLpnFinalizado = form.GetField("restablecerLpnFinalizado").IsChecked();
            inventario.BloquearConteoConsecutivoUsuario = form.GetField("bloquearUsrConteoSucesivo").IsChecked();
            inventario.GenerarPrimerConteo = form.GetField("generarPrimerConteo").IsChecked();
            inventario.PermiteUbicacionesDeOtrosInventarios = form.GetField("permiteAsociarUbicOtrosInv").IsChecked();

            return inventario;
        }

        public virtual void RedirectInventario(GridButtonActionContext context, string app, decimal nuInventario, bool showOnlyPending = false)
        {
            var param = new List<ComponentParameter>
            {
                new ComponentParameter("inventario", nuInventario.ToString(_identity.GetFormatProvider())),
            };

            if (showOnlyPending)
                param.Add(new ComponentParameter("showOnlyPending", "true"));

            context.Redirect($"/inventario/{app}", true, param);
        }

        protected virtual void ResetForm(IUnitOfWork uow, Form form)
        {
            var config = GetInventarioConfiguracion(uow);

            form.Fields.ForEach(f => f.Value = "");

            form.GetField("empresa").Disabled = false;

            form.GetField("actualizarConteoFin").SetChecked(config.ActualizarConteo);
            form.GetField("actualizarConteoFin").ReadOnly = !config.ActualizarConteoHabilitado;

            form.GetField("bloquearUsrConteoSucesivo").SetChecked(config.BloquearConteo);
            form.GetField("bloquearUsrConteoSucesivo").ReadOnly = !config.BloquearConteoHabilitado;

            form.GetField("controlarVencimiento").SetChecked(config.ControlarVencimiento);
            form.GetField("controlarVencimiento").ReadOnly = !config.ControlarVencimientoHabilitado;

            form.GetField("modificarStockEnDif").SetChecked(config.MarcarDiferencia);
            form.GetField("modificarStockEnDif").ReadOnly = !config.MarcarDiferenciaHabilitado;

            form.GetField("permiteIngresoMotivo").SetChecked(config.PermiteIngresarMotivo);
            form.GetField("permiteIngresoMotivo").ReadOnly = !config.PermiteIngresarMotivoHabilitado;

            form.GetField("excluirSueltos").SetChecked(config.ExcluirSueltos);
            form.GetField("excluirSueltos").ReadOnly = !config.ExcluirSueltosHabilitado;

            form.GetField("excluirLpns").SetChecked(config.ExcluirLpns);
            form.GetField("excluirLpns").ReadOnly = !config.ExcluirLpnsHabilitado;

            form.GetField("restablecerLpnFinalizado").SetChecked(config.RestablecerLpnFinalizado);
            form.GetField("restablecerLpnFinalizado").Disabled = !config.RestablecerLpnFinalizadoHabilitado;

            form.GetField("generarPrimerConteo").SetOk();
            form.GetField("generarPrimerConteo").SetChecked(config.GenerarPrimerConteo);
            form.GetField("generarPrimerConteo").Disabled = !config.GenerarPrimerConteoHabilitado;

            form.GetField("permiteAsociarUbicOtrosInv").SetChecked(config.PermiteUbicacionesDeOtrosInventarios);
            form.GetField("permiteAsociarUbicOtrosInv").Disabled = !config.PermiteUbicacionesDeOtrosInventariosHabilitado;
        }
        public static InventarioConfiguracion GetInventarioConfiguracion(IUnitOfWork uow)
        {
            var parametros = uow.ParametroRepository.GetParameters(new List<string>
            {
                ParamManager.INV410_DEFAULT_ACTUALIZAR_CONT,
                ParamManager.INV410_ENABLED_ACTUALIZAR_CONT,
                ParamManager.INV410_DEFAULT_BLOQ_USR_CONTEO,
                ParamManager.INV410_ENABLED_BLOQ_USR_CONTEO,
                ParamManager.INV410_DEFAULT_CTR_VENCIMIENTO,
                ParamManager.INV410_ENABLED_CTR_VENCIMIENTO,
                ParamManager.INV410_DEFAULT_MARCAR_DIF,
                ParamManager.INV410_ENABLED_MARCAR_DIF,
                ParamManager.INV410_DEFAULT_INGR_MOTIVO,
                ParamManager.INV410_ENABLED_INGR_MOTIVO,
                ParamManager.INV410_DEFAULT_EXCLUIR_SUELTOS,
                ParamManager.INV410_ENABLED_EXCLUIR_SUELTOS,
                ParamManager.INV410_DEFAULT_EXCLUIR_LPNS,
                ParamManager.INV410_ENABLED_EXCLUIR_LPNS,
                ParamManager.INV410_DEFAULT_REST_LPN_FIN,
                ParamManager.INV410_ENABLED_REST_LPN_FIN,
                ParamManager.INV410_DEFAULT_PRIMER_CONTEO,
                ParamManager.INV410_ENABLED_PRIMER_CONTEO,
                ParamManager.INV410_DEFAULT_UBIC_OTRO_INV,
                ParamManager.INV410_ENABLED_UBIC_OTRO_INV,
            });

            return new InventarioConfiguracion()
            {
                ActualizarConteo = parametros[ParamManager.INV410_DEFAULT_ACTUALIZAR_CONT] == "S",
                ActualizarConteoHabilitado = parametros[ParamManager.INV410_ENABLED_ACTUALIZAR_CONT] == "S",
                ControlarVencimiento = parametros[ParamManager.INV410_DEFAULT_CTR_VENCIMIENTO] == "S",
                ControlarVencimientoHabilitado = parametros[ParamManager.INV410_ENABLED_CTR_VENCIMIENTO] == "S",
                BloquearConteo = parametros[ParamManager.INV410_DEFAULT_BLOQ_USR_CONTEO] == "S",
                BloquearConteoHabilitado = parametros[ParamManager.INV410_ENABLED_BLOQ_USR_CONTEO] == "S",
                PermiteIngresarMotivo = parametros[ParamManager.INV410_DEFAULT_INGR_MOTIVO] == "S",
                PermiteIngresarMotivoHabilitado = parametros[ParamManager.INV410_ENABLED_INGR_MOTIVO] == "S",
                MarcarDiferencia = parametros[ParamManager.INV410_DEFAULT_MARCAR_DIF] == "S",
                MarcarDiferenciaHabilitado = parametros[ParamManager.INV410_ENABLED_MARCAR_DIF] == "S",
                ExcluirSueltos = parametros[ParamManager.INV410_DEFAULT_EXCLUIR_SUELTOS] == "S",
                ExcluirSueltosHabilitado = parametros[ParamManager.INV410_ENABLED_EXCLUIR_SUELTOS] == "S",
                ExcluirLpns = parametros[ParamManager.INV410_DEFAULT_EXCLUIR_LPNS] == "S",
                ExcluirLpnsHabilitado = parametros[ParamManager.INV410_ENABLED_EXCLUIR_LPNS] == "S",
                RestablecerLpnFinalizado = parametros[ParamManager.INV410_DEFAULT_REST_LPN_FIN] == "S",
                RestablecerLpnFinalizadoHabilitado = parametros[ParamManager.INV410_ENABLED_REST_LPN_FIN] == "S",
                GenerarPrimerConteo = parametros[ParamManager.INV410_DEFAULT_PRIMER_CONTEO] == "S",
                GenerarPrimerConteoHabilitado = parametros[ParamManager.INV410_ENABLED_PRIMER_CONTEO] == "S",
                PermiteUbicacionesDeOtrosInventarios = parametros[ParamManager.INV410_DEFAULT_UBIC_OTRO_INV] == "S",
                PermiteUbicacionesDeOtrosInventariosHabilitado = parametros[ParamManager.INV410_ENABLED_UBIC_OTRO_INV] == "S",
            };

        }

        public virtual void SetGridConfig()
        {
            _gridConfig = new InventarioGridConfig()
            {
                GridKeysInventario = new List<string>
                {
                    "NU_INVENTARIO"
                },
                GridKeysRegistro = new List<string>
                {
                    "CD_ENDERECO",
                    "CD_PRODUTO",
                    "NU_IDENTIFICADOR",
                    "CD_EMPRESA",
                    "CD_FAIXA"
                },
                GridKeysRegistroQuitar = new List<string>
                {
                    "NU_INVENTARIO",
                    "NU_INVENTARIO_ENDERECO",
                    "NU_INVENTARIO_ENDERECO_DET"
                },
                GridKeysUbicacion = new List<string>
                {
                    "CD_ENDERECO"
                },
                GridKeysUbicacionQuitar = new List<string>
                {
                    "NU_INVENTARIO_ENDERECO"
                },
                GridKeysLpn = new List<string>
                {
                    "CD_ENDERECO",
                    "CD_PRODUTO",
                    "CD_EMPRESA",
                    "CD_FAIXA",
                    "NU_IDENTIFICADOR",
                    "NU_LPN"
                },
                GridKeysLpnQuitar = new List<string>
                {
                    "NU_INVENTARIO",
                    "NU_INVENTARIO_ENDERECO",
                    "NU_INVENTARIO_ENDERECO_DET"
                },
                GridKeysDetalleLpn = new List<string>
                {
                    "CD_ENDERECO",
                    "CD_PRODUTO",
                    "CD_EMPRESA",
                    "CD_FAIXA",
                    "NU_IDENTIFICADOR",
                    "NU_LPN",
                    "ID_LPN_DET"
                },
                GridKeysDetalleLpnQuitar = new List<string>
                {
                    "NU_INVENTARIO",
                    "NU_INVENTARIO_ENDERECO",
                    "NU_INVENTARIO_ENDERECO_DET"
                },
                GridKeysAtributo = new List<string>
                {
                    "ID_ATRIBUTO"
                },


                DefaultSortsInventario = new List<SortCommand>
                {
                    new SortCommand("NU_INVENTARIO", SortDirection.Descending),
                },
                DefaultSortsRegistro = new List<SortCommand>
                {
                    new SortCommand("CD_ENDERECO", SortDirection.Ascending),
                },
                DefaultSortsRegistroQuitar = new List<SortCommand>
                {
                    new SortCommand("CD_ENDERECO", SortDirection.Ascending),
                },
                DefaultSortsUbicacion = new List<SortCommand>
                {
                    new SortCommand("CD_ENDERECO", SortDirection.Ascending),
                },
                DefaultSortsUbicacionQuitar = new List<SortCommand>
                {
                    new SortCommand("CD_ENDERECO", SortDirection.Ascending),
                },
                DefaultSortsLpn = new List<SortCommand>
                {
                    new SortCommand("CD_ENDERECO", SortDirection.Ascending),
                },
                DefaultSortsLpnQuitar = new List<SortCommand>
                {
                    new SortCommand("CD_ENDERECO", SortDirection.Ascending),
                },
                DefaultSortsDetalleLpn = new List<SortCommand>
                {
                    new SortCommand("CD_ENDERECO", SortDirection.Ascending),
                },
                DefaultSortsDetalleLpnQuitar = new List<SortCommand>
                {
                    new SortCommand("CD_ENDERECO", SortDirection.Ascending),
                },
                DefaultSortsAtributos = new List<SortCommand>
                {
                    new SortCommand("NM_ATRIBUTO", SortDirection.Ascending),
                },
            };
        }

        protected class InventarioGridConfig
        {
            public List<string> GridKeysInventario;
            public List<string> GridKeysRegistro;
            public List<string> GridKeysRegistroQuitar;
            public List<string> GridKeysUbicacion;
            public List<string> GridKeysUbicacionQuitar;
            public List<string> GridKeysLpn;
            public List<string> GridKeysLpnQuitar;
            public List<string> GridKeysDetalleLpn;
            public List<string> GridKeysDetalleLpnQuitar;
            public List<string> GridKeysAtributo;

            public List<SortCommand> DefaultSortsInventario;
            public List<SortCommand> DefaultSortsRegistro;
            public List<SortCommand> DefaultSortsRegistroQuitar;
            public List<SortCommand> DefaultSortsUbicacion;
            public List<SortCommand> DefaultSortsUbicacionQuitar;
            public List<SortCommand> DefaultSortsLpn;
            public List<SortCommand> DefaultSortsLpnQuitar;
            public List<SortCommand> DefaultSortsDetalleLpn;
            public List<SortCommand> DefaultSortsDetalleLpnQuitar;
            public List<SortCommand> DefaultSortsAtributos;
        }

        #endregion
    }
}
