using System.Collections.Generic;
using WIS.Application.Validation.Rules.Documento;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Documento
{
    public class DOC100GridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public DOC100GridValidationModule(IUnitOfWork uow)
        {
            _uow = uow;

            Schema = new GridValidationSchema
            {
                ["NU_PREPARACION"] = this.Validate,
            };
        }

        public virtual GridValidationGroup Validate(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new DOC100EliminarAsociacionValidationRule(_uow, cell.Value),
                }
            };
        }
    }
}
