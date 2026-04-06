using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Recepcion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.Recepcion;
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
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.REC
{
    public class REC275PanelDeEstrategias : AppController
    {

        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<REC275PanelDeEstrategias> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> EstrategiasGridKeys { get; }
        protected List<SortCommand> EstrategiasDefaultSort { get; }

        protected List<string> InstanciasGridKeys { get; }
        protected List<SortCommand> InstanciasDefaultSort { get; }

        public REC275PanelDeEstrategias(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            ISecurityService security,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            ILogger<REC275PanelDeEstrategias> logger,
            IGridValidationService gridValidationService)
        {
            this.EstrategiasGridKeys = new List<string>
            {
                "NU_ALM_ESTRATEGIA"
            };

            this.EstrategiasDefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_ALM_ESTRATEGIA", SortDirection.Descending)
            };

            this.InstanciasGridKeys = new List<string>
            {
                "NU_ALM_LOGICA_INSTANCIA"
            };

            this.InstanciasDefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_ORDEN", SortDirection.Ascending)
            };

            this._uowFactory = uowFactory;
            this._session = session;
            this._identity = identity;
            this._security = security;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._formValidationService = formValidationService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = true;
            query.IsAddEnabled = false;
            query.IsCommitEnabled = true;

            if (grid.Id == "REC275_grid_1")
            {
                grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
                {
                    new GridButton("btnEditar", "REC275_frm1_btn_MODIFICAR", "fas fa-edit"),
                    new GridButton("btnAsociar", "REC275_frm1_btn_Asociar", "fas fa-bezier-curve"),
                }));
            }
            else if (grid.Id == "REC275_grid_2")
            {
                grid.AddOrUpdateColumn(new GridColumnButton("BTN_UP", new List<GridButton>
                {
                    new GridButton("btnUp", "REC275_frm1_btn_Subir", "fas fa-arrow-up"),
                }));

                grid.AddOrUpdateColumn(new GridColumnButton("BTN_DOWN", new List<GridButton>
                {
                    new GridButton("btnDown", "REC275_frm1_btn_Bajar", "fas fa-arrow-down"),
                }));

                grid.AddOrUpdateColumn(new GridColumnButton("BTN_UPDATE", new List<GridButton>
                {
                    new GridButton("btnEditarInstanciaLogica", "REC275_frm1_btn_MODIFICAR", "fas fa-edit"),
                }));
            }

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new REC275PanelDeEstrategiasFormValidationModule(uow, this._identity), form, context);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                if (grid.Rows.Any())
                {
                    MantenimientoPanelDeEstrategias mantenimientoEstrategias = new MantenimientoPanelDeEstrategias(uow, this._identity.UserId, this._identity.Application);

                    foreach (var row in grid.Rows)
                    {
                        if (grid.Id == "REC275_grid_1")
                        {
                            if (row.IsDeleted)
                            {
                                var codigoEstrategia = row.GetCell("NU_ALM_ESTRATEGIA").Value;
                                short asociaciones = short.Parse(row.GetCell("QT_ASOCIACIONES").Value);
                                var sugerenciasPendientes = uow.EstrategiaRepository.AnySugerenciaEstrategiaPendiente(int.Parse(codigoEstrategia));

                                if (sugerenciasPendientes)
                                    throw new ValidationFailedException("REC275_Sec0_Error_SugerenciasPendientes");
                                else if (asociaciones > 0)
                                    throw new ValidationFailedException("General_Sec0_Error_EstrategiaTieneAsociaciones");
                                else
                                {
                                    var sugerencias = uow.EstrategiaRepository.GetSugerenciasByCodEstrategia(int.Parse(codigoEstrategia));

                                    foreach (var sugerencia in sugerencias)
                                    {
                                        uow.EstrategiaRepository.DeleteSugerencia(sugerencia);
                                        uow.EstrategiaRepository.DeleteSugerenciaDetalle(sugerencia);
                                        //uow.SaveChanges();
                                    }

                                    List<InstanciaLogica> instanciasParametrosEstrategia = uow.EstrategiaRepository.GetAllInstanciaByCodEstrategia(codigoEstrategia);

                                    foreach (var instancia in instanciasParametrosEstrategia)
                                    {
                                        mantenimientoEstrategias.BorrarInstancia(instancia);
                                        uow.SaveChanges();
                                    }

                                    var estrategia = uow.EstrategiaRepository.GetEstrategiaByCod(codigoEstrategia);
                                    mantenimientoEstrategias.BorrarEstrategia(estrategia);
                                }
                            }
                        }
                        else if (grid.Id == "REC275_grid_2")
                        {
                            if (row.IsDeleted)
                            {
                                var codigoInstancia = row.GetCell("NU_ALM_LOGICA_INSTANCIA").Value;

                                var sugerenciasExistentes = uow.EstrategiaRepository.AnySugerenciaLogicaPendiente(int.Parse(codigoInstancia));

                                if (sugerenciasExistentes)
                                    throw new ValidationFailedException("REC275_Sec0_Error_SugerenciasPendientes");

                                var sugerencias = uow.EstrategiaRepository.GetSugerenciasByLogica(int.Parse(codigoInstancia));

                                foreach (var sugerencia in sugerencias)
                                {
                                    uow.EstrategiaRepository.DeleteSugerencia(sugerencia);
                                    uow.EstrategiaRepository.DeleteSugerenciaDetalle(sugerencia);
                                    //uow.SaveChanges();
                                }

                                var instancia = uow.EstrategiaRepository.GetInstanciaByCod(codigoInstancia);
                                mantenimientoEstrategias.BorrarInstancia(instancia);
                            }
                        }
                    }
                }

                uow.SaveChanges();

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "REC275PanelDeEstrategias_GridCommit");
                query.AddErrorNotification(ex.Message);
            }

            return grid;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                MantenimientoPanelDeEstrategias mantenimientoEstrategias = new MantenimientoPanelDeEstrategias(uow, this._identity.UserId, this._identity.Application);

                if (context.ButtonId == "btnSubmitConfirmarEstrategia")
                {
                    var modoEditar = false;

                    if (context.Parameters.Any(x => x.Id == "modoEditar"))
                    {
                        modoEditar = bool.Parse(context.Parameters.Find(x => x.Id == "modoEditar").Value);
                    }

                    if (modoEditar && form.Id == "REC275_form_1")
                    {
                        string codigoEstrategia = form.GetField("codigoEstrategia").Value;
                        Estrategia estrategia = uow.EstrategiaRepository.GetEstrategiaByCod(codigoEstrategia);
                        estrategia.Descripcion = form.GetField("nombreEstrategia").Value;
                        estrategia.FechaModificacion = DateTime.Now;
                        mantenimientoEstrategias.UpdateEstrategia(estrategia);
                    }
                    else if (form.Id == "REC275_form_1")
                    {
                        Estrategia nuevaEstrategia = new Estrategia();
                        nuevaEstrategia.FechaAdicion = DateTime.Now;
                        nuevaEstrategia.Descripcion = form.GetField("nombreEstrategia").Value;
                        mantenimientoEstrategias.RegistrarEstrategia(nuevaEstrategia);

                        context.Parameters.RemoveAll(p => p.Id == "numeroEstrategia");
                        context.Parameters.Add(new ComponentParameter("numeroEstrategia", nuevaEstrategia.NumeroEstrategia.ToString()));
                    }

                    uow.SaveChanges();
                    uow.Commit();

                    context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
                }
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw ex;
            }

            return form;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.Parameters.Any(x => x.Id == "numeroEstrategia"))
            {
                string codigoEstrategia = context.Parameters.FirstOrDefault(s => s.Id == "numeroEstrategia").Value;

                if (!string.IsNullOrEmpty(codigoEstrategia))
                {
                    Estrategia estrategia = uow.EstrategiaRepository.GetEstrategiaByCod(codigoEstrategia);

                    form.GetField("nombreEstrategia").Value = estrategia.Descripcion;
                    form.GetField("codigoEstrategia").Value = estrategia.NumeroEstrategia.ToString();
                }
            }

            return form;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            if (grid.Id == "REC275_grid_1")
            {
                PanelEstrategiasQuery dbQuery = new PanelEstrategiasQuery();
                uow.HandleQuery(dbQuery);

                return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.EstrategiasDefaultSort);
            }
            else if (grid.Id == "REC275_grid_2")
            {
                string codigoEstrategia = query.Parameters.FirstOrDefault(s => s.Id == "numeroEstrategia")?.Value;

                if (!string.IsNullOrEmpty(codigoEstrategia))
                {
                    InstanciaLogicaQuery dbQuery = new InstanciaLogicaQuery(int.Parse(codigoEstrategia));
                    uow.HandleQuery(dbQuery);
                    return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.InstanciasDefaultSort);
                }
                else
                {
                    InstanciaLogicaQuery dbQuery = new InstanciaLogicaQuery();
                    uow.HandleQuery(dbQuery);
                    return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.InstanciasDefaultSort);
                }
            }

            return null;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Id == "REC275_grid_1")
            {
                PanelEstrategiasQuery dbQuery = new PanelEstrategiasQuery();
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else if (grid.Id == "REC275_grid_2")
            {
                if (query.Parameters.Any(x => x.Id == "numeroEstrategia"))
                {
                    string codigoEstrategia = query.Parameters.FirstOrDefault(s => s.Id == "numeroEstrategia").Value;

                    if (!string.IsNullOrEmpty(codigoEstrategia))
                    {
                        InstanciaLogicaQuery dbQuery = new InstanciaLogicaQuery(int.Parse(codigoEstrategia));
                        uow.HandleQuery(dbQuery);
                        dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                        return new GridStats
                        {
                            Count = dbQuery.GetCount()
                        };
                    }
                    else
                    {
                        InstanciaLogicaQuery dbQuery = new InstanciaLogicaQuery();
                        uow.HandleQuery(dbQuery);
                        dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                        return new GridStats
                        {
                            Count = dbQuery.GetCount()
                        };
                    }
                }
            }

            return null;
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Id == "REC275_grid_1")
            {
                PanelEstrategiasQuery dbQuery = new PanelEstrategiasQuery();
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.EstrategiasDefaultSort, this.EstrategiasGridKeys);
            }
            else if (grid.Id == "REC275_grid_2")
            {
                string numeroEstrategia = query.Parameters.FirstOrDefault(s => s.Id == "numeroEstrategia")?.Value;
                if (!string.IsNullOrEmpty(numeroEstrategia))
                {
                    InstanciaLogicaQuery dbQuery = new InstanciaLogicaQuery(int.Parse(numeroEstrategia));
                    uow.HandleQuery(dbQuery);
                    grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.InstanciasDefaultSort, this.InstanciasGridKeys);
                }
                else
                {
                    InstanciaLogicaQuery dbQuery = new InstanciaLogicaQuery();
                    uow.HandleQuery(dbQuery);
                    grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.InstanciasDefaultSort, this.InstanciasGridKeys);
                }

                grid.Rows.ForEach(row =>
                {
                    DisableButtons(row, uow, grid.Rows.Count(), query, numeroEstrategia);
                });
            }

            return grid;
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            switch (context.ButtonId)
            {
                case "btnUp":
                    try
                    {
                        var codigoInstancia = context.Row.GetCell("NU_ALM_LOGICA_INSTANCIA").Value;
                        InstanciaLogica estrategiaSubir = uow.EstrategiaRepository.GetInstanciaByCod(codigoInstancia);

                        int numeroOrden = estrategiaSubir.Orden - 1;
                        int numeroEstrategia = estrategiaSubir.Estrategia;

                        InstanciaLogica estrategiaBajar = uow.EstrategiaRepository.GetInstanciaForChangeOrder(Convert.ToInt16(numeroOrden), numeroEstrategia);

                        estrategiaSubir.Orden--;
                        estrategiaBajar.Orden++;

                        uow.EstrategiaRepository.UpdateEstrategiaInstancia(estrategiaSubir);
                        uow.EstrategiaRepository.UpdateEstrategiaInstancia(estrategiaBajar);

                        uow.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(ex, "REC275GridButtonAction");
                    }
                    break;
                case "btnDown":
                    try
                    {
                        var codigoInstancia = context.Row.GetCell("NU_ALM_LOGICA_INSTANCIA").Value;
                        InstanciaLogica estrategiaBajar = uow.EstrategiaRepository.GetInstanciaByCod(codigoInstancia);

                        int numeroOrden = estrategiaBajar.Orden + 1;
                        int numeroEstrategia = estrategiaBajar.Estrategia;

                        InstanciaLogica estrategiaSubir = uow.EstrategiaRepository.GetInstanciaForChangeOrder(Convert.ToInt16(numeroOrden), numeroEstrategia);

                        estrategiaBajar.Orden++;
                        estrategiaSubir.Orden--;

                        uow.EstrategiaRepository.UpdateEstrategiaInstancia(estrategiaSubir);
                        uow.EstrategiaRepository.UpdateEstrategiaInstancia(estrategiaBajar);

                        uow.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(ex, "REC275GridButtonAction");
                    }
                    break;
            }

            return context;
        }

        public virtual void DisableButtons(GridRow row, IUnitOfWork uow, int cantidadFilas, GridFetchContext query, string _numeroEstrategia)
        {
            string numeroEstrategia = _numeroEstrategia;
            string numeroInstancia = row.GetCell("NU_ALM_LOGICA_INSTANCIA").Value;


            InstanciaLogica logicaInstancia = uow.EstrategiaRepository.GetInstanciaByCod(numeroInstancia);

            //Si es la primera desactivamos el boton
            if (logicaInstancia.Orden == 1)
                row.DisabledButtons.Add("btnUp");

            //Si es la ultima desactivamos
            if (logicaInstancia.Orden == cantidadFilas)
                row.DisabledButtons.Add("btnDown");
        }
    }
}
