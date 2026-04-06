using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ClaseNoExistenteValidationRule : IValidationRule
    {
        protected readonly string _valueClase;
        protected readonly IUnitOfWork _uow;

        public ClaseNoExistenteValidationRule(IUnitOfWork uow, string codigoClase)
        {
            this._valueClase = codigoClase.ToUpper();
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.ClaseRepository.AnyClase(_valueClase))
                errors.Add(new ValidationError("General_Sec0_Error_CodigoUbicacionClaseNoExistente"));

            return errors;
        }
    }
}
