using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class IdControlDeCalidadEnProductoClaseNoExisteValidationRule : IValidationRule
    {
        protected readonly int _valueClase;
        protected readonly IUnitOfWork _uow;

        public IdControlDeCalidadEnProductoClaseNoExisteValidationRule(IUnitOfWork uow, int codigoClase)
        {
            this._valueClase = codigoClase;
            this._uow = uow;
        }
        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!(_uow.ControlDeCalidadRepository.AnyControlDeCalidadClase(_valueClase)))
                errors.Add(new ValidationError("General_Sec0_Error_CodigoControlDeCalidadClaseExistente"));

            return errors;
        }
    }
}
