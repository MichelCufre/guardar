using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Produccion;
using WIS.Application.Validation.Modules.GridModules.Produccion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Enums;
using WIS.Exceptions;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Security;

namespace WIS.Application.Controllers.PRD
{
    public class PRD100ModificarFormula : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public PRD100ModificarFormula(IIdentityService identity, IUnitOfWorkFactory uowFactory, IFormValidationService formValidationService)
        {
            _formValidationService = formValidationService;
            _identity = identity;
            _uowFactory = uowFactory;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var formula = uow.FormulaRepository.GetFormula(context.GetParameter("formula"));
            if (formula == null)
                throw new ValidationFailedException("PRD100_grid1_error_FormulaNoExiste");

            form.GetField("codigo").Value = formula.Id;
            form.GetField("empresa").Value = formula.Empresa.ToString();
            form.GetField("nombre").Value = formula.Nombre;
            form.GetField("descripcion").Value = formula.Descripcion;

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var rowsEntrada = JsonConvert.DeserializeObject<List<GridRow>>(context.GetParameter("rowsEntrada"));
            var rowsSalida = JsonConvert.DeserializeObject<List<GridRow>>(context.GetParameter("rowsSalida"));

            try
            {
                var formula = uow.FormulaRepository.GetFormula(context.GetParameter("formula"));
                if (formula == null)
                    throw new ValidationFailedException("PRD100_grid1_error_FormulaNoExiste");

                if (uow.ProduccionRepository.AnyProduccionActivaParaFormula(formula.Id))
                    throw new ValidationFailedException("PRD100_grid1_error_ExisteProduccionActiva", new string[] { formula.Id });

                ValidateRows(uow, form, rowsEntrada, rowsSalida);

                formula.Nombre = form.GetField("nombre").Value;
                formula.Descripcion = form.GetField("descripcion").Value;
                formula.FechaModificacion = DateTime.Now;

                var cambiosEntrada = MapFormulaEntrada(formula, rowsEntrada);
                var cambiosSalida = MapFormulaSalida(formula, rowsSalida);

                CheckForDuplicatesOnAddedRecords(cambiosEntrada, cambiosSalida, formula);

                formula.UpdateCantidadCompleta();

                uow.FormulaRepository.UpdateFormula(formula, cambiosEntrada, cambiosSalida);

                uow.SaveChanges();

                context.AddSuccessNotification("PRD100_form1_msg_FormulaModificada");

                context.Parameters?.Clear();
            }
            catch (ValidationFailedException ex)
            {
                SetParams(context, rowsEntrada, rowsSalida);

                _logger.Error(ex, ex.Message);
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                SetParams(context, rowsEntrada, rowsSalida);

                _logger.Error(ex, "General_Sec0_Error_Operacion");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new FormulaFormValidationModule(uow, this._identity.GetFormatProvider(), true), form, context);
        }

        #region Metodos Auxiliares

        public virtual EntityChanges<FormulaEntrada> MapFormulaEntrada(Formula formula, List<GridRow> rowsEntrada)
        {
            var culture = this._identity.GetFormatProvider();
            var cambios = new EntityChanges<FormulaEntrada>();

            foreach (var row in rowsEntrada)
            {
                string componente = row.GetCell("CD_COMPONENTE").Value;

                if (row.IsNew)
                {
                    var entrada = new FormulaEntrada
                    {
                        Producto = row.GetCell("CD_PRODUTO").Value,
                        Empresa = formula.Empresa,
                        Faixa = 1,
                        NumeroPasada = 1,
                        FechaAlta = DateTime.Now,
                        CantidadPasadas = 1,
                        CantidadConsumir = decimal.Parse(row.GetCell("QT_CONSUMIDA_LINEA").Value, culture),
                        Prioridad = 1
                    };

                    formula.Entrada.Add(entrada);

                    cambios.AddedRecords.Add(entrada);
                }
                else if (row.IsDeleted)
                {
                    var prioridad = short.Parse(row.GetCell("NU_PRIORIDAD").Value);
                    var entrada = formula.Entrada.Where(d => d.Componente == componente && d.Prioridad == prioridad).FirstOrDefault();

                    if (entrada != null)
                        cambios.DeletedRecords.Add(entrada);
                }
                else
                {
                    var prioridad = short.Parse(row.GetCell("NU_PRIORIDAD").Value);
                    var entrada = formula.Entrada.Where(d => d.Componente == componente && d.Prioridad == prioridad).FirstOrDefault();

                    if (entrada == null)
                        throw new OperationNotAllowedException("PRD100_Sec0_error_EntradaNoExiste");

                    entrada.NumeroPasada = 1;
                    entrada.CantidadConsumir = decimal.Parse(row.GetCell("QT_CONSUMIDA_LINEA").Value, culture);

                    cambios.UpdatedRecords.Add(entrada);
                }
            }

            return cambios;
        }

        public virtual EntityChanges<FormulaSalida> MapFormulaSalida(Formula formula, List<GridRow> rowsSalida)
        {
            var culture = this._identity.GetFormatProvider();
            var cambios = new EntityChanges<FormulaSalida>();

            foreach (var row in rowsSalida)
            {
                string producto = row.GetCell("CD_PRODUTO").Value;

                if (row.IsNew)
                {
                    var salida = new FormulaSalida
                    {
                        Producto = row.GetCell("CD_PRODUTO").Value,
                        Empresa = formula.Empresa,
                        Faixa = 1,
                        NumeroPasada = 1,
                        FechaAlta = DateTime.Now,
                        CantidadProducir = decimal.Parse(row.GetCell("QT_CONSUMIDA_LINEA").Value, culture),
                        TipoResultado = FormulaResultadoTipo.ProductoFinal //Lockeado en producto final hasta decidir que hacer
                    };

                    formula.Salida.Add(salida);

                    cambios.AddedRecords.Add(salida);
                }
                else if (row.IsDeleted)
                {
                    var salida = formula.Salida.Where(d => d.Producto == producto).FirstOrDefault();

                    if (salida != null)
                        cambios.DeletedRecords.Add(salida);
                }
                else
                {
                    var salida = formula.Salida.Where(d => d.Producto == producto).FirstOrDefault();

                    if (salida == null)
                        throw new OperationNotAllowedException("PRD100_Sec0_error_SalidaNoExiste");

                    salida.NumeroPasada = int.Parse(row.GetCell("QT_PASADA_LINEA").Value);
                    salida.CantidadProducir = decimal.Parse(row.GetCell("QT_CONSUMIDA_LINEA").Value, culture);

                    cambios.UpdatedRecords.Add(salida);
                }
            }

            return cambios;
        }

        public virtual void ValidateRows(IUnitOfWork uow, Form form, List<GridRow> rowsEntrada, List<GridRow> rowsSalida)
        {
            var parametros = new List<ComponentParameter>
            {
                new ComponentParameter("empresa", form.GetField("empresa").Value),
                new ComponentParameter("codigo", form.GetField("codigo").Value),
            };

            ValidateRowsEntrada(uow, rowsEntrada, parametros);
            ValidateRowsSalida(uow, rowsSalida, parametros);

            if (rowsEntrada.Any(d => !d.IsValid()) || rowsSalida.Any(d => !d.IsValid()))
                throw new OperationNotAllowedException("PRD100_Sec0_error_FormulaDetalleInvalido");

            var productosRepetidos = rowsEntrada.SelectMany(c => c.Cells.Where(c => c.Column.Id == "CD_PRODUTO").Select(c => c.Value))
                .Intersect(rowsSalida.SelectMany(c => c.Cells.Where(c => c.Column.Id == "CD_PRODUTO").Select(c => c.Value)))
                .Any();

            var validarProductoRepetidos = (uow.ParametroRepository.GetParameter(ParamManager.PRD100_VALIDAR_PROD_DISTINTOS) ?? "N") == "S";

            if (productosRepetidos && validarProductoRepetidos)
                throw new ValidationFailedException("PRD100_grid1_error_FormulaProductosEInsumosRepetidos");
        }

        public virtual void ValidateRowsEntrada(IUnitOfWork uow, List<GridRow> rows, List<ComponentParameter> parametros)
        {
            var entradaValidationModule = new FormulaEntradaGridValidationModule(uow, _identity.GetFormatProvider(), true);
            entradaValidationModule.Validator = new GridValidator(parametros);
            entradaValidationModule.Validate(rows, parametros);
        }

        public virtual void ValidateRowsSalida(IUnitOfWork uow, List<GridRow> rows, List<ComponentParameter> parametros)
        {
            var salidaValidationModule = new FormulaSalidaGridValidationModule(uow, _identity.GetFormatProvider(), true);
            salidaValidationModule.Validator = new GridValidator(parametros);
            salidaValidationModule.Validate(rows, parametros);
        }

        public virtual void CheckForDuplicatesOnAddedRecords(EntityChanges<FormulaEntrada> cambiosEntrada, EntityChanges<FormulaSalida> cambiosSalida, Formula formula)
        {
            if (cambiosEntrada.AddedRecords.GroupBy(d => new { d.Producto }).Any(d => d.Count() > 1))
                throw new ValidationFailedException("PRD100_Sec0_error_LineaEntradaDuplicada");

            if (cambiosSalida.AddedRecords.GroupBy(d => d.Producto).Any(d => d.Count() > 1))
                throw new ValidationFailedException("PRD100_Sec0_error_LineaSalidaDuplicada");

            if (cambiosEntrada.AddedRecords.Any(l => l.CantidadConsumir <= 0) || cambiosSalida.AddedRecords.Any(l => l.CantidadProducir <= 0))
                throw new ValidationFailedException("PRD100_Sec0_error_LineaConCantidad0");

            if (formula.Entrada.GroupBy(d => new { d.Producto }).Any(d => d.Count() > 1))
                throw new ValidationFailedException("PRD100_Sec0_error_LineaEntradaDuplicada");

            if (formula.Salida.GroupBy(d => d.Producto).Any(d => d.Count() > 1))
                throw new ValidationFailedException("PRD100_Sec0_error_LineaSalidaDuplicada");
        }

        public virtual void SetParams(FormSubmitContext context, List<GridRow> rowsEntrada, List<GridRow> rowsSalida)
        {
            var serializerSettings = new JsonSerializerSettings();

            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            context.Parameters = new List<ComponentParameter>
                {
                    new ComponentParameter("rowsEntrada", JsonConvert.SerializeObject(rowsEntrada, serializerSettings)),
                    new ComponentParameter("rowsSalida", JsonConvert.SerializeObject(rowsSalida, serializerSettings)),
                };
        }
        #endregion
    }
}
