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
    public class FAC006ResultadosEjecucion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<FAC006ResultadosEjecucion> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public FAC006ResultadosEjecucion(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<FAC006ResultadosEjecucion> logger,
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
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("btnDetalle", "FAC006_grid1_btn_Detalle", "fas fa-list"),
                new GridButton("btnAsociar", "FAC006_grid1_btn_Asociar", "fas fa-check")
            }));

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int? nroEject = null;
            if (int.TryParse(query.GetParameter("nuEjecucion"), out int parsedValue))
                nroEject = parsedValue;

            var dbQuery = new ResultadosEjecucionQuery(nroEject, SituacionDb.EJECUCION_PENDIENTE);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            DisableButtons(grid.Rows, uow);

            return grid;
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            switch (context.ButtonId)
            {
                case "btnAsociar":
                    this.CreateCodigoComponente(context);
                    break;

                case "btnDetalle":
                    context.Redirect("/facturacion/FAC008", true, new List<ComponentParameter>()
                    {
                        new ComponentParameter(){ Id = "nuEjecucion", Value = context.Row.GetCell("NU_EJECUCION").Value },
                        new ComponentParameter(){ Id = "cdFacturacion", Value = context.Row.GetCell("CD_FACTURACION").Value },
                        new ComponentParameter(){ Id = "cdEmpresa", Value = context.Row.GetCell("CD_EMPRESA").Value },
                        new ComponentParameter(){ Id = "nuComponente", Value = context.Row.GetCell("NU_COMPONENTE").Value },
                    });
                    break;
            }

            return context;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int? nroEject = null;
            if (int.TryParse(query.GetParameter("nuEjecucion"), out int parsedValue))
                nroEject = parsedValue;

            var dbQuery = new ResultadosEjecucionQuery(nroEject, SituacionDb.EJECUCION_PENDIENTE);
            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int? nroEject = null;
            if (int.TryParse(query.GetParameter("nuEjecucion"), out int parsedValue))
                nroEject = parsedValue;

            var dbQuery = new ResultadosEjecucionQuery(nroEject, SituacionDb.EJECUCION_PENDIENTE);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public virtual void DisableButtons(List<GridRow> rows, IUnitOfWork uow)
        {
            foreach (var row in rows)
            {
                //Disable btnAsociar
                if (uow.FacturacionRepository.AnyFacturacion(row.GetCell("NU_COMPONENTE").Value, row.GetCell("CD_FACTURACION").Value))
                    row.DisabledButtons.Add("btnAsociar");
            }
        }

        public virtual void CreateCodigoComponente(GridButtonActionContext context)
        {
            string cdFacturacion = context.Row.GetCell("CD_FACTURACION").Value;
            string nuComponente = context.Row.GetCell("NU_COMPONENTE").Value;
            string cdEmpresa = context.Row.GetCell("CD_EMPRESA").Value;

            using var uow = this._uowFactory.GetUnitOfWork();

            //Verifico que no este ya ingresado
            if (uow.FacturacionRepository.AnyFacturacion(cdFacturacion, nuComponente))
                throw new ValidationFailedException("FAC006_Sec0_Error_FacturacionExiste");

            uow.CreateTransactionNumber("CreateCodigoComponente");

            var facturacionCodigoComponente = new FacturacionCodigoComponente()
            {
                Id = cdFacturacion.ToUpper().Trim(),
                NumeroComponente = nuComponente.ToUpper().Trim(),
                NumeroCuentaContable = context.Row.GetCell("NU_CUENTA_CONTABLE").Value,
                Descripcion = cdFacturacion + "_" + nuComponente.ToUpper(),
                FechaIngresado = DateTime.Now,
                FechaActualizado = DateTime.Now,
                NumeroTransaccion = uow.GetTransactionNumber(),
            };

            uow.FacturacionRepository.AddFacturacion(facturacionCodigoComponente);
            uow.SaveChanges();

            context.AddSuccessNotification("FAC006_Sec0_Success_CreateCodComponente");

            var empresa = uow.EmpresaRepository.GetEmpresa(int.Parse(cdEmpresa));
            var parametros = new List<ComponentParameter>()
            {
                new ComponentParameter(){ Id = "cdFacturacion", Value = context.Row.GetCell("CD_FACTURACION").Value.ToUpper().Trim() },
                new ComponentParameter(){ Id = "nuComponente", Value = context.Row.GetCell("NU_COMPONENTE").Value.ToUpper().Trim() },
            };

            if (empresa.ListaPrecio.HasValue)
            {
                parametros.Add(new ComponentParameter() { Id = "idListaPrecio", Value = empresa.ListaPrecio?.ToString() });
            }

            //Redirect cotizaciones de listas
            context.Redirect("/facturacion/FAC256", true, parametros);
        }
    }
}
