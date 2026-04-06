using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class IdProductoFamiliaNoExistenteValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public IdProductoFamiliaNoExistenteValidationRule(IUnitOfWork uow, string idFamiliaProducto)
        {
            this._value = idFamiliaProducto;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (int.TryParse(_value, out int valor))
                if (!_uow.ProductoFamiliaRepository.AnyFamiliaProducto(valor))
                    errors.Add(new ValidationError("General_Sec0_Error_IdFamiliaProductoNoExistente"));

            return errors;
        }
    }
}
