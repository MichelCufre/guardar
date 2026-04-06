using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Persistence.Database;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ImpresoraPertenecePredioValidatonRule : IValidationRule
    {
        protected readonly string _predio;
        protected readonly string _valor;
        protected readonly IUnitOfWork _uow;

        public ImpresoraPertenecePredioValidatonRule(IUnitOfWork uow, string valor, string predio)
        {
            this._valor = valor;
            this._predio = predio;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            List<IValidationError> errors = new List<IValidationError>();
            if (!this._predio.Equals(GeneralDb.PredioSinDefinir))
                if (!this._uow.ImpresionRepository.ExisteImpresoraPredio(this._valor, this._predio))
                {
                    errors.Add(new ValidationError("IMP050_Sec0_error_NoCorrespondeImpresoraPredio", new List<string> { this._valor, this._predio }));
                }

            return errors;
        }
    }
}
