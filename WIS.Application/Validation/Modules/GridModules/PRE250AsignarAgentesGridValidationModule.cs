using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class PRE250AsignarAgentesGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;


        public PRE250AsignarAgentesGridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["NU_ORDEN"] = this.ValidateNuOrden
            };
        }

        public virtual GridValidationGroup ValidateNuOrden(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(cell.Value)) return null;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new StringMaxLengthValidationRule(cell.Value, 5),
                    new PositiveShortValidationRule(cell.Value),
                },
            };
        }
    }
}
