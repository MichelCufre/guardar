using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class IdProductoRotatividadNoExistenteValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public IdProductoRotatividadNoExistenteValidationRule(IUnitOfWork uow, string idProductoRotatividad)
        {
            this._value = idProductoRotatividad;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (short.TryParse(_value, out short valor))
                if (!_uow.ProductoRotatividadRepository.AnyProductoRotatividad(valor))
                    errors.Add(new ValidationError("General_Sec0_Error_IdProductoRotatividadNoExistente"));

            return errors;
        }
    }
}
