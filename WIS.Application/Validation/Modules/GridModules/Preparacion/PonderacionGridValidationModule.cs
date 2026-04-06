using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Preparacion
{
    public class PonderacionGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public PonderacionGridValidationModule(IUnitOfWork uow)
        {
            _uow = uow;

            Schema = new GridValidationSchema
            {
                ["NU_PONDERACION"] = ValidatePonderacion,
            };
        }
        public virtual GridValidationGroup ValidatePonderacion(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (cell.Old == cell.Value)
                return null;

            return new GridValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new PositiveNumberMaxLengthValidationRule(cell.Value,10),
                },

            };
        }
    }
}
