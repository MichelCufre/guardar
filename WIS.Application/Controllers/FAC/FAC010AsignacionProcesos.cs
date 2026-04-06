using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Facturacion;
using WIS.Domain.Facturacion;
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
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.FAC
{
    public class FAC010AsignacionProcesos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<FAC010AsignacionProcesos> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public FAC010AsignacionProcesos(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            ITrafficOfficerService concurrencyControl,
            IIdentityService identity,
            ISecurityService security,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<FAC010AsignacionProcesos> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "CD_EMPRESA",
                "CD_PROCESO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_EMPRESA", SortDirection.Ascending),
            };

            this._uowFactory = uowFactory;
            this._session = session;
            this._concurrencyControl = concurrencyControl;
            this._identity = identity;
            this._security = security;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
        }

        public override PageContext PageLoad(PageContext data)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string nuEjecucion = data.GetParameter("nuEjecucion");

            if (!this._concurrencyControl.IsLocked("T_FACTURACION_EJECUCION", nuEjecucion))
                this._concurrencyControl.AddLock("T_FACTURACION_EJECUCION", nuEjecucion);

            return base.PageLoad(data);
        }

        public override PageContext PageUnload(PageContext data)
        {
            this._concurrencyControl.ClearToken();

            return base.PageUnload(data);
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            this.InicializarParams(form, query);

            return form;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("btnAsignacionSolapada", "FAC010_frm1_lbl_AsignacionSolapada", "fa fa-exclamation-triangle fa-lg")
            }));

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string nuEjecucion = query.GetParameter("nuEjecucion");
            string parcial = query.GetParameter("parcial");

            if (string.IsNullOrEmpty(nuEjecucion))
                return grid;

            AsignacionProcesosQuery dbQuery = null;

            switch (grid.Id)
            {
                case "FAC010_grid_1":
                    dbQuery = new AsignacionProcesosQuery(parcial, true, nuEjecucion);
                    break;

                case "FAC010_grid_2":
                    dbQuery = new AsignacionProcesosQuery(parcial, false, nuEjecucion);
                    break;
            }

            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            this.ComprobarSolapamientos(uow, grid, query);

            var facturacionEjecucion = uow.FacturacionRepository.GetFacturacionEjecucion(int.Parse(nuEjecucion));

            if (this._concurrencyControl.IsLocked("T_FACTURACION_EJECUCION", nuEjecucion) || facturacionEjecucion.CodigoSituacion != SituacionDb.EJECUCION_EN_PROGRAMACION)
            {
                //Display modoLectura
                query.AddParameter("ModoLectura", "S");
            }

            return grid;
        }

        protected virtual void ComprobarSolapamientos(IUnitOfWork uow, Grid grid, GridFetchContext query)
        {
            int.TryParse(query.GetParameter("nuEjecucion"), out int nuEjecucion);

            foreach (var row in grid.Rows)
            {
                var cdEmpresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                var cdProceso = row.GetCell("CD_PROCESO").Value;
                var sobreProcesosAsignados = grid.Id == "FAC010_grid_2";
                var ejecucionSolapada = uow.FacturacionRepository.GetAnyFacturacionEjecucionSolapada(nuEjecucion, cdEmpresa, cdProceso, true, sobreProcesosAsignados);

                if (ejecucionSolapada != null)
                {
                    row.CssClass = "solapado";
                    row.GetCell("NU_EJECUCION_SOLAPADA").Value = ejecucionSolapada.NumeroEjecucion.ToString();
                }
                else
                {
                    row.DisabledButtons = new List<string> { "btnAsignacionSolapada" };
                }
            }
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            switch (context.ButtonId)
            {
                case "btnAsociar":
                    UpdateSelectionAsociar(context, true);
                    break;
                case "btnDesasociar":
                    UpdateSelectionAsociar(context, false);
                    break;
            }

            return context;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string nuEjecucion = query.GetParameter("nuEjecucion");

            if (string.IsNullOrEmpty(nuEjecucion))
                return null;

            string parcial = query.GetParameter("parcial");

            AsignacionProcesosQuery dbQuery = null;

            switch (grid.Id)
            {
                case "FAC010_grid_1":
                    dbQuery = new AsignacionProcesosQuery(parcial, true, nuEjecucion);
                    break;

                case "FAC010_grid_2":
                    dbQuery = new AsignacionProcesosQuery(parcial, false, nuEjecucion);
                    break;
            }

            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            var columns = new List<IGridColumn>();

            columns.AddRange(grid.Columns.Where(c => c.Id != "NU_EJECUCION_SOLAPADA"));

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, columns, query, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            string nuEjecucion = query.GetParameter("nuEjecucion");
            string parcial = query.GetParameter("parcial");

            if (string.IsNullOrEmpty(nuEjecucion))
                return null;

            AsignacionProcesosQuery dbQuery = null;
            switch (grid.Id)
            {
                case "FAC010_grid_1":
                    dbQuery = new AsignacionProcesosQuery(parcial, true, nuEjecucion);
                    break;

                case "FAC010_grid_2":
                    dbQuery = new AsignacionProcesosQuery(parcial, false, nuEjecucion);
                    break;
            }

            using var uow = this._uowFactory.GetUnitOfWork();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public virtual void InicializarParams(Form form, FormInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string nuEjecucion = query.GetParameter("nuEjecucion");

            if (string.IsNullOrEmpty(nuEjecucion))
                throw new ValidationFailedException("FAC010_Sec0_Error_NroEjecucionVacio");

            var facturacionEjecucion = uow.FacturacionRepository.GetFacturacionEjecucion(int.Parse(nuEjecucion));

            query.AddParameter("nombre", facturacionEjecucion.Nombre);
            query.AddParameter("fechaDesde", facturacionEjecucion.FechaDesde.ToString());
            query.AddParameter("fechaHasta", facturacionEjecucion.FechaHasta.ToString());
        }

        public virtual FacturacionEjecucionEmpresa CreateFacturacionEjecucionEmpresa(IUnitOfWork uow, int nuEjecucion, int cdEmpresa, string cdProceso)
        {
            var facturacionEjecucionEmpresa = new FacturacionEjecucionEmpresa();
            var registro = uow.FacturacionRepository.GetFacturacionEjecucion(nuEjecucion);

            if (registro != null)
            {
                facturacionEjecucionEmpresa.NumeroEjecucion = nuEjecucion;
                facturacionEjecucionEmpresa.CodigoEmpresa = cdEmpresa;
                facturacionEjecucionEmpresa.CodigoProceso = cdProceso;
                facturacionEjecucionEmpresa.CodigoSituacion = SituacionDb.PENDIENTE_EJECUTAR_CALCULO;
                facturacionEjecucionEmpresa.FechaDesde = registro.FechaDesde;
                facturacionEjecucionEmpresa.FechaHasta = registro.FechaHasta;
            }

            return facturacionEjecucionEmpresa;
        }

        public virtual FacturacionResultado UpdateSelectionAsociar(GridMenuItemActionContext context, bool asociar)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var registroModificacionAP = new RegistroModificacionAsignacionProcesos(uow, this._identity.UserId, this._identity.Application);

            try
            {
                List<string[]> keysRowSelected = this.GetSelectedKeys(uow, context, asociar);

                keysRowSelected.ForEach(key =>
                {
                    string nuEjecucion = context.GetParameter("nuEjecucion");
                    string cdEmpresa = key[0];
                    string cdProceso = key[1];

                    if (asociar)
                    {
                        if (!uow.FacturacionRepository.AnyFacturacionEjecucionEmpresa(int.Parse(nuEjecucion), int.Parse(cdEmpresa), cdProceso))
                        {
                            var facturacionEjecucionEmpresa = this.CreateFacturacionEjecucionEmpresa(uow, int.Parse(nuEjecucion), int.Parse(cdEmpresa), cdProceso);
                            registroModificacionAP.RegistrarFacturacionEjecucionEmpresa(facturacionEjecucionEmpresa);
                        }
                    }
                    else if (uow.FacturacionRepository.AnyFacturacionEjecucionEmpresa(int.Parse(nuEjecucion), int.Parse(cdEmpresa), cdProceso))
                    {
                        registroModificacionAP.RemoverFacturacionEjecucionEmpresa(int.Parse(nuEjecucion), int.Parse(cdEmpresa), cdProceso);
                    }
                });

                uow.SaveChanges();

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "FAC010GridCommit");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return null;
        }

        public virtual List<string[]> GetSelectedKeys(IUnitOfWork uow, GridMenuItemActionContext context, bool asociar)
        {
            string nuEjecucion = context.GetParameter("nuEjecucion");
            string parcial = context.GetParameter("parcial");

            var dbQuery = new AsignacionProcesosQuery(parcial, asociar, nuEjecucion);

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys);

            return dbQuery.GetSelectedKeys(context.Selection.Keys);
        }
    }
}
