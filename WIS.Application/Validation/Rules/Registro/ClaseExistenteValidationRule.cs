using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ClaseExistenteValidationRule : IValidationRule
    {
        protected readonly string _valueClase;
        protected readonly IUnitOfWork _uow;

        public ClaseExistenteValidationRule(IUnitOfWork uow, string codigoClase)
        {
            this._valueClase = codigoClase.ToUpper();
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.ClaseRepository.AnyClase(_valueClase))
                errors.Add(new ValidationError("General_Sec0_Error_CodigoUbicacionClaseExistente"));

            return errors;
        }
    }
}
