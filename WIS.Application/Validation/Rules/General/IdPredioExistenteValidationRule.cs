using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class IdPredioExistenteValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;
        private readonly List<string> prediosStringValidos = new List<string> { GeneralDb.PredioSinDefinir, GeneralDb.PredioSinPredio, "MT" };
        public IdPredioExistenteValidationRule(IUnitOfWork uow, string value)
        {
            this._value = value;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {

            var errors = new List<IValidationError>();

            if (_uow.PredioRepository.AnyPredio(_value) || prediosStringValidos.Contains(_value))
                errors.Add(new ValidationError("General_Db_Error_PredioYaExiste"));


            return errors;
        }
    }
}
