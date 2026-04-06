using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Persistence.Database;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteTipoInventarioValidationRule : IValidationRule
    {
        protected readonly string _tipoInventario;
        protected readonly IUnitOfWork _uow;

        public ExisteTipoInventarioValidationRule(IUnitOfWork uow, string tipoInventario)
        {
            this._tipoInventario = tipoInventario;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            List<IValidationError> errors = new List<IValidationError>();

            if (!this._uow.DominioRepository.ExisteDetalleDominioValor(CodigoDominioDb.TipoInventario, _tipoInventario))
            {
                errors.Add(new ValidationError("INV_Db_Error_NoExisteTipoInventario"));
            }

            return errors;
        }
    }
}
