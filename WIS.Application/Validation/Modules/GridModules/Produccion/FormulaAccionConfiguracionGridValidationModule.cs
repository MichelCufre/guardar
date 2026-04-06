using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Produccion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.Produccion;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Produccion
{
    public class FormulaAccionConfiguracionGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;
        protected readonly bool _forceNullValidationOnNonModified;

        public FormulaAccionConfiguracionGridValidationModule(IUnitOfWork uow, IFormatProvider culture, bool forceNullValidationOnNonModified = false)
        {
            this._uow = uow;
            this._culture = culture;
            this.Schema = new GridValidationSchema
            {
                ["CD_ACCION_INSTANCIA"] = this.ValidateAccionInstancia,
                ["QT_PASADAS"] = this.ValidateCantidadPasadas,
                ["NU_ORDEN"] = this.ValidateOrden
            };

            this._forceNullValidationOnNonModified = forceNullValidationOnNonModified;
        }

        public virtual GridValidationGroup ValidateAccionInstancia(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            string formula = parameters.Where(d => d.Id == "formula").FirstOrDefault()?.Value;

            var validationGroup = new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new PositiveShortValidationRule(cell.Value),
                    new FormulaAccionExistsValidationRule(_uow, cell.Value)
                },
                OnSuccess = this.ValidateAccionInstanciaOnSuccess
            };

            if (!string.IsNullOrEmpty(formula))
                validationGroup.Rules.Add(new FormulaConfiguracionNoExisteValidationRule(_uow, formula, cell.Value));

            return validationGroup;
        }
        public virtual GridValidationGroup ValidateCantidadPasadas(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            string cantPasadasParam = parameters.Where(d => d.Id == "cantPasadasFormula").FirstOrDefault().Value;

            if (string.IsNullOrEmpty(cantPasadasParam))
                return null;

            int cantPasadas = int.Parse(cantPasadasParam);

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new PositiveShortValidationRule(cell.Value),
                    new PositiveIntLessThanValidationRule(cell.Value, cantPasadas)
                }
            };
        }
        public virtual GridValidationGroup ValidateOrden(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new PositiveIntValidationRule(cell.Value)
                }
            };
        }

        public virtual void ValidateAccionInstanciaOnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            int id = int.Parse(cell.Value);

            FormulaAccion accion = _uow.FormulaAccionRepository.GetFormulaAccion(id);

            row.GetCell("DS_ACCION_INSTANCIA").Value = accion.Descripcion;
        }

        public virtual void Validate(List<GridRow> rows)
        {

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
