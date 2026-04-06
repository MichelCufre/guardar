using NLog;
using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Application.Validation.Modules.GridModules.Produccion;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Produccion;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Domain.Produccion;
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
using WIS.Sorting;

namespace WIS.Application.Controllers.PRD
{
    public class PRD120 : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected List<string> GridKeys { get; set; }

        public PRD120(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFormValidationService formValidationService,
            IGridValidationService gridValidationService,
            IFilterInterpreter filterInterpreter)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._formValidationService = formValidationService;
            this._gridValidationService = gridValidationService;
            this._filterInterpreter = filterInterpreter;

            this.GridKeys = new List<string>
            {
                "CD_ACCION_INSTANCIA"
            };
        }
                
        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = false;
            context.IsAddEnabled = false;

            grid.SetInsertableColumns(new List<string>
            {
                "DS_ACCION_INSTANCIA",
                "CD_ACCION",
                "VL_PARAMETRO1",
                "VL_PARAMETRO2",
                "VL_PARAMETRO3"
            });

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                this.GridKeys = new List<string>() { "CD_ACCION_INSTANCIA" };

                var dbQuery = new InstanciasAccionQuery();

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("CD_ACCION_INSTANCIA", SortDirection.Descending);

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

                foreach (var row in grid.Rows)
                {
                    row.SetEditableCells(new List<string> { "DS_ACCION_INSTANCIA", "CD_ACCION", "VL_PARAMETRO1", "VL_PARAMETRO2", "VL_PARAMETRO3" });
                }
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var dbQuery = new InstanciasAccionQuery();

                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("CD_ACCION_INSTANCIA", SortDirection.Descending);

                context.FileName = this._identity.Application +"-"+ DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                FormulaAccionMapper mapper = new FormulaAccionMapper();

                foreach (var row in grid.Rows)
                {
                    int cdInstancia = int.Parse(row.GetCell("CD_ACCION_INSTANCIA").Value);

                    FormulaAccion instancia = uow.FormulaAccionRepository.GetFormulaAccion(cdInstancia);
                    instancia.Parametros = new List<string>();

                    instancia.Tipo = mapper.MapAccionTipo(row.GetCell("CD_ACCION").Value);
                    instancia.Descripcion = row.GetCell("DS_ACCION_INSTANCIA").Value;
                    instancia.FechaModificacion = DateTime.Now;
                    instancia.Parametros.Add(row.GetCell("VL_PARAMETRO1").Value);
                    instancia.Parametros.Add(row.GetCell("VL_PARAMETRO2").Value);
                    instancia.Parametros.Add(row.GetCell("VL_PARAMETRO3").Value);

                    uow.FormulaAccionRepository.UpdateFormulaAccion(instancia);
                }

                uow.SaveChanges();

                context.AddSuccessNotification("PRD120_Sec0_Success_InstanciasModificadas");
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new PRD120GridValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }
        
        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            switch (context.ColumnId)
            {
                case "CD_ACCION":
                    return this.SearchAccion(grid, row, context);
            }

            return new List<SelectOption>();
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            this.GridKeys = new List<string>() { "CD_ACCION_INSTANCIA" };

            var dbQuery = new InstanciasAccionQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }


        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new PRD120FormValidationModule(uow, this._identity.UserId, this._identity.Predio), form, context);
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                try
                {
                    FormulaAccionMapper mapper = new FormulaAccionMapper();

                    var cdAccion = form.GetField("cdAccion").Value;
                    var dsInstanca = form.GetField("dsInstancia").Value;
                    var parametro1 = form.GetField("vlParametro1").Value;
                    var parametro2 = form.GetField("vlParametro2").Value;
                    var parametro3 = form.GetField("vlParametro3").Value;

                    FormulaAccion instancia = new FormulaAccion()
                    {
                        Descripcion = dsInstanca,
                        FechaAlta = DateTime.Now,
                        Parametros = new List<string>() { parametro1, parametro2, parametro3 },
                        Tipo = mapper.MapAccionTipo(cdAccion),
                        Id = uow.FormulaAccionRepository.GetNumeroInstancia()
                    };

                    uow.FormulaAccionRepository.AddFormulaAccion(instancia);

                    uow.SaveChanges();

                    context.AddSuccessNotification("PRD120_Sec0_Succes_SeCreoLaInstanciaDeAccion", new List<string>() { instancia.Id.ToString() });
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, ex.Message);
                    throw ex;
                }
            }

            return form;
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "cdAccion":
                    return this.SearchAccion(form, context);
                default:
                    return new List<SelectOption>();
            }
        }

        #region Metodos Auxiliares
        
        public virtual List<SelectOption> SearchAccion(Grid grid, GridRow row, GridSelectSearchContext context)
        {
            List<SelectOption> options = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                List<Accion> acciones = uow.AccionRepository.GetAccionesPorCodigoDescripcion(context.SearchValue);

                if (acciones != null)
                {
                    foreach (var accion in acciones)
                    {
                        options.Add(new SelectOption(accion.Id, accion.Descripcion));
                    }
                }

            }

            return options;
        }

        public virtual List<SelectOption> SearchAccion(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> options = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                List<Accion> acciones = uow.AccionRepository.GetAccionesPorCodigoDescripcion(context.SearchValue);

                if (acciones != null)
                {
                    foreach (var accion in acciones)
                    {
                        options.Add(new SelectOption(accion.Id, accion.Descripcion));
                    }
                }
            }

            return options;
        }

        #endregion
    }
}
