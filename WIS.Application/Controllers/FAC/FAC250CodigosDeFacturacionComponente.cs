using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Eventos;
using WIS.Components.Common;
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
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.FAC
{
    public class FAC250CodigosDeFacturacionComponente : AppController
    {

        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<FAC250CodigosDeFacturacionComponente> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public FAC250CodigosDeFacturacionComponente(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            ISecurityService security,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            ILogger<FAC250CodigosDeFacturacionComponente> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "CD_FACTURACION",
                "NU_COMPONENTE",
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

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsAddEnabled = true;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = false;

            grid.SetInsertableColumns(new List<string> {
                "CD_FACTURACION",
                "NU_COMPONENTE",
                "DS_SIGNIFICADO",
                "NU_CUENTA_CONTABLE",

            });

            //Botones a fac256 Cotizaciones de listas 
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("btnCotizaciones", "FAC250_grid1_btn_Cotizaciones", "fas fa-bars"),

            }));


            //Selects para CD_FACTURACION Codigo de facturacion y NU_CUENTA_CONTABLE  para tipo cuenta contable 
            using var uow = this._uowFactory.GetUnitOfWork();
            grid.AddOrUpdateColumn(new GridColumnSelect("CD_FACTURACION", this.SelectCodigosFacturacion(uow)));


            grid.AddOrUpdateColumn(new GridColumnSelect("NU_CUENTA_CONTABLE", this.SelectFacturacionCuentaContable(uow)));

            return this.GridFetchRows(grid, query.FetchContext);
        }


        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            switch (context.ButtonId)
            {
                case "btnCotizaciones":
                    context.Redirect("/facturacion/FAC256", true, new List<ComponentParameter>() {
                    new ComponentParameter(){ Id = "cdFacturacion", Value = context.Row.GetCell("CD_FACTURACION").Value},
                    new ComponentParameter(){ Id = "nuComponente", Value = context.Row.GetCell("NU_COMPONENTE").Value},
                    new ComponentParameter(){ Id = "descripcionListaPrecio", Value = context.Row.GetCell("DS_SIGNIFICADO").Value},
                    });
                    break;
            }

            return context;
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            CodigosDeFacturacionComponenteQuery dbQuery = new CodigosDeFacturacionComponenteQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> {

                "DS_SIGNIFICADO",
                "NU_CUENTA_CONTABLE",

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
                    RegistroCodigoDeFacturacionComponente registroCodigoFacturacionComponente = new RegistroCodigoDeFacturacionComponente(uow, this._identity.UserId, this._identity.Application);

                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    uow.CreateTransactionNumber(this._identity.Application);

                    foreach (var row in grid.Rows)
                    {
                        if (row.IsNew)
                        {
                            FacturacionCodigoComponente codigoFacturacionComponente = CrearCodigoFacturacionComponente(uow, row, query);
                            registroCodigoFacturacionComponente.RegistrarCodigoFacturacionComponente(codigoFacturacionComponente);
                        }
                        else
                        {
                            // rows editadas
                            FacturacionCodigoComponente codigoFacturacionComponente = ModificarCodigoFacturacionComponente(uow, row, query);
                            registroCodigoFacturacionComponente.UpdateCodigoFacturacionComponente(codigoFacturacionComponente);
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
        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new RegistroCodigoFacturacionComponenteValidationModule(uow), grid, row, context);
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            CodigosDeFacturacionComponenteQuery dbQuery = new CodigosDeFacturacionComponenteQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public virtual FacturacionCodigoComponente CrearCodigoFacturacionComponente(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            FacturacionCodigoComponente nuevoFacturacionCodigoComponente = new FacturacionCodigoComponente();

            nuevoFacturacionCodigoComponente.Id = row.GetCell("CD_FACTURACION").Value;
            nuevoFacturacionCodigoComponente.NumeroComponente = row.GetCell("NU_COMPONENTE").Value;
            nuevoFacturacionCodigoComponente.Descripcion = row.GetCell("DS_SIGNIFICADO").Value.TrimStart();
            nuevoFacturacionCodigoComponente.FechaIngresado = DateTime.Now;
            nuevoFacturacionCodigoComponente.NumeroCuentaContable = row.GetCell("NU_CUENTA_CONTABLE").Value;
            nuevoFacturacionCodigoComponente.NumeroTransaccion = uow.GetTransactionNumber();

            return nuevoFacturacionCodigoComponente;
        }

        public virtual FacturacionCodigoComponente ModificarCodigoFacturacionComponente(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            string codigoFacturacion = row.GetCell("CD_FACTURACION").Value;
            string Componente = row.GetCell("NU_COMPONENTE").Value;

            FacturacionCodigoComponente nuevoFacturacionCodigoComponente = uow.FacturacionRepository.GetCodigoFacturacionComponente(codigoFacturacion, Componente);

            nuevoFacturacionCodigoComponente.Descripcion = row.GetCell("DS_SIGNIFICADO").Value.TrimStart();
            nuevoFacturacionCodigoComponente.FechaActualizado = DateTime.Now;
            nuevoFacturacionCodigoComponente.NumeroCuentaContable = row.GetCell("NU_CUENTA_CONTABLE").Value;
            nuevoFacturacionCodigoComponente.NumeroTransaccion = uow.GetTransactionNumber();

            return nuevoFacturacionCodigoComponente;

        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            CodigosDeFacturacionComponenteQuery dbQuery = new CodigosDeFacturacionComponenteQuery();

            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }
        public virtual List<SelectOption> SelectCodigosFacturacion(IUnitOfWork uow)
        {
            return uow.FacturacionRepository.GetAllFacturacionCodigos()
                .Select(w => new SelectOption(w.CodigoFacturacion, w.CodigoFacturacion))
                .ToList();
        }
        public virtual List<SelectOption> SelectFacturacionCuentaContable(IUnitOfWork uow)
        {
            return uow.FacturacionRepository.GetAllCuentaContables()
                .Select(w => new SelectOption(w.Id, w.Descripcion))
                .ToList();
        }
    }
}
