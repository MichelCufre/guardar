using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Extension;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Documento
{
    public class DOC095GridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public DOC095GridValidationModule(
            IUnitOfWork uow,
            IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;

            Schema = new GridValidationSchema
            {
                ["DT_PROGRAMADO"] = this.ValidateDT_PROGRAMADO,
            };
        }

        public virtual GridValidationGroup ValidateDT_PROGRAMADO(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new DateTimeValidationRule(cell.Value),
                    new DateTimeGreaterThanValidationRule (cell.Value, DateTime.Today.ToIsoString())
                }
            };
        }
    }
}
