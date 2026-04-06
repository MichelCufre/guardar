using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
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
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.FAC
{
    public class FAC005ProcesoFacturacionEmpresa : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<FAC005ProcesoFacturacionEmpresa> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public FAC005ProcesoFacturacionEmpresa(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<FAC005ProcesoFacturacionEmpresa> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "CD_EMPRESA",
                "CD_PROCESO",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_EMPRESA", SortDirection.Ascending),
                new SortCommand("CD_PROCESO", SortDirection.Ascending),
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            _filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsAddEnabled = true;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = true;

            grid.SetInsertableColumns(new List<string> {
                "CD_EMPRESA",
                "CD_PROCESO"
            });

            using var uow = this._uowFactory.GetUnitOfWork();
            grid.AddOrUpdateColumn(new GridColumnSelect("CD_EMPRESA", this.SelectEmpresa(uow)));
            grid.AddOrUpdateColumn(new GridColumnSelect("CD_PROCESO", this.SelectProceso(uow)));

            return this.GridFetchRows(grid, query.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ProcesoFacturacionEmpresaQuery dbQuery = new ProcesoFacturacionEmpresaQuery();

            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            foreach (var row in grid.Rows)
            {
                if (row.GetCell("TP_PROCESO").Value == "P")
                    row.GetCell("QT_RESULTADO").Editable = true;
            }

            return grid;
        }
        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                if (grid.Rows.Any())
                {
                    RegistroModificacionProcesoFacturacionEmpresa registroModificacionPFE = new RegistroModificacionProcesoFacturacionEmpresa(uow, this._identity.UserId, this._identity.Application);

                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    uow.CreateTransactionNumber(this._identity.Application);
                    uow.BeginTransaction();

                    foreach (var row in grid.Rows)
                    {
                        if (row.IsNew)
                        {
                            var facturacionEmpresaProceso = this.CrearProcesoFacturacionEmpresa(uow, row, query);
                            registroModificacionPFE.RegistrarProcesoFacturacionEmpresa(facturacionEmpresaProceso);
                        }
                        else if (row.IsDeleted)
                        {
                            // rows delete
                            this.DeleteProcesoFacturacionEmpresa(uow, row, query);
                        }
                        else
                        {
                            // rows editadas
                            var facturacionEmpresaProceso = this.UpdateProcesoFacturacionEmpresa(uow, row, query);
                            registroModificacionPFE.ModificarProcesoFacturacionEmpresa(facturacionEmpresaProceso);
                        }
                    }

                    uow.SaveChanges();
                    uow.Commit();
                }

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "FAC005GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoProcesoFacturacionEmpresaGridValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ProcesoFacturacionEmpresaQuery dbQuery = new ProcesoFacturacionEmpresaQuery();

            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ProcesoFacturacionEmpresaQuery dbQuery = new ProcesoFacturacionEmpresaQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public virtual List<SelectOption> SelectEmpresa(IUnitOfWork uow)
        {
            return uow.EmpresaRepository.GetEmpresasParaUsuario(this._identity.UserId)
                .Select(w => new SelectOption(w.Id.ToString(), w.Id.ToString() + " - " + w.Nombre))
                .ToList();
        }

        public virtual List<SelectOption> SelectProceso(IUnitOfWork uow)
        {
            return uow.FacturacionRepository.GetFacturacionesProceso()
                .Select(w => new SelectOption(w.CodigoProceso, w.CodigoProceso + " - " + w.DescripcionProceso + " - " + w.TipoProceso))
                .ToList();
        }

        public virtual FacturacionEmpresaProceso CrearProcesoFacturacionEmpresa(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            var nuevaFacturacionEmpresaProceos = new FacturacionEmpresaProceso();
            var qtResultado = 0m;

            decimal.TryParse(row.GetCell("QT_RESULTADO").Value, NumberStyles.Any, this._identity.GetFormatProvider(), out qtResultado);

            nuevaFacturacionEmpresaProceos.CodigoEmpresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
            nuevaFacturacionEmpresaProceos.CodigoProceso = row.GetCell("CD_PROCESO").Value;
            nuevaFacturacionEmpresaProceos.Resultado = qtResultado;
            nuevaFacturacionEmpresaProceos.NumeroTransaccion = uow.GetTransactionNumber();

            return nuevaFacturacionEmpresaProceos;
        }

        public virtual FacturacionEmpresaProceso UpdateProcesoFacturacionEmpresa(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            var cdEmpresa = row.GetCell("CD_EMPRESA").Value;
            var cdProceso = row.GetCell("CD_PROCESO").Value;
            var qtResultado = 0m;
            var facturacionEmpresaProceso = uow.FacturacionRepository.GetFacturacionEmpresaProceso(int.Parse(cdEmpresa), cdProceso);

            decimal.TryParse(row.GetCell("QT_RESULTADO").Value, NumberStyles.Any, this._identity.GetFormatProvider(), out qtResultado);

            facturacionEmpresaProceso.Resultado = qtResultado;
            facturacionEmpresaProceso.NumeroTransaccion = uow.GetTransactionNumber();

            return facturacionEmpresaProceso;
        }

        public virtual void DeleteProcesoFacturacionEmpresa(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            var cdEmpresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
            var cdProceso = row.GetCell("CD_PROCESO").Value;

            if (uow.FacturacionRepository.AnyFacturacionEmpresaProceso(cdEmpresa, cdProceso))
            {
                var facturacionEmpresaProceso = uow.FacturacionRepository.GetFacturacionEmpresaProceso(cdEmpresa, cdProceso);
                var nuTransaccion = uow.GetTransactionNumber();

                facturacionEmpresaProceso.NumeroTransaccion = nuTransaccion;
                facturacionEmpresaProceso.NumeroTransaccionDelete = nuTransaccion;
                facturacionEmpresaProceso.FechaModificacion = DateTime.Now;

                uow.FacturacionRepository.UpdateFacturacionEmpresaProceso(facturacionEmpresaProceso);
                uow.SaveChanges();

                uow.FacturacionRepository.DeleteFacturacionEmpresaProceso(cdEmpresa, cdProceso);
            }
            else
            {
                throw new EntityNotFoundException("FAC005_Sec0_Error_Er001_FacProcesoEmpresaNoExisteEliminar");
            }
        }
    }
}
