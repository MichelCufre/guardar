using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Persistence.Database;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteEstadoDetalleInventario : IValidationRule
    {
        protected readonly string _estado;
        protected readonly IUnitOfWork _uow;

        public ExisteEstadoDetalleInventario(IUnitOfWork uow, string estado)
        {
            this._estado = estado;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            List<IValidationError> errors = new List<IValidationError>();

            if (!this._uow.DominioRepository.ExisteDetalleDominio(_estado))
            {
                errors.Add(new ValidationError("INV_Db_Error_NoExisteCierreConteo"));
            }

            return errors;
        }
    }
}
