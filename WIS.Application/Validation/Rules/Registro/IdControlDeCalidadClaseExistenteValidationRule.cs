using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class IdControlDeCalidadClaseExistenteValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public IdControlDeCalidadClaseExistenteValidationRule(IUnitOfWork uow, string codigoClase)
        {
            this._value = codigoClase;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.ControlDeCalidadRepository.AnyControlDeCalidadClase(int.Parse(_value)))
                errors.Add(new ValidationError("General_Sec0_Error_CodigoControlDeCalidadClaseExistente"));

            return errors;
        }
    }
}
