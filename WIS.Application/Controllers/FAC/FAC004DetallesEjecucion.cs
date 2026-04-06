using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Components.Common;
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
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.FAC
{
    public class FAC004DetallesEjecucion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<FAC004DetallesEjecucion> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public FAC004DetallesEjecucion(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            ILogger<FAC004DetallesEjecucion> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "NU_EJECUCION",
                "CD_PROCESO",
                "CD_EMPRESA"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_EJECUCION", SortDirection.Ascending),
            };

            this._uowFactory = uowFactory;
            this._session = session;
            this._identity = identity;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._formValidationService = formValidationService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            this.InicializarParams(form, query);

            return form;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton> {
                new GridButton("btnCancelFAC", "FAC004_grid1_btn_CancelFac", "fas fa-times-circle", new ConfirmMessage("FAC004_grid1_msg_ConfirmarCancelFac"))
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(context.GetParameter("nuEjecucion"), out int nuEjecucion))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var dbQuery = new DetallesEjecucionQuery(nuEjecucion);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            DisableButtons(uow, grid.Rows, context);

            return grid;
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber(this._identity.Application);
            uow.BeginTransaction();

            try
            {
                switch (context.ButtonId)
                {
                    case "btnCancelFAC":
                        CancelarFacturacion(uow, context);
                        context.AddSuccessNotification("FAC004_Sec0_Success_CancelarFacturacion");
                        break;
                }

                uow.SaveChanges();
                uow.Commit();
            }
            catch (ValidationFailedException ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    context.AddErrorNotification(ex.Message, new List<string>(ex.StrArguments ?? new string[0]));
                uow.Rollback();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }
            return context;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(query.GetParameter("nuEjecucion"), out int nuEjecucion))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var dbQuery = new DetallesEjecucionQuery(nuEjecucion);
            uow.HandleQuery(dbQuery);
            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(query.GetParameter("nuEjecucion"), out int nuEjecucion))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var dbQuery = new DetallesEjecucionQuery(nuEjecucion);
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
                throw new ValidationFailedException("FAC004_Sec0_Error_NroEjecucionVacio");

            FacturacionEjecucion facturacionEjecucion = uow.FacturacionRepository.GetFacturacionEjecucion(int.Parse(nuEjecucion));

            query.AddParameter("nombre", facturacionEjecucion.Nombre);
            query.AddParameter("fechaDesde", facturacionEjecucion.FechaDesde.ToString());
            query.AddParameter("fechaHasta", facturacionEjecucion.FechaHasta.ToString());
            query.AddParameter("fechaProgramacion", facturacionEjecucion.FechaProgramacion.ToString());
            query.AddParameter("facturaParcial", facturacionEjecucion.EjecucionPorHora);
            query.AddParameter("situacion", facturacionEjecucion.CodigoSituacion.ToString());
        }
        public virtual void DisableButtons(IUnitOfWork uow, List<GridRow> rows, GridFetchContext context)
        {
            short.TryParse(context.Parameters.Find(x => x.Id == "Situacion")?.Value, out short situacionEjecucion);

            foreach (var row in rows)
            {
                string estado = row.GetCell("ID_ESTADO").Value;

                if (estado == FacturacionDb.ESTADO_CAN || situacionEjecucion != SituacionDb.EJECUCION_REALIZADA)
                    row.DisabledButtons.Add("btnCancelFAC");
            }
        }

        public virtual void DescargarExcel(GridButtonActionContext context)
        {
            string nuEjecucion = context.Row.GetCell("NU_EJECUCION").Value;
            string cdProceso = context.Row.GetCell("CD_PROCESO").Value;
            string cdEmpresa = context.Row.GetCell("CD_EMPRESA").Value;

            using var uow = this._uowFactory.GetUnitOfWork();

            FacturacionProceso proceso = uow.FacturacionRepository.GetFacturacionProceso(cdProceso);
            string cdFacturacion = proceso.CodigoFacturacion;

            context.Redirect("/api/File/Download", true, new List<ComponentParameter> {
                        new ComponentParameter() { Id = "fileId", Value = $"{nuEjecucion},{cdEmpresa},{cdFacturacion}"} ,
                        new ComponentParameter() { Id = "application", Value = _identity.Application }
                    });
        }

        public virtual void CancelarFacturacion(IUnitOfWork uow, GridButtonActionContext context)
        {
            var nuEjecucion = int.Parse(context.Row.GetCell("NU_EJECUCION").Value);
            var empresa = int.Parse(context.Row.GetCell("CD_EMPRESA").Value);
            string cdProceso = context.Row.GetCell("CD_PROCESO").Value;

            FacturacionLogic.CancelarFacturacion(uow, nuEjecucion, empresa, cdProceso);
        }
    }
}
