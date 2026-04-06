using WIS.Domain.DataModel;
using WIS.GridComponent.Validation;
using WIS.Security;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class AutomatismoEditarEjecucionGridValidationModule : GridValidationModule
    {
        protected IUnitOfWork _uow;
        protected IIdentityService _identity;

        public AutomatismoEditarEjecucionGridValidationModule(IUnitOfWork uow, IIdentityService identity)
        {
            this._uow = uow;
            this._identity = identity;

            this.Schema = new GridValidationSchema
            {

            };
        }


    }
}
