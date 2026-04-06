using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Persistence.Database;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteEstadoCierreConteo : IValidationRule
    {
        protected readonly string _tipoCierreConteo;
        protected readonly IUnitOfWork _uow;

        public ExisteEstadoCierreConteo(IUnitOfWork uow, string tipoCierreConteo)
        {
            this._tipoCierreConteo = tipoCierreConteo;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            List<IValidationError> errors = new List<IValidationError>();

            if (!this._uow.DominioRepository.ExisteDetalleDominio(_tipoCierreConteo))
            {
                errors.Add(new ValidationError("INV_Db_Error_NoExisteCierreConteo"));
            }

            return errors;
        }
    }
}
