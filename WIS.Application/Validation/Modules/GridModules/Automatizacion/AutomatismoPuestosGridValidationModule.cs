using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Configuracion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class AutomatismoPuestosGridValidationModule : GridValidationModule
    {
        protected IUnitOfWork _uow;
        protected IIdentityService _identity;

        public AutomatismoPuestosGridValidationModule(IUnitOfWork uow, IIdentityService identity)
        {
            this._uow = uow;
            this._identity = identity;

            this.Schema = new GridValidationSchema
            {
                ["ID_PUESTO"] = this.ValidateIntegracionServicio,
            };
        }

        public virtual GridValidationGroup ValidateIntegracionServicio(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 200),
                    new IdPuestoAutomatismoShouldBeUniqueValidationRule(_uow, cell.Value,row.GetCell("NU_AUTOMATISMO_PUESTO").Value)
                },
            };
        }
    }
}
