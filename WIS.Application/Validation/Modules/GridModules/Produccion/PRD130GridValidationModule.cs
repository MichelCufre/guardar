using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Produccion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Produccion
{
    public class PRD130GridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public PRD130GridValidationModule(IUnitOfWork uow,
            IFormatProvider culture)
        {
            _uow = uow;
            _culture = culture;

            Schema = new GridValidationSchema
            {
                ["NU_PRDC_INGRESO"] = this.ValidateNU_PRDC_INGRESO,
            };
        }

        public virtual GridValidationGroup ValidateNU_PRDC_INGRESO(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 10), 
                    new IngresoExistsValidationRule(this._uow, cell.Value)
                }
            };
        }
    }
}
