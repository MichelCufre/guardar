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
using WIS.Domain.General;
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
    public class FAC230UnidadMedidaEmpresa : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<FAC230UnidadMedidaEmpresa> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public FAC230UnidadMedidaEmpresa(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<FAC230UnidadMedidaEmpresa> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "CD_UNIDADE_MEDIDA",
                "CD_EMPRESA",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_UNIDADE_MEDIDA", SortDirection.Ascending),
                new SortCommand("CD_EMPRESA", SortDirection.Ascending),
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
                "CD_UNIDADE_MEDIDA",
                "CD_EMPRESA"
            });

            using var uow = this._uowFactory.GetUnitOfWork();

            //Cargo select
            grid.AddOrUpdateColumn(new GridColumnSelect("CD_UNIDADE_MEDIDA", this.SelectUnidadMedida(uow)));
            grid.AddOrUpdateColumn(new GridColumnSelect("CD_EMPRESA", this.SelectEmpresa(uow)));

            //Cargo default values
            var defaultColumns = new Dictionary<string, string>();
            defaultColumns.Add("CD_FUNCIONARIO", this._identity.UserId.ToString());
            defaultColumns.Add("NM_FUNCIONARIO", uow.SecurityRepository.GetUserFullname(this._identity.UserId));

            grid.SetColumnDefaultValues(defaultColumns);

            return this.GridFetchRows(grid, query.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            UnidadMedidaEmpresaQuery dbQuery = new UnidadMedidaEmpresaQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            return grid;
        }
        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                if (grid.Rows.Any())
                {
                    RegistroModificacionFacturacionUnidadMedidaEmpresa registroModificacionFUME = new RegistroModificacionFacturacionUnidadMedidaEmpresa(uow, this._identity.UserId, this._identity.Application);

                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {

                        if (row.IsNew)
                        {
                            FacturacionUnidadMedidaEmpresa facturacionUnidadMedidaEmpresa = this.CrearFacturacionUnidadMedidaEmpresa(uow, row, query);
                            registroModificacionFUME.RegistrarFacturacionUnidadMedidaEmpresa(facturacionUnidadMedidaEmpresa);
                        }
                        else if (row.IsDeleted)
                        {
                            // rows delete
                            this.DeleteFacturacionUnidadMedidaEmpresa(uow, row, query);
                        }
                    }
                }

                uow.SaveChanges();

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "FAC230GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoFacturacionUnidadMedidaEmpresaGridValidationModule(uow), grid, row, context);
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            UnidadMedidaEmpresaQuery dbQuery = new UnidadMedidaEmpresaQuery();
            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            UnidadMedidaEmpresaQuery dbQuery = new UnidadMedidaEmpresaQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public virtual List<SelectOption> SelectUnidadMedida(IUnitOfWork uow)
        {
            List<FacturacionUnidadMedida> facturacionUnidadMedida = uow.FacturacionRepository.GetFacturacionesUnidadMedida();

            List<UnidadMedida> unidadMedida = uow.UnidadMedidaRepository.GetUnidadesMedida();

            return unidadMedida.Where(w => facturacionUnidadMedida.Select(s => s.UnidadMedida).Contains(w.Id)).ToList()
                .Select(w => new SelectOption(w.Id, w.Id + " - " + w.Descripcion))
                .ToList();
        }
        public virtual List<SelectOption> SelectEmpresa(IUnitOfWork uow)
        {
            return uow.EmpresaRepository.GetEmpresas()
                .Select(w => new SelectOption(w.Id.ToString(), w.Id.ToString() + " - " + w.Nombre))
                .ToList();
        }
        public virtual FacturacionUnidadMedidaEmpresa CrearFacturacionUnidadMedidaEmpresa(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            FacturacionUnidadMedidaEmpresa nuevaFacturacionUnidadMedidaEmpresa = new FacturacionUnidadMedidaEmpresa();

            nuevaFacturacionUnidadMedidaEmpresa.UnidadMedida = row.GetCell("CD_UNIDADE_MEDIDA").Value;
            nuevaFacturacionUnidadMedidaEmpresa.Empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
            nuevaFacturacionUnidadMedidaEmpresa.Funcionario = this._identity.UserId;
            nuevaFacturacionUnidadMedidaEmpresa.Fecha = DateTime.Now;

            return nuevaFacturacionUnidadMedidaEmpresa;
        }
        public virtual void DeleteFacturacionUnidadMedidaEmpresa(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            string unidadMedida = row.GetCell("CD_UNIDADE_MEDIDA").Value;
            string cdEmpresa = row.GetCell("CD_EMPRESA").Value;

            if (uow.FacturacionRepository.AnyFacturacionUnidadMedidaEmpresa(unidadMedida, int.Parse(cdEmpresa)))
            {
                uow.FacturacionRepository.DeleteFacturacionUnidadMedidaEmpresa(unidadMedida, int.Parse(cdEmpresa));
            }
            else
            {
                throw new EntityNotFoundException("FAC230_Sec0_Error_Er001_FacUndMdidEmpNoExisteEliminar");
            }
        }
    }
}
