using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class IdControlDeCalidadEnProductoClaseExistenteValidationRule : IValidationRule
    {
        protected readonly string _valueControl;
        protected readonly IUnitOfWork _uow;
        public IdControlDeCalidadEnProductoClaseExistenteValidationRule(IUnitOfWork uow, string idControl)
        {
            this._valueControl = idControl;
            this._uow = uow;
        }
        public virtual List<IValidationError> Validate()
        {
            var cdControl = int.Parse(this._valueControl);
            var errors = new List<IValidationError>();

            if (!_uow.ControlDeCalidadRepository.AnyControlDeCalidadClase(int.Parse(_valueControl)))
                errors.Add(new ValidationError("General_Sec0_Error_Error65"));

            return errors;
        }
    }
}
