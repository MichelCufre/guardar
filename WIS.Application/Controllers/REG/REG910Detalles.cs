using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Eventos;
using WIS.Application.Validation.Modules.GridModules.Registro;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.General;
using WIS.Domain.Registro;
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

namespace WIS.Application.Controllers.REG
{
    public class REG910Detalles : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<REG910Detalles> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REG910Detalles(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            ISecurityService security,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            ILogger<REG910Detalles> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "NU_DOMINIO",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_DOMINIO", SortDirection.Descending),
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
            string dominioInterno = "";

            if (!string.IsNullOrEmpty(query.GetParameter("dominioInterno")))
            {
                dominioInterno = query.GetParameter("dominioInterno");
                
            }
            if(dominioInterno == "N")
            {
                query.IsAddEnabled = true;
                query.IsEditingEnabled = true;
                query.IsRemoveEnabled = true;
                query.IsCommitEnabled = true;
            }
            else
            {
                query.IsAddEnabled = false;
                query.IsEditingEnabled = false;
                query.IsRemoveEnabled = false;
                query.IsCommitEnabled = false;
            }
            

            grid.SetInsertableColumns(new List<string> {
                "NU_DOMINIO",
                "DS_DOMINIO_VALOR",
                "CD_DOMINIO_VALOR"
            });

            var defaultColumns = new Dictionary<string, string>();
            string codigoDominio = "";

            if (!string.IsNullOrEmpty(query.GetParameter("codigoDominio")))
            {
                codigoDominio = query.GetParameter("codigoDominio");
                defaultColumns.Add("CD_DOMINIO", codigoDominio);
                grid.SetColumnDefaultValues(defaultColumns);
            }     

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new REG910MantenimientoDetallesDominioGridValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");

            return form;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            string codigoDominio = "";

            if (!string.IsNullOrEmpty(query.GetParameter("codigoDominio")))
            {
                codigoDominio = query.GetParameter("codigoDominio");
            }

            DetalleDominioQuery dbQuery = new DetalleDominioQuery();

            if (!string.IsNullOrEmpty(codigoDominio))
            {
                dbQuery = new DetalleDominioQuery(codigoDominio);
            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string codigoDominio = "";
            string wisInterno = "";

            if (!string.IsNullOrEmpty(query.GetParameter("codigoDominio")))
            {
                codigoDominio = query.GetParameter("codigoDominio");
                wisInterno = query.GetParameter("dominioInterno");
            }

            DetalleDominioQuery dbQuery = new DetalleDominioQuery();

            if (!string.IsNullOrEmpty(codigoDominio))
            {
                dbQuery = new DetalleDominioQuery(codigoDominio);
            }

            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            if (wisInterno == "N")
            {
                grid.Rows.ForEach(row =>
                {
                    var celda = row.GetCell("DS_DOMINIO_VALOR");
                    celda.Editable = true;
                });
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
                    RegistroDominio registroDominio = new RegistroDominio(uow, this._identity.UserId, this._identity.Application);

                    if (grid.HasNewDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    if (grid.HasDuplicates(this.GridKeys))
                        throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {

                        if (row.IsNew)
                        {
                            DominioDetalle detalleDominio = this.CrearDetalleDominio(uow, row, query);
                            registroDominio.RegistrarDetalleDominio(detalleDominio);
                        }
                        else if(row.IsModified)
                        {
                            DominioDetalle detalleDominio = this.UpdateDetalleDominio(uow, row, query);
                            registroDominio.UpdateDetalleDominio(detalleDominio);
                        }
                        else
                        {
                            string numeroDominio = row.GetCell("NU_DOMINIO").Value;
                            uow.DominioRepository.DeleteDetalleDominio(numeroDominio);
                        }
                    }
                }

                uow.SaveChanges();

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "REG910GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        public virtual DominioDetalle CrearDetalleDominio(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            DominioDetalle nuevoDetalle = new DominioDetalle();

            nuevoDetalle.Id = row.GetCell("NU_DOMINIO").Value;
            nuevoDetalle.Descripcion = row.GetCell("DS_DOMINIO_VALOR").Value;
            nuevoDetalle.Codigo = row.GetCell("CD_DOMINIO").Value;
            nuevoDetalle.Valor = row.GetCell("CD_DOMINIO_VALOR").Value;

            return nuevoDetalle;
        }
        public virtual DominioDetalle UpdateDetalleDominio(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            string numeroDominio = row.GetCell("NU_DOMINIO").Value;
            DominioDetalle detalleDominio = uow.DominioRepository.GetDominio(numeroDominio);

            detalleDominio.Descripcion = row.GetCell("DS_DOMINIO_VALOR").Value;

            return detalleDominio;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string codigoDominio = "";

            if (!string.IsNullOrEmpty(query.GetParameter("codigoDominio")))
            {
                codigoDominio = query.GetParameter("codigoDominio");
            }

            DetalleDominioQuery dbQuery = new DetalleDominioQuery();

            if (!string.IsNullOrEmpty(codigoDominio))
            {
                dbQuery = new DetalleDominioQuery(codigoDominio);
            }

            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }

    }
}
