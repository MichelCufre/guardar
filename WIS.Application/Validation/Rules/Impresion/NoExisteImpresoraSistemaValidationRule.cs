using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Persistence.Database;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class NoExisteImpresoraSistemaValidationRule : IValidationRule
    {
        protected readonly string _predio;
        protected readonly string _codigo;
        protected readonly IUnitOfWork _uow;

        public NoExisteImpresoraSistemaValidationRule(IUnitOfWork uow, string predio, string codigo)
        {
            this._predio = predio;
            this._codigo = codigo;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            List<IValidationError> errors = new List<IValidationError>();

            if (this._uow.ImpresoraRepository.ExisteImpresoraPredio(this._codigo, this._predio))
            {
                errors.Add(new ValidationError("COF040_Sec0_error_ExisteLaImpresora", new List<string> { this._codigo, this._predio }));
            }

            return errors;
        }
    }
}
