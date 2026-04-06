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
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.FAC
{
    public class FAC007IngresoResultadosManuales : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<FAC007IngresoResultadosManuales> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public FAC007IngresoResultadosManuales(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            ISecurityService security,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<FAC007IngresoResultadosManuales> logger,
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
                new SortCommand("NU_EJECUCION", SortDirection.Ascending),
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

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            return form;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsAddEnabled = false;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = true;

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string nuEjecucion = query.GetParameter("nuEjecucion");

            if (string.IsNullOrEmpty(nuEjecucion))
                return grid;

            IngresoResultadosManualesQuery dbQuery = new IngresoResultadosManualesQuery(int.Parse(nuEjecucion), "FAC007");

            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            grid.Rows.ForEach(row =>
            {
                string situacion = row.GetCell("CD_SITUACAO").Value;
                if (situacion == SituacionDb.CALCULO_EJECUTADO.ToString() || situacion == SituacionDb.CALCULO_CON_ERRORES.ToString())
                {
                    row.SetEditableCells(new List<string> {
                        "QT_RESULTADO",
                        "DS_ADICIONAL"
                    });
                }
            });

            return grid;
        }
        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                if (grid.Rows.Any())
                {
                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    uow.CreateTransactionNumber(this._identity.Application);
                    uow.BeginTransaction();

                    var registroModificacionIRM = new RegistroModificacionIngresoResultadosManuales(uow, this._identity.UserId, this._identity.Application);

                    foreach (var row in grid.Rows)
                    {
                        if (row.IsDeleted)
                        {
                            // rows delete
                            this.DeleteFacturacionResultado(uow, row, query);
                        }
                        else
                        {
                            // rows editadas
                            var facturacionResultado = this.UpdateFacturacionResultado(uow, row, query);
                            registroModificacionIRM.ModificarFacturacionResultado(facturacionResultado);
                        }
                    }

                    uow.SaveChanges();
                    uow.Commit();
                }

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "FAC007GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string nuEjecucion = query.GetParameter("nuEjecucion");

            IngresoResultadosManualesQuery dbQuery = new IngresoResultadosManualesQuery(int.Parse(nuEjecucion), "FAC007");

            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string nuEjecucion = query.GetParameter("nuEjecucion");

            if (string.IsNullOrEmpty(nuEjecucion))
                return null;

            IngresoResultadosManualesQuery dbQuery = new IngresoResultadosManualesQuery(int.Parse(nuEjecucion), "FAC007");

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public virtual void DeleteFacturacionResultado(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            var cdEmpresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
            var cdFacturacion = row.GetCell("CD_FACTURACION").Value;
            var nuEjecucion = int.Parse(row.GetCell("NU_EJECUCION").Value);
            var nuComponente = row.GetCell("NU_COMPONENTE").Value;

            if (uow.FacturacionRepository.AnyFacturacionResultado(cdEmpresa, cdFacturacion, nuEjecucion, nuComponente))
            {
                var facturacionResultado = uow.FacturacionRepository.GetFacturacionResultado(cdEmpresa, cdFacturacion, nuEjecucion, nuComponente);
                var nuTransaccion = uow.GetTransactionNumber();

                facturacionResultado.NumeroTransaccion = nuTransaccion;
                facturacionResultado.NumeroTransaccionDelete = nuTransaccion;
                facturacionResultado.FechaActualizacion = DateTime.Now;

                uow.FacturacionRepository.UpdateFacturacionResultado(facturacionResultado);
                uow.SaveChanges();

                uow.FacturacionRepository.DeleteFacturacionResultado(cdEmpresa, cdFacturacion, nuEjecucion, nuComponente);
            }
            else
            {
                throw new EntityNotFoundException("FAC007_Sec0_Error_Er001_FacturacionResultadoNoExisteEliminar");
            }
        }

        public virtual FacturacionResultado UpdateFacturacionResultado(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            string cdEmpresa = row.GetCell("CD_EMPRESA").Value;
            string nuEjecucion = row.GetCell("NU_EJECUCION").Value;
            string cdFacturacion = row.GetCell("CD_FACTURACION").Value;
            string componente = row.GetCell("NU_COMPONENTE").Value;

            var facturacionResultado = new FacturacionResultado();
            facturacionResultado = uow.FacturacionRepository.GetFacturacionResultado(int.Parse(cdEmpresa), cdFacturacion, int.Parse(nuEjecucion), componente);

            facturacionResultado.DescripcionAdicional = row.GetCell("DS_ADICIONAL").Value;
            facturacionResultado.FechaActualizacion = DateTime.Now;
            facturacionResultado.NumeroTransaccion = uow.GetTransactionNumber();

            string resultado = row.GetCell("QT_RESULTADO").Value;
            if (!string.IsNullOrEmpty(resultado))
                facturacionResultado.CantidadResultado = decimal.Parse(resultado, this._identity.GetFormatProvider());

            return facturacionResultado;
        }
    }
}
