using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Expedicion
{
    public class RutaExistsValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _codigoRuta;
        protected readonly string _predio;

        public RutaExistsValidationRule(IUnitOfWork uow, string codigoRuta, string predio)
        {
            this._uow = uow;
            this._codigoRuta = codigoRuta;
            this._predio = predio;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!this._uow.RutaRepository.AnyRuta(short.Parse(this._codigoRuta), this._predio))
                errors.Add(new ValidationError("General_Sec0_Error_CodigoRutaInvalido"));

            return errors;
        }
    }
}
