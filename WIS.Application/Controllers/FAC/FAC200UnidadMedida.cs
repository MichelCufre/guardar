using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
    public class FAC200UnidadMedida : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<FAC006ResultadosEjecucion> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public FAC200UnidadMedida(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<FAC006ResultadosEjecucion> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "CD_UNIDADE_MEDIDA",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_UNIDADE_MEDIDA", SortDirection.Ascending),
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
            query.IsRemoveEnabled = false;

            grid.SetInsertableColumns(new List<string> {
                "CD_UNIDADE_MEDIDA",
                "NU_COMPONENTE",
                "ID_USO"
            });

            using var uow = this._uowFactory.GetUnitOfWork();
            grid.AddOrUpdateColumn(new GridColumnSelect("CD_UNIDADE_MEDIDA", this.SelectUnidadMedida(uow)));

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            UnidadMedidaQuery dbQuery = new UnidadMedidaQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> {
                "NU_COMPONENTE",
                "ID_USO"
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
                    RegistroModificacionFacturacionUnidadMedida registroModificacionFUM = new RegistroModificacionFacturacionUnidadMedida(uow, this._identity.UserId, this._identity.Application);

                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    uow.CreateTransactionNumber(this._identity.Application);

                    foreach (var row in grid.Rows)
                    {
                        if (row.IsNew)
                        {
                            FacturacionUnidadMedida facturacionUnidadMedida = this.CrearFacturacionUnidadMedida(uow, row, query);
                            registroModificacionFUM.RegistrarFacturacionUnidadMedida(facturacionUnidadMedida);
                        }
                        else
                        {
                            // rows editadas
                            FacturacionUnidadMedida facturacionUnidadMedida = this.UpdateFacturacionUnidadMedida(uow, row, query);
                            registroModificacionFUM.ModificarFacturacionUnidadMedida(facturacionUnidadMedida);
                        }
                    }
                }

                uow.SaveChanges();

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "FAC200GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoFacturacionUnidadMedidaGridValidationModule(uow), grid, row, context);
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            UnidadMedidaQuery dbQuery = new UnidadMedidaQuery();
            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            UnidadMedidaQuery dbQuery = new UnidadMedidaQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public virtual List<SelectOption> SelectUnidadMedida(IUnitOfWork uow)
        {
            return uow.UnidadMedidaRepository.GetUnidadesMedida()
                .Select(w => new SelectOption(w.Id, w.Id + " - " + w.Descripcion))
                .ToList();
        }

        public virtual FacturacionUnidadMedida CrearFacturacionUnidadMedida(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            var nuevaFacturacionUnidadMedida = new FacturacionUnidadMedida();

            nuevaFacturacionUnidadMedida.UnidadMedida = row.GetCell("CD_UNIDADE_MEDIDA").Value;
            nuevaFacturacionUnidadMedida.NumeroComponente = row.GetCell("NU_COMPONENTE").Value;
            nuevaFacturacionUnidadMedida.Uso = row.GetCell("ID_USO").Value ?? "N";
            nuevaFacturacionUnidadMedida.NumeroTransaccion = uow.GetTransactionNumber();

            return nuevaFacturacionUnidadMedida;
        }

        public virtual FacturacionUnidadMedida UpdateFacturacionUnidadMedida(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            var unidadMedida = row.GetCell("CD_UNIDADE_MEDIDA").Value;
            var facturacionUnidadMedida = new FacturacionUnidadMedida();
            facturacionUnidadMedida = uow.FacturacionRepository.GetFacturacionUnidadMedida(unidadMedida);

            facturacionUnidadMedida.NumeroComponente = row.GetCell("NU_COMPONENTE").Value;
            facturacionUnidadMedida.Uso = row.GetCell("ID_USO").Value;
            facturacionUnidadMedida.NumeroTransaccion = uow.GetTransactionNumber();

            return facturacionUnidadMedida;
        }
    }
}
