using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class PuertaEmbarqueInexistenteValidationRule : IValidationRule
    {
        protected readonly string _cdPuerta;
        protected readonly IUnitOfWork _uow;

        public PuertaEmbarqueInexistenteValidationRule(IUnitOfWork uow, string idPuerta)
        {
            this._cdPuerta = idPuerta;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.PuertaEmbarqueRepository.AnyPuertaEmbarque(short.Parse(_cdPuerta)))
                errors.Add(new ValidationError("General_Sec0_Error_IdPuertaEmbarqueExistente"));

            return errors;
        }
    }
}
