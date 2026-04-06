using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Expedicion
{
    public class PuertaEmbarquePertenecePredioValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _codigoPuerta;
        protected readonly string _predio;

        public PuertaEmbarquePertenecePredioValidationRule(IUnitOfWork uow, string codigoPuerta, string predio)
        {
            this._uow = uow;
            this._codigoPuerta = codigoPuerta;
            this._predio = predio;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!this._uow.PuertaEmbarqueRepository.AnyPuertaEmbarquePertenecePredio(short.Parse(this._codigoPuerta), this._predio))
                errors.Add(new ValidationError("General_Sec0_Error_Er054_PuestaNoPertenecePredio"));

            return errors;
        }
    }
}
