using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Registro;
using WIS.Application.Validation.Rules.Stock;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoAsignacionInfoValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public MantenimientoAsignacionInfoValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["VL_ASIGNADO"] = this.ValidateAsignado

            };
        }

        public virtual GridValidationGroup ValidateAsignado(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),

                },


            };
        }
    }
}
