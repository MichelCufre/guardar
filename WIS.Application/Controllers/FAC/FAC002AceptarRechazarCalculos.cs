using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Facturacion;
using WIS.Domain.Facturacion;
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
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.FAC
{
    public class FAC002AceptarRechazarCalculos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<FAC002AceptarRechazarCalculos> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public FAC002AceptarRechazarCalculos(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            ISecurityService security,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<FAC002AceptarRechazarCalculos> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "NU_EJECUCION",
                "CD_EMPRESA",
                "CD_FACTURACION",
                "NU_COMPONENTE"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_EJECUCION", SortDirection.Descending),
            };

            this._uowFactory = uowFactory;
            this._session = session;
            this._identity = identity;
            this._security = security;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            _filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsAddEnabled = false;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = false;

            grid.MenuItems = new List<IGridItem> {
                    new GridButton("btnAprobar", "FAC002_Sec0_btn_btnAprobar"),
                    new GridButton("btnRechazar", "FAC002_Sec0_btn_btnRechazar"),
                    new GridButton("btnErrores", "FAC002_Sec0_btn_btnErrores")
            };

            //Select
            using var uow = this._uowFactory.GetUnitOfWork();
            grid.AddOrUpdateColumn(new GridColumnSelect("CD_SITUACAO", this.SelectSituacion(uow)));

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string nuEjecucion = query.GetParameter("nuEjecucion");

            AceptarRechazarCalculosQuery dbQuery;
            if (!string.IsNullOrEmpty(nuEjecucion))
                dbQuery = new AceptarRechazarCalculosQuery(int.Parse(nuEjecucion));
            else
                dbQuery = new AceptarRechazarCalculosQuery();

            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> { "CD_SITUACAO" });

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string nuEjecucion = query.GetParameter("nuEjecucion");

            AceptarRechazarCalculosQuery dbQuery;
            if (!string.IsNullOrEmpty(nuEjecucion))
                dbQuery = new AceptarRechazarCalculosQuery(int.Parse(nuEjecucion));
            else
                dbQuery = new AceptarRechazarCalculosQuery();

            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoAceptarRechazarCalculosGridValidationModule(uow), grid, row, context);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var ejecucionesRechazables = new HashSet<int>();

            try
            {
                if (grid.Rows.Any())
                {
                    var registroModificacionARC = new RegistroModificacionAceptarRechazarCalculos(uow, this._identity.UserId, this._identity.Application);

                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    uow.CreateTransactionNumber(this._identity.Application);

                    // rows editadas
                    foreach (var row in grid.Rows)
                    {
                        var facturacionResultado = this.UpdateFacturacionResultado(uow, row, query);

                        if (facturacionResultado.CodigoSituacion == SituacionDb.CALCULO_RECHAZADO
                            && !ejecucionesRechazables.Contains(facturacionResultado.NumeroEjecucion))
                        {
                            ejecucionesRechazables.Add(facturacionResultado.NumeroEjecucion);
                        }

                        registroModificacionARC.ModificarFacturacionResultado(facturacionResultado);
                    }
                }

                uow.SaveChanges();

                foreach (var nuEjecucion in ejecucionesRechazables)
                    TryRechazarEjecucion(uow, nuEjecucion);

                uow.SaveChanges();

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "FAC002GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            switch (context.ButtonId)
            {
                case "btnAprobar":
                    UpdateSelectionFacturacionResultado(context, SituacionDb.CALCULO_ACEPTADO.ToString());
                    break;
                case "btnRechazar":
                    UpdateSelectionFacturacionResultado(context, SituacionDb.CALCULO_RECHAZADO.ToString());
                    break;
                case "btnErrores":
                    UpdateSelectionFacturacionResultado(context, SituacionDb.CALCULO_CON_ERRORES.ToString());
                    break;
            }

            return context;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string nuEjecucion = query.GetParameter("nuEjecucion");

            AceptarRechazarCalculosQuery dbQuery;
            if (!string.IsNullOrEmpty(nuEjecucion))
                dbQuery = new AceptarRechazarCalculosQuery(int.Parse(nuEjecucion));
            else
                dbQuery = new AceptarRechazarCalculosQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public virtual FacturacionResultado UpdateFacturacionResultado(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            string cdEmpresa = row.GetCell("CD_EMPRESA").Value;
            string cdFacturacion = row.GetCell("CD_FACTURACION").Value;
            string nuEjecucion = row.GetCell("NU_EJECUCION").Value;
            string nuComponente = row.GetCell("NU_COMPONENTE").Value;

            var facturacionResultado = uow.FacturacionRepository.GetFacturacionResultado(int.Parse(cdEmpresa), cdFacturacion, int.Parse(nuEjecucion), nuComponente);

            if (facturacionResultado.CodigoSituacion == SituacionDb.CALCULO_FACTURADO)
                throw new ValidationFailedException("FAC002_Sec0_Error_Er001_CodigoYaFacturado", new string[] { facturacionResultado.CodigoFacturacion, facturacionResultado.NumeroComponente, Convert.ToString(facturacionResultado.CodigoEmpresa) });

            string situacion = row.GetCell("CD_SITUACAO").Value;
            if (situacion == SituacionDb.CALCULO_ACEPTADO.ToString())
            {
                facturacionResultado.CodigoSituacion = SituacionDb.CALCULO_ACEPTADO;
            }
            else if (situacion == SituacionDb.CALCULO_RECHAZADO.ToString())
            {
                facturacionResultado.CodigoSituacion = SituacionDb.CALCULO_RECHAZADO;
            }
            else if (situacion == SituacionDb.CALCULO_CON_ERRORES.ToString())
            {
                facturacionResultado.CodigoSituacion = SituacionDb.CALCULO_CON_ERRORES;
            }

            facturacionResultado.NumeroTransaccion = uow.GetTransactionNumber();

            return facturacionResultado;
        }

        public virtual FacturacionResultado UpdateSelectionFacturacionResultado(GridMenuItemActionContext context, string codigoSituacion)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber("UpdateSelectionFacturacionResultado");

            var registroModificacionARC = new RegistroModificacionAceptarRechazarCalculos(uow, this._identity.UserId, this._identity.Application);

            try
            {
                var keysRowSelected = this.GetSelectedKeys(uow, context);
                var ejecucionesRechazables = new HashSet<int>();

                keysRowSelected.ForEach(key =>
                {
                    string nuEjecucion = key[0];
                    string cdEmpresa = key[1];
                    string cdFacturacion = key[2];
                    string nuComponente = key[3];

                    var facturacionResultado = uow.FacturacionRepository.GetFacturacionResultado(int.Parse(cdEmpresa), cdFacturacion, int.Parse(nuEjecucion), nuComponente);

                    if (facturacionResultado.CodigoSituacion == SituacionDb.CALCULO_FACTURADO)
                        throw new ValidationFailedException("FAC002_Sec0_Error_Er001_CodigoYaFacturado", new string[] { facturacionResultado.CodigoFacturacion, facturacionResultado.NumeroComponente, Convert.ToString(facturacionResultado.CodigoEmpresa) });

                    if (codigoSituacion == SituacionDb.CALCULO_ACEPTADO.ToString())
                    {
                        facturacionResultado.CodigoSituacion = SituacionDb.CALCULO_ACEPTADO;
                    }
                    else if (codigoSituacion == SituacionDb.CALCULO_RECHAZADO.ToString())
                    {
                        facturacionResultado.CodigoSituacion = SituacionDb.CALCULO_RECHAZADO;

                        if (!ejecucionesRechazables.Contains(facturacionResultado.NumeroEjecucion))
                        {
                            ejecucionesRechazables.Add(facturacionResultado.NumeroEjecucion);
                        }
                    }
                    else if (codigoSituacion == SituacionDb.CALCULO_CON_ERRORES.ToString())
                    {
                        facturacionResultado.CodigoSituacion = SituacionDb.CALCULO_CON_ERRORES;
                    }

                    facturacionResultado.NumeroTransaccion = uow.GetTransactionNumber();

                    registroModificacionARC.ModificarFacturacionResultado(facturacionResultado);
                });

                uow.SaveChanges();

                foreach (var nuEjecucion in ejecucionesRechazables)
                    TryRechazarEjecucion(uow, nuEjecucion);

                uow.SaveChanges();

                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "FAC002GridCommit");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return null;
        }

        public virtual void TryRechazarEjecucion(IUnitOfWork uow, int nuEjecucion)
        {
            var ejecucion = uow.FacturacionRepository.GetFacturacionEjecucion(nuEjecucion);

            if (ejecucion != null)
            {
                var estadoActual = ejecucion.CodigoSituacion ?? -1;
                var rechazable = estadoActual != SituacionDb.CALCULO_FACTURADO;

                if (rechazable && !uow.FacturacionRepository.AnyFacturacionResultadoNoRechazado(nuEjecucion))
                {
                    ejecucion.CodigoSituacion = SituacionDb.CALCULO_RECHAZADO;
                    uow.FacturacionRepository.UpdateFacturacionEjecucion(ejecucion);
                }
            }
        }

        public virtual List<string[]> GetSelectedKeys(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            string nuEjecucion = context.GetParameter("nuEjecucion");

            AceptarRechazarCalculosQuery dbQuery;
            if (!string.IsNullOrEmpty(nuEjecucion))
                dbQuery = new AceptarRechazarCalculosQuery(int.Parse(nuEjecucion));
            else
                dbQuery = new AceptarRechazarCalculosQuery();

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys);

            return dbQuery.GetSelectedKeys(context.Selection.Keys);
        }

        public virtual List<SelectOption> SelectSituacion(IUnitOfWork uow)
        {
            List<int> situaciones = new List<int>
            {
                SituacionDb.CALCULO_CON_ERRORES,
                SituacionDb.CALCULO_ACEPTADO,
                SituacionDb.CALCULO_RECHAZADO
            };

            return uow.SituacionRepository.GetSituaciones(situaciones).Select(w => new SelectOption(w.Id.ToString(), w.Id + " - " + w.Descripcion)).ToList();
        }
    }
}
