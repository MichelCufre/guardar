using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Expedicion
{
    public class PuertaEmbarqueExistsValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _codigoPuerta;

        public PuertaEmbarqueExistsValidationRule(IUnitOfWork uow, string codigoPuerta)
        {
            this._uow = uow;
            this._codigoPuerta = codigoPuerta;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!this._uow.PuertaEmbarqueRepository.AnyPuertaEmbarque(short.Parse(this._codigoPuerta)))
                errors.Add(new ValidationError("General_Sec0_Error_Er063_PuertaEmbarqueNoExiste"));

            return errors;
        }
    }
}
