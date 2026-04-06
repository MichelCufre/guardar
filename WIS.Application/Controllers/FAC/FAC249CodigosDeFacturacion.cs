using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Eventos;
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
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.FAC
{
    public class FAC249CodigosDeFacturacion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<FAC249CodigosDeFacturacion> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public FAC249CodigosDeFacturacion(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            ISecurityService security,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            ILogger<FAC249CodigosDeFacturacion> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "CD_FACTURACION",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_FACTURACION", SortDirection.Descending),
            };

            this._uowFactory = uowFactory;
            this._session = session;
            this._identity = identity;
            this._security = security;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._formValidationService = formValidationService;
            _filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
        }


        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            CodigoDeFacturacionQuery dbQuery = new CodigoDeFacturacionQuery();

            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }


        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            query.IsAddEnabled = true;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = false;

            grid.SetInsertableColumns(new List<string> {
                "CD_FACTURACION",
                "DS_FACTURACION",
                "TP_CALCULO"
            });

            GridColumnSelect selectTipos = new GridColumnSelect("TP_CALCULO", this.SelectTipos(uow));
            selectTipos.Translate = true;
            grid.AddOrUpdateColumn(selectTipos);

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            CodigoDeFacturacionQuery dbQuery = new CodigoDeFacturacionQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> {
                "DS_FACTURACION",
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
                    RegistroCodigoDeFacturacion registroCodioFacturacion = new RegistroCodigoDeFacturacion(uow, this._identity.UserId, this._identity.Application);

                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {

                        if (row.IsNew)
                        {
                            FacturacionCodigo codigoFacturacion = this.CrearCodigoFacturacion(uow, row, query);
                            registroCodioFacturacion.RegistrarCodigoFacturacion(codigoFacturacion);
                        }
                        else
                        {
                            FacturacionCodigo codigoFacturacion = this.UpdateCodigoFacturacion(uow, row, query);
                            registroCodioFacturacion.UpdateCodigoFacturacion(codigoFacturacion);
                        }
                    }
                }

                uow.SaveChanges();

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "FAC249GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            CodigoDeFacturacionQuery dbQuery = new CodigoDeFacturacionQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public virtual FacturacionCodigo CrearCodigoFacturacion(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            FacturacionCodigo nuevoCodigoFacturacion = new FacturacionCodigo();

            nuevoCodigoFacturacion.CodigoFacturacion = row.GetCell("CD_FACTURACION").Value;
            nuevoCodigoFacturacion.DescripcionFacturacion = row.GetCell("DS_FACTURACION").Value;
            nuevoCodigoFacturacion.TipoCalculo = row.GetCell("TP_CALCULO").Value;
            nuevoCodigoFacturacion.FechaIngresado = DateTime.Now;

            return nuevoCodigoFacturacion;
        }
        public virtual FacturacionCodigo UpdateCodigoFacturacion(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            string codigoFacturacion = row.GetCell("CD_FACTURACION").Value;
            FacturacionCodigo entidadFacturacionCodigo = uow.FacturacionRepository.GetFacturacionCodigo(codigoFacturacion);

            entidadFacturacionCodigo.DescripcionFacturacion = row.GetCell("DS_FACTURACION").Value;
            entidadFacturacionCodigo.FechaModificacion = DateTime.Now;

            return entidadFacturacionCodigo;
        }
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new RegistroCodigoFacturacionValidationModule(uow), grid, row, context);
        }
        public virtual List<SelectOption> SelectTipos()
        {
            List<SelectOption> colOpciones = new List<SelectOption>
            {
                new SelectOption(FacturacionDb.TIPO_DE_CALCULO_MANUAL , "FAC249_frm1_select_TIPO_FACTURACION_MANUAL" ),
                new SelectOption(FacturacionDb.TIPO_DE_CALCULO_CALCULADO , "FAC249_frm1_select_TIPO_FACTURACION_CALCULADO" ),
                new SelectOption(FacturacionDb.TIPO_DE_CALCULO_TAREA , "FAC249_frm1_select_TIPO_FACTURACION_TAREA" ),
                new SelectOption(FacturacionDb.TIPO_DE_CALCULO_PROGRAMABLE , "FAC249_frm1_select_TIPO_FACTURACION_PROGRAMABLE" ),
                new SelectOption(FacturacionDb.TIPO_DE_CALCULO_AMIGABLE , "FAC249_frm1_select_TIPO_FACTURACION_AMIGABLE" ),
            };
            return colOpciones;
        }

        public virtual List<SelectOption> SelectTipos(IUnitOfWork uow)
        {
            return uow.FacturacionRepository.GetTpCalculo()
                .Where(t => t.Valor != "C")
                .Select(t => new SelectOption(t.Valor, t.Descripcion))
                .ToList();
        }
    }
}
