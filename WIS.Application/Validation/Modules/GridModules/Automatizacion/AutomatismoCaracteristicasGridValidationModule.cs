using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class AutomatismoCaracteristicasGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider formatProvider;
        protected readonly bool rowIsInserting;

        public AutomatismoCaracteristicasGridValidationModule(IUnitOfWork uow, IFormatProvider formatProvider, bool rowIsInserting)
        {
            this._uow = uow;
            this.formatProvider = formatProvider;
            this.rowIsInserting = rowIsInserting;
            this.Schema = new GridValidationSchema
            {
                ["CD_AUTOMATISMO_CARACTERISTICA"] = this.ValidateCodigoCaracteristica,
                ["VL_AUTOMATISMO_CARACTERISTICA"] = this.ValidateValorCaracteristica,
                ["DS_AUTOMATISMO_CARACTERISTICA"] = this.ValidateDescripcion,
                ["VL_AUX1"] = this.ValidateAux1,
                ["NU_AUX1"] = this.ValidateNumAux1,
                ["QT_AUX1"] = this.ValidateCantidadAux1,
                ["FL_AUX1"] = this.ValidateFlagAux1,
            };
        }

        public virtual GridValidationGroup ValidateCodigoCaracteristica(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule> { new StringMaxLengthValidationRule(cell.Value, 100) };

            if (!row.IsNew) rules.Add(new KeepNullWhenOldWasValidationRule(cell.Value, cell.Old));

            if (rowIsInserting) rules.Add(new NonNullValidationRule(cell.Value));

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual GridValidationGroup ValidateValorCaracteristica(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule> { new StringMaxLengthValidationRule(cell.Value, 400) };

            if (!row.IsNew) rules.Add(new KeepNullWhenOldWasValidationRule(cell.Value, cell.Old));

            if (rowIsInserting) rules.Add(new NonNullValidationRule(cell.Value));

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual GridValidationGroup ValidateDescripcion(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule> { new StringMaxLengthValidationRule(cell.Value, 400) };

            if (!row.IsNew) rules.Add(new KeepNullWhenOldWasValidationRule(cell.Value, cell.Old));

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual GridValidationGroup ValidateAux1(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule> { new StringMaxLengthValidationRule(cell.Value, 100) };

            if (!row.IsNew) rules.Add(new KeepNullWhenOldWasValidationRule(cell.Value, cell.Old));

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules
            };
        }

        public virtual GridValidationGroup ValidateNumAux1(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule> { new PositiveLongValidationRule(cell.Value) };

            if (!row.IsNew) rules.Add(new KeepNullWhenOldWasValidationRule(cell.Value, cell.Old));

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules
            };
        }

        public virtual GridValidationGroup ValidateCantidadAux1(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule> { new PositiveDecimalValidationRule(formatProvider, cell.Value) };

            if (!row.IsNew) rules.Add(new KeepNullWhenOldWasValidationRule(cell.Value, cell.Old));

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules
            };
        }

        public virtual GridValidationGroup ValidateFlagAux1(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule> { new StringMaxLengthValidationRule(cell.Value, 1) };

            if (!row.IsNew) rules.Add(new KeepNullWhenOldWasValidationRule(cell.Value, cell.Old));

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules
            };
        }
    }
}
