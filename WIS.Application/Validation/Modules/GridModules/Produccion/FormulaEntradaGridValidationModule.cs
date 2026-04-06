using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Produccion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.Produccion;
using WIS.Exceptions;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Produccion
{
    public class FormulaEntradaGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;
        protected readonly bool _forceNullValidationOnNonModified;

        public FormulaEntradaGridValidationModule(IUnitOfWork uow, IFormatProvider culture, bool forceNullValidationOnNonModified = false)
        {
            this._uow = uow;
            this._culture = culture;
            this._forceNullValidationOnNonModified = forceNullValidationOnNonModified;

            this.Schema = new GridValidationSchema
            {
                ["CD_PRODUTO"] = this.ValidateProducto,
                ["QT_CONSUMIDA_LINEA"] = this.ValidateCantConsumidaLinea
            };
        }

        public virtual GridValidationGroup ValidateProducto(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var empresaParam = parameters.Where(d => d.Id == "empresa").FirstOrDefault()?.Value;

            if (string.IsNullOrEmpty(empresaParam))
                return null;

            var validationGroup = new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 40),
                    new ProductoExistsValidationRule(_uow, empresaParam, cell.Value)
                },
                OnSuccess = this.ValidateProductoOnSuccess
            };

            return validationGroup;
        }

        public virtual GridValidationGroup ValidateCantConsumidaLinea(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new PositiveDecimalValidationRule(this._culture, cell.Value, allowZero:false),
                    new DecimalLengthWithPresicionValidationRule(cell.Value, 12, 3, this._culture),
                }
            };
        }

        public virtual void ValidateProductoOnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var empresa = int.Parse(parameters.Where(d => d.Id == "empresa").FirstOrDefault().Value);
            row.GetCell("DS_PRODUTO").Value = _uow.ProductoRepository.GetDescripcion(empresa, cell.Value);
        }

        public virtual void Validate(List<GridRow> rows, List<ComponentParameter> parametros)
        {
            Formula formula = null;
            var codigoFormula = parametros.Any(s => s.Id == "codigo") ? parametros.FirstOrDefault(s => s.Id == "codigo").Value : "";

            if (!string.IsNullOrEmpty(codigoFormula))
                formula = this._uow.FormulaRepository.GetFormula(codigoFormula);

            if (formula == null && rows.Count == 0)
                throw new ValidationFailedException("PRD100_grid1_error_EntradaVacia");
            else if (!string.IsNullOrEmpty(codigoFormula) && formula.Entrada.Count == 0 && rows.Count == 0)
                throw new ValidationFailedException("PRD100_grid1_error_EntradaVacia");

            foreach (var row in rows)
            {
                if (this._forceNullValidationOnNonModified)
                {
                    foreach (var cell in row.Cells)
                    {
                        cell.Modified = this.Schema.Keys.Contains(cell.Column.Id) && string.IsNullOrEmpty(cell.Value);
                    }
                }

                base.Validate(row);
            }
        }
    }
}
