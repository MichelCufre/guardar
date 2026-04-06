using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.Picking;
using WIS.Domain.Recepcion;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.REC
{
    public class REC200CrossDocking : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ILogger<REC200CrossDocking> _logger;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridService _gridService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridExcelService _excelService;

        protected List<string> GridKeysPedidos { get; }
        protected List<string> GridKeysCrossDock { get; }
        protected List<SortCommand> DefaultSort { get; }
     
        public REC200CrossDocking(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            ILogger<REC200CrossDocking> logger,
            IFormValidationService formValidationService,
            IGridService gridService,
            IFilterInterpreter filterInterpreter,
            IGridExcelService excelService)
        {
            this.GridKeysPedidos = new List<string>
            {
                "NU_PEDIDO", "CD_EMPRESA", "CD_CLIENTE", "NU_AGENDA"
            };
            
            this.GridKeysCrossDock = new List<string>
            {
                "NU_AGENDA", "NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA"
            };
            
            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_PEDIDO", SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._logger = logger;
            this._formValidationService = formValidationService;
            this._identity = identity;
            this._gridService = gridService;
            this._filterInterpreter = filterInterpreter;
            this._excelService = excelService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            switch (grid.Id)
            {
                case "REC200_grid_1": return this.InitializeGridPedidos(grid, context);
                case "REC200_grid_2": return this.InitializeGridCrossDock(grid, context);
            }

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            switch (grid.Id)
            {
                case "REC200_grid_1": return this.FetchRowsPedidos(grid, context);
                case "REC200_grid_2": return this.FetchRowsCrossDock(grid, context);
            }

            return grid;
        }
        
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            switch (grid.Id)
            {
                case "REC200_grid_1": return this.GridExportExcelPedidos(grid, context);
                case "REC200_grid_2": return this.GridExportExcelCrossDock(grid, context);
            }
            return null;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Id == "REC200_grid_1")
            {
                string tipoSeleccionParameter = context.GetParameter("tpSeleccion");

                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "agenda")?.Value, out int nroAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                CrossDockingSeleccionTipo tipoSeleccion = this.GetTipoSeleccionParameter(tipoSeleccionParameter);

                Agenda agenda = uow.AgendaRepository.GetAgenda(nroAgenda);

                var dbQuery = new PedidosCrossDockQuery(agenda, tipoSeleccion);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "agenda")?.Value, out int nroAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                Agenda agenda = uow.AgendaRepository.GetAgenda(nroAgenda);

                var dbQuery = new PedidosIncluidosCrossDockQuery(agenda.IdEmpresa, agenda.Id, agenda.Predio);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                switch (context.ButtonId)
                {
                    case "btnAgregarPedido": MenuItemActionAddPedido(uow, context); break;
                    case "btnQuitarPedido": MenuItemActionRemovePedido(uow, context); break;
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("REC200_frm1_error_ExitoEnLaOperacion");
            }
            catch (ValidationFailedException ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message);

                this._logger.LogError($"GridMenuItemAction: {ex.Message}");
            }

            return context;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (form.Id == "REC200_form_1")
            {
                var selectionField = form.GetField("tpSeleccion");

                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "agenda")?.Value, out int nroAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");
                Agenda agenda = uow.AgendaRepository.GetAgenda(nroAgenda);

                List<SelectOption> opciones = new List<SelectOption>();
                opciones.Add(new SelectOption(Convert.ToString((int)CrossDockingSeleccionTipo.Todo), "REC200_frm1_opt_TipoSeleccionTodo"));

                if (!string.IsNullOrEmpty(agenda.NumeroDocumento) && agenda.NumeroDocumento != "(VARIOS)")
                    opciones.Add(new SelectOption(Convert.ToString((int)CrossDockingSeleccionTipo.OrdenDeCompra), "REC200_frm1_opt_TipoSeleccionOC"));

                selectionField.Options = opciones;
                selectionField.Value = Convert.ToString((int)CrossDockingSeleccionTipo.Todo);

                var selectionFieldTpCross = form.GetField("tpCrossDocking");
                List<SelectOption> opcionesFieldTpCross = uow.CrossDockingRepository.GetTpCrossDockingDisponiblesForSelect(agenda.Estado);
                selectionFieldTpCross.Options = opcionesFieldTpCross;

                string tpCrossDocking = string.Empty;
                if (opcionesFieldTpCross.Count() == 1)
                {
                    selectionFieldTpCross.Value = opcionesFieldTpCross.FirstOrDefault().Value;
                    tpCrossDocking = opcionesFieldTpCross.FirstOrDefault().Value;
                    selectionFieldTpCross.ReadOnly = true;
                    context.AddParameter("tpCrossDockinDefault", opcionesFieldTpCross.FirstOrDefault().Value);
                }

                if (!uow.EmpresaRepository.IsEmpresaDocumental(agenda.IdEmpresa) || tpCrossDocking != TipoCrossDockingDb.SegundaFase)
                {
                    selectionFieldTpCross.ReadOnly = true;
                }

                var fieldConsumirOtrosDocs = form.GetField("consumirOtrosDocs");
                fieldConsumirOtrosDocs.Value = "false";

                if (!uow.EmpresaRepository.IsEmpresaDocumental(agenda.IdEmpresa) || tpCrossDocking != "DF")
                {
                    fieldConsumirOtrosDocs.Disabled = true;
                    fieldConsumirOtrosDocs.ReadOnly = true;
                }
                else
                {
                    fieldConsumirOtrosDocs.Disabled = false;
                    fieldConsumirOtrosDocs.ReadOnly = false;
                }
            }
            return form;
        }

        public override Form FormButtonAction(Form form, FormButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber("Iniciar cross-docking");
            uow.BeginTransaction();

            try
            {
                if (context.ButtonId == "btnIniciarCrossDocking")
                {
                    if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "NU_AGENDA")?.Value, out int nroAgenda))
                        throw new ValidationFailedException("General_Sec0_Error_ParametrosURI");

                    Agenda agenda = uow.AgendaRepository.GetAgenda(nroAgenda);
                    ICrossDocking crossDock = uow.CrossDockingRepository.GetCrossDockingByAgenda(nroAgenda);

                    if (crossDock == null)
                        throw new ValidationFailedException("REC200_frm1_error_SinCrossDocking");
                    
                    if (!crossDock.CanEdit())
                        throw new ValidationFailedException("REC200_frm1_error_CrossDockingYaIniciado");
                    
                    if (crossDock.Estado == EstadoCrossDockingDb.Iniciado)
                        throw new ValidationFailedException("REC200_frm1_error_CrossDockingYaIniciado");

                    bool consumirOtrosDocumentos = context.GetParameter("consumirOtrosDocs") == "true";

                    this._logger.LogDebug($"REC200 - Agenda : {agenda.Id} - IniciarCrossDocking");

                    crossDock.Iniciar(uow, agenda, consumirOtrosDocumentos);

                    uow.SaveChanges();
                    uow.Commit();

                    this._logger.LogDebug($"REC200 - Agenda : {agenda.Id} - FinCrossDocking");
                    context.AddSuccessNotification("REC200_frm1_success_CrossDockingIniciado");
                }
            }
            catch (ValidationFailedException ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message);

                this._logger.LogError($"FormButtonAction: {ex.Message}");
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new REC200FormValidationModule(), form, context);
        }
        
        #region Metodos Auxiliares

        public virtual Grid InitializeGridPedidos(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems = new List<IGridItem> {
                new GridButton("btnAgregarPedido", "REC200_grid1_btn_AddPedido")
            };

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public virtual Grid InitializeGridCrossDock(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems = new List<IGridItem> {
                new GridButton("btnQuitarPedido", "REC200_grid1_btn_RemovePedido")
            };

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public virtual Grid FetchRowsPedidos(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string tipoSeleccionParameter = context.GetParameter("tpSeleccion");

            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "NU_AGENDA")?.Value, out int nroAgenda))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            CrossDockingSeleccionTipo tipoSeleccion = this.GetTipoSeleccionParameter(tipoSeleccionParameter);
            Agenda agenda = uow.AgendaRepository.GetAgenda(nroAgenda);

            var dbQuery = new PedidosCrossDockQuery(agenda, tipoSeleccion);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeysPedidos);

            string empresaDesc = uow.EmpresaRepository.GetNombre(agenda.IdEmpresa);

            context.AddParameter("REC200_NU_AGENDA", nroAgenda.ToString());
            context.AddParameter("REC200_CD_EMPRESA", agenda.IdEmpresa.ToString());
            context.AddParameter("REC200_NM_EMPRESA", empresaDesc);
            context.AddParameter("REC200_NU_DOCUMENTO", agenda.NumeroDocumento ?? "");

            return grid;
        }
        
        public virtual Grid FetchRowsCrossDock(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "NU_AGENDA")?.Value, out int nroAgenda))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            Agenda agenda = uow.AgendaRepository.GetAgenda(nroAgenda);

            var dbQuery = new PedidosIncluidosCrossDockQuery(agenda.IdEmpresa, agenda.Id, agenda.Predio);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeysCrossDock);

            context.AddParameter("REC200_NU_AGENDA", nroAgenda.ToString());

            return grid;
        }

        public virtual GridMenuItemActionContext MenuItemActionAddPedido(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var tpCrossDocking = context.GetParameter("tpCrossDocking");

            if (string.IsNullOrEmpty(tpCrossDocking))
                throw new ValidationFailedException("REC200_frm1_error_DebeSeleccionarTipoCross");

            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "NU_AGENDA")?.Value, out int nroAgenda))
                throw new ValidationFailedException("General_Sec0_Error_ParametrosURI");

            var tipoSeleccion = this.GetTipoSeleccionParameter(context.GetParameter("tpSeleccion"));

            var agenda = uow.AgendaRepository.GetAgenda(nroAgenda);

            uow.CreateTransactionNumber("Agregar pedidos");

            ICrossDocking crossDock = CrossDockingLogic.GetOrCreateCrossDocking(uow, agenda, _identity.UserId, tpCrossDocking);

            if (crossDock == null)
                throw new ValidationFailedException("REC200_grid1_error_SinCrossDocking");

            if (!crossDock.CanEdit())
                throw new ValidationFailedException("REC200_frm1_error_CrossDockingYaIniciado");

            if (crossDock.Estado == EstadoCrossDockingDb.Iniciado)
                throw new ValidationFailedException("REC200_frm1_error_CrossDockingYaIniciado");

            var pedidos = crossDock.GetPedidosToAdd(uow, agenda, tipoSeleccion, context, GridKeysPedidos, _filterInterpreter);

            crossDock.AddPedidos(uow, pedidos);

            uow.SaveChanges();

            return context;
        }

        public virtual GridMenuItemActionContext MenuItemActionRemovePedido(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "NU_AGENDA")?.Value, out int nroAgenda))
                throw new ValidationFailedException("General_Sec0_Error_ParametrosURI");

            var agenda = uow.AgendaRepository.GetAgenda(nroAgenda);

            uow.CreateTransactionNumber("Quitar pedidos");

            ICrossDocking crossDock = uow.CrossDockingRepository.GetCrossDockingActivoByAgenda(agenda.Id);

            if (crossDock == null)
                throw new ValidationFailedException("REC200_grid1_error_SinCrossDocking");

            if (!crossDock.CanEdit())
                throw new ValidationFailedException("REC200_frm1_error_CrossDockingYaIniciado");

            if (crossDock.Estado == EstadoCrossDockingDb.Iniciado)
                throw new ValidationFailedException("REC200_frm1_error_CrossDockingYaIniciado");


            List<Pedido> pedidos = uow.PedidoRepository.GetPedidosPreparacionProgramada(crossDock.Preparacion);

            List<CrossDockingPedidoSelection> keyGroup = new List<CrossDockingPedidoSelection>();
            if (context.Selection.AllSelected)
            {
                var dbQuery = new PedidosIncluidosCrossDockQuery(agenda.IdEmpresa, agenda.Id, agenda.Predio);
                uow.HandleQuery(dbQuery);
                keyGroup = dbQuery.GetDetallesCross();
            }
            else
            {
                List<Dictionary<string, string>> selection = context.Selection.GetSelection(this.GridKeysCrossDock);

                keyGroup = selection.Select(item => new CrossDockingPedidoSelection
                {
                    Pedido = item["NU_PEDIDO"],
                    Empresa = int.Parse(item["CD_EMPRESA"]),
                    Cliente = item["CD_CLIENTE"],
                }).ToList();
            }

            List<Pedido> pedidosQuitar = pedidos.Where(pedido => keyGroup.Any(e => e.Pedido == pedido.Id && e.Cliente == pedido.Cliente && e.Empresa == pedido.Empresa)).ToList();

            crossDock.RemovePedidos(uow, pedidosQuitar);
            crossDock.RemovePreparacion(uow);

            uow.SaveChanges();

            return context;
        }

        public virtual byte[] GridExportExcelCrossDock(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "NU_AGENDA")?.Value, out int nroAgenda))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            Agenda agenda = uow.AgendaRepository.GetAgenda(nroAgenda);

            var dbQuery = new PedidosIncluidosCrossDockQuery(agenda.IdEmpresa, agenda.Id, agenda.Predio);

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public virtual byte[] GridExportExcelPedidos(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            string tipoSeleccionParameter = context.GetParameter("tpSeleccion");

            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "NU_AGENDA")?.Value, out int nroAgenda))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            CrossDockingSeleccionTipo tipoSeleccion = this.GetTipoSeleccionParameter(tipoSeleccionParameter);
            Agenda agenda = uow.AgendaRepository.GetAgenda(nroAgenda);

            var dbQuery = new PedidosCrossDockQuery(agenda, tipoSeleccion);

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public virtual CrossDockingSeleccionTipo GetTipoSeleccionParameter(string parameter)
        {
            return (!string.IsNullOrEmpty(parameter) ? (CrossDockingSeleccionTipo)int.Parse(parameter) : CrossDockingSeleccionTipo.Unknown);
        }

        #endregion
    }
}
