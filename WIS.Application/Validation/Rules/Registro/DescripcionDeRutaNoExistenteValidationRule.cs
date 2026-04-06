using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class DescripcionDeRutaNoExistenteValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;


        public DescripcionDeRutaNoExistenteValidationRule(IUnitOfWork uow, string descripcionRuta)
        {
            this._value = descripcionRuta;
            this._uow = uow;

        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.RutaRepository.AnyRuta(_value))
                errors.Add(new ValidationError("General_Sec0_Error_DescripcionRutaExistente"));

            return errors;
        }
    }
}
