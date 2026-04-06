using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Produccion;
using WIS.Application.Validation.Modules.GridModules.Produccion;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Domain.General;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Enums;
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
using WIS.GridComponent.Validation;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRD
{
    public class PRD100Formula : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected List<string> _gridKeys { get; }

        protected List<SortCommand> _defaultSort { get; }

        public PRD100Formula(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            IFormValidationService formValidationService,
            IGridValidationService gridValidationService)
        {
            _gridKeys = new List<string>
            {
                "CD_PRDC_DEFINICION", "CD_EMPRESA"
            };

            _defaultSort = new List<SortCommand>
            {
                new SortCommand("DT_ADDROW", SortDirection.Descending),
            };

            _identity = identity;
            _uowFactory = uowFactory;
            _gridService = gridService;
            _excelService = excelService;
            _filterInterpreter = filterInterpreter;
            _formValidationService = formValidationService;
            _gridValidationService = gridValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsAddEnabled = false;
            context.IsRemoveEnabled = true;
            context.IsCommitEnabled = true;

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("editar", "General_Sec0_btn_Editar", "fas fa-edit")
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new FormulaProduccionQuery();
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, _defaultSort, _gridKeys);

            grid.SetEditableCells(new List<string> { "ACTIVO" });
            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new FormulaProduccionQuery();
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, _defaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new FormulaProduccionQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            try
            {
                using var uow = this._uowFactory.GetUnitOfWork();

                foreach (var row in grid.Rows)
                {
                    var codigoFormula = row.GetCell("CD_PRDC_DEFINICION").Value;

                    var formula = uow.FormulaRepository.GetFormula(codigoFormula);
                    if (formula == null)
                        throw new ValidationFailedException("PRD100_grid1_error_FormulaNoExiste");

                    if (row.IsDeleted)
                    {
                        if (uow.ProduccionRepository.AnyProduccionParaFormula(codigoFormula))
                            throw new ValidationFailedException("PRD100_grid1_error_ExisteProduccion", [codigoFormula]);

                        uow.FormulaRepository.DeleteFormula(formula);
                    }
                    else
                    {
                        if (uow.ProduccionRepository.AnyProduccionActivaParaFormula(codigoFormula))
                            throw new ValidationFailedException("PRD100_grid1_error_ExisteProduccionActiva", [codigoFormula]);

                        var activo = row.GetCell("ACTIVO").Value == "S";

                        if (activo)
                            formula.Enable();
                        else
                            formula.Disable();

                        uow.FormulaRepository.UpdateFormula(formula);
                    }
                }

                uow.SaveChanges();
                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ValidationFailedException ex)
            {
                _logger.Error(ex, ex.Message);
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "General_Sec0_Error_ErrorOperacionGrilla");
                context.AddErrorNotification("General_Sec0_Error_ErrorOperacionGrilla");
            }

            return grid;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                var rowsEntrada = JsonConvert.DeserializeObject<List<GridRow>>(context.GetParameter("rowsEntrada"));
                var rowsSalida = JsonConvert.DeserializeObject<List<GridRow>>(context.GetParameter("rowsSalida"));

                ValidateRows(uow, form, rowsEntrada, rowsSalida);

                var formula = new Formula
                {
                    Id = form.GetField("codigo").Value,
                    Empresa = int.Parse(form.GetField("empresa").Value),
                    Nombre = form.GetField("nombre").Value,
                    Descripcion = form.GetField("descripcion").Value,
                    CantidadPasadasPorFormula = 1,
                    FechaAlta = DateTime.Now,
                    Estado = SituacionDb.Activo
                };

                var insumos = MapFormulaEntrada(rowsEntrada, formula.Empresa);
                var salidas = MapFormulaSalida(rowsSalida, formula.Empresa);

                //Chequear lineas repetidas y que niguna tenga cantidad 0
                CheckForDuplicatesAndCount(insumos, salidas);

                formula.Entrada.AddRange(insumos);
                formula.Salida.AddRange(salidas);

                formula.UpdateCantidadCompleta();

                uow.FormulaRepository.AddFormula(formula);

                uow.SaveChanges();

                context.AddSuccessNotification("PRD100_form1_msg_FormulaCreada");

                context.Parameters?.Clear();
            }
            catch (ValidationFailedException ex)
            {
                _logger.Error(ex, ex.Message);
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "General_Sec0_Error_Operacion");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new PRD100IngresoFormulaValidationModule(uow, this._identity), form, context);
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "empresa":
                    return this.SearchEmpresa(form, context);
            }

            return new List<SelectOption>();
        }

        #region Metodos Auxiliares

        public virtual void CheckForDuplicatesAndCount(List<FormulaEntrada> lineasEntrada, List<FormulaSalida> lineasSalida)
        {
            if (lineasEntrada.GroupBy(d => d.Producto).Any(d => d.Count() > 1))
                throw new ValidationFailedException("PRD100_Sec0_error_LineaEntradaDuplicada");

            if (lineasSalida.GroupBy(d => d.Producto).Any(d => d.Count() > 1))
                throw new ValidationFailedException("PRD100_Sec0_error_LineaSalidaDuplicada");

            if (lineasEntrada.Any(l => l.CantidadConsumir <= 0) || lineasSalida.Any(l => l.CantidadProducir <= 0))
                throw new ValidationFailedException("PRD100_Sec0_error_LineaConCantidad0");
        }

        public virtual List<SelectOption> SearchEmpresa(Form form, FormSelectSearchContext context)
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            List<Empresa> empresas = uow.EmpresaRepository.GetByNombreOrCodePartial(context.SearchValue);

            foreach (Empresa empresa in empresas)
            {
                opciones.Add(new SelectOption(empresa.Id.ToString(), $"{empresa.Id} - {empresa.Nombre}"));
            }


            return opciones;
        }

        public virtual List<FormulaEntrada> MapFormulaEntrada(List<GridRow> rowsEntrada, int empresa)
        {
            var lineasEntrada = new List<FormulaEntrada>();

            foreach (var row in rowsEntrada)
            {
                lineasEntrada.Add(new FormulaEntrada
                {
                    Producto = row.GetCell("CD_PRODUTO").Value,
                    Empresa = empresa,
                    Faixa = 1,
                    NumeroPasada = 1,
                    CantidadPasadas = 1,
                    FechaAlta = DateTime.Now,
                    CantidadConsumir = decimal.Parse(row.GetCell("QT_CONSUMIDA_LINEA").Value, this._identity.GetFormatProvider()),
                    Prioridad = 1
                });
            }

            return lineasEntrada;
        }

        public virtual List<FormulaSalida> MapFormulaSalida(List<GridRow> rowsSalida, int empresa)
        {
            var lineasSalida = new List<FormulaSalida>();

            foreach (var row in rowsSalida)
            {
                lineasSalida.Add(new FormulaSalida
                {
                    Producto = row.GetCell("CD_PRODUTO").Value,
                    Empresa = empresa,
                    Faixa = 1,
                    NumeroPasada = 1,
                    FechaAlta = DateTime.Now,
                    CantidadProducir = decimal.Parse(row.GetCell("QT_CONSUMIDA_LINEA").Value, this._identity.GetFormatProvider()),
                    TipoResultado = FormulaResultadoTipo.ProductoFinal //Lockeado en producto final hasta decidir que hacer
                });
            }

            return lineasSalida;
        }

        public virtual void ValidateRows(IUnitOfWork uow, Form form, List<GridRow> rowsEntrada, List<GridRow> rowsSalida)
        {
            var parametros = new List<ComponentParameter>
            {
                new ComponentParameter("empresa", form.GetField("empresa").Value),
            };

            ValidateRowsEntrada(uow, rowsEntrada, parametros);
            ValidateRowsSalida(uow, rowsSalida, parametros);

            if (rowsEntrada.Any(d => !d.IsValid()) || rowsSalida.Any(d => !d.IsValid()))
                throw new ValidationFailedException("PRD100_grid1_error_FormulaDetalleInvalido");

            var productosRepetidos = rowsEntrada.SelectMany(c => c.Cells.Where(c => c.Column.Id == "CD_PRODUTO").Select(c => c.Value))
                .Intersect(rowsSalida.SelectMany(c => c.Cells.Where(c => c.Column.Id == "CD_PRODUTO").Select(c => c.Value)))
                .Any();

            var validarProductoRepetidos = (uow.ParametroRepository.GetParameter(ParamManager.PRD100_VALIDAR_PROD_DISTINTOS) ?? "N") == "S";

            if (productosRepetidos && validarProductoRepetidos)
                throw new ValidationFailedException("PRD100_grid1_error_FormulaProductosEInsumosRepetidos");
        }

        public virtual void ValidateRowsEntrada(IUnitOfWork uow, List<GridRow> rows, List<ComponentParameter> parametros)
        {
            var entradaValidationModule = new FormulaEntradaGridValidationModule(uow, this._identity.GetFormatProvider());
            entradaValidationModule.Validator = new GridValidator(parametros);
            entradaValidationModule.Validate(rows, parametros);
        }

        public virtual void ValidateRowsSalida(IUnitOfWork uow, List<GridRow> rows, List<ComponentParameter> parametros)
        {
            var salidaValidationModule = new FormulaSalidaGridValidationModule(uow, _identity.GetFormatProvider());
            salidaValidationModule.Validator = new GridValidator(parametros);
            salidaValidationModule.Validate(rows, parametros);
        }

        #endregion

    }
}
